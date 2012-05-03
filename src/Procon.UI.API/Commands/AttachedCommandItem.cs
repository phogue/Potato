using System;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandItem : Freezable
    {
        // The command to be called when the routed event is raised, passed with an optional custom parameter.
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command",
                typeof(ICommand),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty RoutedEventProperty =
            DependencyProperty.Register("RoutedEvent",
                typeof(RoutedEvent),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty CustomParameterProperty =
            DependencyProperty.Register("CustomParameter",
                typeof(Object),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));

        // Accessors / Mutators for the dependency properties.
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public RoutedEvent RoutedEvent
        {
            get { return (RoutedEvent)GetValue(RoutedEventProperty); }
            set { SetValue(RoutedEventProperty, value); }
        }
        public Object CustomParameter
        {
            get { return (Object)GetValue(CustomParameterProperty); }
            set { SetValue(CustomParameterProperty, value); }
        }

        // Attaches the event handler to it's appropriate command.
        public void AttachItem(FrameworkElement element)
        {
            if (!mAttached && Command != null) {
                element.AddHandler(RoutedEvent, (RoutedEventHandler)((s, e) => Command.Execute(new AttachedCommandArgs(s, e, CustomParameter))));
                mAttached = true;
            }
        }
        private Boolean mAttached = false;

        // Freezable stuff.
        protected override Freezable CreateInstanceCore()
        {
            return new AttachedCommandItem();
        }
    }
}
