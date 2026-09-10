using System.Collections.Generic;

namespace TheHeadHunter;

internal static class ArsenalDefinitions
{
    public static IReadOnlyList<WeaponDefinition> CreateWeapons()
    {
        return new[]
        {
            new WeaponDefinition(
                "HuntersEdge",
                "item_hunters_edge",
                "Hunter's Edge",
                "SwordIron",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 44),
                        P(slash: 51, fire: 6, lightning: 7),
                        P(slash: 67, fire: 8, lightning: 9),
                        P(slash: 78, fire: 13, lightning: 13),
                        P(slash: 83, fire: 20, lightning: 21),
                        P(slash: 94, fire: 25, lightning: 25),
                        P(slash: 111, fire: 37, lightning: 37),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 8f, 10f, 12f, 14f, 16f, 16f, 16f },
                    knockback: Repeat(40f),
                    block: new[] { 13f, 22f, 31f, 40f, 49f, 58f, 66f }),
                new[] { 10, 25, 50, 25, 13, 25, 25 }),
            new WeaponDefinition(
                "HuntersHammer",
                "item_hunters_hammer",
                "Hunter's Knell",
                "MaceIron",
                ApplySecondary(
                    new[]
                    {
                        P(blunt: 44),
                        P(blunt: 51, frost: 6, spirit: 7),
                        P(blunt: 83, frost: 10, spirit: 11),
                        P(blunt: 83, frost: 13, spirit: 14),
                        P(blunt: 83, frost: 20, spirit: 21),
                        P(blunt: 94, frost: 25, spirit: 25),
                        P(blunt: 111, frost: 37, spirit: 37),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 8f, 10f, 12f, 14f, 16f, 16f, 16f },
                    knockback: new[] { 80f, 90f, 100f, 100f, 100f, 100f, 100f },
                    block: new[] { 13f, 22f, 31f, 40f, 49f, 58f, 66f }),
                new[] { 10, 25, 38, 25, 13, 25, 25 }),
            new WeaponDefinition(
                "HuntersReach",
                "item_hunters_reach",
                "Hunter's Reach",
                "AtgeirIron",
                ApplySecondary(
                    new[]
                    {
                        P(pierce: 54),
                        P(pierce: 59, frost: 7, lightning: 8),
                        P(pierce: 75, frost: 9, lightning: 10),
                        P(pierce: 86, frost: 14, lightning: 14),
                        P(pierce: 90, frost: 22, lightning: 22),
                        P(pierce: 108, frost: 29, lightning: 29),
                        P(pierce: 118, frost: 39, lightning: 40),
                    },
                    durability: new[] { 225f, 225f, 225f, 225f, 300f, 375f, 450f },
                    stamina: new[] { 12f, 14f, 16f, 18f, 20f, 20f, 20f },
                    knockback: Repeat(30f),
                    block: new[] { 17f, 29f, 41f, 53f, 65f, 77f, 88f }),
                new[] { 10, 38, 38, 38, 19, 38, 38 }),
            new WeaponDefinition(
                "HuntersGaze",
                "item_hunters_gaze",
                "Hunter's Gaze",
                "BowHuntsman",
                ApplySecondary(
                    new[]
                    {
                        P(pierce: 37),
                        P(pierce: 38, poison: 4, spirit: 5),
                        P(pierce: 51, poison: 6, spirit: 7),
                        P(pierce: 59, poison: 9, spirit: 10),
                        P(pierce: 61, poison: 15, spirit: 15),
                        P(pierce: 62, poison: 17, spirit: 17),
                        P(pierce: 84, poison: 28, spirit: 28),
                    },
                    durability: new[] { 150f, 150f, 150f, 150f, 300f, 350f, 450f },
                    stamina: new[] { 6f, 8f, 10f, 12f, 14f, 14f, 14f },
                    knockback: new[] { 5f, 10f, 20f, 20f, 25f, 25f, 25f },
                    block: Repeat(3f),
                    drawStamina: new[] { 6f, 8f, 10f, 12f, 14f, 14f, 14f }),
                new[] { 5, 13, 19, 19, 13, 7, 13 }),
            new WeaponDefinition(
                "HuntersGuard",
                "item_hunters_guard",
                "Hunter's Guard",
                "ShieldBanded",
                ApplySecondary(
                    new[]
                    {
                        P(blockPower: 24),
                        P(blockPower: 42),
                        P(blockPower: 60),
                        P(blockPower: 78),
                        P(blockPower: 96),
                        P(blockPower: 114),
                        P(blockPower: 138),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: Repeat(0f),
                    knockback: Repeat(0f),
                    block: new[] { 24f, 42f, 60f, 78f, 96f, 114f, 138f }),
                new[] { 6, 13, 13, 10, 13, 13, 13 }),
            new WeaponDefinition(
                "HuntersBulwark",
                "item_hunters_bulwark",
                "Hunter's Bulwark",
                "ShieldIronTower",
                ApplySecondary(
                    new[]
                    {
                        P(blockPower: 32),
                        P(blockPower: 52),
                        P(blockPower: 70),
                        P(blockPower: 104),
                        P(blockPower: 122),
                        P(blockPower: 140),
                        P(blockPower: 164),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: Repeat(0f),
                    knockback: Repeat(0f),
                    block: new[] { 32f, 52f, 70f, 104f, 122f, 140f, 164f }),
                new[] { 10, 25, 19, 19, 19, 19, 19 }),
            new WeaponDefinition(
                "HuntersFang",
                "item_hunters_fang",
                "Hunter's Fang",
                "KnifeChitin",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 16, pierce: 17),
                        P(slash: 17, pierce: 17, lightning: 4, poison: 5),
                        P(slash: 21, pierce: 21, lightning: 5, poison: 6),
                        P(slash: 26, pierce: 27, lightning: 9, poison: 9),
                        P(slash: 31, pierce: 31, lightning: 15, poison: 16),
                        P(slash: 42, pierce: 42, lightning: 22, poison: 23),
                        P(slash: 49, pierce: 50, lightning: 33, poison: 33),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 6f, 8f, 10f, 12f, 14f, 14f, 14f },
                    knockback: Repeat(10f),
                    block: Repeat(3f)),
                new[] { 5, 10, 13, 13, 13, 13, 13 }),
            new WeaponDefinition(
                "HuntersSpear",
                "item_hunters_spear",
                "Hunter's Thorn",
                "SpearElderbark",
                ApplySecondary(
                    new[]
                    {
                        P(pierce: 44),
                        P(pierce: 51, frost: 6, poison: 7),
                        P(pierce: 67, frost: 8, poison: 9),
                        P(pierce: 78, frost: 13, poison: 13),
                        P(pierce: 83, frost: 20, poison: 21),
                        P(pierce: 94, frost: 25, poison: 25),
                        P(pierce: 111, frost: 37, poison: 37),
                    },
                    durability: new[] { 150f, 150f, 150f, 150f, 300f, 350f, 450f },
                    stamina: new[] { 8f, 10f, 12f, 14f, 16f, 16f, 16f },
                    knockback: Repeat(20f),
                    block: new[] { 13f, 22f, 31f, 40f, 49f, 58f, 66f }),
                new[] { 6, 13, 13, 13, 13, 8, 13 }),
            new WeaponDefinition(
                "HuntersCleaver",
                "item_hunters_cleaver",
                "Hunter's Bite",
                "AxeIron",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 48, chop: 45),
                        P(slash: 54, chop: 55, fire: 7, poison: 7),
                        P(slash: 70, chop: 60, fire: 9, poison: 9),
                        P(slash: 81, chop: 65, fire: 13, poison: 14),
                        P(slash: 86, chop: 75, fire: 21, poison: 21),
                        P(slash: 99, chop: 85, fire: 26, poison: 27),
                        P(slash: 115, chop: 95, fire: 38, poison: 38),
                    },
                    durability: new[] { 225f, 225f, 225f, 225f, 300f, 375f, 450f },
                    stamina: new[] { 8f, 10f, 12f, 14f, 16f, 16f, 16f },
                    knockback: new[] { 50f, 50f, 60f, 60f, 60f, 60f, 60f },
                    block: new[] { 13f, 22f, 31f, 40f, 49f, 58f, 66f }),
                new[] { 10, 25, 25, 25, 13, 25, 25 }),
            new WeaponDefinition(
                "HuntersFury",
                "item_hunters_fury",
                "Hunter's Fury",
                "AxeBerzerkr",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 48, chop: 45),
                        P(slash: 54, chop: 55, fire: 7, poison: 7),
                        P(slash: 70, chop: 60, fire: 9, poison: 9),
                        P(slash: 81, chop: 65, fire: 13, poison: 14),
                        P(slash: 86, chop: 75, fire: 21, poison: 21),
                        P(slash: 99, chop: 85, fire: 26, poison: 27),
                        P(slash: 115, chop: 95, fire: 38, poison: 38),
                    },
                    durability: new[] { 225f, 225f, 225f, 225f, 300f, 375f, 450f },
                    stamina: new[] { 8f, 10f, 12f, 14f, 16f, 16f, 16f },
                    knockback: Repeat(20f),
                    block: new[] { 13f, 22f, 31f, 40f, 49f, 58f, 66f }),
                new[] { 0, 50, 50, 50, 26, 50, 50 },
                "HuntersCleaver"),
            new WeaponDefinition(
                "HuntersJaw",
                "item_hunters_jaw",
                "Hunter's Jaw",
                "KnifeSkollAndHati",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 16, pierce: 17),
                        P(slash: 17, pierce: 17, lightning: 4, poison: 5),
                        P(slash: 21, pierce: 21, lightning: 5, poison: 6),
                        P(slash: 26, pierce: 27, lightning: 9, poison: 9),
                        P(slash: 31, pierce: 31, lightning: 15, poison: 16),
                        P(slash: 42, pierce: 42, lightning: 22, poison: 23),
                        P(slash: 49, pierce: 50, lightning: 33, poison: 33),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 6f, 8f, 10f, 12f, 14f, 14f, 14f },
                    knockback: Repeat(10f),
                    block: new[] { 3f, 3f, 3f, 3f, 25f, 25f, 25f }),
                new[] { 0, 20, 26, 26, 26, 26, 26 },
                "HuntersFang"),
            new WeaponDefinition(
                "HuntersOath",
                "item_hunters_oath",
                "Hunter's Oath",
                "THSwordKrom",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 59),
                        P(slash: 63, fire: 8, spirit: 8),
                        P(slash: 103, fire: 13, spirit: 13),
                        P(slash: 108, fire: 18, spirit: 18),
                        P(slash: 107, fire: 26, spirit: 26),
                        P(slash: 116, fire: 31, spirit: 32),
                        P(slash: 135, fire: 45, spirit: 45),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 16f, 16f, 18f, 20f, 20f, 20f, 20f },
                    knockback: Repeat(55f),
                    block: new[] { 29f, 29f, 41f, 53f, 53f, 65f, 76f }),
                new[] { 15, 38, 63, 38, 19, 38, 38 }),
            new WeaponDefinition(
                "HuntersJudgment",
                "item_hunters_judgment",
                "Hunter's Judgment",
                "SledgeIron",
                ApplySecondary(
                    new[]
                    {
                        P(blunt: 34),
                        P(blunt: 51, lightning: 6, spirit: 7),
                        P(blunt: 67, lightning: 8, spirit: 9),
                        P(blunt: 78, lightning: 13, spirit: 13),
                        P(blunt: 103, lightning: 25, spirit: 26),
                        P(blunt: 128, lightning: 34, spirit: 35),
                        P(blunt: 144, lightning: 48, spirit: 48),
                    },
                    durability: new[] { 150f, 150f, 150f, 150f, 300f, 350f, 450f },
                    stamina: new[] { 12f, 20f, 22f, 24f, 28f, 28f, 28f },
                    knockback: new[] { 150f, 200f, 200f, 200f, 210f, 210f, 210f },
                    block: new[] { 5f, 29f, 29f, 29f, 53f, 53f, 52f }),
                new[] { 15, 38, 50, 38, 19, 38, 38 }),
            new WeaponDefinition(
                "HuntersRuin",
                "item_hunters_ruin",
                "Hunter's Ruin",
                "Battleaxe",
                ApplySecondary(
                    new[]
                    {
                        P(slash: 59, chop: 44),
                        P(slash: 63, chop: 44, fire: 8, frost: 8),
                        P(slash: 79, chop: 54, fire: 10, frost: 10),
                        P(slash: 89, chop: 64, fire: 15, frost: 15),
                        P(slash: 93, chop: 74, fire: 23, frost: 23),
                        P(slash: 111, chop: 74, fire: 30, frost: 30),
                        P(slash: 122, chop: 74, fire: 40, frost: 41),
                    },
                    durability: new[] { 250f, 250f, 250f, 250f, 300f, 350f, 450f },
                    stamina: new[] { 16f, 16f, 18f, 20f, 22f, 22f, 22f },
                    knockback: Repeat(70f),
                    block: new[] { 29f, 29f, 41f, 53f, 65f, 65f, 76f }),
                new[] { 15, 44, 50, 38, 19, 38, 38 }),
        };
    }

    private static float[] Repeat(float value)
    {
        return new[] { value, value, value, value, value, value, value };
    }

    private static CombatProfile[] ApplySecondary(
        CombatProfile[] profiles,
        float[] durability,
        float[] stamina,
        float[] knockback,
        float[] block,
        float[]? drawStamina = null)
    {
        CombatProfile[] result = new CombatProfile[profiles.Length];
        for (int i = 0; i < profiles.Length; i++)
        {
            result[i] = profiles[i].WithSecondary(
                durability[i],
                stamina[i],
                knockback[i],
                block[i],
                drawStamina?[i] ?? 0f);
        }

        return result;
    }

    private static CombatProfile P(
        float slash = 0,
        float blunt = 0,
        float pierce = 0,
        float chop = 0,
        float fire = 0,
        float frost = 0,
        float lightning = 0,
        float poison = 0,
        float spirit = 0,
        float blockPower = 0)
    {
        return new CombatProfile(
            slash: slash,
            blunt: blunt,
            pierce: pierce,
            chop: chop,
            fire: fire,
            frost: frost,
            lightning: lightning,
            poison: poison,
            spirit: spirit,
            blockPower: blockPower);
    }
}
