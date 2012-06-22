using System;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConverterCountryCodeToImage : BaseConverter, IValueConverter
    {
        // Value = The Country Code String.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String) {
                String tValue = (value.ToString().Length > 2) ? value.ToString().ToUpper().Remove(2) : value.ToString().ToUpper();
                if (ViewModelBase.PublicProperties["Images"]["Countries"].ContainsKey(tValue) && ViewModelBase.PublicProperties["Images"]["Countries"][tValue].Value != null)
                    return ViewModelBase.PublicProperties["Images"]["Countries"][tValue].Value;
                return ViewModelBase.PublicProperties["Images"]["Countries"]["UNK"].Value;
            }
            return ViewModelBase.PublicProperties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
