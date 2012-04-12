// Copyright 2011 Cameron 'Imisnew2' Gunnin
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

using Procon.Core.Localization;

namespace Procon.UI.API.Utils
{
    /// <summary>Allows global localization.</summary>
    public static class Localizer
    {
        /// <summary>The controller for localizing text.</summary>
        private static LanguageController mLocalization;

        /// <summary>Gets the localized text for a given key.</summary>
        /// <param name="key">The full namespace + key of the localized string.</param>
        /// <param name="parameters">[Optional] The parameters the localized string takes.</param>
        public static String Loc(String key, params Object[] parameters)
        {
            // Initialize the language controlle if is has not been intialized yet.
            if (mLocalization == null)
            {
                mLocalization = new LanguageController();
                mLocalization.Execute();
            }
            // Get the localized text.
            Int32 nsIndex = key.LastIndexOf('.');
            if (nsIndex >= 0)
                return mLocalization.Loc(null, key.Substring(0, nsIndex), key.Substring(nsIndex + 1), parameters);
            return mLocalization.Loc(null, key, key, parameters);
        }
    }
}
