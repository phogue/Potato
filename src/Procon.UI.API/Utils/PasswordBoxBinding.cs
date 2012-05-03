using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Procon.UI.API.Utils
{
    public static class PasswordBoxBinding
    {
        // PasswordBox - Allows binding to "Password".
        public static DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(ICommand),
                typeof(PasswordBoxBinding),
                new PropertyMetadata(null, PasswordPropertyChanged));

        // PasswordBox - Getters and Setters.
        public static ICommand GetPassword(DependencyObject o)
        {
            return (ICommand)o.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject o, ICommand i)
        {
            o.SetValue(PasswordProperty, i);
        }

        // PasswordBox - Handles when the password is changed.
        private static void PasswordPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox tElement = o as PasswordBox;
            if (tElement == null)
                return;

            ICommand tOldCommand = e.OldValue as ICommand;
            ICommand tNewCommand = e.NewValue as ICommand;
            if (tOldCommand == null && tNewCommand != null)
                tElement.PasswordChanged += (s, a) => {
                    tNewCommand.Execute(tElement.Password);
            };
        }
    }
}
