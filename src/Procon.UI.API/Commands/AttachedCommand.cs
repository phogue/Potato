using System;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommand
    {
        // Necessary to get the event bound to the element.
        private static readonly DependencyProperty RoutineProperty =
            DependencyProperty.RegisterAttached("Routine",
                typeof(AttachedCommandRoutine),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null));
        private static void                   SetRoutine(DependencyObject element, AttachedCommandRoutine routine)
        {
            element.SetValue(RoutineProperty, routine);
        }
        private static AttachedCommandRoutine GetRoutine(DependencyObject element)
        {
            AttachedCommandRoutine routine = (AttachedCommandRoutine)element.GetValue(RoutineProperty);
            if (routine == null) {
                routine = new AttachedCommandRoutine(element);
                SetRoutine(element, routine);
            }
            return routine;
        }

        // Allows for a single event to be bound to.
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter",
                typeof(Object),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, (s, e) => GetRoutine(s).Parameter = (Object)e.NewValue));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, (s, e) => GetRoutine(s).Command = (ICommand)e.NewValue));
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached("Event",
                typeof(String),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, EventPropertyChanged));

        // Accessors / Mutators.
        public static void SetParameter(DependencyObject element, Object   item) { element.SetValue(ParameterProperty, item); }
        public static void SetCommand(  DependencyObject element, ICommand item) { element.SetValue(CommandProperty,   item); }
        public static void SetEvent(    DependencyObject element, String   item) { element.SetValue(EventProperty,     item); }
        public static Object   GetParameter(DependencyObject element) { return (Object)   element.GetValue(ParameterProperty); }
        public static ICommand GetCommand(  DependencyObject element) { return (ICommand) element.GetValue(CommandProperty);   }
        public static String   GetEvent(    DependencyObject element) { return (String)   element.GetValue(EventProperty);     }
        
        // Adds a handler to the event in order to execute our command.
        private static void EventPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            AttachedCommandRoutine routine = GetRoutine(sender);
            routine.EventName = (String)args.NewValue;
            routine.Bind();
        }
    }
}