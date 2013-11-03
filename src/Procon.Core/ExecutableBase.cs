using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Net.Utils;

namespace Procon.Core {

    [Serializable]
    public abstract class ExecutableBase : MarshalByRefObject, INotifyPropertyChanged, IDisposable, ICloneable, IExecutableBase {

        /// <summary>
        /// List of dispatch attributes to the method to call, provided the parameter list matches.
        /// </summary>
        protected readonly Dictionary<CommandAttribute, CommandDispatchHandler> CommandDispatchHandlers = new Dictionary<CommandAttribute, CommandDispatchHandler>();

        protected delegate CommandResultArgs CommandDispatchHandler(Command command, Dictionary<String, CommandParameter> parameters);

        /// <summary>
        /// Appends a list of dispatch handlers to the internal list, updating existing handlers if they exist.
        /// </summary>
        /// <param name="handlers"></param>
        protected void AppendDispatchHandlers(Dictionary<CommandAttribute, CommandDispatchHandler> handlers) {
            foreach (var handler in handlers) {
                if (this.CommandDispatchHandlers.ContainsKey(handler.Key) == false) {
                    this.CommandDispatchHandlers.Add(handler.Key, handler.Value);
                }
                else {
                    this.CommandDispatchHandlers[handler.Key] = handler.Value;
                }
            }
        }

        /// <summary>
        /// Fired after the disposal method has been executed on this object.
        /// </summary>
        [field: NonSerialized, XmlIgnore, JsonIgnore]
        public event EventHandler Disposed;

        protected virtual void OnDisposed() {
            EventHandler handler = Disposed;

            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Allows for an optional child implementation.
        /// </summary>
        public virtual void Dispose() {
            this.OnDisposed();

            this.Disposed = null;
        }

        /// <summary>
        /// Allows for an optional child implementation.
        /// </summary>
        /// <param name="config"></param>
        public virtual void WriteConfig(Config config) { }

        /// <summary>
        /// Loads the specified configuration file.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual ExecutableBase Execute(Config config) {
            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, config);

            return this;
        }

        public virtual ExecutableBase Execute() {
            return this;
        }

        /// <summary>
        /// Finds the commands specified in the config file and invokes them with the specified attributes.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        protected void Execute(Command command, Config config) {
            if (config != null && config.Root != null) {

                // Allocate space for this objects type and the config's nodes.
                Type oType = GetType();

                // Drill down in the config to this object's type.
                var oNodes = oType.FullName.Split('`').First().Split('.').Skip(1).Aggregate(config.Root.Elements(), (current, oName) => current.DescendantsAndSelf(oName));

                // For each of the commands for this object...
                foreach (XElement xCommand in oNodes.Descendants("Command")) {
                    Command loadedCommand = xCommand.FromXElement<Command>();

                    if (loadedCommand != null && loadedCommand.Name != null) {
                        command.ParseCommandType(loadedCommand.Name);
                        command.Parameters = loadedCommand.Parameters;

                        this.Execute(command);
                    }
                }
            }
        }

        // Events.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(Object sender, String property) {
            if (PropertyChanged != null) {
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Compares an expected parameter list against the parameters supplied. If the types match (or can be converted) then
        /// a dictionary of parameter names to the parameters supplied is returned, otherwise null is returned implying
        /// and error was encountered or a type wasn't found.
        /// </summary>
        /// <param name="expectedParameterTypes"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Dictionary<String, CommandParameter> BuildParameterDictionary(IList<CommandParameterType> expectedParameterTypes, IList<CommandParameter> parameters) {
            Dictionary<String, CommandParameter> parameterDictionary = new Dictionary<string, CommandParameter>();

            // If we're not expecting any parameters
            if (expectedParameterTypes != null) {
                if (parameters != null && expectedParameterTypes.Count == parameters.Count) {
                    for (int offset = 0; offset < expectedParameterTypes.Count && parameterDictionary != null; offset++) {

                        if (expectedParameterTypes[offset].IsList == true) {
                            if (parameters[offset].HasMany(expectedParameterTypes[offset].Type, expectedParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(expectedParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                        else {
                            if (parameters[offset].HasOne(expectedParameterTypes[offset].Type, expectedParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(expectedParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                    }
                }
                else {
                    // Parameter count mismatch. Return null.
                    parameterDictionary = null;
                }
            }

            return parameterDictionary;
        }

        private CommandResultArgs Run(CommandAttributeType attributeType, Command command, CommandResultType maintainStatus) {

            // Loop through all matching commands with the identical name and type
            foreach (var dispatch in this.CommandDispatchHandlers.Where(dispatch => dispatch.Key.CommandAttributeType == attributeType && dispatch.Key.Name == command.Name)) {
                
                // Check if we can build a parameter list.
                Dictionary<String, CommandParameter> parameters = this.BuildParameterDictionary(dispatch.Key.ParameterTypes, command.Parameters);

                if (parameters != null) {
                    command.Result = dispatch.Value(command, parameters);

                    // Our status has changed, break our loop.
                    if (command.Result.Status != maintainStatus) {
                        break;
                    }
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunPreview(Command command) {
            command.Result = this.Run(CommandAttributeType.Preview, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> bubbleList = this.BubbleExecutableObjects(command);

                for (int offset = 0; offset < bubbleList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    command.Result = bubbleList[offset].RunPreview(command);
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunHandler(Command command) {
            command.Result = this.Run(CommandAttributeType.Handler, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> bubbleList = this.BubbleExecutableObjects(command);

                for (int offset = 0; offset < bubbleList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    command.Result = bubbleList[offset].RunHandler(command);
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunExecuted(Command command) {
            command.Result = this.Run(CommandAttributeType.Executed, command, command.Result.Status);

            IList<IExecutableBase> bubbleList = this.BubbleExecutableObjects(command);

            foreach (ExecutableBase executable in bubbleList) {
                command.Result = executable.RunExecuted(command);
            }

            return command.Result;
        }

        /// <summary>
        /// Executes a command against this object, provided the command attribute matches as well as the types of each parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Execute(Command command) {
            // Setup the initial command result.
            command.Result = new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Continue
            };

            command.Result = this.RunPreview(command);

            if (command.Result.Status == CommandResultType.Continue) {
                command.Result = this.RunHandler(command);

                command.Result = this.RunExecuted(command);
            }

            return command.Result;
        }

        protected virtual IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return new List<IExecutableBase>();
        } 

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
