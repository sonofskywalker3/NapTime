# Nap Time

[**⬇ Download on Nexus Mods**](https://www.nexusmods.com/stardewvalley/mods/42616)

**Nap in your bed to regain energy without ending the day.**
A quick, free way to recover stamina after passing out in the mines or staying up too late — instead of pushing through the day at half energy. PC and Android.

> This README mirrors the [Nexus Mods description](https://www.nexusmods.com/stardewvalley/mods/42616); the two are kept identical.

## What's New in v1.0.1 — Translations

**Nap Time now supports SMAPI's translation system.** All player-facing text (the bed dialogue options and the config labels) can be localized — drop a language file in the `i18n` folder and it's picked up automatically. Translation pull requests welcome.

## The problem

In vanilla Stardew Valley, if you pass out in the mines or stay up too late, you wake up with reduced energy and no free way to get it back — you either spend money, hunt forageables, or push through the day at half stamina. Early game, that really stings.

## The solution

Nap Time adds a third option to the bed dialogue: **take a nap**. The game fades out, advances time, restores your stamina, and fades back in. No ending the day early — just a quick nap and back to work.

## How it works

When you interact with your bed while missing energy, a **Nap** option appears alongside "Go to bed" and "Cancel":

> **Nap until 8:20 AM (regain full energy)**
> Go to bed for the night
> Cancel

- Nap duration is calculated from how much energy you're missing.
- The dialogue tells you exactly when you'll wake up and how much you'll recover.
- If the wake-up time would exceed your configured limit, the nap is capped (partial recovery).
- Works any time of day — after passing out, after heavy mining, whenever.
- If you're already at full energy, the vanilla dialogue appears as normal.

At the default 1.0 in-game minute per stamina point: ~70 missing stamina → wake 7:10 AM; ~135 → 8:20 AM; ~270 → 10:30 AM.

## Requirements

- Stardew Valley 1.6+
- [SMAPI](https://smapi.io/) 4.0+
- (Optional) [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game settings (Android players: the [controller-enabled fork](https://github.com/sonofskywalker3/GenericModConfigMenu/releases))

Works on **PC and Android**; compatible with other mods (only patches bed interaction).

## Install

1. Install [SMAPI](https://smapi.io/).
2. Download this mod and unzip it into your `Stardew Valley/Mods` folder (so you have `Mods/NapTime/`).
3. Run the game with SMAPI.

## Configuration

Settings live in `config.json` (created on first run) and can also be edited via GMCM:

- **Enable Napping** (default on) — show the nap option when interacting with a bed while missing energy.
- **Minutes Per Stamina** (default 1.0, range 0.1–5.0) — in-game minutes of nap per stamina point. Lower = faster recovery, higher = more punishing.
- **Latest Wake-Up Time** (default 6:00 PM, range 7:00 AM–midnight) — latest a nap can extend to; beyond it the nap is capped with partial recovery.

## Also by this author

- [**Android Consolizer**](https://www.nexusmods.com/stardewvalley/mods/41869) — Full console-style controller support for Stardew Valley on Android. Play the whole game on a gamepad.
- [**Cart Catalog**](https://www.nexusmods.com/stardewvalley/mods/47146) — Order from the Traveling Cart's daily stock; items arrive in a package on your porch the next morning.

## Source

Open source (MIT) — [github.com/sonofskywalker3/NapTime](https://github.com/sonofskywalker3/NapTime)
