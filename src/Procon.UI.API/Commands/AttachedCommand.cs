using System;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public static class AttachedCommand
    {
        // Allows for easy use of only a single event to be bound to.
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, CommandPropertyChanged));
        public static DependencyProperty RoutedEventProperty =
            DependencyProperty.RegisterAttached("RoutedEvent",
                typeof(RoutedEvent),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null));
        public static DependencyProperty CustomParameterProperty =
            DependencyProperty.RegisterAttached("CustomParameter",
                typeof(Object),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null));
        // Allows for multiple events to be bound to.
        public static DependencyProperty CommandsProperty =
            DependencyProperty.RegisterAttached("Commands",
                typeof(AttachedCommandGroup),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, CommandsPropertyChanged));

        // Accessors / Mutators for the dependency properties.
        public static void SetCommand(        DependencyObject element, ICommand             item) { element.SetValue(CommandProperty,         item); }
        public static void SetRoutedEvent(    DependencyObject element, RoutedEvent          item) { element.SetValue(RoutedEventProperty,     item); }
        public static void SetCustomParameter(DependencyObject element, Object               item) { element.SetValue(CustomParameterProperty, item); }
        public static void SetCommands(       DependencyObject element, AttachedCommandGroup item) { element.SetValue(CommandsProperty,        item); }
        public static ICommand             GetCommand(        DependencyObject element) { return (ICommand)            element.GetValue(CommandProperty);         }
        public static RoutedEvent          GetRoutedEvent(    DependencyObject element) { return (RoutedEvent)         element.GetValue(RoutedEventProperty);     }
        public static Object               GetCustomParameter(DependencyObject element) { return (Object)              element.GetValue(CustomParameterProperty); }
        public static AttachedCommandGroup GetCommands(       DependencyObject element) { return (AttachedCommandGroup)element.GetValue(CommandsProperty);        }

        // Attaches a single command to the element's event.
        private static void CommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement tElement    = sender        as FrameworkElement;
            ICommand         tOldCommand = args.OldValue as ICommand;
            ICommand         tNewCommand = args.NewValue as ICommand;
            if (tElement != null && tOldCommand == null && tNewCommand != null)
                tElement.AddHandler(GetRoutedEvent(tElement), (RoutedEventHandler)((s, e) => tNewCommand.Execute(new AttachedCommandArgs(s, e, GetCustomParameter(tElement)))));
        }

        // Attaches multiple commands to the element's various events.
        private static void CommandsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement     tElement = sender as FrameworkElement;
            AttachedCommandGroup tItem    = args.NewValue as AttachedCommandGroup;
            if (tElement != null && tItem != null)
                tItem.AttachItems(tElement);
        }
    }
}