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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Localization {
    using Procon.Core.Utils;

    public class LanguageController : Executable<LanguageController>
    {
        // Public Properties
        public Language       Default   { get; private set; }
        public List<Language> Languages { get; private set; }

        // Default Initialization
        public LanguageController() {
            Default = null;
            Languages = new List<Language>();
        }



        #region Events

        // -- Default Language Changed --
        public delegate void DefaultLanguageChangeHandler(LanguageController parent, Language item);
        public event         DefaultLanguageChangeHandler DefaultLanguageChanged;
        protected void OnDefaultLanguageChanged(LanguageController parent, Language item)
        {
            if (DefaultLanguageChanged != null)
                DefaultLanguageChanged(parent, item);
        }

        #endregion
        #region Executable

        /// <summary>
        /// Loads the languages found in the languages directory.
        /// Sets the default language to english before looking at config file.
        /// Executes the commands specified in the configuration file.
        /// Returns a reference back to this object.
        /// </summary>
        new public LanguageController Execute()
        {
            DirectoryInfo langsDirectory = new DirectoryInfo(Defines.LOCALIZATION_DIRECTORY);
            foreach (DirectoryInfo langDirectory in langsDirectory.GetDirectories())
            {
                Language lang = new Language().LoadDirectory(langDirectory) as Language;
                if (lang.LanguageCode != null)
                    Languages.Add(lang);
            }
            SetDefaultLanguage(CommandInitiator.Local, "en");

            return base.Execute();
        }

        /// <summary>
        /// Saves the default language.
        /// </summary>
        protected override void WriteConfig(XElement xNamespace, ref FileInfo xFile)
        {
            xNamespace.Add(new XElement("command",
                new XAttribute("name", CommandName.LanguageSetDefaultLanguage),
                new XElement("directory", Default.LanguageCode)
            ));
        }

        #endregion



        /// <summary>
        /// Gets a localized string based on a key value specified, limited to a specific namespace,
        /// with additional arguments for formatting if necessary. Uses the default language if the
        /// language code is not specified. Returns an empty string if a localization could not be
        /// found.
        /// </summary>
        /// <param name="languageCode">The two letter iso 639-1 code.</param>
        /// <param name="namespace">The namespace to limit the search for the key to.</param>
        /// <param name="key">The key representing the localized string.</param>
        /// <param name="args">Arguments to use in String.Format() for the value obtained by key.</param>
        public String Loc(String languageCode, String @namespace, String key, params Object[] args)
        {
            Language lang = Languages.Where(x => x.LanguageCode == languageCode).FirstOrDefault();
            if (lang == null)
                lang = Default;

            return lang.Loc(@namespace, key, args);
        }



        /// <summary>
        /// Sets the default language. The default language is used whenever a language is not
        /// specified during a localization lookup.
        /// </summary>
        [Command(Command = CommandName.LanguageSetDefaultLanguage)]
        private void SetDefaultLanguage(CommandInitiator initiator, String languageCode)
        {
            Language language = Languages.Where(x => x.LanguageCode == languageCode).FirstOrDefault();
            if (language != null)
            {
                Default = language;
                OnDefaultLanguageChanged(this, Default);
            }
        }
    }
}
