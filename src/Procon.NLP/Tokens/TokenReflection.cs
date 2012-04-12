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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Procon.NLP.Tokens {
    using Procon.NLP.Utils;

    public class TokenReflection {

        private static Dictionary<XElement, Dictionary<Type, List<XElement>>> SelectedDescendants { get; set; }
        private static Dictionary<string, List<Type>> TokenClasses { get; set; }
        private static Dictionary<string, List<MethodInfo>> ParseMethods { get; set; }
        private static Dictionary<string, List<MethodInfo>> CombineMethods { get; set; }
        private static Dictionary<string, List<MethodInfo>> ReduceMethods { get; set; }
        private static Dictionary<string, Regex> CompiledRegexes { get; set; }

        public static List<MethodInfo> GetParseMethods(string tokenNamespace) {

            if (TokenReflection.ParseMethods == null) {
                TokenReflection.ParseMethods = new Dictionary<string, List<MethodInfo>>();
            }

            if (TokenReflection.ParseMethods.ContainsKey(tokenNamespace) == false) {

                List<MethodInfo> returnMethods = new List<MethodInfo>();

                foreach (Type token in TokenReflection.GetTokenClasses(tokenNamespace)) {

                    var methods = from method in token.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                  let parameters = method.GetParameters()
                                  where String.Compare(method.Name, "Parse") == 0
                                     && method.ReturnType == typeof(Phrase)
                                     && parameters.Length >= 2
                                     && parameters[0].ParameterType == typeof(IStateNLP)
                                     && parameters[1].ParameterType == typeof(Phrase)
                                  select method;

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.OrderByDescending(x => x.DeclaringType.Namespace.Length).ToList();

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

                    var methods = from method in token.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                  let parameters = method.GetParameters()
                                  where String.Compare(method.Name, "Combine") == 0
                                     && method.ReturnType == typeof(Phrase)
                                     && parameters.Length >= 2
                                     && parameters[0].ParameterType == typeof(IStateNLP)
                                     && parameters.Where(x => typeof(Token).IsAssignableFrom(x.ParameterType)).Count() == parameters.Length - 1
                                  select method;

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.OrderByDescending(x => x.DeclaringType.Namespace.Length).ToList();

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
                    
                    var methods = from method in token.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                  let parameters = method.GetParameters()
                                  where String.Compare(method.Name, "Reduce") == 0
                                     && method.ReturnType == typeof(Phrase)
                                     && parameters.Length >= 2
                                     && parameters[0].ParameterType == typeof(IStateNLP)
                                     && parameters.Where(x => typeof(Token).IsAssignableFrom(x.ParameterType)).Count() == parameters.Length - 1
                                  select method;

                    returnMethods.AddRange(methods.ToList());
                }

                // This makes it so reduction methods of inherited methods are called before their parents
                returnMethods = returnMethods.OrderByDescending(x => x.DeclaringType.Namespace.Length).ToList();
                
                TokenReflection.ReduceMethods.Add(tokenNamespace, returnMethods);
            }

            return TokenReflection.ReduceMethods[tokenNamespace];
        }

        public static List<Type> GetTokenClasses(string tokenNamespace) {

            if (TokenReflection.TokenClasses == null) {
                TokenReflection.TokenClasses = new Dictionary<string, List<Type>>();
            }

            if (TokenReflection.TokenClasses.ContainsKey(tokenNamespace) == false) {

                Regex tokenNamespame = new Regex(String.Format(@"^{0}[\.]?.*$", tokenNamespace.Replace(".", "\\."), RegexOptions.Compiled));

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

            var replacements = (from l in document.Descendants("nlp")
                                                  .Descendants("tokens")
                                                  .Descendants("tokenreflection")
                                                  .Descendants("diacritic")
                                select l
                               ).ToLookup(x => x.Attribute("key").Value, x => x.Attribute("value").Value);

            foreach (XElement element in document.Descendants("nlp").Descendants("match")) {
                if (element.Attribute("text") != null && element.Attribute("replaced_diacritics") == null) {

                    string replaced_diacritics = element.Attribute("text").Value;

                    foreach (var diacritic in replacements) {
                        replaced_diacritics = replaced_diacritics.Replace(diacritic.Key, diacritic.First());
                    }

                    element.SetAttributeValue("replaced_diacritics", replaced_diacritics.RemoveDiacritics());
                }

                if (element.Attribute("text") != null && element.Attribute("removed_diacritics") == null) {
                    element.SetAttributeValue("removed_diacritics", element.Attribute("text").Value.RemoveDiacritics());
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

            if (TokenReflection.SelectedDescendants[document].ContainsKey(type) == false) {

                var descendants = document.Elements();

                foreach (string name in type.Namespace.Split('.').Skip(1)) {
                    descendants = descendants.DescendantsAndSelf(name.ToLower());
                }

                TokenReflection.SelectedDescendants[document][type] = descendants.ToList();
            }

            return TokenReflection.SelectedDescendants[document][type];

            /*
            var returnList = document.Elements();

            foreach (string name in type.Namespace.Split('.').Skip(1)) {
                returnList = returnList.DescendantsAndSelf(name.ToLower());
            }

            return returnList.ToList();
            */
        }

        protected static T CreateToken<T>(XElement element, Phrase phrase) where T : Token, new() {

            T token = default(T);

            if (TokenReflection.CompiledRegexes == null) {
                TokenReflection.CompiledRegexes = new Dictionary<string,Regex>();
            }

            if (element.Attribute("text") != null && element.Attribute("replaced_diacritics") != null && element.Attribute("removed_diacritics") != null) {
                float similarity =  Math.Max(
                        Math.Max(
                            element.Attribute("text").Value.LevenshteinRatio(phrase.Text),
                            element.Attribute("replaced_diacritics").Value.LevenshteinRatio(phrase.Text)
                        ), element.Attribute("removed_diacritics").Value.LevenshteinRatio(phrase.Text)
                    );
                
                if (similarity >= Token.MINIMUM_SIMILARITY) {
                    token = new T() {
                        Text = phrase.Text,
                        Similarity = similarity
                    };

                    if (element.Attribute("value") == null) {
                        token.Value = element.Attribute("text").Value;
                    }
                    else {
                        token.Value = element.Attribute("value").Value;
                    }
                }
            }
            else if (element.Attribute("regex") != null) {
                
                if (TokenReflection.CompiledRegexes.ContainsKey(element.Attribute("regex").Value) == false) {
                    TokenReflection.CompiledRegexes.Add(element.Attribute("regex").Value, new Regex(element.Attribute("regex").Value, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }

                Match match = null;
                if ((match = TokenReflection.CompiledRegexes[element.Attribute("regex").Value].Match(phrase.Text)).Success == true) {
                    token = new T() {
                        Text = phrase.Text,
                        Value = match.Groups["value"].Value,
                        Similarity = 100.0F
                    };
                }
            }

            return token;
        }

        public static Phrase CreateDescendants<T>(IStateNLP state, Phrase phrase, out List<T> created) where T : Token, new() {

            var list = (from element in TokenReflection.SelectDescendants(state.Document, typeof(T))
                           .Descendants(typeof(T).Name.ToLower())
                           .Descendants("match")
                        let token = TokenReflection.CreateToken<T>(element, phrase)
                        where token != null
                        select token).ToList();

            created = list;

            list.ToList().ForEach(x => phrase.Add(x));
            //phrase.AddRange(list.AsEnumerable() as IEnumerable<Token>);

            return phrase;
        }

        public static Phrase CreateDescendants<T>(IStateNLP state, Phrase phrase) where T : Token, new() {

            var list = from element in TokenReflection.SelectDescendants(state.Document, typeof(T))
                           .Descendants(typeof(T).Name.ToLower())
                           .Descendants("match")
                       let token = TokenReflection.CreateToken<T>(element, phrase)
                       where token != null
                       select token;

            list.ToList().ForEach(x => phrase.Add(x));

            return phrase;
        }
    }
}
