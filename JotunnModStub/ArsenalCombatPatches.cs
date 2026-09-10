using System;
using HarmonyLib;
using UnityEngine;

namespace TheHeadHunter;

[HarmonyPatch(
    typeof(ItemDrop.ItemData),
    nameof(ItemDrop.ItemData.GetDamage),
    new Type[] { typeof(int), typeof(float) })]
internal static class ArsenalDamagePatch
{
    [HarmonyPostfix]
    private static void UseExplicitQualityProfile(
        ItemDrop.ItemData __instance,
        int quality,
        ref HitData.DamageTypes __result)
    {
        if (TheHeadHunterPlugin.TryGetCombatProfile(__instance, quality, out CombatProfile profile))
        {
            __result = profile.Damage;
        }
    }
}

[HarmonyPatch(
    typeof(ItemDrop.ItemData),
    nameof(ItemDrop.ItemData.GetBaseBlockPower),
    new Type[] { typeof(int) })]
internal static class ArsenalBlockPatch
{
    [HarmonyPostfix]
    private static void UseProfileBlock(
        ItemDrop.ItemData __instance,
        int quality,
        ref float __result)
    {
        if (TheHeadHunterPlugin.TryGetCombatProfile(__instance, quality, out CombatProfile profile))
        {
            __result = profile.BlockPower;
        }
    }
}

[HarmonyPatch(
    typeof(ItemDrop.ItemData),
    nameof(ItemDrop.ItemData.GetMaxDurability),
    new Type[] { typeof(int) })]
internal static class ArsenalDurabilityPatch
{
    [HarmonyPostfix]
    private static void UseProfileDurability(
        ItemDrop.ItemData __instance,
        int quality,
        ref float __result)
    {
        if (TheHeadHunterPlugin.TryGetCombatProfile(__instance, quality, out CombatProfile profile) &&
            profile.Durability > 0f)
        {
            __result = profile.Durability;
        }
    }
}

[HarmonyPatch(
    typeof(Attack),
    nameof(Attack.Start),
    new Type[]
    {
        typeof(Humanoid),
        typeof(Rigidbody),
        typeof(ZSyncAnimation),
        typeof(CharacterAnimEvent),
        typeof(VisEquipment),
        typeof(ItemDrop.ItemData),
        typeof(Attack),
        typeof(float),
        typeof(float),
    })]
internal static class ArsenalAttackStartPatch
{
    [HarmonyPostfix]
    private static void ApplyProfileStamina(Attack __instance, ItemDrop.ItemData weapon)
    {
        if (weapon is not null &&
            TheHeadHunterPlugin.TryGetCombatProfile(weapon, weapon.m_quality, out CombatProfile profile) &&
            profile.AttackStamina > 0f)
        {
            __instance.m_attackStamina = profile.AttackStamina;
        }
    }
}

[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetDrawStaminaDrain))]
internal static class ArsenalDrawStaminaPatch
{
    [HarmonyPostfix]
    private static void UseProfileDrawStamina(ItemDrop.ItemData __instance, ref float __result)
    {
        if (TheHeadHunterPlugin.TryGetCombatProfile(__instance, __instance.m_quality, out CombatProfile profile) &&
            profile.DrawStaminaDrain > 0f)
        {
            __result = profile.DrawStaminaDrain;
        }
    }
}
