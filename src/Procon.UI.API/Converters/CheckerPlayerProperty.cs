using System;
using System.Windows.Data;

using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class CheckerPlayerProperty : BaseConverter, IValueConverter
    {
        // Value     = The value of the property of the player.
        // Parameter = The property name of the player.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && parameter != null && parameter is String) {
                Double tLimit = (Double)ViewModelBase.PublicProperties["Main"]["Players"]["List"][(String)parameter].Value;
                if ((value is Int32 && (Int32)value >= tLimit) || (value is Double && (Double)value >= tLimit))
                    return ViewModelBase.PublicProperties["Images"]["General"]["Warn"].Value;
            }
            return ViewModelBase.PublicProperties["Images"]["Empty"].Value;
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
