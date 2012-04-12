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
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Globalization;

namespace Procon.Net.Utils {
    public static class StringExtensions {



        public static List<string> Wordify(this string command) {
            List<string> returnList = new List<string>();
            //lstReturn.RemoveAll(String.IsNullOrEmpty);

            string fullWord = String.Empty;
            int quoteStack = 0;
            bool isEscaped = false;

            //for (int i = 0; i < strCommand.Length; i++) {
            foreach (char input in command) {

                if (input == ' ') {
                    if (quoteStack == 0) {
                        returnList.Add(fullWord);
                        fullWord = String.Empty;
                    }
                    else {
                        fullWord += ' ';
                    }
                }
                else if (input == 'n' && isEscaped == true) {
                    fullWord += '\n';
                    isEscaped = false;
                }
                else if (input == 'r' && isEscaped == true) {
                    fullWord += '\r';
                    isEscaped = false;
                }
                else if (input == 't' && isEscaped == true) {
                    fullWord += '\t';
                    isEscaped = false;
                }
                else if (input == '"') {
                    if (isEscaped == false) {
                        if (quoteStack == 0) {
                            quoteStack++;
                        }
                        else {
                            quoteStack--;
                        }
                    }
                    else {
                        fullWord += '"';
                    }
                }
                else if (input == '\\') {
                    if (isEscaped == true) {
                        fullWord += '\\';
                        isEscaped = false;
                    }
                    else {
                        isEscaped = true;
                    }
                }
                else {
                    fullWord += input;
                    isEscaped = false;
                }
            }

            returnList.Add(fullWord);

            return returnList;
        }

        /// <summary>
        /// Takes a string and splits it on words based on characters 
        /// string testString = "this is a string with some words in it";
        /// testString.WordWrap(10) == List<string>() { "this is a", "string", "with some", "words in", "it" }
        ///        
        /// Useful if you want to output a long string to the game and want all of the data outputed without
        /// losing any data.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static List<string> WordWrap(this string text, int column) {
            List<string> result = new List<string>(text.Split(' '));

            for (int i = 0; i < result.Count - 1; i++) {
                if (result[i].Length + result[i + 1].Length + 1 <= column) {
                    result[i] = String.Format("{0} {1}", result[i], result[i + 1]);
                    result.RemoveAt(i + 1);
                    i--;
                }
            }

            return result;
        }

        private static readonly Dictionary<string, string> LeetRules = new Dictionary<string, string>() {
                    { "4", "A" },
                    { @"/\", "A" },
                    { "@", "A" },
                    { "^", "A" },
                    { "13", "B" },
                    { "/3", "B" },
                    { "|3", "B" },
                    { "8", "B" },
                    { "><", "X" },
                    { "<", "C" },
                    { "(", "C" },
                    { "|)", "D" },
                    { "|>", "D" },
                    { "3", "E" },
                    { "6", "G" },
                    { "/-/", "H" },
                    { "[-]", "H" },
                    { "]-[", "H" },
                    { "!", "I" },
                    { "|_", "L" },
                    { "_/", "J" },
                    { "_|", "J" },
                    { "1", "L" },
                    { "0", "O" },
                    { "5", "S" },
                    { "7", "T" },
                    { @"\/\/", "W" },
                    { @"\/", "V" },
                    { "2", "Z" }
                };

        /// <summary>
        /// Removes diacritics, such as the umlaut (ë => e).  Though not linguistically
        /// correct it does allow UTF8 to be represented in ASCII, at least to a degree
        /// that will alert translators that further translation is required.
        /// </summary>
        /// <param name="s">String containing diacritics</param>
        /// <returns>Normalized s</returns>
        public static string RemoveDiacritics(this string s) {
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++) {
                if (CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]) != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(normalizedString[i]);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Removes leet speak from a name
        /// 
        /// P]-[0gu3 => Phogue
        /// </summary>
        /// <param name="s">String containing leet speak</param>
        /// <returns>Normalized s</returns>
        public static string RemoveLeetSpeek(this string s) {
            string normalizedString = s;

            foreach (KeyValuePair<string, string> x in StringExtensions.LeetRules) {
                normalizedString = normalizedString.Replace(x.Key, x.Value);
            }

            return normalizedString;
        }

        /// <summary>
        /// Strips both leet and diacritic from a string so it is represented
        /// in basic ASCII.
        /// </summary>
        /// <param name="s">String containing leet speak and diacritics</param>
        /// <returns>Normalized s</returns>
        public static string Strip(this string s) {
            string stripped = s;

            if (s != null) {
                stripped = stripped.RemoveLeetSpeek().RemoveDiacritics();
                stripped = new string(stripped.Where(ch => char.IsLetterOrDigit(ch)).ToArray());
            }

            return stripped;
        }
    }
}
