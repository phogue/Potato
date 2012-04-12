// Copyright 2011 Geoffrey 'Phogue' Green
// Modified by Cameron 'Imisnew2' Gunnin
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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Localization
{
    [Serializable]
    public class Language : Config
    {
        // Public Properties
        // Uses ISO 639-1 Language codes for the LanguageCode string.
        // http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        public String LanguageCode { get; private set; }

        // Default Initialization
        public Language() {
            LanguageCode = String.Empty;
        }



        #region Config

        /// <summary>
        /// Loads the specified file into this language file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public override Config LoadFile(FileInfo mFile) {
            base.LoadFile(mFile);

            // Parse out extra instruction information used in a language config file.
            var instructions = Document.Nodes()
                                .Where( x => x is XProcessingInstruction)
                                .Select(x => x as XProcessingInstruction);

            // These instructions could be expanded to include details about the author.
            foreach (var instruction in instructions)
                switch (instruction.Target.ToLower())
                {
                    case "iso-639-1":
                        LanguageCode = instruction.Data;
                        break;
                }

            return this;
        }

        #endregion



        /// <summary>
        /// Attempts to find a localized string within this language file. Returns an empty
        /// string if the key could not be found.
        /// </summary>
        /// <param name="namespace">The namespace to limit the search for the key to.</param>
        /// <param name="key">The key representing the localized string.</param>
        /// <param name="args">Arguments to use in String.Format() for the value obtained by key.</param>
        public String Loc(String @namespace, String key, params Object[] args) {
            String result = String.Empty;

            // Drill down to namespace that was specified.
            var descendants = Document.Elements();
            foreach (String name in @namespace.ToLower().Split('.').Skip(1))
                descendants = descendants.DescendantsAndSelf(name);

            // Attempt to find key.
            var loc = descendants.Descendants("loc")
                                 .Where(x => x.Attribute("key") != null && x.Attribute("key").Value == key)
                                 .Select(x => x.Attribute("value"))
                                 .FirstOrDefault();

            // Attempt to format key.
            if (loc != null)
                try               { result = String.Format(loc.Value, args); }
                catch (Exception) { /* Format Error */ }

            // Return the result.
            return result;
        }
    }
}
