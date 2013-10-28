using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Localization {

    [Serializable]
    public class Language : Config {
        /// <summary>
        /// IETF language tag for the LanguageCode string.
        /// http://en.wikipedia.org/wiki/IETF_language_tag
        /// http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// http://en.wikipedia.org/wiki/ISO_3166-1
        /// </summary>
        public String LanguageCode { get; set; }

        // Default Initialization
        public Language() {
            this.LanguageCode = String.Empty;
        }

        /// <summary>
        /// Loads the specified file into this language file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public override Config LoadFile(FileInfo mFile) {
            base.LoadFile(mFile);

            // Parse out extra instruction information used in a language config file.
            var instructions = this.Document.Nodes().OfType<XProcessingInstruction>();

            // These instructions could be expanded to include details about the author.
            foreach (var instruction in instructions) {
                switch (instruction.Target) {
                    case "ietf-language-tag":
                        LanguageCode = instruction.Data;
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
