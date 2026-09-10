using System.Collections.Generic;
using BepInEx.Configuration;

namespace TheHeadHunter;

internal sealed class WeaponDefinition
{
    public WeaponDefinition(
        string prefabName,
        string nameToken,
        string displayName,
        string modelPrefab,
        CombatProfile[] combatProfiles,
        int[] metalCosts,
        string? combinationSourcePrefab = null)
    {
        if (combatProfiles.Length != metalCosts.Length)
        {
            throw new System.ArgumentException(
                $"{displayName} has {combatProfiles.Length} combat profiles but {metalCosts.Length} metal costs");
        }

        PrefabName = prefabName;
        NameToken = nameToken;
        DisplayName = displayName;
        ModelPrefab = modelPrefab;
        CombatProfiles = combatProfiles;
        MetalCosts = metalCosts;
        CombinationSourcePrefab = combinationSourcePrefab;
    }

    public string PrefabName { get; }
    public string NameToken { get; }
    public string DescriptionToken => $"{NameToken}_description";
    public string DisplayName { get; }
    public string ModelPrefab { get; }
    public IReadOnlyList<CombatProfile> CombatProfiles { get; }
    public IReadOnlyList<int> MetalCosts { get; }
    public Dictionary<int, ConfigEntry<int>> ConfiguredMetalCosts { get; } = new();
    public string? CombinationSourcePrefab { get; }
    public bool IsCombination => CombinationSourcePrefab is not null;

    public CombatProfile GetCombatProfile(int quality)
    {
        int index = System.Math.Max(1, System.Math.Min(quality, CombatProfiles.Count)) - 1;
        return CombatProfiles[index];
    }

    public int GetMetalCost(int quality)
    {
        return ConfiguredMetalCosts[quality].Value;
    }
}
