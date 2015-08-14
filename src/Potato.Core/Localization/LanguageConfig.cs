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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Localization {
    /// <summary>
    /// A language config file, loaded and combined from various xml sources in the localization folder
    /// </summary>
    [Serializable]
    public class LanguageConfig {
        /// <summary>
        /// Holds the underlying information loaded from the file used to describe the language
        /// </summary>
        public LanguageModel LanguageModel { get; set; }

        /// <summary>
        /// The internal config loaded for this model.
        /// </summary>
        public IConfig Config { get; set; }

        /// <summary>
        /// The cached conversions from the JObject to LanguageEntryModel
        /// </summary>
        public Dictionary<string, List<LanguageEntryModel>> Cached { get; set; }

        /// <summary>
        /// Initializes default values for the language config
        /// </summary>
        public LanguageConfig() {
            LanguageModel = new LanguageModel();
            Config = new Config();
            Cached = new Dictionary<string, List<LanguageEntryModel>>();
        }

        /// <summary>
        /// Loads the specified file into this language file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public void Load(FileInfo file) {
            Config.Load(file);

            if (Config.RootOf<LanguageConfig>().First != null) {
                LanguageModel = Config.RootOf<LanguageConfig>().First.ToObject<LanguageModel>();
            }
        }

        /// <summary>
        /// Loads the specified directory into this config object.
        /// </summary>
        /// <param name="directory"></param>
        public void Load(DirectoryInfo directory) {
            Config.Load(directory);

            if (Config.RootOf<LanguageConfig>().First != null) {
                LanguageModel = Config.RootOf<LanguageConfig>().First.ToObject<LanguageModel>();
            }
        }

        /// <summary>
        /// Attempts to find a localized string within this language file. Returns an empty
        /// string if the name could not be found.
        /// </summary>
        /// <param name="namespace">The namespace to limit the search for the name to.</param>
        /// <param name="name">The name representing the localized string.</param>
        /// <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
        public string Localize(string @namespace, string name, params object[] args) {
            var result = string.Empty;

            if (Cached.ContainsKey(@namespace) == false) {
                Cached.Add(@namespace, Config.RootOf(@namespace).Select(item => item.ToObject<LanguageEntryModel>()).ToList());
            }

            var loc = Cached[@namespace].FirstOrDefault(item => item.Name == name);


            // Attempt to format name.
            if (loc != null) {
                try {
                    result = string.Format(loc.Text, args);
                }
                catch (Exception) {
                    // We don't want to output general errors to a user, but we need to make a clear
                    // "There isn't something here, but you should probably think there is".
                    // It's a step up from our old Potato 1.X method that would have an error in place.
                    result = string.Empty;
                }
            }

            // Return the result.
            return result;
        }
    }
}
