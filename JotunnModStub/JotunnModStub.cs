using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace TheHeadHunter;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
internal sealed class TheHeadHunterPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.thefizzzz.huntersarsenal";
    public const string PluginName = "Hunter's Arsenal";
    public const string PluginVersion = "0.2.0";
    public const int MaxQuality = 7;

    private Harmony? _harmony;

    internal static ConfigEntry<float> BonusTrophyChance { get; private set; } = null!;
    internal static ConfigEntry<float> GuardMarkChance { get; private set; } = null!;
    internal static ConfigEntry<float> BulwarkMarkChance { get; private set; } = null!;
    internal static ConfigEntry<float> HuntingMarkDuration { get; private set; } = null!;
    private static readonly Dictionary<Piece.Requirement, QualityRequirement> QualityRequirements = new();
    private static readonly HashSet<Recipe> ArsenalRecipes = new();
    private static readonly HashSet<Recipe> CombinationRecipes = new();

    private static readonly IReadOnlyList<ProgressionTier> Progression =
        new[]
        {
            new ProgressionTier(1, "Black Forest", "TrophyEikthyr", "Bronze", "forge", 1),
            new ProgressionTier(2, "Swamp", "TrophyTheElder", "Iron", "forge", 2),
            new ProgressionTier(3, "Mountains", "TrophyBonemass", "Silver", "forge", 3),
            new ProgressionTier(4, "Plains", "TrophyDragonQueen", "BlackMetal", "forge", 4),
            new ProgressionTier(5, "Mistlands", "TrophyGoblinKing", "Eitr", "blackforge", 1),
            new ProgressionTier(6, "Ashlands", "TrophySeekerQueen", "FlametalNew", "blackforge", 3),
            new ProgressionTier(7, "Deep North", "TrophyFader", "Gold", "blackforge", 4),
        };

    private static readonly IReadOnlyList<WeaponDefinition> Weapons =
        ArsenalDefinitions.CreateWeapons();
    private static readonly IReadOnlyDictionary<string, WeaponDefinition> WeaponsByPrefab =
        Weapons.ToDictionary(weapon => weapon.PrefabName, StringComparer.Ordinal);
    private static readonly IReadOnlyDictionary<string, WeaponDefinition> WeaponsByNameToken =
        Weapons.ToDictionary(weapon => $"${weapon.NameToken}", StringComparer.Ordinal);

    private CustomLocalization Localization { get; } = LocalizationManager.Instance.GetLocalization();

    private void Awake()
    {
        BindProgressionConfiguration();
        AddLocalization();
        HuntingMarkManager.Initialize();
        CommandManager.Instance.AddConsoleCommand(new ArsenalTestKitCommand());
        PrefabManager.OnVanillaPrefabsAvailable += RegisterItems;

        _harmony = Harmony.CreateAndPatchAll(typeof(TheHeadHunterPlugin).Assembly, PluginGuid);
        Jotunn.Logger.LogInfo($"Loaded {PluginName} {PluginVersion}");
    }

    private void BindProgressionConfiguration()
    {
        BonusTrophyChance = Config.Bind(
            "Trophy hunting",
            "Bonus trophy chance",
            6f,
            new ConfigDescription(
                "Second-roll percent after vanilla trophy failure on an Arsenal killing blow. 6% takes a 10% trophy (Draugr) to about 15%.",
                new AcceptableValueRange<float>(0f, 100f)));
        GuardMarkChance = Config.Bind(
            "Trophy hunting",
            "Guard parry mark chance",
            11f,
            new ConfigDescription(
                "Second-roll percent on prey parried with Hunter's Guard. 11% takes a 10% trophy (Draugr) to about 20%.",
                new AcceptableValueRange<float>(0f, 100f)));
        BulwarkMarkChance = Config.Bind(
            "Trophy hunting",
            "Bulwark block mark chance",
            5f,
            new ConfigDescription(
                "Second-roll percent on prey blocked with Hunter's Bulwark. 5% takes a 10% trophy (Draugr) to about 14.5%.",
                new AcceptableValueRange<float>(0f, 100f)));
        HuntingMarkDuration = Config.Bind(
            "Trophy hunting",
            "Hunting mark duration",
            6f,
            new ConfigDescription(
                "Seconds before a Guard or Bulwark hunting mark expires.",
                new AcceptableValueRange<float>(1f, 120f)));

        foreach (WeaponDefinition weapon in Weapons)
        {
            weapon.ConfiguredMetalCosts.Clear();
            foreach (ProgressionTier tier in Progression)
            {
                if (weapon.IsCombination && tier.Quality == 1)
                {
                    continue;
                }

                int defaultCost = weapon.MetalCosts[tier.Quality - 1];
                weapon.ConfiguredMetalCosts[tier.Quality] = Config.Bind(
                    $"Progression - {SanitizeConfigName(weapon.DisplayName)}",
                    $"{tier.DisplayName} {tier.Metal} amount",
                    defaultCost,
                    new ConfigDescription(
                        $"Amount of {tier.Metal} needed for {weapon.DisplayName} to reach {tier.DisplayName} quality.",
                        new AcceptableValueRange<int>(1, 200)));
            }
        }
    }

    private static string SanitizeConfigName(string value)
    {
        return value.Replace("'", string.Empty);
    }

    private void AddLocalization()
    {
        foreach (WeaponDefinition weapon in Weapons)
        {
            Localization.AddTranslation("English", weapon.NameToken, weapon.DisplayName);
            Localization.AddTranslation(
                "English",
                weapon.DescriptionToken,
                "A weapon that grows stronger when reforged with the trophies of forsaken prey.");
        }
    }

    private static void RegisterItem(WeaponDefinition weapon)
    {
        ItemConfig itemConfig = new()
        {
            Name = $"${weapon.NameToken}",
            Description = $"${weapon.DescriptionToken}",
            CraftingStation = Progression[0].CraftingStation,
            RepairStation = Progression[0].CraftingStation,
            MinStationLevel = Progression[0].StationLevel,
            Amount = 1,
        };

        if (weapon.IsCombination)
        {
            itemConfig.AddRequirement(new RequirementConfig(
                weapon.CombinationSourcePrefab!,
                2,
                0,
                false));
        }

        foreach (ProgressionTier tier in Progression)
        {
            if (weapon.IsCombination && tier.Quality == 1)
            {
                continue;
            }

            bool isBaseQuality = tier.Quality == 1;
            itemConfig.AddRequirement(new RequirementConfig(
                tier.BossTrophy,
                isBaseQuality ? 1 : 0,
                isBaseQuality ? 0 : 1,
                false));
            itemConfig.AddRequirement(new RequirementConfig(
                tier.Metal,
                isBaseQuality ? weapon.GetMetalCost(tier.Quality) : 0,
                isBaseQuality ? 0 : 1,
                false));
        }

        CustomItem item = new(weapon.PrefabName, weapon.ModelPrefab, itemConfig);
        var shared = item.ItemDrop.m_itemData.m_shared;
        shared.m_maxQuality = Progression.Count;
        SetWeaponDamage(shared, weapon);

        Piece.Requirement[] requirements = item.Recipe.Recipe.m_resources;
        int expectedRequirements = weapon.IsCombination
            ? 1 + ((Progression.Count - 1) * 2)
            : Progression.Count * 2;
        if (requirements.Length != expectedRequirements)
        {
            throw new InvalidOperationException(
                $"Expected {expectedRequirements} requirements for {weapon.PrefabName}, got {requirements.Length}");
        }

        ArsenalRecipes.Add(item.Recipe.Recipe);
        int requirementIndex = 0;
        if (weapon.IsCombination)
        {
            QualityRequirements.Add(requirements[requirementIndex++], new QualityRequirement(1, 2));
            CombinationRecipes.Add(item.Recipe.Recipe);
        }

        foreach (ProgressionTier tier in Progression)
        {
            if (weapon.IsCombination && tier.Quality == 1)
            {
                continue;
            }

            QualityRequirements.Add(
                requirements[requirementIndex++],
                new QualityRequirement(tier.Quality, weapon.IsCombination ? 2 : 1));
            QualityRequirements.Add(
                requirements[requirementIndex++],
                new QualityRequirement(tier.Quality, weapon.GetMetalCost(tier.Quality)));
        }

        ItemManager.Instance.AddItem(item);
    }

    private static void SetWeaponDamage(ItemDrop.ItemData.SharedData shared, WeaponDefinition weapon)
    {
        CombatProfile profile = weapon.GetCombatProfile(1);
        shared.m_damages = profile.Damage;
        shared.m_damagesPerLevel = new HitData.DamageTypes();
        shared.m_blockPower = profile.BlockPower;
        shared.m_blockPowerPerLevel = 0f;
        shared.m_maxDurability = profile.Durability;
        shared.m_durabilityPerLevel = 0f;
        shared.m_attack.m_attackStamina = profile.AttackStamina;
        shared.m_attackForce = profile.Knockback;
        shared.m_attack.m_drawStaminaDrain = profile.DrawStaminaDrain;
    }

    private void RegisterItems()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= RegisterItems;
        QualityRequirements.Clear();
        ArsenalRecipes.Clear();
        CombinationRecipes.Clear();

        foreach (WeaponDefinition weapon in Weapons)
        {
            RegisterItem(weapon);
        }

        HuntingMarkManager.RegisterVisual();

        Jotunn.Logger.LogInfo(
            $"Registered {Weapons.Count} Hunter's Arsenal weapons with {Progression.Count} quality levels each");
    }

    private void OnDestroy()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= RegisterItems;
        _harmony?.UnpatchSelf();
    }

    internal static bool TryGetQualityRequirement(
        Piece.Requirement requirement,
        int quality,
        out int amount)
    {
        if (QualityRequirements.TryGetValue(requirement, out QualityRequirement configured))
        {
            amount = configured.Quality == quality ? configured.Amount : 0;
            return true;
        }

        amount = 0;
        return false;
    }

    internal static bool IsArsenalWeapon(string prefabName)
    {
        return WeaponsByPrefab.ContainsKey(prefabName);
    }

    internal static IEnumerable<WeaponDefinition> GetWeapons()
    {
        return Weapons;
    }

    internal static bool TryGetWeapon(
        ItemDrop.ItemData item,
        out WeaponDefinition weapon)
    {
        string? prefabName = GetItemPrefabName(item);
        if (prefabName is not null &&
            WeaponsByPrefab.TryGetValue(prefabName, out WeaponDefinition found))
        {
            weapon = found;
            return true;
        }

        string? sharedName = item.m_shared?.m_name;
        if (sharedName is not null &&
            WeaponsByNameToken.TryGetValue(sharedName, out found))
        {
            weapon = found;
            return true;
        }

        weapon = null!;
        return false;
    }

    internal static bool TryGetCombatProfile(
        ItemDrop.ItemData item,
        int quality,
        out CombatProfile profile)
    {
        if (TryGetWeapon(item, out WeaponDefinition weapon))
        {
            profile = weapon.GetCombatProfile(quality);
            return true;
        }

        profile = null!;
        return false;
    }

    internal static string? GetItemPrefabName(ItemDrop.ItemData item)
    {
        GameObject? prefab = item.m_dropPrefab;
        if (prefab is null)
        {
            return null;
        }

        string name = prefab.name;
        int clone = name.IndexOf("(Clone)", System.StringComparison.Ordinal);
        return clone >= 0 ? name.Substring(0, clone).Trim() : name;
    }

    internal static bool IsCombinationRecipe(Recipe recipe)
    {
        return CombinationRecipes.Contains(recipe);
    }

    internal static bool TryGetProgressionTier(
        Recipe recipe,
        int quality,
        out ProgressionTier tier)
    {
        if (ArsenalRecipes.Contains(recipe) &&
            quality >= 1 &&
            quality <= Progression.Count)
        {
            tier = Progression[quality - 1];
            return true;
        }

        tier = null!;
        return false;
    }

    private readonly struct QualityRequirement
    {
        public QualityRequirement(int quality, int amount)
        {
            Quality = quality;
            Amount = amount;
        }

        public int Quality { get; }
        public int Amount { get; }
    }
}

[HarmonyPatch(typeof(Recipe), nameof(Recipe.GetRequiredStation))]
internal static class ArsenalCraftingStationPatch
{
    [HarmonyPostfix]
    private static void UseTierCraftingStation(
        Recipe __instance,
        int quality,
        ref CraftingStation __result)
    {
        if (!TheHeadHunterPlugin.TryGetProgressionTier(__instance, quality, out ProgressionTier tier))
        {
            return;
        }

        GameObject? stationPrefab = PrefabManager.Instance.GetPrefab(tier.CraftingStation);
        CraftingStation? station = stationPrefab?.GetComponent<CraftingStation>();
        if (station is not null)
        {
            __result = station;
        }
    }
}

[HarmonyPatch(typeof(Recipe), nameof(Recipe.GetRequiredStationLevel))]
internal static class ArsenalCraftingStationLevelPatch
{
    [HarmonyPostfix]
    private static void UseTierCraftingStationLevel(
        Recipe __instance,
        int quality,
        ref int __result)
    {
        if (TheHeadHunterPlugin.TryGetProgressionTier(__instance, quality, out ProgressionTier tier))
        {
            __result = tier.StationLevel;
        }
    }
}

[HarmonyPatch(typeof(Piece.Requirement), nameof(Piece.Requirement.GetAmount))]
internal static class QualityRequirementPatch
{
    [HarmonyPostfix]
    private static void UseConfiguredQualityRequirement(
        Piece.Requirement __instance,
        int qualityLevel,
        ref int __result)
    {
        if (TheHeadHunterPlugin.TryGetQualityRequirement(__instance, qualityLevel, out int amount))
        {
            __result = amount;
        }
    }
}

internal static class DualCraftingContext
{
    public static string? OutputPrefab { get; private set; }
    public static int Quality { get; private set; }
    public static bool IsActive => OutputPrefab is not null;

    public static void Begin(string outputPrefab, int quality)
    {
        OutputPrefab = outputPrefab;
        Quality = quality;
    }

    public static void Clear()
    {
        OutputPrefab = null;
        Quality = 0;
    }
}

[HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
internal static class DualRecipeCraftPatch
{
    [HarmonyPrefix]
    private static void CaptureMatchingQuality(
        Recipe ___m_craftRecipe,
        ItemDrop.ItemData ___m_craftUpgradeItem,
        Player player)
    {
        DualCraftingContext.Clear();
        if (___m_craftRecipe is null ||
            ___m_craftUpgradeItem is not null ||
            !TheHeadHunterPlugin.IsCombinationRecipe(___m_craftRecipe))
        {
            return;
        }

        Piece.Requirement sourceRequirement = ___m_craftRecipe.m_resources[0];
        string sourceName = sourceRequirement.m_resItem.m_itemData.m_shared.m_name;
        for (int quality = ___m_craftRecipe.m_item.m_itemData.m_shared.m_maxQuality; quality >= 1; quality--)
        {
            if (player.GetInventory().CountItems(sourceName, quality) >= 2)
            {
                DualCraftingContext.Begin(___m_craftRecipe.m_item.gameObject.name, quality);
                return;
            }
        }
    }

    [HarmonyFinalizer]
    private static Exception? ClearMatchingQuality(Exception? __exception)
    {
        DualCraftingContext.Clear();
        return __exception;
    }
}

[HarmonyPatch(
    typeof(Inventory),
    nameof(Inventory.AddItem),
    new Type[]
    {
        typeof(string),
        typeof(int),
        typeof(int),
        typeof(int),
        typeof(long),
        typeof(string),
        typeof(Vector2i),
        typeof(bool),
        typeof(bool),
        typeof(bool),
    })]
internal static class DualRecipeOutputQualityPatch
{
    [HarmonyPrefix]
    private static void ApplyMatchingQuality(string name, ref int quality)
    {
        if (DualCraftingContext.IsActive &&
            name.Equals(DualCraftingContext.OutputPrefab, StringComparison.Ordinal))
        {
            quality = DualCraftingContext.Quality;
        }
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.ConsumeResources))]
internal static class DualRecipeConsumptionPatch
{
    [HarmonyPrefix]
    private static void ConsumeMatchingQuality(ref int itemQuality)
    {
        if (DualCraftingContext.IsActive)
        {
            itemQuality = DualCraftingContext.Quality;
        }
    }
}

[HarmonyPatch(typeof(Humanoid), "BlockAttack")]
internal static class ShieldHuntingMarkPatch
{
    [HarmonyPostfix]
    private static void MarkBlockedPrey(
        Humanoid __instance,
        Character attacker,
        bool __result,
        float ___m_blockTimer)
    {
        if (!__result || attacker is null)
        {
            return;
        }

        ItemDrop.ItemData? blocker = __instance.GetCurrentBlocker() ?? __instance.LeftItem;
        if (blocker is null ||
            !TheHeadHunterPlugin.TryGetWeapon(blocker, out WeaponDefinition weapon))
        {
            return;
        }

        bool timedParry = blocker.m_shared.m_timedBlockBonus > 1f &&
            ___m_blockTimer >= 0f &&
            ___m_blockTimer < 0.25f;
        if (weapon.PrefabName == "HuntersGuard" && timedParry)
        {
            HuntingMarkManager.Apply(
                attacker,
                TheHeadHunterPlugin.GuardMarkChance.Value / 100f);
        }
        else if (weapon.PrefabName == "HuntersBulwark")
        {
            HuntingMarkManager.Apply(
                attacker,
                TheHeadHunterPlugin.BulwarkMarkChance.Value / 100f);
        }
    }
}

[HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
internal static class TrophyDropPatch
{
    [HarmonyPostfix]
    private static void AddHeadHunterTrophyRoll(
        CharacterDrop __instance,
        ref List<KeyValuePair<GameObject, int>> __result)
    {
        Character? victim = __instance.GetComponent<Character>();
        if (victim is null)
        {
            return;
        }

        HitData? lastHit = victim.m_lastHit;
        if (lastHit?.GetAttacker() is not Humanoid attacker)
        {
            return;
        }

        float chance = HuntingMarkManager.Consume(victim);
        ItemDrop.ItemData? weapon = attacker.GetCurrentWeapon();
        if (weapon is not null && TheHeadHunterPlugin.TryGetWeapon(weapon, out _))
        {
            chance = Mathf.Max(
                chance,
                TheHeadHunterPlugin.BonusTrophyChance.Value / 100f);
        }

        if (chance <= 0f)
        {
            return;
        }

        foreach (CharacterDrop.Drop drop in __instance.m_drops)
        {
            GameObject? trophyPrefab = drop.m_prefab;
            if (trophyPrefab is null)
            {
                continue;
            }

            ItemDrop? itemDrop = trophyPrefab.GetComponent<ItemDrop>();
            if (itemDrop?.m_itemData.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Trophy ||
                __result.Any(result => result.Key == trophyPrefab))
            {
                continue;
            }

            if (UnityEngine.Random.value <= chance)
            {
                __result.Add(new KeyValuePair<GameObject, int>(trophyPrefab, 1));
            }
        }
    }
}