using System;
using System.Windows.Data;

namespace Procon.UI.API.Converters
{
    public class LocalizationEnum : BaseConverter, IValueConverter
    {
        // Value     = The enum to localize.
        // Parameter = The namespace of the localized text.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Enum   locKey       = (value     as Enum);
            String locNamespace = (parameter as String);

            if      (locKey != null && locNamespace != null) return ExtensionApi.Localize(locNamespace + "." + locKey.ToString());
            else if (locKey != null && locNamespace == null) return ExtensionApi.Localize("Procon.UI." + locKey.ToString());
            return String.Empty;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
