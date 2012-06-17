using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Procon.Net;
using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class StatusTypeConverter : IValueConverter
    {
        // Value = The status to get the icon for.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ConnectionState)
                switch ((ConnectionState)value) {
                    case ConnectionState.LoggedIn:
                        return InstanceViewModel.PublicProperties["Images"]["Status"]["Good"].Value as BitmapImage;
                    case ConnectionState.Connecting:
                    case ConnectionState.Connected:
                    case ConnectionState.Ready:
                        return InstanceViewModel.PublicProperties["Images"]["Status"]["Flux"].Value as BitmapImage;
                    case ConnectionState.Disconnecting:
                    case ConnectionState.Disconnected:
                        return InstanceViewModel.PublicProperties["Images"]["Status"]["Bad"].Value as BitmapImage;
                }
            return new BitmapImage();
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
