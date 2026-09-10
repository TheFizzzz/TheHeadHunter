using System;
using Jotunn.Entities;
using UnityEngine;

namespace TheHeadHunter;

internal sealed class ArsenalTestKitCommand : ConsoleCommand
{
    public override string Name => "hunters_testkit";
    public override string Help =>
        "hunters_testkit <quality 1-7> — add every available Hunter's Arsenal item at that quality";
    public override bool IsCheat => true;

    public override void Run(string[] args, Terminal context)
    {
        if (args.Length != 1 ||
            !int.TryParse(args[0], out int quality) ||
            quality < 1 ||
            quality > TheHeadHunterPlugin.MaxQuality)
        {
            context.AddString($"Usage: {Name} <quality 1-{TheHeadHunterPlugin.MaxQuality}>");
            return;
        }

        Player? player = Player.m_localPlayer;
        if (player is null)
        {
            context.AddString("A local player is required.");
            return;
        }

        int added = 0;
        foreach (WeaponDefinition weapon in TheHeadHunterPlugin.GetWeapons())
        {
            if (weapon.IsCombination && quality == 1)
            {
                continue;
            }

            ItemDrop.ItemData? item = player.GetInventory().AddItem(
                weapon.PrefabName,
                1,
                quality,
                0,
                player.GetPlayerID(),
                player.GetPlayerName(),
                new Vector2i(-1, -1),
                false,
                false,
                false);
            if (item is not null)
            {
                added++;
            }
        }

        context.AddString(
            $"Added {added} Hunter's Arsenal items at quality {quality}. " +
            "Fury and Jaw are unavailable at quality 1.");
    }
}
