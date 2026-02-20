namespace NapTime
{
    public class ModConfig
    {
        /// <summary>Whether the nap option is enabled.</summary>
        public bool EnableNapping { get; set; } = true;

        /// <summary>
        /// How many in-game minutes of nap time per stamina point restored.
        /// Default: 1 minute per 2 stamina (so 135 missing stamina = ~68 minutes ≈ 1 hour).
        /// Lower = faster recovery, higher = slower.
        /// </summary>
        public float MinutesPerStamina { get; set; } = 0.5f;

        /// <summary>
        /// Maximum time (in-game hour, 24h format) the nap can extend to.
        /// If the calculated wake-up time exceeds this, the nap is capped here
        /// and stamina is only partially restored.
        /// Default: 1800 (6:00 PM).
        /// </summary>
        public int MaxWakeUpTime { get; set; } = 1800;
    }
}
