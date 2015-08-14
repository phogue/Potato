#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Service.Shared;

namespace Potato.Core.Variables {
    /// <summary>
    /// Stores variables and handles commands to edit/fetch a variable
    /// </summary>
    public class VariableController : CoreController, ISharedReferenceAccess {

        /// <summary>
        /// Anything in this list is volatile and will not be saved on
        /// exit.
        /// </summary>
        public ConcurrentDictionary<string, VariableModel> VolatileVariables { get; set; }
        
        /// <summary>
        /// Anything in this list will be saved to the config
        /// </summary>
        public ConcurrentDictionary<string, VariableModel> ArchiveVariables { get; set; }

        /// <summary>
        /// Anything in this list will be saved to the config, but saved as a volatile set. The variable
        /// value will only last until the instance is restarted and the config consumed.
        /// </summary>
        public ConcurrentDictionary<string, VariableModel> FlashVariables { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the variable controller with default values and sets up command handlers.
        /// </summary>
        public VariableController() : base() {
            Shared = new SharedReferences();
            VolatileVariables = new ConcurrentDictionary<string, VariableModel>();
            ArchiveVariables = new ConcurrentDictionary<string, VariableModel>();
            FlashVariables = new ConcurrentDictionary<string, VariableModel>();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string),
                            IsList = true
                        }
                    },
                    Handler = CommandSetCollection
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSet,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string)
                        }
                    },
                    Handler = CommandSetSingular
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSetA,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string),
                            IsList = true
                        }
                    },
                    Handler = CommandSetACollection
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSetA,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string)
                        }
                    },
                    Handler = CommandSetASingular
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSetF,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string),
                            IsList = true
                        }
                    },
                    Handler = CommandSetFCollection
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesSetF,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "value",
                            Type = typeof(string)
                        }
                    },
                    Handler = CommandSetFSingular
                },
                new CommandDispatch() {
                    CommandType = CommandType.VariablesGet,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        }
                    },
                    Handler = CommandGet
                }
            });
        }

        // @todo this should be moved to something like "InstanceVariableController" or something.
        // It's otherwise too specialized for something that could be used in plugins.
        private void SetupDefaultVariables() {
            Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandPublicPrefix, "!");

            Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandProtectedPrefix, "#");

            Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandPrivatePrefix, "@");

            Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.DatabaseMaximumSelectedRows, 20);

            var sourceRepositoryUri = Variable(CommonVariableNames.PackagesDefaultSourceRepositoryUri);
            sourceRepositoryUri.Value = Defines.PackagesDefaultSourceRepositoryUri;
            sourceRepositoryUri.Readonly = true;
        }

        /// <summary>
        /// Begins the execution of this variable controller.
        /// Assigns events and loads the config for this file.
        /// </summary>
        public override ICoreController Execute() {
            AssignEvents();

            SetupDefaultVariables();

            return base.Execute();
        }

        /// <summary>
        /// Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() {
            foreach (var variable in VolatileVariables) {
                variable.Value.Dispose();
            }

            foreach (var archiveVariable in ArchiveVariables) {
                archiveVariable.Value.Dispose();
            }

            VolatileVariables.Clear();
            VolatileVariables = null;

            ArchiveVariables.Clear();
            ArchiveVariables = null;
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void WriteConfig(IConfig config, string password = null) {
            // Use the .Value.Name to maintain the case
            foreach (var archiveVariable in ArchiveVariables) {
                // Don't save empty values, even empty values in a list.
                var values = archiveVariable.Value.ToList<string>().Where(item => item.Length > 0).ToList();

                if (values.Count > 0) {
                    config.Append(CommandBuilder.VariablesSetA(archiveVariable.Value.Name, values).ToConfigCommand());
                }
            }

            foreach (var flashVariable in FlashVariables) {
                // Don't save empty values, even empty values in a list.
                var values = flashVariable.Value.ToList<string>().Where(item => item.Length > 0).ToList();

                if (values.Count > 0) {
                    config.Append(CommandBuilder.VariablesSet(flashVariable.Value.Name, values).ToConfigCommand());
                }
            }
        }

        protected void AssignEvents() {
            
        }

        /// <summary>
        /// Fetches a variable by name
        /// </summary>
        /// <param name="name">The name of the variable object</param>
        /// <returns>The variable, if available. False otherwise.</returns>
        public VariableModel Variable(string name) {
            return VolatileVariables.GetOrAdd(name.ToLowerInvariant(), s => new VariableModel() {
                Name = name
            });
        }

        /// <summary>
        /// Alias of Variable(String)
        /// </summary>
        /// <param name="name">The name of the variable object</param>
        /// <returns>The variable, if available. False otherwise.</returns>
        public VariableModel Variable(CommonVariableNames name) {
            return Variable(name.ToString());
        }

        /// <summary>
        /// Supports '-' and '--' arguments.
        /// </summary>
        /// <param name="arguments">A list of arguments to pass.</param>
        public void ParseArguments(List<string> arguments) {
            for (var offset = 0; offset < arguments.Count; offset++) {
                var argument = arguments[offset];

                // if the argument is a switch.
                if (argument[0] == '-') {
                    // Trims any hyphens from the start of the argument. Allows for "-argument" and "--argument"
                    argument = argument.TrimStart('-');

                    VariableModel variable = null;

                    // Does another argument exist?
                    if (offset + 1 < arguments.Count && arguments[offset + 1][0] != '-') {
                        // No, the next string is not an argument switch. It's the value of the
                        // argument.
                        variable = Set(new Command() { Origin = CommandOrigin.Local }, argument, arguments[offset + 1]).Now.Variables.FirstOrDefault();
                    }
                    else {
                        // Set to "true"
                        variable = Set(new Command() { Origin = CommandOrigin.Local }, argument, true).Now.Variables.FirstOrDefault();
                    }

                    if (variable != null) {
                        variable.Readonly = true;
                    }
                }
            }
        }

        protected ICommandResult CommandSetCollection(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].All<string>();

            return Set(command, name, value);
        }

        protected ICommandResult CommandSetSingular(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].First<string>();

            return Set(command, name, value);
        }

        protected ICommandResult CommandSetACollection(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].All<string>();

            return SetA(command, name, value);
        }

        protected ICommandResult CommandSetASingular(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].First<string>();

            return SetA(command, name, value);
        }

        protected ICommandResult CommandSetFCollection(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].All<string>();

            return SetF(command, name, value);
        }

        protected ICommandResult CommandSetFSingular(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();
            var value = parameters["value"].First<string>();

            return SetF(command, name, value);
        }

        protected ICommandResult CommandGet(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var name = parameters["name"].First<string>();

            return Get(command, name);
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public ICommandResult Set(ICommand command, string name, object value) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (name.Length > 0) {
                    var variable = Variable(name);

                    if (variable.Readonly == false) {
                        variable.Value = value;

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                            Now = new CommandData() {
                                Variables = new List<VariableModel>() {
                                    variable
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSet));
                        }
                    }
                    else {
                        // Variable set to read only and cannot be modified.
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.Failed,
                            Message = string.Format(@"Variable name ""{0}"" is set to read-only.", variable.Name)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
                        Message = "A variable name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public ICommandResult Set(ICommand command, CommonVariableNames name, object value) {
            return Set(command, name.ToString(), value);
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public ICommandResult SetA(ICommand command, string name, object value) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var volatileSetResult = Set(command, name, value);

                if (volatileSetResult.Success == true) {
                    // All good.
                    var variable = volatileSetResult.Now.Variables.First();

                    // Upsert he archive variable
                    ArchiveVariables.AddOrUpdate(variable.Name.ToLowerInvariant(), s => variable, (s, model) => variable);

                    // Remove the flash value (so archive + flash are not being saved)
                    VariableModel removed;
                    FlashVariables.TryRemove(variable.Name.ToLowerInvariant(), out removed);

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = string.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                        Now = new CommandData() {
                            Variables = new List<VariableModel>() {
                                variable
                            }
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSetA));
                    }
                }
                else {
                    // Bubble the error.
                    result = volatileSetResult;
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// This will first set the value, then set the value in the flash list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public ICommandResult SetF(ICommand command, string name, object value) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var volatileSetResult = Set(command, name, value);

                if (volatileSetResult.Success == true) {
                    // All good.
                    var variable = volatileSetResult.Now.Variables.First();

                    FlashVariables.AddOrUpdate(variable.Name.ToLowerInvariant(), s => variable, (s, model) => variable);

                    // Remove the archived value (so archive + flash are not being saved)
                    VariableModel removed;
                    ArchiveVariables.TryRemove(variable.Name.ToLowerInvariant(), out removed);

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = string.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                        Now = new CommandData() {
                            Variables = new List<VariableModel>() {
                                variable
                            }
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSetF));
                    }
                }
                else {
                    // Bubble the error.
                    result = volatileSetResult;
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Gets and converts a value given a name
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="name">The unique name of the variable to fetch</param>
        /// <param name="defaultValue"></param>
        /// <returns>The converted value of the variable with the specified name</returns>
        public T Get<T>(string name, T defaultValue = default(T)) {
            var result = defaultValue;

            var variable = Variable(name);

            result = variable.ToType(defaultValue);

            return result;
        }

        /// <summary>
        /// Gets and converts a value given a name
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="name">The unique name of the variable to fetch</param>
        /// <param name="defaultValue"></param>
        /// <returns>The converted value of the variable with the specified kenamey</returns>
        public T Get<T>(CommonVariableNames name, T defaultValue = default(T)) {
            return Get(name.ToString(), defaultValue);
        }

        /// <summary>
        /// Gets a raw value given a knameey, returned as a Object
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to fetch</param>
        /// <param name="defaultValue"></param>
        /// <returns>The raw object with no conversion</returns>
        public ICommandResult Get(ICommand command, string name, object defaultValue = null) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (name.Length > 0) {
                    var variable = Variable(name);

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = string.Format(@"Value of variable with name ""{0}"" is ""{1}"".", variable.Name, variable.Value),
                        Now = new CommandData() {
                            Variables = new List<VariableModel>() {
                                variable
                            }
                        }
                    };
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
                        Message = "A variable name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
    }
}
