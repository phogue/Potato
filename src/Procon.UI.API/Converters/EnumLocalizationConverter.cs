using System;
using System.Windows.Data;

using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.UI.API.Enums;

namespace Procon.UI.API.Converters
{
    public class EnumLocalizationConverter : IValueConverter
    {
        // Value = The enum to localize.
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
                locValue = ExtensionApi.Localize("Procon.UI.API.Action.Chat." + locEnum.ToString());
            else if (locEnum is ActionPlayerType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Action.Player." + locEnum.ToString());
            else if (locEnum is ActionMapType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Action.Map." + locEnum.ToString());
            else if (locEnum is ActionBanType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Action.Ban." + locEnum.ToString());
            // Procon.UI.API.Enums.FilterType
            // Procon.UI.API.Enums.FilterChatField
            // Procon.UI.API.Enums.FilterBanField
            else if (locEnum is FilterType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Filter.Type." + locEnum.ToString());
            else if (locEnum is FilterChatField)
                locValue = ExtensionApi.Localize("Procon.UI.API.Filter.Chat." + locEnum.ToString());
            else if (locEnum is FilterBanField)
                locValue = ExtensionApi.Localize("Procon.UI.API.Filter.Ban." + locEnum.ToString());
            // Procon.UI.API.Enums.EventType
            // Procon.Net.Protocols.GameType
            else if (locEnum is EventType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Event." + locEnum.ToString());
            else if (locEnum is GameType)
                locValue = ExtensionApi.Localize("Procon.UI.API.Game." + locEnum.ToString());
            // Procon.Net.Protocols.Objects.Team
            // Procon.Net.Protocols.Objects.Squad
            else if (locEnum is Team)
                locValue = ExtensionApi.Localize("Procon.UI.API.Team." + locEnum.ToString());
            else if (locEnum is Squad)
                locValue = ExtensionApi.Localize("Procon.UI.API.Squad." + locEnum.ToString());
            // Procon.Net.Protocols.Objects.TimeSubsetContext
            // Procon.Net.Protocols.Objects.PlayerSubsetContext
            else if (locEnum is TimeSubsetContext)
                locValue = ExtensionApi.Localize("Procon.UI.API.Subset.Time." + locEnum.ToString());
            else if (locEnum is PlayerSubsetContext)
                locValue = ExtensionApi.Localize("Procon.UI.API.Subset.Player." + locEnum.ToString());
            // Default
            else
                locValue = locEnum.ToString();

            return locValue;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
