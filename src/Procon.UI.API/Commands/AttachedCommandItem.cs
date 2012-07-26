using System;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandItem : Freezable
    {
        // Necessary to get the event bound to the element.
        internal AttachedCommandRoutine Routine = new AttachedCommandRoutine(null);

        // Allows for a single event to be bound to.
        public static DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter",
                typeof(Object),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null, (s, e) => { ((AttachedCommandItem)s).Routine.Parameter = (Object)e.NewValue; }));
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null, (s, e) => { ((AttachedCommandItem)s).Routine.Command = (ICommand)e.NewValue; }));
        public static DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached("Event",
                typeof(String),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null, EventPropertyChanged));

        // Accessors / Mutators.
        public Object   Parameter
        {
            get { return (Object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public String   Event
        {
            get { return (String)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        // Adds a handler to the event in order to execute our command.
        private static void EventPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            AttachedCommandRoutine routine = ((AttachedCommandItem)sender).Routine;
            routine.EventName = (String)args.NewValue;
            routine.Bind();
        }


        // Freezable Stuff.
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}