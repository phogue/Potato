// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Procon.Net.Protocols.Frostbite.Objects {

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
