using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core {
    using Procon.Core.Utils;

    [Serializable]
    public abstract class ExecutableBase : MarshalByRefObject, INotifyPropertyChanged, IDisposable, ICloneable, IExecutableBase {

        internal static readonly Dictionary<Type, Dictionary<MethodInfo, String>> CachedCommandPreviewMethods = new Dictionary<Type, Dictionary<MethodInfo, String>>();
        internal static readonly Dictionary<Type, Dictionary<MethodInfo, String>> CachedCommandHandlerMethods = new Dictionary<Type, Dictionary<MethodInfo, String>>();
        internal static readonly Dictionary<Type, Dictionary<MethodInfo, String>> CachedCommandExecutedMethods = new Dictionary<Type, Dictionary<MethodInfo, String>>();

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

        // Attempts to get a method by searching the type for the method who has the specified command attribute.
        private Boolean TryGetCommandMethods(IDictionary<Type, Dictionary<MethodInfo, String>> cachedMethodList, CommandAttributeType attributeType, String commandName, out List<MethodInfo> commandMethods) {
            // Init commands if it has not been initialized yet.
            if (cachedMethodList.ContainsKey(this.GetType()) == false) {
                cachedMethodList.Add(this.GetType(), new Dictionary<MethodInfo, String>());

                foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
                    CommandAttribute attribute = method.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute;

                    if (attribute != null && attribute.CommandAttributeType == attributeType && cachedMethodList[this.GetType()].ContainsKey(method) == false) {
                        cachedMethodList[this.GetType()].Add(method, attribute.Name);
                    }
                }
            }

            commandMethods = cachedMethodList.Where(type => type.Key == this.GetType())
                                             .SelectMany(type => type.Value)
                                             .Where(method => method.Value == commandName)
                                             .Select(method => method.Key)
                                             .ToList();

            // Return whether we were successful.
            return commandMethods.Count > 0;
        }

        // Events.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(Object sender, String property) {
            if (PropertyChanged != null) {
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
            }
        }

        private CommandResultArgs Run(IDictionary<Type, Dictionary<MethodInfo, String>> cachedMethodList, CommandAttributeType attributeType, Command command, CommandResultType maintainStatus) {
            List<MethodInfo> methods = null;
            
            // Check if we have any commands that have an attribute Name identical to our command.
            if (this.TryGetCommandMethods(cachedMethodList, attributeType, command.Name, out methods) == true) {
                // Loop over the available methods (there may be multiple with different signatures, or identical but one passes itself on execution)
                // Keep looping through the methods while our status remains the same as when we started (break when a method does something)
                for (int offset = 0; offset < methods.Count && command.Result.Status == maintainStatus; offset++) {

                    // This is a new list of parameters that we can alter, without altering what is in the command.
                    // It's also the result of being parsed in IsMethodParametersMatch which will determine if
                    // the method signatures match and pull the list of parameters needed to be passed in when Invoked.
                    List<Object> parsedParameter = command.Parameters != null && command.Parameters.Count > 0 ? Enumerable.Repeat<Object>(null, command.Parameters.Count).ToList() : new List<Object>();

                    // If the method can be matched to our provided parameters..
                    if (ExecutableBase.IsMethodParametersMatch(methods[offset], command.Parameters ?? new List<CommandParameter>(), parsedParameter) == true) {
                        // Insert the initial command 
                        parsedParameter.Insert(0, command);

                        command.Result = (CommandResultArgs) methods[offset].Invoke(this, parsedParameter.ToArray());
                    }
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunPreview(Command command) {
            command.Result = this.Run(ExecutableBase.CachedCommandPreviewMethods, CommandAttributeType.Preview, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> bubbleList = this.BubbleExecutableObjects(command);

                for (int offset = 0; offset < bubbleList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    command.Result = bubbleList[offset].RunPreview(command);
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunHandler(Command command) {
            command.Result = this.Run(ExecutableBase.CachedCommandHandlerMethods, CommandAttributeType.Handler, command, CommandResultType.Continue);

            if (command.Result.Status == CommandResultType.Continue) {
                IList<IExecutableBase> bubbleList = this.BubbleExecutableObjects(command);

                for (int offset = 0; offset < bubbleList.Count && command.Result.Status == CommandResultType.Continue; offset++) {
                    command.Result = bubbleList[offset].RunHandler(command);
                }
            }

            return command.Result;
        }

        public virtual CommandResultArgs RunExecuted(Command command) {
            command.Result = this.Run(ExecutableBase.CachedCommandExecutedMethods, CommandAttributeType.Executed, command, command.Result.Status);

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

        private static bool IsMethodParametersMatch(MethodInfo method, IList<CommandParameter> parameters, IList<Object> parsedParameters) {

            bool isMatch = true;

            List<ParameterInfo> parameterInfo = method.GetParameters().Skip(1).ToList();

            if (parameters.Count == parameterInfo.Count) {
                for (int offset = 0; offset < parameterInfo.Count && offset < parameters.Count && isMatch == true; offset++) {

                    try {

                        if (parameters[offset] != null) {
                            Type[] genericArgumentTypes = parameterInfo[offset].ParameterType.GetGenericArguments();

                            // If whatever the value is has a single generic type, is assignable to a collection and we have some data
                            // of the generic type of the collection..
                            // Note: No conversion because we don't know the types at compile time (though we should if we removed this stupid reflection).
                            //       Since we don't know the types then we can fetch a List<Object> of the converted type, but we can't fetch a List<Int> even
                            //       though the Object's in the List<Object> will actually be converted to integer, we still won't be able to pass it into
                            //       the parameter of the method.
                            if (genericArgumentTypes.Length == 1 && typeof(ICollection).IsAssignableFrom(parameterInfo[offset].ParameterType) && parameters[offset].HasOne(genericArgumentTypes[0], false) == true) {
                                // Return the entire collection, which while it's returned as an Object it will be whatever is in the CommandParameter data for this type.
                                parsedParameters[offset] = parameters[offset].All(genericArgumentTypes[0]);
                            }
                            // If the parameter is "System.Object". I think this is a good way to test beside string matching?
                            else if (parameterInfo[offset].ParameterType.BaseType == null) {
                                // Do we have multiple strings?
                                if (parameters[offset].HasMany<String>() == true) {
                                    parsedParameters[offset] = parameters[offset].All<String>();
                                }
                                else {
                                    parsedParameters[offset] = parameters[offset].First<String>();
                                }
                            }
                            else if (parameters[offset].HasOne(parameterInfo[offset].ParameterType) == true) {
                                parsedParameters[offset] = parameters[offset].First(parameterInfo[offset].ParameterType);
                            }
                        }

                        // Results in a null for this parsed parameter, which isn't a match.

                        // We don't allow null parameters with commands. Given the type conversions it can get a little hinky.
                        // Yeah I said Hinky. That's how real shit can get.
                        if (parsedParameters[offset] == null) {
                            isMatch = false;
                        }
                    }
                    catch (Exception) {
                        // @todo this should be removed, or at least only wrapped around the ChangeType fallback.
                        isMatch = false;
                    }
                }
            }
            else {
                isMatch = false;
            }

            return isMatch;
        }

        protected virtual IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return new List<IExecutableBase>();
        } 

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
