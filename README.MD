# Hunter's Arsenal

A Valheim weapon family that increases trophy-hunting rewards. It grants a configurable second trophy roll when the normal roll fails.

## Weapons

- Hunter's Edge — sword using the iron sword model
- Hunter's Knell — mace using the iron mace model
- Hunter's Reach — atgeir using the iron atgeir model
- Hunter's Gaze — bow using the Huntsman bow model
- Hunter's Guard — round shield using the banded shield model
- Hunter's Bulwark — tower shield using the iron tower shield model
- Hunter's Fang — dagger using the abyssal razor model
- Hunter's Thorn — spear using the ancient-bark spear model
- Hunter's Bite — axe using the iron axe model
- Hunter's Fury — dual axes made from two equal-quality Hunter's Bites
- Hunter's Jaw — dual daggers made from two equal-quality copies of the single dagger, Hunter's Fang
- Hunter's Oath — two-handed sword using Krom's model
- Hunter's Judgment — two-handed hammer using the iron sledge model
- Hunter's Ruin — two-handed axe using the battleaxe model

## Progression

Each weapon is forged once and then upgraded as one persistent item:

- Quality 1: Eikthyr trophy + bronze → Black Forest strength
- Quality 2: Elder trophy + iron → Swamp strength
- Quality 3: Bonemass trophy + silver → Mountains strength
- Quality 4: Moder trophy + black metal → Plains strength
- Quality 5: Yagluth trophy + refined eitr → Mistlands strength
- Quality 6: Queen trophy + flametal → Ashlands strength
- Quality 7: Fader trophy + bloodgold → Deep North strength

Qualities 1–4 use the Forge. Qualities 5–7 use the Black Forge at levels 1, 3, and 4.
The tier definitions and configurable archetype-specific premium costs live in the plugin source.

## Combat and trophy behavior

- Black Forest profiles are purely physical.
- Two signature elements begin in the Swamp and grow from 20% to 40% of listed damage by the Deep North.
- Raw damage targets the midpoint between representative vanilla quality-2 and quality-3 gear.
- Weapon stamina, block, durability, and knockback match the regional vanilla quality-2 item. Shield block uses the dedicated hunting-shield table.
- An Arsenal killing blow gets a configurable 6% second trophy roll after the vanilla roll fails.
- Hunter's Guard parries mark prey for a configurable 11% second roll.
- Hunter's Bulwark blocks mark prey for a configurable 5% second roll.
- Marks last 6 seconds by default; only the highest eligible second-roll chance applies.

## Development

The project targets .NET Framework 4.8 and Jötunn 2.30.0.

1. Install BepInEx and Jötunn in an r2modman profile.
2. Create `Environment.props` with `VALHEIM_INSTALL`, `BEPINEX_PATH`, and `MOD_DEPLOYPATH`.
3. Build `JotunnModStub.sln` in Debug configuration.
4. Launch the same r2modman profile and inspect `BepInEx/LogOutput.log`.

Debug builds deploy to `MOD_DEPLOYPATH/HuntersArsenal`. See the [Jötunn developer quickstart](https://valheim-modding.github.io/Jotunn/guides/quickstart.html) for general setup.

### Field-test commands

Enable the console with Valheim's `-console` launch argument, press F5, and enter `devcommands`.

- `hunters_testkit 1` through `hunters_testkit 7` grants every available Arsenal item at that quality.
- Fury and Jaw intentionally begin at quality 2.
- Vanilla comparison items can still be created with `spawn <prefab> 1 <quality>`.

For repeatable TTK tests, keep weapon skill, enemy level, attack sequence, stagger state, and ammunition constant.
Compare each Arsenal tier with both quality-2 and quality-4 vanilla gear.

### Debugging checklist

1. Confirm the log contains `Loaded Hunter's Arsenal 0.2.0`.
2. Confirm it reports 14 registered items and 7 quality levels.
3. If an item is missing, search the log for `Failed to clone prefab`.
4. If startup stops during Harmony initialization, search for `Undefined target method` or `No target method`.
5. If stats look linear or retain model damage, verify only one `HuntersArsenal.dll` exists in the profile.
6. If the test command is unavailable, run `devcommands` first and verify Jötunn loaded before Hunter's Arsenal.
7. Multiplayer tests require every player to use the same mod version and configuration.

The current models remain temporary vanilla placeholders.
