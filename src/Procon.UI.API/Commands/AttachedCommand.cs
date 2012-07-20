using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public static class AttachedCommand
    {
        // Allows for easy use of only a single event to be bound to.
        public static DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached("Event",
                typeof(String),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null));
        public static DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter",
                typeof(Object),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null));
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, CommandPropertyChanged));
        // Allows for multiple events to be bound to.
        public static DependencyProperty CommandsProperty =
            DependencyProperty.RegisterAttached("Commands",
                typeof(AttachedCommandGroup),
                typeof(AttachedCommand),
                new FrameworkPropertyMetadata(null, CommandsPropertyChanged));

        // Accessors / Mutators for the dependency properties.
        public static void SetCommand(  DependencyObject element, ICommand             item) { element.SetValue(CommandProperty,         item); }
        public static void SetEvent(    DependencyObject element, String               item) { element.SetValue(EventProperty,           item); }
        public static void SetParameter(DependencyObject element, Object               item) { element.SetValue(ParameterProperty,       item); }
        public static void SetCommands( DependencyObject element, AttachedCommandGroup item) { element.SetValue(CommandsProperty,        item); }
        public static ICommand             GetCommand(  DependencyObject element) { return (ICommand)            element.GetValue(CommandProperty);   }
        public static String               GetEvent(    DependencyObject element) { return (String)              element.GetValue(EventProperty);     }
        public static Object               GetParameter(DependencyObject element) { return (Object)              element.GetValue(ParameterProperty); }
        public static AttachedCommandGroup GetCommands( DependencyObject element) { return (AttachedCommandGroup)element.GetValue(CommandsProperty);  }

        // Cache for the proxy handlers.
        private static Dictionary<String, Delegate> mHandlers = new Dictionary<String, Delegate>();

        // Attaches a single command to the element's event.
        private static void CommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement tElement   = sender as FrameworkElement;
            String           tEventName = GetEvent(tElement);
            // If the dependency properties were set.
            if (tElement != null && tEventName != null) {
                EventInfo tEventInfo = tElement.GetType().GetEvent(tEventName);
                if (tEventInfo != null) {
                    // We're attempting to remove the previous binding.
                    if (args.OldValue != null) {
                        if (mHandlers.ContainsKey(tEventName))
                            tEventInfo.RemoveEventHandler(tElement, mHandlers[tEventName]);
                    }
                    // We're attempting to bind to a new command.
                    if (args.NewValue != null) {
                        // The command hasn't been created yet.
                        if (!mHandlers.ContainsKey(tEventName)) {
                            // Hard-code the handler to the method defined in our class.
                            Action<Object, Object> tHandler = OnEventFired;

                            // Get the event handler type and the method parent type.
                            Type tEventType   = tEventInfo.EventHandlerType;
                            Type tHandlerType = tHandler.GetType();

                            // Get all the parameters for the specified event handler type.
                            List<Type> tEventParams = new List<Type>();
                            tEventParams.Add(tHandlerType);
                            tEventParams.AddRange(tEventType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType));

                            // Create a dynamic method with the specified name, return type, and parameters.  Create the method body.
                            DynamicMethod tProxyMethod = new DynamicMethod("Proxy: " + tEventName, typeof(void), tEventParams.ToArray(), tHandlerType, true);
                            ILGenerator   tProxyBody   = tProxyMethod.GetILGenerator();

                            // Load the sender and args onto the stack, boxing the value types for saftey.
                            tProxyBody.Emit(OpCodes.Ldarg_1);
                            if (tEventParams[1].IsValueType)
                                tProxyBody.Emit(OpCodes.Box, tEventParams[1]);
                            tProxyBody.Emit(OpCodes.Ldarg_2);
                            if (tEventParams[2].IsValueType)
                                tProxyBody.Emit(OpCodes.Box, tEventParams[2]);
                            tProxyBody.Emit(OpCodes.Call, tHandler.Method);
                            tProxyBody.Emit(OpCodes.Ret);

                            // Finally, create the delegate that represents this method and add it to our cache.
                            mHandlers.Add(tEventName, tProxyMethod.CreateDelegate(tEventType, tHandler.Target));
                        }
                        // Bind to the event handler.
                        if (mHandlers[tEventName] != null)
                            tEventInfo.AddEventHandler(tElement, mHandlers[tEventName]);
                    }
                }
            }
        }
        private static void OnEventFired(Object sender, Object args)
        {
            FrameworkElement tElement   = sender as FrameworkElement;
            ICommand         tCommand   = GetCommand(tElement);
            Object           tParameter = GetParameter(tElement);
            if (tCommand != null)
                tCommand.Execute(new AttachedCommandArgs(sender, args, tParameter));
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