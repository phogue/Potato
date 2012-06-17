using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class InterfaceTypeConverter : IValueConverter
    {
        // Value = The interface to get the icon for.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean) {
                if ((Boolean)value)
                    return ViewModelBase.PublicProperties["Images"]["Interfaces"]["Local"].Value as BitmapImage;
                return ViewModelBase.PublicProperties["Images"]["Interfaces"]["Remote"].Value as BitmapImage;
            }
            return new BitmapImage();
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
