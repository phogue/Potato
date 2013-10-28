using System.Text.RegularExpressions;

namespace Procon.Core.Utils {
    public static class PathValidator {
        /// <summary>
        /// Makes sure the given path just contains legal characters
        /// </summary>
        /// <param name="validate">string to validate</param>
        /// <returns>valid string</returns>
        public static string Valdiate(string validate) {
            Regex pattern = new Regex(@"[^A-Za-z0-9._-]");
            Regex trimPattern = new Regex(@"_+");

            if (pattern.IsMatch(validate)) {
                MatchEvaluator matchEvaluator = new MatchEvaluator(m => {
                    switch (m.Value.ToLower()) {
                        case "ä":
                            return "ae";
                        case "ö":
                            return "oe";
                        case "ü":
                            return "ue";
                        case "ß":
                            return "ss";
                        default:
                            return "_";
                    }
                });

                validate = pattern.Replace(validate, matchEvaluator);
                validate = trimPattern.Replace(validate, matchEvaluator);
            }

            return validate;
        }
    }
}
