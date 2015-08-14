#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;

namespace Potato.Net.Shared.Models {
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

        /// <summary>
        /// Initializes the various groups of settings
        /// </summary>
        public Settings() {
            Current = new SettingsData();
            Minimum = new SettingsData();
            Maximum = new SettingsData();
        }
    }
}