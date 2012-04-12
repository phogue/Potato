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
using System.Windows.Data;

using Procon.UI.API.Utils;

namespace Procon.UI.API.Converters
{
    /// <summary>Localizes the parameter, or value if parameter is null.</summary>
    public class TextLocalizationConverter : IValueConverter
    {
        /// <summary>
        /// The *parameter* passed in is:
        /// String - The key of the localized text.
        /// The *value* passed in is:
        /// String - The key of the localized text.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String locKey = (parameter as String);
            if (locKey != null && locKey is String)
                return Localizer.Loc(locKey);

            locKey = (value as String);
            if (locKey != null && locKey is String)
                return Localizer.Loc(locKey.Contains("Procon.UI.") ? locKey : "Procon.UI." + locKey);

            return String.Empty;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
