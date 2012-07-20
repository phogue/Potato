using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandItem : Freezable
    {
        // The command to be called when the routed event is raised, passed with an optional custom parameter.
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event",
                typeof(String),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter",
                typeof(Object),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command",
                typeof(ICommand),
                typeof(AttachedCommandItem),
                new FrameworkPropertyMetadata(null));

        // Accessors / Mutators for the dependency properties.
        public String   Event
        {
            get { return (String)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }
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

        // Attaches the event handler to it's appropriate command.
        public void AttachItem(FrameworkElement element)
        {
            FrameworkElement tElement   = element as FrameworkElement;
            String           tEventName = Event;
            // If the dependency properties were set.
            if (!mAttached && tElement != null && tEventName != null) {
                EventInfo tEventInfo = tElement.GetType().GetEvent(tEventName);
                // If the event was found.
                if (tEventInfo != null) {
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

                    // Finally, create the delegate that represents this method and bind it to the event.
                    tEventInfo.AddEventHandler(tElement, tProxyMethod.CreateDelegate(tEventType, tHandler.Target));
                    mAttached = true;
                }
            }
        }
        private void OnEventFired(Object sender, Object args)
        {
            FrameworkElement tElement   = sender as FrameworkElement;
            ICommand         tCommand   = Command;
            Object           tParameter = Parameter;
            if (tCommand != null)
                tCommand.Execute(new AttachedCommandArgs(sender, args, tParameter));
        }
        private Boolean mAttached = false;

        // Freezable stuff.
        protected override Freezable CreateInstanceCore()
        {
            return new AttachedCommandItem();
        }
    }
}
