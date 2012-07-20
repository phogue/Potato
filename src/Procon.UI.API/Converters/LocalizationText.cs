using System;
using System.Windows.Data;

namespace Procon.UI.API.Converters
{
    public class LocalizationText : BaseConverter, IValueConverter
    {
        // Value     = The key of the localized text.
        // Parameter = The namespace of the localized text.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String locKey       = (value     as String);
            String locNamespace = (parameter as String);

            if      (locKey != null && locNamespace != null) return ExtensionApi.Localize("Procon.UI." + locNamespace + "." + locKey);
            else if (locKey != null && locNamespace == null) return ExtensionApi.Localize("Procon.UI." + locKey);
            else if (locKey == null && locNamespace != null) return ExtensionApi.Localize("Procon.UI." + locNamespace);
            return String.Empty;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
