# Hunter's Arsenal

An arsenal that follows your progression through Valheim and rewards dedicated trophy hunters.

## Installation (manual)

Install BepInEx and Jötunn, then place `HuntersArsenal.dll` in `BepInEx/plugins/HuntersArsenal`.

## Features

- Adds one- and two-handed swords, hammers, axes, atgeirs, bows, shields, daggers, spears, and dual weapons.
- Uses corresponding iron or closest available vanilla models while custom art is in development.
- Progresses through seven biome quality upgrades using the previous boss's trophy and the current biome's material.
- Archetype-specific premium metal costs are configurable and synchronized by Jötunn.
- Grants a configurable second trophy roll when a creature's normal trophy roll fails.
- Gives every weapon family a unique two-element profile from the Swamp onward.
- Marks prey for stronger trophy rolls when Hunter's Guard parries or Hunter's Bulwark blocks.
- Combines two equal-quality Hunter's Bite axes or two Hunter's Fang daggers into Hunter's Fury or Hunter's Jaw at matching quality.

### Progression

- Black Forest: Eikthyr trophy + bronze
- Swamp upgrade: Elder trophy + iron
- Mountains upgrade: Bonemass trophy + silver
- Plains upgrade: Moder trophy + black metal
- Mistlands upgrade: Yagluth trophy + refined eitr at Black Forge level 1
- Ashlands upgrade: Queen trophy + flametal at Black Forge level 3
- Deep North upgrade: Fader trophy + bloodgold at Black Forge level 4

## Changelog

### 0.2.0

- Added explicit Q1–Q7 physical and elemental combat profiles.
- Added Mistlands, Ashlands, and Deep North progression.
- Added regional quality-2 stamina, block, durability, and knockback values.
- Added configurable 6-second Guard and Bulwark hunting marks.
- Added the `hunters_testkit <quality>` field-test command.
- Added quality-aware crafting stations and station levels.

### 0.1.0

- Added fourteen items with Black Forest through Plains quality upgrades.
- Added quality-preserving dual-axe and dual-dagger recipes.
- Rebalanced metal costs by weapon archetype.
- Added a configurable 6% bonus trophy roll for Hunter's Arsenal killing blows.

## Known issues

- Bow killing-blow attribution uses the weapon equipped when prey dies; switching weapons while an arrow is in flight can misattribute the bonus.
- Elemental visual effects still come from the temporary cloned model, so damage behavior may not have matching custom VFX.

## Field testing and debugging

Enable Valheim's console with the `-console` launch argument, press F5, run `devcommands`, then use
`hunters_testkit 1` through `hunters_testkit 7`. Fury and Jaw intentionally begin at quality 2.

Compare each Arsenal quality against both quality-2 and quality-4 vanilla gear. Keep weapon skill,
enemy level, attack sequence, stagger state, and ammunition fixed.

Debug builds deploy to `MOD_DEPLOYPATH/HuntersArsenal`. In `BepInEx/LogOutput.log`:

1. Confirm `Loaded Hunter's Arsenal 0.2.0`.
2. Confirm `Registered 14 Hunter's Arsenal weapons with 7 quality levels each`.
3. Search missing-item failures for `Failed to clone prefab`.
4. Search Harmony failures for `Undefined target method` or `No target method`.
5. If damage looks linear or matches the cloned iron model, delete duplicate `HuntersArsenal.dll` copies.
6. If `hunters_testkit` is unknown, run `devcommands` and confirm Jötunn loaded first.
7. Tooltip damage should change at each quality; stamina and block should match the regional vanilla quality-2 weapon, not the cloned iron model.

Useful vanilla spawn comparisons: `spawn SwordIron 1 2`, `spawn SwordIron 1 4`, `spawn SwordSilver 1 2`.

Source: https://github.com/TheFizzzz/TheHeadHunter
