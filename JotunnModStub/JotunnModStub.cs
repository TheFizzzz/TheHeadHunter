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
    public const string PluginVersion = "0.1.0";

    private Harmony? _harmony;

    internal static ConfigEntry<float> BonusTrophyChance { get; private set; } = null!;
    private static readonly Dictionary<Piece.Requirement, QualityRequirement> QualityRequirements = new();
    private static readonly HashSet<Recipe> CombinationRecipes = new();

    private static readonly IReadOnlyList<ProgressionTier> Progression =
        new[]
        {
            new ProgressionTier(1, "Black Forest", "TrophyEikthyr", "Bronze", 1),
            new ProgressionTier(2, "Swamp", "TrophyTheElder", "Iron", 2),
            new ProgressionTier(3, "Mountains", "TrophyBonemass", "Silver", 3),
            new ProgressionTier(4, "Plains", "TrophyDragonQueen", "BlackMetal", 4),
        };

    private static readonly IReadOnlyList<WeaponDefinition> Weapons =
        new[]
        {
            new WeaponDefinition(
                "HuntersEdge",
                "item_hunters_edge",
                "Hunter's Edge",
                "SwordIron",
                WeaponDamageType.Slash,
                35f,
                20f,
                new[] { 10, 25, 50, 25 }),
            new WeaponDefinition(
                "HuntersHammer",
                "item_hunters_hammer",
                "Hunter's Knell",
                "MaceIron",
                WeaponDamageType.Blunt,
                35f,
                20f,
                new[] { 10, 25, 38, 25 }),
            new WeaponDefinition(
                "HuntersReach",
                "item_hunters_reach",
                "Hunter's Reach",
                "AtgeirIron",
                WeaponDamageType.Pierce,
                45f,
                20f,
                new[] { 10, 38, 38, 38 }),
            new WeaponDefinition(
                "HuntersGaze",
                "item_hunters_gaze",
                "Hunter's Gaze",
                "BowHuntsman",
                WeaponDamageType.Pierce,
                32f,
                10f,
                new[] { 5, 13, 19, 19 }),
            new WeaponDefinition(
                "HuntersGuard",
                "item_hunters_guard",
                "Hunter's Guard",
                "ShieldBanded",
                WeaponDamageType.Shield,
                24f,
                18f,
                new[] { 6, 13, 13, 10 }),
            new WeaponDefinition(
                "HuntersBulwark",
                "item_hunters_bulwark",
                "Hunter's Bulwark",
                "ShieldIronTower",
                WeaponDamageType.Shield,
                34f,
                16f,
                new[] { 10, 25, 19, 19 }),
            new WeaponDefinition(
                "HuntersFang",
                "item_hunters_fang",
                "Hunter's Fang",
                "KnifeChitin",
                WeaponDamageType.Dagger,
                12f,
                7.333f,
                new[] { 5, 10, 13, 13 }),
            new WeaponDefinition(
                "HuntersSpear",
                "item_hunters_spear",
                "Hunter's Thorn",
                "SpearElderbark",
                WeaponDamageType.Pierce,
                35f,
                20f,
                new[] { 6, 13, 13, 13 }),
            new WeaponDefinition(
                "HuntersCleaver",
                "item_hunters_cleaver",
                "Hunter's Bite",
                "AxeIron",
                WeaponDamageType.Slash,
                35f,
                20f,
                new[] { 10, 25, 25, 25 }),
            new WeaponDefinition(
                "HuntersFury",
                "item_hunters_fury",
                "Hunter's Fury",
                "AxeBerzerkr",
                WeaponDamageType.Slash,
                35f,
                20f,
                new[] { 0, 50, 50, 50 },
                "HuntersCleaver"),
            new WeaponDefinition(
                "HuntersJaw",
                "item_hunters_jaw",
                "Hunter's Jaw",
                "KnifeSkollAndHati",
                WeaponDamageType.Dagger,
                12f,
                7.333f,
                new[] { 0, 20, 26, 26 },
                "HuntersFang"),
            new WeaponDefinition(
                "HuntersOath",
                "item_hunters_oath",
                "Hunter's Oath",
                "THSwordKrom",
                WeaponDamageType.Slash,
                50f,
                25f,
                new[] { 15, 38, 63, 38 }),
            new WeaponDefinition(
                "HuntersJudgment",
                "item_hunters_judgment",
                "Hunter's Judgment",
                "SledgeIron",
                WeaponDamageType.Blunt,
                45f,
                20f,
                new[] { 15, 38, 50, 38 }),
            new WeaponDefinition(
                "HuntersRuin",
                "item_hunters_ruin",
                "Hunter's Ruin",
                "Battleaxe",
                WeaponDamageType.Slash,
                45f,
                25f,
                new[] { 15, 44, 50, 38 }),
        };

    private CustomLocalization Localization { get; } = LocalizationManager.Instance.GetLocalization();

    private void Awake()
    {
        BindProgressionConfiguration();
        AddLocalization();
        PrefabManager.OnVanillaPrefabsAvailable += RegisterItems;

        _harmony = Harmony.CreateAndPatchAll(typeof(TheHeadHunterPlugin).Assembly, PluginGuid);
        Jotunn.Logger.LogInfo($"Loaded {PluginName} {PluginVersion}");
    }

    private void BindProgressionConfiguration()
    {
        BonusTrophyChance = Config.Bind(
            "Trophy hunting",
            "Bonus trophy chance",
            25f,
            new ConfigDescription(
                "Percent chance to award a trophy when the creature's normal trophy roll fails.",
                new AcceptableValueRange<float>(0f, 100f)));

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
                    $"Progression - {weapon.DisplayName}",
                    $"{tier.DisplayName} {tier.Metal} amount",
                    defaultCost,
                    new ConfigDescription(
                        $"Amount of {tier.Metal} needed for {weapon.DisplayName} to reach {tier.DisplayName} quality.",
                        new AcceptableValueRange<int>(1, 200)));
            }
        }
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
            CraftingStation = "forge",
            RepairStation = "forge",
            MinStationLevel = Progression[0].ForgeLevel,
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
        switch (weapon.DamageType)
        {
            case WeaponDamageType.Slash:
                shared.m_damages.m_slash = weapon.BaseDamage;
                shared.m_damagesPerLevel.m_slash = weapon.DamagePerQuality;
                break;
            case WeaponDamageType.Blunt:
                shared.m_damages.m_blunt = weapon.BaseDamage;
                shared.m_damagesPerLevel.m_blunt = weapon.DamagePerQuality;
                break;
            case WeaponDamageType.Pierce:
                shared.m_damages.m_pierce = weapon.BaseDamage;
                shared.m_damagesPerLevel.m_pierce = weapon.DamagePerQuality;
                break;
            case WeaponDamageType.Dagger:
                shared.m_damages.m_slash = weapon.BaseDamage;
                shared.m_damages.m_pierce = weapon.BaseDamage;
                shared.m_damagesPerLevel.m_slash = weapon.DamagePerQuality;
                shared.m_damagesPerLevel.m_pierce = weapon.DamagePerQuality;
                break;
            case WeaponDamageType.Shield:
                shared.m_blockPower = weapon.BaseDamage;
                shared.m_blockPowerPerLevel = weapon.DamagePerQuality;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(weapon.DamageType));
        }
    }

    private void RegisterItems()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= RegisterItems;
        QualityRequirements.Clear();
        CombinationRecipes.Clear();

        foreach (WeaponDefinition weapon in Weapons)
        {
            RegisterItem(weapon);
        }

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
        return Weapons.Any(weapon => weapon.PrefabName.Equals(prefabName, StringComparison.Ordinal));
    }

    internal static bool IsCombinationRecipe(Recipe recipe)
    {
        return CombinationRecipes.Contains(recipe);
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

[HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
internal static class TrophyDropPatch
{
    [HarmonyPostfix]
    private static void AddHeadHunterTrophyRoll(
        CharacterDrop __instance,
        ref List<KeyValuePair<GameObject, int>> __result)
    {
        Character victim = __instance.GetComponent<Character>();
        HitData? lastHit = victim?.m_lastHit;
        if (lastHit?.GetAttacker() is not Humanoid attacker)
        {
            return;
        }

        ItemDrop.ItemData? weapon = attacker.GetCurrentWeapon();
        string? weaponPrefabName = weapon?.m_dropPrefab?.name;
        if (weaponPrefabName is null ||
            !TheHeadHunterPlugin.IsArsenalWeapon(weaponPrefabName))
        {
            return;
        }

        float chance = TheHeadHunterPlugin.BonusTrophyChance.Value / 100f;
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