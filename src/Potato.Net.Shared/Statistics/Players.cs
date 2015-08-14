#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Concurrent;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Statistics {
    /// <summary>
    /// Builds statistics information about a collection of players
    /// </summary>
    public class Players {
        /// <summary>
        /// Builds information and sets outliers on applicable players for the field
        /// </summary>
        /// <param name="players">The sample</param>
        /// <param name="field">The field within each item to find</param>
        /// <param name="value">How to find the field within each item</param>
        public static void PropertyOutliers(ConcurrentDictionary<string, PlayerModel> players, string field, Func<PlayerModel, float> value) {
            var mean = players.Values.Mean(value);

            var standardDeviation = players.Values.StdDev(value, mean);

            foreach (var player in players.Values) {
                var deviations = ((value(player) - mean) / standardDeviation);

                if (deviations >= 2.0f) {
                    player.Outliers.Add(new OutlierModel() {
                        Field = field,
                        Mean = mean,
                        StandardDeviation = standardDeviation,
                        Deviations = deviations,
                        Value = value(player)
                    });
                }
            }
        }

        /// <summary>
        /// Calculates the outlier information for each field in the collection of players.
        /// </summary>
        /// <param name="players">The list of players to find the outliers for</param>
        public static void Outliers(ConcurrentDictionary<string, PlayerModel> players) {
            foreach (var player in players.Values) {
                player.Outliers.Clear();
            }

            PropertyOutliers(players, "Kills", model => model.Kills);
            PropertyOutliers(players, "Deaths", model => model.Deaths);
            PropertyOutliers(players, "Kdr", model => model.Kdr);
            PropertyOutliers(players, "Score", model => model.Score);
            PropertyOutliers(players, "Ping", model => model.Ping);
        }
    }
}
