using System;

namespace Procon.Net.Data {

    /// <summary>
    /// The current, maximum and minimum settings 
    /// </summary>
    [Serializable]
    public sealed class Settings {

        /// <summary>
        /// Stores all the current information (like the player count for right now)
        /// </summary>
        public SettingsData Current { get; set; }

        /// <summary>
        /// Stores the minimums of all the information (like the minimum number of players before starting a round)
        /// </summary>
        public SettingsData Minimum { get; set; }

        /// <summary>
        /// Stores the maximums of all the information (like the maximum number of players allowed)
        /// </summary>
        public SettingsData Maximum { get; set; }

        public Settings() {
            this.Current = new SettingsData();
            this.Minimum = new SettingsData();
            this.Maximum = new SettingsData();
        }
    }
}