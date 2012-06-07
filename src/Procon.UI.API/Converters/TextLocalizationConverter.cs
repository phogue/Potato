using System;
using System.Windows.Data;

namespace Procon.UI.API.Converters
{
    public class TextLocalizationConverter : IValueConverter
    {
        // Value     = The key of the localized text (via a binding).
        // Parameter = The key of the localized text (via a command parameter).
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String locKey = (parameter as String);
            if (locKey != null && locKey is String)
                return ExtensionApi.Localize(locKey);

            locKey = (value as String);
            if (locKey != null && locKey is String)
                return ExtensionApi.Localize(locKey.Contains("Procon.UI.") ? locKey : "Procon.UI." + locKey);

            return String.Empty;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
