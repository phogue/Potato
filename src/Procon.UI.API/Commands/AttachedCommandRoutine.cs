using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandRoutine
    {
        // The control we're looking at.
        public DependencyObject Element { get; internal set; }
        
        // The routine's properties to match up with the command's dependency properties.
        public Object    Parameter { get; set; }
        public ICommand  Command   { get; set; }
        public String    EventName { get; set; }

        // The event and the handler bound to it.
        private EventInfo mEvent;
        private Delegate  mMethod;


        // Constructor.
        public AttachedCommandRoutine(DependencyObject element)
        {
            Element = element;
        }

        // Binds to the event.
        public void Bind()
        {
            // Remove old binding first.
            if (mMethod != null) {
                mEvent.RemoveEventHandler(Element, mMethod);
                mMethod = null;
            }

            // Add new binding next.
            if (Element != null && EventName != null) {
                mEvent = Element.GetType().GetEvent(EventName);

                // Create the method for the event.
                if (mEvent != null) {
                    // Hard-code the handler to the method defined in our class.
                    Action<Object[]> tHandler = OnEventFired;

                    // Get the event type and the event's invoke method.
                    Type tEventType = mEvent.EventHandlerType;
                    MethodInfo tEventInfo = tEventType.GetMethod("Invoke");

                    // Get all the parameters for the specified event handler type.
                    if (tEventInfo.ReturnParameter.ParameterType == typeof(void)) {
                        List<Type> tEventParams = new List<Type>();
                        tEventParams.Add(typeof(AttachedCommandRoutine));
                        tEventParams.AddRange(tEventInfo.GetParameters().Select(x => x.ParameterType));

                        // Setup the wrapper method to be called.
                        DynamicMethod tWrapper = new DynamicMethod("", typeof(void), tEventParams.ToArray(), typeof(AttachedCommandRoutine), true);
                        ILGenerator tILGen = tWrapper.GetILGenerator();

                        // Setup an array to hold all the parameters.
                        LocalBuilder tParams = tILGen.DeclareLocal(typeof(object[]));
                        tILGen.Emit(OpCodes.Ldc_I4, tEventParams.Count);
                        tILGen.Emit(OpCodes.Newarr, typeof(object));
                        tILGen.Emit(OpCodes.Stloc, tParams);

                        // Fill the array with the parameters (skipping the instance).
                        for (int i = 1; i < tEventParams.Count; i++) {
                            tILGen.Emit(OpCodes.Ldloc, tParams);
                            tILGen.Emit(OpCodes.Ldc_I4, i);
                            tILGen.Emit(OpCodes.Ldarg, i);
                            tILGen.Emit(OpCodes.Stelem_Ref);
                        }

                        // Load the sender and args onto the stack and call the handler.
                        tILGen.Emit(OpCodes.Ldarg_0);
                        tILGen.Emit(OpCodes.Ldloc, tParams);
                        tILGen.EmitCall(OpCodes.Call, tHandler.Method, null);

                        // Clean up the stack and exit the method.
                        tILGen.Emit(OpCodes.Ret);

                        // Set the handler.
                        mMethod = tWrapper.CreateDelegate(tEventType, tHandler.Target);
                        mEvent.AddEventHandler(Element, mMethod);
                    }
                }
            }
        }

        // Executes the command whenever the event is fired.
        private void OnEventFired(Object[] args)
        {
            if (Command != null)
                Command.Execute(new AttachedCommandArgs(args[1], args[2], Parameter));
        }
    }
}