using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Procon.Nlp.Tokens {
    using Procon.Nlp.Utils;

    public class TokenReflection {

        public delegate Phrase ParseDelegateHandler(IStateNlp state, Phrase phrase);

        private static Dictionary<XElement, Dictionary<Type, List<XElement>>> SelectedDescendants { get; set; }
        private static Dictionary<XElement, Dictionary<Type, List<XElement>>> SelectedMatchDescendants { get; set; }
        private static Dictionary<string, List<Type>> TokenClasses { get; set; }
        private static Dictionary<string, List<Delegate>> ParseMethods { get; set; }
        private static Dictionary<string, List<MethodInfo>> CombineMethods { get; set; }
        private static Dictionary<string, List<MethodInfo>> ReduceMethods { get; set; }
        private static Dictionary<string, Regex> CompiledRegexes { get; set; }

        public static List<Delegate> GetParseMethods(string tokenNamespace) {

            if (TokenReflection.ParseMethods == null) {
                TokenReflection.ParseMethods = new Dictionary<string, List<Delegate>>();
            }

            if (TokenReflection.ParseMethods.ContainsKey(tokenNamespace) == false) {

                List<Delegate> returnMethods = new List<Delegate>();

                foreach (Type token in TokenReflection.GetTokenClasses(tokenNamespace)) {

                    var methods = token.GetMethods(BindingFlags.Public | BindingFlags.Static).Select(method => new {
                        Method = method,
                        Parameters = method.GetParameters()
                    }).Where(method => String.CompareOrdinal(method.Method.Name, "Parse") == 0)
                    .Where(method => method.Method.ReturnType == typeof(Phrase))
                    .Where(method => method.Parameters.Length >= 2)
                    .Where(method => method.Parameters[0].ParameterType == typeof(IStateNlp))
                    .Where(method => method.Parameters[1].ParameterType == typeof(Phrase))
                    .Select(method => Delegate.CreateDelegate(typeof(ParseDelegateHandler), method.Method));

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.Where(method => method.Method.DeclaringType != null && method.Method.DeclaringType.Namespace != null)
// ReSharper disable PossibleNullReferenceException
                                             .OrderByDescending(method => method.Method.DeclaringType.Namespace.Length)
// ReSharper restore PossibleNullReferenceException
                                             .ToList();

                TokenReflection.ParseMethods.Add(tokenNamespace, returnMethods);
            }

            return TokenReflection.ParseMethods[tokenNamespace];
        }

        public static List<MethodInfo> GetCombineMethods(string tokenNamespace) {
            if (TokenReflection.CombineMethods == null) {
                TokenReflection.CombineMethods = new Dictionary<string, List<MethodInfo>>();
            }

            if (TokenReflection.CombineMethods.ContainsKey(tokenNamespace) == false) {

                List<MethodInfo> returnMethods = new List<MethodInfo>();

                foreach (Type token in TokenReflection.GetTokenClasses(tokenNamespace)) {

                    var methods = token.GetMethods(BindingFlags.Public | BindingFlags.Static).Select(method => new {
                        Method = method,
                        Parameters = method.GetParameters()
                    }).Where(method => String.CompareOrdinal(method.Method.Name, "Combine") == 0)
                    .Where(method => method.Method.ReturnType == typeof(Phrase))
                    .Where(method => method.Parameters.Length >= 2)
                    .Where(method => method.Parameters[0].ParameterType == typeof(IStateNlp))
                    .Where(method => method.Parameters.Count(x => typeof(Token).IsAssignableFrom(x.ParameterType)) == method.Parameters.Length - 1)
                    .Select(method => method.Method);

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.Where(method => method.DeclaringType != null && method.DeclaringType.Namespace != null)
                    // ReSharper disable PossibleNullReferenceException
                                             .OrderByDescending(method => method.DeclaringType.Namespace.Length)
                    // ReSharper restore PossibleNullReferenceException
                                             .ToList();

                TokenReflection.CombineMethods.Add(tokenNamespace, returnMethods);
            }

            return TokenReflection.CombineMethods[tokenNamespace];
        }

        public static List<MethodInfo> GetReduceMethods(string tokenNamespace) {

            if (TokenReflection.ReduceMethods == null) {
                TokenReflection.ReduceMethods = new Dictionary<string, List<MethodInfo>>();
            }

            if (TokenReflection.ReduceMethods.ContainsKey(tokenNamespace) == false) {

                List<MethodInfo> returnMethods = new List<MethodInfo>();

                foreach (Type token in TokenReflection.GetTokenClasses(tokenNamespace)) {

                    var methods = token.GetMethods(BindingFlags.Public | BindingFlags.Static).Select(method => new {
                        Method = method,
                        Parameters = method.GetParameters()
                    }).Where(method => String.CompareOrdinal(method.Method.Name, "Reduce") == 0)
                    .Where(method => method.Method.ReturnType == typeof(Phrase))
                    .Where(method => method.Parameters.Length >= 2)
                    .Where(method => method.Parameters[0].ParameterType == typeof(IStateNlp))
                    .Where(method => method.Parameters.Count(x => typeof(Token).IsAssignableFrom(x.ParameterType)) == method.Parameters.Length - 1)
                    .Select(method => method.Method);

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.Where(method => method.DeclaringType != null && method.DeclaringType.Namespace != null)
                    // ReSharper disable PossibleNullReferenceException
                                             .OrderByDescending(method => method.DeclaringType.Namespace.Length)
                    // ReSharper restore PossibleNullReferenceException
                                             .ToList();
                
                TokenReflection.ReduceMethods.Add(tokenNamespace, returnMethods);
            }

            return TokenReflection.ReduceMethods[tokenNamespace];
        }

        public static List<Type> GetTokenClasses(string tokenNamespace) {

            if (TokenReflection.TokenClasses == null) {
                TokenReflection.TokenClasses = new Dictionary<string, List<Type>>();
            }

            if (TokenReflection.TokenClasses.ContainsKey(tokenNamespace) == false) {

                Regex tokenNamespame = new Regex(String.Format(@"^{0}[\.]?.*$", tokenNamespace.Replace(".", "\\.")), RegexOptions.Compiled);

                var classes = from type in Assembly.GetExecutingAssembly().GetTypes()
                              where type != null
                                 && type.IsClass == true
                                 && type.Namespace != null
                                 && tokenNamespame.IsMatch(type.Namespace) == true
                              select type;

                TokenReflection.TokenClasses.Add(tokenNamespace, classes.ToList());
            }

            return TokenReflection.TokenClasses[tokenNamespace];
        }


        protected static void AddStrippedDiacriticsAttributes(XElement document) {

            var replacements = document.Descendants("Nlp").Descendants("Tokens").Descendants("TokenReflection").Descendants("Diacritic").Select(element => element).ToLookup(element => {
                XAttribute key = element.Attribute("key");

                return key != null ? key.Value : null;
            }, element => {
                XAttribute value = element.Attribute("value");

                return value != null ? value.Value : null;
            });

            foreach (XElement element in document.Descendants("Nlp").Descendants("Match")) {
                XAttribute text = element.Attribute("text");
                if (text != null && element.Attribute("replacedDiacritics") == null) {
                    string replacedDiacritics = text.Value;

                    replacedDiacritics = replacements.Aggregate(replacedDiacritics, (current, diacritic) => current.Replace(diacritic.Key, diacritic.First()));

                    element.SetAttributeValue("replacedDiacritics", replacedDiacritics.RemoveDiacritics());
                }

                if (text != null && element.Attribute("removedDiacritics") == null) {
                    element.SetAttributeValue("removedDiacritics", text.Value.RemoveDiacritics());
                }
            }
        }

        public static List<XElement> SelectDescendants(XElement document, Type type) {

            if (TokenReflection.SelectedDescendants == null) {
                TokenReflection.SelectedDescendants = new Dictionary<XElement, Dictionary<Type, List<XElement>>>();
            }

            if (TokenReflection.SelectedDescendants.ContainsKey(document) == false) {
                TokenReflection.AddStrippedDiacriticsAttributes(document);

                TokenReflection.SelectedDescendants.Add(document, new Dictionary<Type,List<XElement>>());
            }

            if (TokenReflection.SelectedDescendants[document].ContainsKey(type) == false && type.Namespace != null) {

                var descendants = type.Namespace.Split('.').Skip(1).Aggregate(document.Elements(), (current, name) => current.DescendantsAndSelf(name));

                TokenReflection.SelectedDescendants[document][type] = descendants.ToList();
            }

            return TokenReflection.SelectedDescendants[document][type];
        }

        /// <summary>
        /// Finds and caches all "match" elements in a given types namespace.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> SelectMatchDescendants(XElement document, Type type) {
            if (TokenReflection.SelectedMatchDescendants == null) {
                TokenReflection.SelectedMatchDescendants = new Dictionary<XElement, Dictionary<Type, List<XElement>>>();
            }

            if (TokenReflection.SelectedMatchDescendants.ContainsKey(document) == false) {
                TokenReflection.AddStrippedDiacriticsAttributes(document);

                TokenReflection.SelectedMatchDescendants.Add(document, new Dictionary<Type, List<XElement>>());
            }

            if (TokenReflection.SelectedMatchDescendants[document].ContainsKey(type) == false && type.Namespace != null) {

                var descendants = type.Namespace.Split('.').Skip(1).Aggregate(document.Elements(), (current, name) => current.DescendantsAndSelf(name));

                descendants = descendants.Descendants(type.Name).Descendants("Match");

                TokenReflection.SelectedMatchDescendants[document][type] = descendants.ToList();
            }

            return TokenReflection.SelectedMatchDescendants[document][type];
        }

        protected static T CreateToken<T>(XElement element, Phrase phrase) where T : Token, new() {

            T token = default(T);

            if (TokenReflection.CompiledRegexes == null) {
                TokenReflection.CompiledRegexes = new Dictionary<string,Regex>();
            }

            XAttribute text = element.Attribute("text");
            XAttribute value = element.Attribute("value");
            XAttribute regex = element.Attribute("regex");
            XAttribute replacedDiacritics = element.Attribute("replacedDiacritics");
            XAttribute removedDiacritics = element.Attribute("removedDiacritics");

            if (text != null && replacedDiacritics != null && removedDiacritics != null) {
                float similarity =  Math.Max(
                        Math.Max(
                            text.Value.LevenshteinRatio(phrase.Text),
                            replacedDiacritics.Value.LevenshteinRatio(phrase.Text)
                        ), removedDiacritics.Value.LevenshteinRatio(phrase.Text)
                    );
                
                if (similarity >= Token.MinimumSimilarity) {
                    token = new T() {
                        Text = phrase.Text,
                        Similarity = similarity
                    };

                    token.Value = value == null ? text.Value : value.Value;
                }
            }
            else if (regex != null) {

                if (TokenReflection.CompiledRegexes.ContainsKey(regex.Value) == false) {
                    TokenReflection.CompiledRegexes.Add(regex.Value, new Regex(regex.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }

                Match match = null;
                if ((match = TokenReflection.CompiledRegexes[regex.Value].Match(phrase.Text)).Success == true) {
                    token = new T() {
                        Text = phrase.Text,
                        Value = match.Groups["value"].Value,
                        Similarity = 100.0F
                    };
                }
            }

            return token;
        }

        public static Phrase CreateDescendants<T>(IStateNlp state, Phrase phrase, out List<T> created) where T : Token, new() {

            var list = (from element in TokenReflection.SelectMatchDescendants(state.Document, typeof(T))
                        let token = TokenReflection.CreateToken<T>(element, phrase)
                        where token != null
                        select token).ToList();

            created = list;

            list.ToList().ForEach(phrase.Add);

            return phrase;
        }

        public static Phrase CreateDescendants<T>(IStateNlp state, Phrase phrase) where T : Token, new() {

            var list = from element in TokenReflection.SelectMatchDescendants(state.Document, typeof(T))
                       let token = TokenReflection.CreateToken<T>(element, phrase)
                       where token != null
                       select token;

            list.ToList().ForEach(phrase.Add);

            return phrase;
        }
    }
}
