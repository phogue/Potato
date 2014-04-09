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
using Newtonsoft.Json.Linq;
using Procon.Fuzzy.Tokens.Object;

namespace Procon.Fuzzy {
    /// <summary>
    /// A state passed through to all combination and parse methods
    /// </summary>
    public interface IFuzzyState {
        /// <summary>
        /// Language file, used to lookup values to attach to each object. The xml should be structured like
        /// the namespace of the object.
        /// </summary>
        JObject Document { get; set; }

        /// <summary>
        /// Converts a phrase into a token if the token matches an object
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseThing(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parses the text for any commands it can use.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseMethod(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parse a property, building the property reference.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        Phrase ParseProperty(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// Parses a self reflection token to define who "me" is.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="selfThing"></param>
        /// <returns></returns>
        SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing);
    }
}