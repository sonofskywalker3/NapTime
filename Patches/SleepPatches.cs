using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace NapTime
{
    internal static class SleepPatches
    {
        private static IMonitor Monitor;

        /// <summary>Whether we're currently executing a nap (to prevent re-entrant dialogue).</summary>
        private static bool _napping;

        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;

            // Intercept the bed's "Sleep" touch action to inject our nap option
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "performTouchAction", new[] { typeof(string[]), typeof(Microsoft.Xna.Framework.Vector2) }),
                prefix: new HarmonyMethod(typeof(SleepPatches), nameof(PerformTouchAction_Prefix))
            );

            // Handle our custom dialogue responses
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "answerDialogueAction"),
                prefix: new HarmonyMethod(typeof(SleepPatches), nameof(AnswerDialogueAction_Prefix))
            );
        }

        /// <summary>
        /// Intercept the "Sleep" touch action. If the player is missing energy and napping is enabled,
        /// replace the vanilla yes/no dialogue with our three-option dialogue (Nap / Sleep / Cancel).
        /// </summary>
        private static bool PerformTouchAction_Prefix(GameLocation __instance, string[] action)
        {
            if (!ModEntry.Config.EnableNapping)
                return true; // let vanilla handle it

            if (action == null || action.Length == 0 || action[0] != "Sleep")
                return true;

            // Same preconditions as vanilla
            if (Game1.newDay || !Game1.shouldTimePass() || !Game1.player.hasMoved || Game1.player.passedOut)
                return true;

            float missing = Game1.player.MaxStamina - Game1.player.Stamina;
            if (missing <= 0)
                return true; // full energy, use vanilla dialogue

            // Calculate nap details
            int napMinutes = (int)Math.Ceiling(missing * ModEntry.Config.MinutesPerStamina);
            napMinutes = RoundUpToTen(napMinutes);
            if (napMinutes < 10)
                napMinutes = 10;

            int wakeUpTime = AddGameMinutes(Game1.timeOfDay, napMinutes);
            float staminaRecovered = missing;
            bool capped = false;

            // Cap at configured max wake-up time
            if (wakeUpTime > ModEntry.Config.MaxWakeUpTime)
            {
                wakeUpTime = ModEntry.Config.MaxWakeUpTime;
                int actualMinutes = GameMinutesBetween(Game1.timeOfDay, wakeUpTime);
                staminaRecovered = actualMinutes / ModEntry.Config.MinutesPerStamina;
                capped = true;
            }

            // Don't offer nap if wake-up time would be past 2AM (2600) or not meaningful
            if (wakeUpTime >= 2600 || wakeUpTime <= Game1.timeOfDay)
                return true; // vanilla dialogue only

            // Build nap label
            string timeStr = FormatGameTime(wakeUpTime);
            string napLabel;
            if (capped)
            {
                int pct = (int)Math.Round(staminaRecovered / Game1.player.MaxStamina * 100);
                napLabel = ModEntry.I18n.Get("nap-label.capped", new { time = timeStr, percent = pct });
            }
            else
            {
                napLabel = ModEntry.I18n.Get("nap-label.full", new { time = timeStr });
            }

            // Create custom dialogue with Nap + Go to Bed + Cancel
            var responses = new Response[]
            {
                new Response("Nap", napLabel),
                new Response("Yes", Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep")),
                new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
            };

            __instance.createQuestionDialogue(
                Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"),
                responses,
                "Sleep"
            );

            return false; // skip vanilla
        }

        /// <summary>
        /// Handle the "Sleep_Nap" response. Let vanilla handle "Sleep_Yes" and "Sleep_No".
        /// </summary>
        private static bool AnswerDialogueAction_Prefix(GameLocation __instance, string questionAndAnswer)
        {
            if (questionAndAnswer != "Sleep_Nap")
                return true;

            if (_napping)
                return false;

            DoNap();
            return false; // we handled it
        }

        private static void DoNap()
        {
            _napping = true;

            float missing = Game1.player.MaxStamina - Game1.player.Stamina;
            int napMinutes = (int)Math.Ceiling(missing * ModEntry.Config.MinutesPerStamina);
            napMinutes = RoundUpToTen(napMinutes);
            if (napMinutes < 10)
                napMinutes = 10;

            int wakeUpTime = AddGameMinutes(Game1.timeOfDay, napMinutes);
            float staminaToRestore = missing;

            if (wakeUpTime > ModEntry.Config.MaxWakeUpTime)
            {
                wakeUpTime = ModEntry.Config.MaxWakeUpTime;
                int actualMinutes = GameMinutesBetween(Game1.timeOfDay, wakeUpTime);
                staminaToRestore = actualMinutes / ModEntry.Config.MinutesPerStamina;
            }

            if (wakeUpTime >= 2600)
                wakeUpTime = 2590;

            int targetTime = wakeUpTime;
            float targetStamina = staminaToRestore;

            Monitor.Log($"Napping: {Game1.timeOfDay} -> {targetTime}, restoring {targetStamina:F0} stamina", LogLevel.Info);

            Game1.player.isInBed.Value = true;
            Game1.player.doEmote(24); // sleep emote

            Game1.globalFadeToBlack(delegate
            {
                // Advance time
                Game1.timeOfDay = targetTime;
                Game1.gameTimeInterval = 0;

                // Restore stamina
                Game1.player.Stamina = Math.Min(
                    Game1.player.Stamina + targetStamina,
                    Game1.player.MaxStamina
                );

                Game1.player.isInBed.Value = false;
                Game1.player.doEmote(20); // happy emote

                // Update outdoor lighting for new time of day
                Game1.outdoorLight = Game1.ambientLight;

                Monitor.Log($"Nap complete. Time: {Game1.timeOfDay}, Stamina: {Game1.player.Stamina:F0}/{Game1.player.MaxStamina}", LogLevel.Info);

                _napping = false;

                Game1.globalFadeToClear();
            }, 0.02f);
        }

        #region Time Helpers

        /// <summary>Round up to the nearest multiple of 10.</summary>
        private static int RoundUpToTen(int value)
        {
            return (int)Math.Ceiling(value / 10.0) * 10;
        }

        /// <summary>
        /// Add real minutes to a Stardew time value (where 630 = 6:30, 700 = 7:00, etc.).
        /// Stardew time: last two digits are 00-50 (in 10-min increments), first digits are hour.
        /// </summary>
        private static int AddGameMinutes(int startTime, int minutes)
        {
            int hour = startTime / 100;
            int min = startTime % 100;
            int totalMinutes = hour * 60 + min + minutes;
            int newHour = totalMinutes / 60;
            int newMin = totalMinutes % 60;
            // Stardew rounds to 10-minute increments
            newMin = (newMin / 10) * 10;
            return newHour * 100 + newMin;
        }

        /// <summary>Calculate real minutes between two Stardew time values.</summary>
        private static int GameMinutesBetween(int startTime, int endTime)
        {
            int startTotal = (startTime / 100) * 60 + (startTime % 100);
            int endTotal = (endTime / 100) * 60 + (endTime % 100);
            return Math.Max(0, endTotal - startTotal);
        }

        /// <summary>Format a Stardew time value as "H:MM AM/PM".</summary>
        private static string FormatGameTime(int time)
        {
            int hour = time / 100;
            int min = time % 100;

            // Handle past-midnight times (2500 = 1:00 AM next day)
            if (hour >= 24)
                hour -= 24;

            string ampm = hour >= 12 ? "PM" : "AM";
            int displayHour = hour % 12;
            if (displayHour == 0)
                displayHour = 12;

            return $"{displayHour}:{min:D2} {ampm}";
        }

        #endregion
    }
}
