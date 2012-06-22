using System;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConverterInterfaceToImage : BaseConverter, IValueConverter
    {
        // Value = The Interface View Model.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is InterfaceViewModel)
                return ViewModelBase.PublicProperties["Images"]["Interfaces"][((InterfaceViewModel)value).IsLocal ? "Local" : "Remote"].Value;
            return ViewModelBase.PublicProperties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
