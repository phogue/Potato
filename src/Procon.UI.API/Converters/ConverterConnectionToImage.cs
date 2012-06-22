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
                if (ViewModelBase.PublicProperties["Images"]["Connections"].ContainsKey(tValue) && ViewModelBase.PublicProperties["Images"]["Connections"][tValue].Value != null)
                    return ViewModelBase.PublicProperties["Images"]["Connections"][tValue].Value;
                return ViewModelBase.PublicProperties["Images"]["Connections"]["Unknown"].Value;
            }
            return ViewModelBase.PublicProperties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
