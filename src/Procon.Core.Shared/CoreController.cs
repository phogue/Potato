using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Shared {
    /// <summary>
    /// Handles command routing and config handling
    /// </summary>
    [Serializable]
    public abstract class CoreController : MarshalByRefObject, INotifyPropertyChanged, IDisposable, ICloneable, ICoreController {
        /// <summary>
        /// List of dispatch attributes to the method to call, provided the parameter list matches.
        /// </summary>
        protected readonly Dictionary<CommandAttribute, CommandDispatchHandler> CommandDispatchHandlers = new Dictionary<CommandAttribute, CommandDispatchHandler>();

        protected delegate CommandResultArgs CommandDispatchHandler(Command command, Dictionary<String, CommandParameter> parameters);

        /// <summary>
        /// All objects to tunnel downwards
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<ICoreController> TunnelObjects { get; set; }

        /// <summary>
        /// All objects to bubble upwards
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<ICoreController> BubbleObjects { get; set; }

        protected CoreController() : base() {
            this.TunnelObjects = new List<ICoreController>();
            this.BubbleObjects = new List<ICoreController>();
        }

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

            if (this.BubbleObjects != null) this.BubbleObjects.Clear();
            this.BubbleObjects = null;

            if (this.TunnelObjects != null) this.TunnelObjects.Clear();
            this.TunnelObjects = null;

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
        public virtual ICoreController Execute(Config config) {
            this.TunnelObjects = this.TunnelObjects ?? new List<ICoreController>();
            this.BubbleObjects = this.BubbleObjects ?? new List<ICoreController>();

            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, config);

            return this;
        }

        /// <summary>
        /// Called after the constructor is called
        /// </summary>
        /// <returns></returns>
        public virtual ICoreController Execute() {
            this.TunnelObjects = this.TunnelObjects ?? new List<ICoreController>();
            this.BubbleObjects = this.BubbleObjects ?? new List<ICoreController>();

            return this;
        }

        /// <summary>
        /// Finds the commands specified in the config file and invokes them with the specified attributes.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="config"></param>
        protected void Execute(Command command, Config config) {
            if (config != null && config.Root != null) {

                // Drill down in the config to this object's type.
                var nodes = this.GetType().FullName.Split('`').First().Split('.').Skip(1).Aggregate(config.Root.Elements(), (current, name) => current.DescendantsAndSelf(name));

                // For each of the commands for this object...
                foreach (XElement xCommand in nodes.Descendants("Command")) {
                    Command loadedCommand = xCommand.FromXElement<Command>();

                    if (loadedCommand != null && loadedCommand.Name != null) {
                        command.ParseCommandType(loadedCommand.Name);
                        command.Parameters = loadedCommand.Parameters;
                        command.Scope = loadedCommand.Scope;

                        this.Tunnel(command);
                    }
                }
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

        public virtual CommandResultArgs PropogatePreview(Command command, CommandDirection direction) {
            command.Result = this.Run(CommandAttributeType.Preview, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<ICoreController> propogationList = direction == CommandDirection.Tunnel ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

                for (int offset = 0; propogationList != null && offset < propogationList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    if (propogationList[offset] != null) {
                        command.Result = propogationList[offset].PropogatePreview(command, direction);
                    }
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs PropogateHandler(Command command, CommandDirection direction) {
            command.Result = this.Run(CommandAttributeType.Handler, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<ICoreController> propogationList = direction == CommandDirection.Tunnel ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

                for (int offset = 0; propogationList != null && offset < propogationList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    if (propogationList[offset] != null) {
                        command.Result = propogationList[offset].PropogateHandler(command, direction);
                    }
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs PropogateExecuted(Command command, CommandDirection direction) {
            command.Result = this.Run(CommandAttributeType.Executed, command, command.Result.Status);

            IList<ICoreController> propogationList = direction == CommandDirection.Tunnel ? this.TunnelExecutableObjects(command) : this.BubbleExecutableObjects(command);

            if (propogationList != null) {
                foreach (CoreController executable in propogationList) {
                    if (executable != null) {
                        command.Result = executable.PropogateExecuted(command, direction);
                    }
                }
            }

            return command.Result;
        }

        /// <summary>
        /// Executes a command against this object, provided the command attribute matches as well as the types of each parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Tunnel(Command command) {
            // Setup the initial command result.
            command.Result = new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Continue
            };

            command.Result = this.PropogatePreview(command, CommandDirection.Tunnel);

            if (command.Result.Status == CommandResultType.Continue) {
                command.Result = this.PropogateHandler(command, CommandDirection.Tunnel);

                command.Result = this.PropogateExecuted(command, CommandDirection.Tunnel);
            }
            // If the preview stole the command and executed it, let everyone know it has been executed.
            else if (command.Result.Status == CommandResultType.Success) {
                command.Result = this.PropogateExecuted(command, CommandDirection.Tunnel);
            }

            return command.Result;
        }

        /// <summary>
        /// Execute a command, then bubble it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Bubble(Command command) {
            // Setup the initial command result.
            command.Result = new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Continue
            };

            command.Result = this.PropogatePreview(command, CommandDirection.Bubble);

            if (command.Result.Status == CommandResultType.Continue) {
                command.Result = this.PropogateHandler(command, CommandDirection.Bubble);

                command.Result = this.PropogateExecuted(command, CommandDirection.Bubble);
            }
            // If the preview stole the command and executed it, let everyone know it has been executed.
            else if (command.Result.Status == CommandResultType.Success) {
                command.Result = this.PropogateExecuted(command, CommandDirection.Bubble);
            }

            return command.Result;
        }

        protected virtual IList<ICoreController> TunnelExecutableObjects(Command command) {
            return this.TunnelObjects;
        }

        protected virtual IList<ICoreController> BubbleExecutableObjects(Command command) {
            return this.BubbleObjects;
        } 

        public object Clone() {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Event for whenever a property is modified on this executable object
        /// </summary>
        /// <remarks>I think this is only used for variables, which I would like to move specifically to
        /// the variables controlle. There is no need for other variables to use this functionality.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="property"></param>
        protected void OnPropertyChanged(Object sender, String property) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(sender, new PropertyChangedEventArgs(property));
            }
        }
    }
}
