using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Localization {

    /// <summary>
    /// A language config file, loaded and combined from various xml sources in the localization folder
    /// </summary>
    [Serializable]
    public class LanguageConfig : Config {
        /// <summary>
        /// Holds the underlying information loaded from the file used to describe the language
        /// </summary>
        public LanguageModel LanguageModel { get; set; }

        /// <summary>
        /// Initializes default values for the language config
        /// </summary>
        public LanguageConfig() {
            this.LanguageModel = new LanguageModel();
        }

        /// <summary>
        /// Loads the specified file into this language file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public override Config Load(FileInfo mFile) {
            base.Load(mFile);

            // Parse out extra instruction information used in a language config file.
            var instructions = this.Document.Nodes().OfType<XProcessingInstruction>();

            // These instructions could be expanded to include details about the author.
            foreach (var instruction in instructions) {
                switch (instruction.Target) {
                    case "ietf-language-tag":
                        this.LanguageModel.LanguageCode = instruction.Data;
                        break;
                    case "iso-3166-1-alpha-2":
                        this.LanguageModel.CountryCode = instruction.Data;
                        break;
                    case "country-name-english":
                        this.LanguageModel.EnglishName = instruction.Data;
                        break;
                    case "country-name-native":
                        this.LanguageModel.NativeName = instruction.Data;
                        break;
                }
            }

            return this;
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

            // Drill down to namespace that was specified.
            var children = this.Document.Elements();

            children = @namespace.Split('.').Skip(1).Aggregate(children, (current, n) => current.Elements(n));

            // Attempt to find name.
            var loc = children.Elements("Loc").Where(attribute => {
                var xAttribute = attribute.Attribute("name");
                return xAttribute != null && xAttribute.Value == name;
            }).Select(attribute => attribute.Attribute("value"))
            .FirstOrDefault();

            // Attempt to format name.
            if (loc != null) {
                try {
                    result = String.Format(loc.Value, args);
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
