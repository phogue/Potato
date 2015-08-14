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

namespace Potato.Fuzzy.Utils {
    public class MatchDictionaryKey : IComparable<MatchDictionaryKey> {
        public MatchDictionaryKey(string strMatchedText) {
            MatchedText = strMatchedText;
            MatchedScoreCharacters = 0;
            MatchedScore = int.MaxValue;
        }

        public int CompareTo(MatchDictionaryKey other) {
            return MatchedScore.CompareTo(other.MatchedScore);
        }

        public string MatchedText { get; private set; }

        public string LowerCaseMatchedText {
            get { return MatchedText.ToLower(); }
        }

        public int MatchedScoreCharacters { get; set; }

        public int MatchedScore { get; set; }
    }
}