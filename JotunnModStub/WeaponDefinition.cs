using System.Collections.Generic;
using BepInEx.Configuration;

namespace TheHeadHunter;

internal enum WeaponDamageType
{
    Slash,
    Blunt,
    Pierce,
    Dagger,
    Shield,
}

internal sealed class WeaponDefinition
{
    public WeaponDefinition(
        string prefabName,
        string nameToken,
        string displayName,
        string modelPrefab,
        WeaponDamageType damageType,
        float baseDamage,
        float damagePerQuality,
        int[] metalCosts,
        string? combinationSourcePrefab = null)
    {
        PrefabName = prefabName;
        NameToken = nameToken;
        DisplayName = displayName;
        ModelPrefab = modelPrefab;
        DamageType = damageType;
        BaseDamage = baseDamage;
        DamagePerQuality = damagePerQuality;
        MetalCosts = metalCosts;
        CombinationSourcePrefab = combinationSourcePrefab;
    }

    public string PrefabName { get; }
    public string NameToken { get; }
    public string DescriptionToken => $"{NameToken}_description";
    public string DisplayName { get; }
    public string ModelPrefab { get; }
    public WeaponDamageType DamageType { get; }
    public float BaseDamage { get; }
    public float DamagePerQuality { get; }
    public IReadOnlyList<int> MetalCosts { get; }
    public Dictionary<int, ConfigEntry<int>> ConfiguredMetalCosts { get; } = new();
    public string? CombinationSourcePrefab { get; }
    public bool IsCombination => CombinationSourcePrefab is not null;

    public int GetMetalCost(int quality)
    {
        return ConfiguredMetalCosts[quality].Value;
    }
}
