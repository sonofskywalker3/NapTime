# Nap Time

A [Stardew Valley](https://www.stardewvalley.net/) mod that lets you nap in bed to regain energy without ending the day.

## What It Does

In vanilla Stardew Valley, if you pass out in the mines or stay up too late, you wake up the next morning with reduced energy and there's no free way to get it back — your only option is to spend your money, spend time looking for forageables, or push through the day at half stamina. This can really suck in the early game.

**Nap Time** adds a third option to the bed interaction dialogue: instead of just "Go to bed" or "Cancel", you can choose to **nap until a calculated wake-up time** and regain your missing energy. The game fades out, advances time, restores your stamina, and fades back in. No waiting around, no ending the day early — just a quick nap and back to work.

### How the Nap Works

- When you interact with your bed while missing energy, a **Nap** option appears alongside the usual "Go to bed" and "Cancel"
- The nap duration is calculated from how much energy you're missing (configurable)
- The dialogue shows exactly when you'll wake up and how much energy you'll regain
- If the calculated wake-up time exceeds your configured limit, the nap is capped and you get partial energy recovery
- Works any time of day — morning after passing out, midday after heavy mining, whenever you need it

### Example

You passed out in the mines and wake up at 6:00 AM with 135/270 energy. You walk to your bed and see:

> **Nap until 8:20 AM (regain full energy)**
> Go to bed for the night
> Cancel

You select "Nap", the screen fades out and back in, and it's 8:20 AM with full energy. Time to get back to work.

## Install

1. Install [SMAPI](https://smapi.io/)
2. Install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional, for in-game settings — Android users may prefer the [controller-enabled fork](https://github.com/sonofskywalker3/GenericModConfigMenu/releases))
3. Drop the `NapTime` folder into your `Mods` directory
4. Launch the game through SMAPI

## Configuration

All settings are configurable via [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (or the [controller-enabled fork](https://github.com/sonofskywalker3/GenericModConfigMenu/releases)) or by editing `config.json`:

| Setting | Default | Description |
|---------|---------|-------------|
| **Enable Napping** | `true` | Show the nap option when interacting with a bed while missing energy |
| **Minutes Per Stamina** | `1.0` | In-game minutes of nap per stamina point restored. Higher = slower recovery |
| **Latest Wake-Up Time** | `1800` (6:00 PM) | Latest time a nap can extend to (24h format). Nap is capped at this time with partial energy recovery |

### Tuning Guide

The default rate of **1.0 minute per stamina point** means:

| Scenario | Missing Energy | Nap Duration | Wake Up At |
|----------|---------------|-------------|------------|
| Stayed up a bit late | ~70 stamina | ~1 hour 10 min | 7:10 AM |
| Passed out / exhausted | ~135 stamina | ~2 hours 20 min | 8:20 AM |
| Worst case (nearly zero) | ~270 stamina | ~4 hours 30 min | 10:30 AM |

Want faster recovery? Set Minutes Per Stamina to `0.5` (half the nap time). Want it more punishing? Set it to `2.0`.

## Translations

Nap Time supports [SMAPI's i18n system](https://stardewvalleywiki.com/Modding:Translations). All player-facing text (dialogue options and config menu labels) can be translated.

To add a translation, create a file in the `i18n` folder named with the [language code](https://stardewvalleywiki.com/Modding:Translations#Language_codes) (e.g. `fr.json`, `es.json`, `pt-BR.json`) and translate the values from `default.json`. Pull requests welcome!

## Compatibility

- Stardew Valley 1.6+
- SMAPI 4.0+
- Works on PC and Android
- Compatible with other mods (uses Harmony patches on bed interaction only)

## License

MIT
