using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Localization {
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
        public Dictionary<String, List<LanguageEntryModel>> Cached { get; set; }

        /// <summary>
        /// Initializes default values for the language config
        /// </summary>
        public LanguageConfig() {
            this.LanguageModel = new LanguageModel();
            this.Config = new Config();
            this.Cached = new Dictionary<String, List<LanguageEntryModel>>();
        }

        /// <summary>
        /// Loads the specified file into this language file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public void Load(FileInfo file) {
            this.Config.Load(file);

            if (this.Config.RootOf<LanguageConfig>().First != null) {
                this.LanguageModel = this.Config.RootOf<LanguageConfig>().First.ToObject<LanguageModel>();
            }
        }

        /// <summary>
        /// Loads the specified directory into this config object.
        /// </summary>
        /// <param name="directory"></param>
        public void Load(DirectoryInfo directory) {
            this.Config.Load(directory);

            if (this.Config.RootOf<LanguageConfig>().First != null) {
                this.LanguageModel = this.Config.RootOf<LanguageConfig>().First.ToObject<LanguageModel>();
            }
        }

        /// <summary>
        /// Attempts to find a localized string within this language file. Returns an empty
        /// string if the name could not be found.
        /// </summary>
        /// <param name="namespace">The namespace to limit the search for the name to.</param>
        /// <param name="name">The name representing the localized string.</param>
        /// <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
        public String Localize(String @namespace, String name, params Object[] args) {
            String result = String.Empty;

            if (this.Cached.ContainsKey(@namespace) == false) {
                this.Cached.Add(@namespace, this.Config.RootOf(@namespace).Select(item => item.ToObject<LanguageEntryModel>()).ToList());
            }

            var loc = this.Cached[@namespace].FirstOrDefault(item => item.Name == name);


            // Attempt to format name.
            if (loc != null) {
                try {
                    result = String.Format(loc.Text, args);
                }
                catch (Exception) {
                    // We don't want to output general errors to a user, but we need to make a clear
                    // "There isn't something here, but you should probably think there is".
                    // It's a step up from our old Procon 1.X method that would have an error in place.
                    result = String.Empty;
                }
            }

            // Return the result.
            return result;
        }
    }
}
