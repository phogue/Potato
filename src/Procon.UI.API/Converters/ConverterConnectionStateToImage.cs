using System;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConverterConnectionStateToImage : BaseConverter, IValueConverter
    {
        // Value = The Connection State.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Procon.Net.ConnectionState) {
                String tValue = value.ToString();
                if (ExtensionApi.Properties["Images"]["Status"].ContainsKey(tValue) && ExtensionApi.Properties["Images"]["Status"][tValue].Value != null)
                    return ExtensionApi.Properties["Images"]["Status"][tValue].Value;
                return ExtensionApi.Properties["Images"]["Status"]["Unknown"].Value;
            }
            return ExtensionApi.Properties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
