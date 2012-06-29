using System;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConverterConnectionToImage : BaseConverter, IValueConverter
    {
        // Value = The Connection View Model.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectionViewModel) {
                String tValue = ((ConnectionViewModel)value).GameType.ToString();
                if (ExtensionApi.Properties["Images"]["Connections"].ContainsKey(tValue) && ExtensionApi.Properties["Images"]["Connections"][tValue].Value != null)
                    return ExtensionApi.Properties["Images"]["Connections"][tValue].Value;
                return ExtensionApi.Properties["Images"]["Connections"]["Unknown"].Value;
            }
            return ExtensionApi.Properties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
