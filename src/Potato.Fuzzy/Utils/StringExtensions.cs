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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Potato.Fuzzy.Utils {
    public static class StringExtensions {
        public static string RemoveDiacritics(this string s) {
            var normalized = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)) {
                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Helper method to determine if a levenshtein lookup is necessary, or if we should just trash
        /// the lookup. This is an optimization which pretty much cut the unit tests in half.
        /// </summary>
        /// <param name="s">The first string</param>
        /// <param name="t">THe second string to compare it against</param>
        /// <returns>True if the strings are vagely similar lengths, false if they are no where near.</returns>
        public static bool HaltStringSimularity(string s, string t) {
            // Note, this might be changed to 0.3F. It's 
            // This function isn't supposed to do any filtering, but instead just prevent a check
            // that couldn't possibly yield over a 50% match.
            return (s == null || t == null || Math.Min(s.Length, t.Length) < Math.Max(s.Length, t.Length) * 0.4F);
        }

        public static float Sift3Distance(this string s1, string s2, int maxOffset) {
            if (string.IsNullOrEmpty(s1)) {
                return string.IsNullOrEmpty(s2) ? 0 : s2.Length;
            }
            if (string.IsNullOrEmpty(s2)) {
                return s1.Length;
            }
            var c = 0;
            var offset1 = 0;
            var offset2 = 0;
            var lcs = 0;
            while ((c + offset1 < s1.Length) && (c + offset2 < s2.Length)) {
                if (s1[c + offset1] == s2[c + offset2])
                    lcs++;
                else {
                    offset1 = 0;
                    offset2 = 0;
                    for (var i = 0; i < maxOffset; i++) {
                        if ((c + i < s1.Length) && (s1[c + i] == s2[c])) {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length) && (s1[c] == s2[c + i])) {
                            offset2 = i;
                            break;
                        }
                    }
                }
                c++;
            }
            return (s1.Length + s2.Length) / 2F - lcs;
        }

        /// <summary>
        /// Compute Levenshtein distance
        /// Single Dimensional array vector version
        /// Memory efficient version
        /// Sten Hjelmqvist
        /// http://www.codeproject.com/cs/algorithms/Levenshtein.asp
        /// </summary>
        /// <returns>Levenshtein edit distance</returns>
        public static int Levenshtein(this string s, string t) {
            var n = s.Length; // length of s
            var m = t.Length; // length of t
            int cost; // cost

            // Step 1
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Create the two vectors
            var v0 = new int[n + 1];
            var v1 = new int[n + 1];
            int[] vTmp;


            // Step 2
            // Initialize the first vector
            for (var i = 1; i <= n; i++)
                v0[i] = i;


            // Step 3
            // Fore each column
            for (var j = 1; j <= m; j++) {
                // Set the 0'th element to the column number
                v1[0] = j;

                // Step 4
                // Fore each row
                for (var i = 1; i <= n; i++) {
                    // Step 5
                    cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    // Step 6
                    // Find minimum
                    var m_min = v0[i] + 1;
                    var b = v1[i - 1] + 1;
                    var c = v0[i - 1] + cost;

                    if (b < m_min)
                        m_min = b;
                    if (c < m_min)
                        m_min = c;

                    v1[i] = m_min;
                }

                // Swap the vectors
                vTmp = v0;
                v0 = v1;
                v1 = vTmp;
            }

            // Step 7
            return v0[n];
        }

        // Mostly an english hack - will enhance
        public static int DePluralStringSimularity(this string s, string t) {
            if (HaltStringSimularity(s, t) == true) {
                return 0;
            }

            int returnRatio = s.StringSimularitySubsetBonusRatio(t);

            if (t.Length > 0 && t[t.Length - 1] == 's') {
                var newT = t.Remove(t.Length - 1);

                if (newT.Length > 0 && newT[newT.Length - 1] == '\'') {
                    newT = newT.Remove(newT.Length - 1);
                }

                int newTRatio = s.StringSimularitySubsetBonusRatio(newT);

                if (newTRatio > returnRatio) {
                    returnRatio = newTRatio;
                }
            }

            return returnRatio;
        }

        public static int StringSimularityRatio(this string s, string t) {
            if (HaltStringSimularity(s, t) == true) {
                return 0;
            }

            double levenshtein = s.ToLower().Sift3Distance(t.ToLower(), 5);

            return (int) Math.Round((1.0D - levenshtein / Math.Max(s.Length, t.Length)) * 100.0D);
        }

        public static int StringSimularitySubsetBonusRatio(this string s, string t) {
            if (HaltStringSimularity(s, t) == true) {
                return 0;
            }

            double levenshtein = s.ToLower().Sift3Distance(t.ToLower(), 5);

            // Direct subsets earn a bonus towards the final score.
            if (levenshtein > 0.0D && s.Length != t.Length && s.ToLower().IndexOf(t.ToLower(), StringComparison.Ordinal) >= 0) {
                levenshtein = Math.Max(0.1D, levenshtein - Math.Min(s.Length, t.Length) / 2.0D);
            }

            return (int) Math.Round((1.0D - levenshtein / Math.Max(s.Length, t.Length)) * 100.0D);
        }

        public static int GetClosestMatch(this string input, List<string> dictionary, out string matchedDictionaryKey) {
            var similarity = int.MaxValue;

            matchedDictionaryKey = string.Empty;

            if (dictionary.Count >= 1) {
                var iLargestDictionaryKey = 0;

                // Build array of default matches from the dictionary to store a rank for each match.
                // (it's designed to work on smaller dictionaries with say.. 32 player names in it =)
                var matches = new List<MatchDictionaryKey>();

                foreach (var strDictionaryKey in dictionary) {
                    matches.Add(new MatchDictionaryKey(strDictionaryKey));

                    if (strDictionaryKey.Length > iLargestDictionaryKey) {
                        iLargestDictionaryKey = strDictionaryKey.Length;
                    }
                }

                // Rank each match, find the remaining characters for a match (arguements)
                for (var x = 1; x <= Math.Min(input.Length, iLargestDictionaryKey); x++) {
                    // Skip it if it's a space (a space breaks a name and moves onto arguement.
                    // but the space could also be included in the dictionarykey, which will be checked
                    // on the next loop.
                    if (x + 1 < input.Length && input[x] != ' ')
                        continue;

                    foreach (var match in matches) {
                        var score = (int) input.Substring(0, x).ToLower().Sift3Distance(match.LowerCaseMatchedText, match.LowerCaseMatchedText.Length);

                        if (score < match.MatchedScore || (score == match.MatchedScore && x > match.MatchedScoreCharacters)) {
                            match.MatchedScore = score;
                            match.MatchedScoreCharacters = x;
                        }
                    }
                }

                // Sort the matches
                matches.Sort();

                var bestCharactersMatched = matches[0].MatchedScoreCharacters;
                similarity = matches[0].MatchedScore;
                matchedDictionaryKey = matches[0].MatchedText;

                // Now though we want to loop through from start to end and see if a subset of what we entered is found.
                // if so then this will find the highest ranked item with a subset of what was entered and favour that instead.
                var bestCharsSubstringLower = input.Substring(0, bestCharactersMatched).ToLower();

                foreach (var match in matches) {
                    if (match.LowerCaseMatchedText.Contains(bestCharsSubstringLower) == true) {
                        similarity = match.MatchedScore;
                        matchedDictionaryKey = match.MatchedText;
                        bestCharactersMatched = match.MatchedScoreCharacters;

                        break;
                    }
                }

                if (bestCharactersMatched < input.Length) {
                    similarity = int.MaxValue;
                }
            }

            return similarity;
        }

        public static List<string> Wordify(this string input) {
            var returnList = new List<string>();

            var fullWord = string.Empty;
            var quoteStack = 0;
            var isEscaped = false;

            var punctuation = new[] {'(', ')', ',', '.', '?', '!', '/', '*', '-', '+', ':'};

            foreach (var c in input) {
                if (c == ' ') {
                    if (quoteStack == 0) {
                        if (fullWord.Length > 0) {
                            returnList.Add(fullWord);
                            fullWord = string.Empty;
                        }
                    }
                    else {
                        fullWord += ' ';
                    }
                }
                else if (punctuation.Contains(c)) {
                    if (quoteStack == 0) {
                        if (fullWord.Length > 0) {
                            returnList.Add(fullWord);
                            fullWord = string.Empty;
                        }

                        returnList.Add("" + c);
                    }
                    else {
                        fullWord += c;
                    }
                }
                else if (c == 'n' && isEscaped == true) {
                    fullWord += '\n';
                    isEscaped = false;
                }
                else if (c == 'r' && isEscaped == true) {
                    fullWord += '\r';
                    isEscaped = false;
                }
                else if (c == 't' && isEscaped == true) {
                    fullWord += '\t';
                    isEscaped = false;
                }
                else if (c == '"') {
                    if (isEscaped == false) {
                        if (quoteStack == 0) {
                            quoteStack++;
                        }
                        else {
                            quoteStack--;
                        }
                    }
                    //else {
                    fullWord += '"';
                    //}
                }
                else if (c == '\\') {
                    if (isEscaped == true) {
                        fullWord += '\\';
                        isEscaped = false;
                    }
                    else {
                        isEscaped = true;
                    }
                }
                else {
                    fullWord += c;
                    isEscaped = false;
                }
            }

            returnList.Add(fullWord);

            return returnList.Where(x => string.IsNullOrEmpty(x) == false).ToList();
        }
    }
}