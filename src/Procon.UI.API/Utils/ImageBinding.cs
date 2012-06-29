using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Procon.UI.API.ViewModels;

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
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(ImageBinding),
                new PropertyMetadata(null));
        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(Object),
                typeof(ImageBinding),
                new PropertyMetadata(null, CommandParameterChanged));

        // Image - Getters and Setters.
        public static String   GetCountryCode(DependencyObject o)
        {
            return (String)o.GetValue(CountryCodeProperty);
        }
        public static ICommand GetCommand(DependencyObject o)
        {
            return (ICommand)o.GetValue(CommandProperty);
        }
        public static Object   GetCommandParameter(DependencyObject o)
        {
            return (Object)o.GetValue(CommandParameterProperty);
        }
        public static void SetCountryCode(DependencyObject o, String i)
        {
            o.SetValue(CountryCodeProperty, i);
        }
        public static void SetCommand(DependencyObject o, ICommand i)
        {
            o.SetValue(CommandProperty, i);
        }
        public static void SetCommandParameter(DependencyObject o, Object i)
        {
            o.SetValue(CommandParameterProperty, i);
        }

        // Image - Handles when the country code is changed.
        private static void CountryCodeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Image tElement = o as Image;
            if (tElement == null)
                return;

            tElement.Source = ExtensionApi.Properties["Images"]["Countries"][GetCountryCode(tElement)].Value as BitmapImage;
        }
        private static void CommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Image tElement = o as Image;
            if (tElement == null)
                return;

            ICommand tCommand = GetCommand(o);
            if (tCommand == null)
                return;

            tCommand.Execute(new Object[] { tElement, e.NewValue });
        }
    }
}
