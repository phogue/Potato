using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Procon.Setup.Converters {
    /// <summary>
    /// Switches the boolean, true = false, false = true. Used for a psuedo "IsDisabled" attribute.
    /// </summary>
    public class IsDisabledConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool v = (bool)value;
            return !v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
