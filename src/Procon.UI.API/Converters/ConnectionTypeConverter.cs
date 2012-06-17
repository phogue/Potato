using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Procon.Net.Protocols;
using Procon.UI.API.ViewModels;

namespace Procon.UI.API.Converters
{
    public class ConnectionTypeConverter : IValueConverter
    {
        // Value = The connection to get the icon for.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is GameType)
                switch ((GameType)value) {
                    case GameType.BF_3:
                        return ViewModelBase.PublicProperties["Images"]["Connections"]["BF_3"].Value as BitmapImage;
                    case GameType.BF_BC2:
                        return ViewModelBase.PublicProperties["Images"]["Connections"]["BF_BC2"].Value as BitmapImage;
                    case GameType.COD_BO:
                        return ViewModelBase.PublicProperties["Images"]["Connections"]["COD_BO"].Value as BitmapImage;
                    case GameType.Homefront:
                        return ViewModelBase.PublicProperties["Images"]["Connections"]["HOMEFRONT"].Value as BitmapImage;
                    case GameType.MOH_2010:
                        return ViewModelBase.PublicProperties["Images"]["Connections"]["MOH_2010"].Value as BitmapImage;
            }
            return new BitmapImage();
        }

        // Invalid.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}
