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

namespace Procon.NLP.Utils {
    public static class StringExtensions {

        public static string RemoveDiacritics(this string s) {
            string normalized = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalized.Length; i++) {
                if (CharUnicodeInfo.GetUnicodeCategory(normalized[i]) != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(normalized[i]);
                }
            }

            return stringBuilder.ToString();
        }

        // Thanks Dr. Levenshtein and Sam Allen @ http://dotnetperls.com/levenshtein
        public static int Levenshtein(this string s, string t) {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0) {
                return m;
            }

            if (m == 0) {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) {
            }

            for (int j = 0; j <= m; d[0, j] = j++) {
            }

            // Step 3
            for (int i = 1; i <= n; i++) {
                //Step 4
                for (int j = 1; j <= m; j++) {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        // Mostly an english hack - will enhance
        public static int DePluralLevenshtein(this string s, string t) {

            int returnRatio = s.LevenshteinSubsetBonusRatio(t);

            if (t.Length > 0 && t[t.Length - 1] == 's') {
                string newT = t.Remove(t.Length - 1);

                if (newT.Length > 0 && newT[newT.Length - 1] == '\'') {
                    newT = newT.Remove(newT.Length - 1);
                }

                int newTRatio = s.LevenshteinSubsetBonusRatio(newT);

                if (newTRatio > returnRatio) {
                    returnRatio = newTRatio;
                }
            }

            return returnRatio;
        }

        public static int LevenshteinRatio(this string s, string t) {
            if (s == null || t == null) {
                return 0;
            }

            double levenshtein = (double)s.ToLower().Levenshtein(t.ToLower());

            // Direct subsets earn a bonus towards the final score.
            //if (levenshtein > 0.0D && s.Length != t.Length && s.ToLower().IndexOf(t.ToLower()) >= 0) {
            //    levenshtein = Math.Max(0.1D, levenshtein - Math.Min(s.Length, t.Length) / 2.0D);
            //}

            return (int)Math.Round((1.0D - levenshtein / Math.Max(s.Length, t.Length)) * 100.0D);
        }

        public static int LevenshteinSubsetBonusRatio(this string s, string t) {
            if (s == null || t == null) {
                return 0;
            }

            double levenshtein = (double)s.ToLower().Levenshtein(t.ToLower());

            // Direct subsets earn a bonus towards the final score.
            if (levenshtein > 0.0D && s.Length != t.Length && s.ToLower().IndexOf(t.ToLower()) >= 0) {
                levenshtein = Math.Max(0.1D, levenshtein - Math.Min(s.Length, t.Length) / 2.0D);
            }

            return (int)Math.Round((1.0D - levenshtein / Math.Max(s.Length, t.Length)) * 100.0D);
        }

        public static int GetClosestMatch(this string input, List<string> dictionary, out string matchedDictionaryKey) {
            int iSimilarity = int.MaxValue;
            int iScore = 0;

            matchedDictionaryKey = String.Empty;

            if (dictionary.Count >= 1) {

                int iLargestDictionaryKey = 0;

                // Build array of default matches from the dictionary to store a rank for each match.
                // (it's designed to work on smaller dictionaries with say.. 32 player names in it =)
                List<MatchDictionaryKey> lstMatches = new List<MatchDictionaryKey>();
                foreach (string strDictionaryKey in dictionary) {
                    lstMatches.Add(new MatchDictionaryKey(strDictionaryKey));

                    if (strDictionaryKey.Length > iLargestDictionaryKey) {
                        iLargestDictionaryKey = strDictionaryKey.Length;
                    }
                }

                // Rank each match, find the remaining characters for a match (arguements)
                for (int x = 1; x <= Math.Min(input.Length, iLargestDictionaryKey); x++) {
                    // Skip it if it's a space (a space breaks a name and moves onto arguement.
                    // but the space could also be included in the dictionarykey, which will be checked
                    // on the next loop.
                    if (x + 1 < input.Length && input[x] != ' ')
                        continue;

                    for (int i = 0; i < lstMatches.Count; i++) {
                        iScore = input.Substring(0, x).ToLower().Levenshtein(lstMatches[i].LowerCaseMatchedText);

                        if (iScore < lstMatches[i].MatchedScore || (iScore == lstMatches[i].MatchedScore && x > lstMatches[i].MatchedScoreCharacters)) {
                            lstMatches[i].MatchedScore = iScore;
                            lstMatches[i].MatchedScoreCharacters = x;
                        }
                    }
                }

                // Sort the matches
                lstMatches.Sort();

                int iBestCharactersMatched = lstMatches[0].MatchedScoreCharacters;
                iSimilarity = lstMatches[0].MatchedScore;
                matchedDictionaryKey = lstMatches[0].MatchedText;

                // Now though we want to loop through from start to end and see if a subset of what we entered is found.
                // if so then this will find the highest ranked item with a subset of what was entered and favour that instead.
                string strBestCharsSubstringLower = input.Substring(0, iBestCharactersMatched).ToLower();
                for (int i = 0; i < lstMatches.Count; i++) {
                    if (lstMatches[i].LowerCaseMatchedText.Contains(strBestCharsSubstringLower) == true) {
                        iSimilarity = lstMatches[i].MatchedScore;
                        matchedDictionaryKey = lstMatches[i].MatchedText;
                        iBestCharactersMatched = lstMatches[i].MatchedScoreCharacters;

                        break;
                    }
                }

                if (iBestCharactersMatched < input.Length) {
                    iSimilarity = int.MaxValue;
                }

            }

            return iSimilarity;
        }

        public static List<string> Wordify(this string input) {
            List<string> returnList = new List<string>();

            string fullWord = String.Empty;
            int quoteStack = 0;
            bool isEscaped = false;

            char[] punctuation = new char[] { '(', ')', ',', '.', '?', '!', '/', '*', '-', '+', ':' };

            //for (int i = 0; i < strCommand.Length; i++) {
            foreach (char c in input) {

                if (c == ' ') {
                    if (quoteStack == 0) {
                        if (fullWord.Length > 0) {
                            returnList.Add(fullWord);
                            fullWord = String.Empty;
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
                            fullWord = String.Empty;
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

            return returnList.Where(x => String.IsNullOrEmpty(x) == false).ToList();
        }

    }
}
