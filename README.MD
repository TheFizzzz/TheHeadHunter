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

The tier definitions and archetype-specific premium costs live in `JotunnModStub/JotunnModStub.cs`; metal costs are configurable through the BepInEx config file.

## Development

The project targets .NET Framework 4.8 and Jötunn 2.30.0.

1. Install BepInEx and Jötunn in an r2modman profile.
2. Create `Environment.props` with `VALHEIM_INSTALL`, `BEPINEX_PATH`, and `MOD_DEPLOYPATH`.
3. Build `JotunnModStub.sln` in Debug configuration.
4. Launch the same r2modman profile and inspect `BepInEx/LogOutput.log`.

Debug builds deploy to `MOD_DEPLOYPATH/HuntersArsenal`. See the [Jötunn developer quickstart](https://valheim-modding.github.io/Jotunn/guides/quickstart.html) for general setup.

## Roadmap

- Add Mistlands, Ashlands, and 1.0 endgame tiers after confirming their intended recipes.
- Replace the temporary iron sword model with a custom asset.
