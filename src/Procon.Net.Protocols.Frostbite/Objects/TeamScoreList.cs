using System;
using System.Collections.Generic;
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
