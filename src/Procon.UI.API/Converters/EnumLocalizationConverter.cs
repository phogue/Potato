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

using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.UI.API.Enums;
using Procon.UI.API.Utils;

namespace Procon.UI.API.Converters
{
    /// <summary>Localizes the passed in enum.</summary>
    public class EnumLocalizationConverter : IValueConverter
    {
        /// <summary>
        /// The *value* passed in is:
        /// Enum - The enum to localize.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Enum   locEnum  = (value as Enum);
            String locValue = String.Empty;


            // Null
            if (locEnum == null)
                locValue = String.Empty;
            // Procon.Net.Protocols.Objects.ChatActionType
            // Procon.UI.API.Enums.ActionPlayerType
            // Procon.UI.API.Enums.ActionMapType
            // Procon.UI.API.Enums.ActionBanType
            else if (locEnum is ActionChatType)
                locValue = Localizer.Loc("Procon.UI.API.Action.Chat." + locEnum.ToString());
            else if (locEnum is ActionPlayerType)
                locValue = Localizer.Loc("Procon.UI.API.Action.Player." + locEnum.ToString());
            else if (locEnum is ActionMapType)
                locValue = Localizer.Loc("Procon.UI.API.Action.Map." + locEnum.ToString());
            else if (locEnum is ActionBanType)
                locValue = Localizer.Loc("Procon.UI.API.Action.Ban." + locEnum.ToString());
            // Procon.UI.API.Enums.FilterType
            // Procon.UI.API.Enums.FilterChatField
            // Procon.UI.API.Enums.FilterBanField
            else if (locEnum is FilterType)
                locValue = Localizer.Loc("Procon.UI.API.Filter.Type." + locEnum.ToString());
            else if (locEnum is FilterChatField)
                locValue = Localizer.Loc("Procon.UI.API.Filter.Chat." + locEnum.ToString());
            else if (locEnum is FilterBanField)
                locValue = Localizer.Loc("Procon.UI.API.Filter.Ban." + locEnum.ToString());
            // Procon.UI.API.Enums.EventType
            // Procon.Net.Protocols.GameType
            else if (locEnum is EventType)
                locValue = Localizer.Loc("Procon.UI.API.Event." + locEnum.ToString());
            else if (locEnum is GameType)
                locValue = Localizer.Loc("Procon.UI.API.Game." + locEnum.ToString());
            // Procon.Net.Protocols.Objects.Team
            // Procon.Net.Protocols.Objects.Squad
            else if (locEnum is Team)
                locValue = Localizer.Loc("Procon.UI.API.Team." + locEnum.ToString());
            else if (locEnum is Squad)
                locValue = Localizer.Loc("Procon.UI.API.Squad." + locEnum.ToString());
            // Procon.Net.Protocols.Objects.TimeSubsetContext
            // Procon.Net.Protocols.Objects.PlayerSubsetContext
            else if (locEnum is TimeSubsetContext)
                locValue = Localizer.Loc("Procon.UI.API.Subset.Time." + locEnum.ToString());
            else if (locEnum is PlayerSubsetContext)
                locValue = Localizer.Loc("Procon.UI.API.Subset.Player." + locEnum.ToString());
            // Default
            else
                locValue = locEnum.ToString();


            return locValue;
        }

        /// <summary>
        /// Does not exist.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
