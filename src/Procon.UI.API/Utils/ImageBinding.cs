using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Procon.UI.API.Utils
{
    public static class ImageBinding
    {
        // Image - Allows binding to "CountryCode".
        public static DependencyProperty CountryCodeProperty =
            DependencyProperty.RegisterAttached(
                "CountryCode",
                typeof(String),
                typeof(ImageBinding),
                new PropertyMetadata(null, CountryCodeChanged));

        // Image - Getters and Setters.
        public static String GetCountryCode(DependencyObject o)
        {
            return (String)o.GetValue(CountryCodeProperty);
        }
        public static void   SetCountryCode(DependencyObject o, String i)
        {
            o.SetValue(CountryCodeProperty, i);
        }

        // Image - Handles when the country code is changed.
        private static void CountryCodeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Image tElement = o as Image;
            if (tElement == null)
                return;

            if (GetCountryCode(tElement) != null)
                tElement.Source = ExtensionApi.Properties["Images"]["Countries"][GetCountryCode(tElement)].Value as BitmapImage;
            else
                tElement.Source = ExtensionApi.Properties["Images"]["Countries"]["UNK"].Value as BitmapImage;
        }
    }
}
