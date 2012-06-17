using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Procon.UI.API.Converters
{
    public class CountryConverter : IValueConverter
    {
        // Value = The status to get the icon for.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try               { return new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/" + value.ToString().ToLower() + ".png")); }
            catch (Exception) { return new BitmapImage(); }
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
