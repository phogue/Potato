using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Converters
{
    public class ScoreConverter : IValueConverter
    {
        // Value = The score of the player.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try {
                Double tLimit = (Double)ViewModelBase.PublicProperties["Main"]["Players"]["List"]["Score"].Value;
                if ((value is Int32 && (Int32)value >= tLimit) || (value is Double && (Double)value >= tLimit))
                    return ViewModelBase.PublicProperties["Images"]["General"]["Warn"].Value as BitmapImage;
            } catch (Exception) { }

            return new BitmapImage();
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
