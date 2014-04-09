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

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Describes a language, it's origin and simplified readable text acount it
    /// </summary>
    [Serializable]
    public class LanguageModel : CoreModel {
        /// <summary>
        /// IETF language tag for the LanguageCode string.
        /// http://en.wikipedia.org/wiki/IETF_language_tag
        /// http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// http://en.wikipedia.org/wiki/ISO_3166-1
        /// </summary>
        public String LanguageCode { get; set; }

        /// <summary>
        /// The ISO 3166 (alpha 2) country code used to represent this language. Though
        /// not globally accepted for a 1:1 country to language, this should be either
        /// blank or the country of origin for the language.
        /// </summary>
        public String CountryCode { get; set; }

        /// <summary>
        /// The english name of language
        /// </summary>
        public String EnglishName { get; set; }

        /// <summary>
        /// The native name of the language.
        /// </summary>
        public String NativeName { get; set; }
    }
}
