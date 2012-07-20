using System;
using System.Windows;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConverterConnectionToVisibility : BaseConverter, IValueConverter
    {
        // Value = The Connection View Model.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ConnectionViewModel tValue    = value as ConnectionViewModel;
            ConnectionViewModel tSelected = ExtensionApi.Connection;
            if (tValue != null && tSelected != null)
                if (tValue.Hostname == tSelected.Hostname && tValue.Port == tSelected.Port)
                    return Visibility.Visible;
            return Visibility.Collapsed;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
