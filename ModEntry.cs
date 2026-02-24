using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NapTime
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config;
        internal static IMonitor ModMonitor;
        internal static ITranslationHelper I18n;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ModMonitor = Monitor;
            I18n = helper.Translation;

            var harmony = new Harmony(ModManifest.UniqueID);
            SleepPatches.Apply(harmony, Monitor);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableNapping,
                setValue: value => Config.EnableNapping = value,
                name: () => I18n.Get("config.enable-napping.name"),
                tooltip: () => I18n.Get("config.enable-napping.tooltip")
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MinutesPerStamina,
                setValue: value => Config.MinutesPerStamina = value,
                name: () => I18n.Get("config.minutes-per-stamina.name"),
                tooltip: () => I18n.Get("config.minutes-per-stamina.tooltip"),
                min: 0.1f,
                max: 5.0f,
                interval: 0.1f
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MaxWakeUpTime,
                setValue: value => Config.MaxWakeUpTime = value,
                name: () => I18n.Get("config.max-wake-up-time.name"),
                tooltip: () => I18n.Get("config.max-wake-up-time.tooltip"),
                min: 700,
                max: 2400,
                interval: 100
            );
        }
    }

    /// <summary>GMCM API interface.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
    }
}
