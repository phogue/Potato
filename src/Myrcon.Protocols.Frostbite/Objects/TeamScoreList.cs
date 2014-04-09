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
using System.Collections.Generic;
using System.Globalization;

namespace Myrcon.Protocols.Frostbite.Objects {

    [Serializable]
    public class TeamScoreList : List<TeamScore> {

        public int WinningScore { get; set; }

        public TeamScoreList Parse(List<string> words) {

            int totalScores = 0;
            float score = 0;
            int winningScore = 0;

            if (words.Count >= 1 && int.TryParse(words[0], out totalScores) == true && words.Count >= totalScores + 1) {

                int.TryParse(words[words.Count - 1], out winningScore);
                this.WinningScore = winningScore;

                for (int i = 0; i < totalScores; i++) {

                    if (float.TryParse(words[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out score) == true) {
                        this.Add(new TeamScore() { TeamID = i + 1, Score = Convert.ToInt32(score) });
                    }
                }
            }

            return this;
        }

    }
}
