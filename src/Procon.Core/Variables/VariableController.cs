using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Service.Shared;

namespace Procon.Core.Variables {
    /// <summary>
    /// Stores variables and handles commands to edit/fetch a variable
    /// </summary>
    public class VariableController : CoreController, ISharedReferenceAccess {

        /// <summary>
        /// Anything in this list is volatile and will not be saved on
        /// exit.
        /// </summary>
        public List<VariableModel> VolatileVariables { get; set; }

        /// <summary>
        /// Anything in this list will be saved to the config
        /// </summary>
        public List<VariableModel> ArchiveVariables { get; set; }

        /// <summary>
        /// Anything in this list will be saved to the config, but saved as a volatile set. The variable
        /// value will only last until the instance is restarted and the config consumed.
        /// </summary>
        public List<VariableModel> FlashVariables { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the variable controller with default values and sets up command handlers.
        /// </summary>
        public VariableController() : base() {
            this.Shared = new SharedReferences();
            this.VolatileVariables = new List<VariableModel>();
            this.ArchiveVariables = new List<VariableModel>();
            this.FlashVariables = new List<VariableModel>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String),
                                IsList = true
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetCollection)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetSingular)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSetA,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String),
                                IsList = true
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetACollection)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSetA,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetASingular)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSetF,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String),
                                IsList = true
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetFCollection)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSetF,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "value",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandSetFSingular)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesGet,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.CommandGet)
                }
            });
        }

        // @todo this should be moved to something like "InstanceVariableController" or something.
        // It's otherwise too specialized for something that could be used in plugins.
        private void SetupDefaultVariables() {
            this.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandPublicPrefix, "!");

            this.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandProtectedPrefix, "#");

            this.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.TextCommandPrivatePrefix, "@");

            this.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.DatabaseMaximumSelectedRows, 20);

            var sourceRepositoryUri = this.Variable(CommonVariableNames.PackagesDefaultSourceRepositoryUri);
            sourceRepositoryUri.Value = Defines.PackagesDefaultSourceRepositoryUri;
            sourceRepositoryUri.Readonly = true;
        }

        /// <summary>
        /// Begins the execution of this variable controller.
        /// Assigns events and loads the config for this file.
        /// </summary>
        public override ICoreController Execute() {
            this.AssignEvents();

            this.SetupDefaultVariables();

            return base.Execute();
        }

        /// <summary>
        /// Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() {
            foreach (VariableModel variable in this.VolatileVariables) {
                variable.Dispose();
            }

            foreach (VariableModel archiveVariable in this.ArchiveVariables) {
                archiveVariable.Dispose();
            }

            this.VolatileVariables.Clear();
            this.VolatileVariables = null;

            this.ArchiveVariables.Clear();
            this.ArchiveVariables = null;
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void WriteConfig(Config config) {
            foreach (VariableModel archiveVariable in this.ArchiveVariables) {
                config.Root.Add(CommandBuilder.VariablesSetA(archiveVariable.Name, archiveVariable.ToList<String>()).ToConfigCommand());
            }

            foreach (VariableModel flashVariable in this.FlashVariables) {
                config.Root.Add(CommandBuilder.VariablesSet(flashVariable.Name, flashVariable.ToList<String>()).ToConfigCommand());
            }
        }

        protected void AssignEvents() {
            
        }

        /// <summary>
        /// Fetches a variable by name
        /// </summary>
        /// <param name="name">The name of the variable object</param>
        /// <returns>The variable, if available. False otherwise.</returns>
        public VariableModel Variable(String name) {
            VariableModel variable = this.VolatileVariables.Find(x => String.Compare(x.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (variable == null) {
                variable = new VariableModel() {
                    Name = name
                };

                this.VolatileVariables.Add(variable);
            }

            return variable;
        }

        /// <summary>
        /// Alias of Variable(String)
        /// </summary>
        /// <param name="name">The name of the variable object</param>
        /// <returns>The variable, if available. False otherwise.</returns>
        public VariableModel Variable(CommonVariableNames name) {
            return this.Variable(name.ToString());
        }

        /// <summary>
        /// Supports '-' and '--' arguments.
        /// </summary>
        /// <param name="arguments">A list of arguments to pass.</param>
        public void ParseArguments(List<String> arguments) {
            for (int offset = 0; offset < arguments.Count; offset++) {
                String argument = arguments[offset];

                // if the argument is a switch.
                if (argument[0] == '-') {
                    // Trims any hyphens from the start of the argument. Allows for "-argument" and "--argument"
                    argument = argument.TrimStart('-');

                    VariableModel variable = null;

                    // Does another argument exist?
                    if (offset + 1 < arguments.Count && arguments[offset + 1][0] != '-') {
                        // No, the next string is not an argument switch. It's the value of the
                        // argument.
                        variable = this.Set(new Command() { Origin = CommandOrigin.Local }, argument, arguments[offset + 1]).Now.Variables.FirstOrDefault();
                    }
                    else {
                        // Set to "true"
                        variable = this.Set(new Command() { Origin = CommandOrigin.Local }, argument, true).Now.Variables.FirstOrDefault();
                    }

                    if (variable != null) {
                        variable.Readonly = true;
                    }
                }
            }
        }

        protected CommandResult CommandSetCollection(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            List<String> value = parameters["value"].All<String>();

            return this.Set(command, name, value);
        }

        protected CommandResult CommandSetSingular(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            String value = parameters["value"].First<String>();

            return this.Set(command, name, value);
        }

        protected CommandResult CommandSetACollection(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            List<String> value = parameters["value"].All<String>();

            return this.SetA(command, name, value);
        }

        protected CommandResult CommandSetASingular(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            String value = parameters["value"].First<String>();

            return this.SetA(command, name, value);
        }

        protected CommandResult CommandSetFCollection(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            List<String> value = parameters["value"].All<String>();

            return this.SetF(command, name, value);
        }

        protected CommandResult CommandSetFSingular(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();
            String value = parameters["value"].First<String>();

            return this.SetF(command, name, value);
        }

        protected CommandResult CommandGet(Command command, Dictionary<String, CommandParameter> parameters) {
            String name = parameters["name"].First<String>();

            return this.Get(command, name);
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public CommandResult Set(Command command, String name, Object value) {
            CommandResult result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (name.Length > 0) {
                    VariableModel variable = this.Variable(name);

                    if (variable.Readonly == false) {
                        variable.Value = value;

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                            Now = new CommandData() {
                                Variables = new List<VariableModel>() {
                                    variable
                                }
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSet));
                        }
                    }
                    else {
                        // Variable set to read only and cannot be modified.
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.Failed,
                            Message = String.Format(@"Variable name ""{0}"" is set to read-only.", variable.Name)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
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
        public CommandResult Set(Command command, CommonVariableNames name, Object value) {
            return this.Set(command, name.ToString(), value);
        }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        public CommandResult SetA(Command command, String name, Object value) {
            CommandResult result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                CommandResult volatileSetResult = this.Set(command, name, value);

                if (volatileSetResult.Success == true) {
                    // All good.
                    var variable = volatileSetResult.Now.Variables.First();

                    if (this.ArchiveVariables.Find(x => x.Name == variable.Name) == null) {
                        this.ArchiveVariables.Add(variable);
                    }
                    else {
                        this.ArchiveVariables.Find(x => x.Name == variable.Name).Value = variable.Value;
                    }

                    // Remove the archived value
                    this.FlashVariables.RemoveAll(v => v.Name == variable.Name);

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                        Now = new CommandData() {
                            Variables = new List<VariableModel>() {
                                variable
                            }
                        }
                    };

                    if (this.Shared.Events != null) {
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSetA));
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
        public CommandResult SetF(Command command, String name, Object value) {
            CommandResult result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                CommandResult volatileSetResult = this.Set(command, name, value);

                if (volatileSetResult.Success == true) {
                    // All good.
                    var variable = volatileSetResult.Now.Variables.First();

                    if (this.FlashVariables.Find(v => v.Name == variable.Name) == null) {
                        this.FlashVariables.Add(variable);
                    }
                    else {
                        this.FlashVariables.Find(v => v.Name == variable.Name).Value = variable.Value;
                    }

                    // Remove the archived value
                    this.ArchiveVariables.RemoveAll(v => v.Name == variable.Name);

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format(@"Successfully set value of variable name ""{0}"" to ""{1}"".", variable.Name, variable.Value),
                        Now = new CommandData() {
                            Variables = new List<VariableModel>() {
                                variable
                            }
                        }
                    };

                    if (this.Shared.Events != null) {
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.VariablesSetF));
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
        public T Get<T>(String name, T defaultValue = default(T)) {
            T result = defaultValue;

            VariableModel variable = this.Variable(name);

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
            return this.Get(name.ToString(), defaultValue);
        }

        /// <summary>
        /// Gets a raw value given a knameey, returned as a Object
        /// </summary>
        /// <param name="command">Details of the commands origin</param>
        /// <param name="name">The unique name of the variable to fetch</param>
        /// <param name="defaultValue"></param>
        /// <returns>The raw object with no conversion</returns>
        public CommandResult Get(Command command, String name, Object defaultValue = null) {
            CommandResult result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (name.Length > 0) {
                    VariableModel variable = this.Variable(name);

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format(@"Value of variable with name ""{0}"" is ""{1}"".", variable.Name, variable.Value),
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
                        Status = CommandResultType.InvalidParameter,
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
