#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using System.Collections.Generic;
using System.Linq;
using Potato.Core.Connections.TextCommands.Parsers;
using Potato.Core.Localization;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands {
    /// <summary>
    /// Manages registering, dispatching text commands
    /// </summary>
    public class TextCommandController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// Full list of text commands to check against.
        /// </summary>
        public List<TextCommandModel> TextCommands { get; protected set; }

        /// <summary>
        /// The owner of this controller, used to lookup the game with all the player data and such in it.
        /// </summary>
        public IConnectionController Connection { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Creates new controller with the default attributes set
        /// </summary>
        public TextCommandController() {
            Shared = new SharedReferences();
            TextCommands = new List<TextCommandModel>();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsExecute,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "text",
                            Type = typeof(string)
                        }
                    },
                    Handler = TextCommandsExecute
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsPreview,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "text",
                            Type = typeof(string)
                        }
                    },
                    Handler = TextCommandsPreview
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsRegister,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = TextCommandsRegister
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsUnregister,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = TextCommandsUnregister
                }
            });
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() {
            foreach (var textCommand in TextCommands) {
                textCommand.Dispose();
            }

            TextCommands.Clear();
            TextCommands = null;

            Connection = null;

            base.Dispose();
        }

        /// <summary>
        /// Fetches a fuzzy text parser
        /// </summary>
        /// <param name="speaker">The player executing the command</param>
        /// <param name="speakerAccount">The account executing the command</param>
        /// <returns>The fuzzy parser, provided a language could be found.</returns>
        protected ITextCommandParser BuildFuzzyParser(PlayerModel speaker, AccountModel speakerAccount) {
            LanguageConfig selectedLanguage = null;

            if (speakerAccount != null && speakerAccount.PreferredLanguageCode != string.Empty) {
                selectedLanguage = Shared.Languages.FindOptimalLanguageConfig(speakerAccount.PreferredLanguageCode);
            }
            else {
                selectedLanguage = Shared.Languages.Default;
            }

            return new FuzzyParser() {
                Connection = Connection,
                TextCommands = TextCommands.Where(textCommand => textCommand.Parser == TextCommandParserType.Any || textCommand.Parser == TextCommandParserType.Fuzzy).ToList(),
                Document = selectedLanguage.Config.Document,
                SpeakerPlayer = speaker,
                SpeakerAccount = speakerAccount
            };
        }

        /// <summary>
        /// Fetches a route text parser
        /// </summary>
        /// <param name="speaker">The player executing the command</param>
        /// <param name="speakerAccount">The account executing the command</param>
        /// <returns>The route parser, provided a language could be found.</returns>
        protected ITextCommandParser BuildRouteParser(PlayerModel speaker, AccountModel speakerAccount) {
            return new RouteParser() {
                Connection = Connection,
                TextCommands = TextCommands.Where(textCommand => textCommand.Parser == TextCommandParserType.Any || textCommand.Parser == TextCommandParserType.Route).ToList(),
                SpeakerPlayer = speaker,
                SpeakerAccount = speakerAccount
            };
        }

        /// <summary>
        /// Runs through the various parsers for all of the text commands.
        /// </summary>
        /// <remarks>
        /// This method may fire multiple events to execute multiple commands
        /// when more than one parser is included in the future. This is expected
        /// behaviour.
        /// </remarks>
        /// <param name="speakerNetworkPlayer"></param>
        /// <param name="speakerAccount"></param>
        /// <param name="prefix"></param>
        /// <param name="text"></param>
        /// <returns>The generated event, if any.</returns>
        protected ICommandResult Parse(PlayerModel speakerNetworkPlayer, AccountModel speakerAccount, string prefix, string text) {
            var parsers = new List<ITextCommandParser>() {
                BuildFuzzyParser(speakerNetworkPlayer, speakerAccount),
                BuildRouteParser(speakerNetworkPlayer, speakerAccount)
            };

            return parsers.Select(parser => parser.Parse(prefix, text)).FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Fetches a player in the current game connection that is
        /// asociated with the account executing this command.
        /// 
        /// This is used so an account over a layer can issue a command like
        /// "kick me", but we only know "me" from the context of the account
        /// issuing the command. We use this to fetch the player in the game
        /// so we know who to kick.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="speaker"></param>
        /// <returns></returns>
        protected PlayerModel GetAccountNetworkPlayer(ICommand command, AccountModel speaker) {
            var player = Connection.ProtocolState.Players.Values.FirstOrDefault(p => p.Uid == command.Authentication.Uid);

            if (speaker != null) {
                var accountPlayer = speaker.Players.FirstOrDefault(p => p.ProtocolType == Connection.ConnectionModel.ProtocolType.Type);

                if (accountPlayer != null) {
                    player = Connection.ProtocolState.Players.Values.FirstOrDefault(p => p.Uid == accountPlayer.Uid);
                }
            }

            return player;
        }

        /// <summary>
        /// Parses then fires an event to execute a text command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>The generated event, if any.</returns>
        public ICommandResult TextCommandsExecute(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var text = parameters["text"].First<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var speakerAccount = Shared.Security.GetAccount(command);

                // @todo pull the prefix from the text?

                var prefix = Shared.Variables.Get<string>(CommonVariableNames.TextCommandPublicPrefix);

                result = Parse(GetAccountNetworkPlayer(command, speakerAccount), speakerAccount, prefix, text);

                if (result != null) {
                    result.Scope = new CommandData() {
                        Content = new List<string>() {
                            text
                        },
                        Connections = new List<ConnectionModel>() {
                            Connection.ConnectionModel
                        }
                    };
                }
                else {
                    result = command.Result;
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Parses then fires an event to preview the results of a text command.
        /// 
        /// Essentially does everything that parsing does, but fires a different event.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns>The generated event, if any.</returns>
        public ICommandResult TextCommandsPreview(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var text = parameters["text"].First<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var speakerAccount = Shared.Security.GetAccount(command);

                // @todo pull the prefix from the text?

                var prefix = Shared.Variables.Get<string>(CommonVariableNames.TextCommandPublicPrefix);

                result = Parse(GetAccountNetworkPlayer(command, speakerAccount), speakerAccount, prefix, text);

                if (result != null) {
                    result.Scope = new CommandData() {
                        Content = new List<string>() {
                            text
                        },
                        Connections = new List<ConnectionModel>() {
                            Connection.ConnectionModel
                        }
                    };
                }
                else {
                    result = command.Result;
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Register a text command with this controller
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult TextCommandsRegister(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var textCommand = parameters["textCommand"].First<TextCommandModel>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var existingRegisteredCommand = TextCommands.Find(existingCommand => existingCommand.PluginGuid == textCommand.PluginGuid && existingCommand.PluginCommand == textCommand.PluginCommand);

                if (existingRegisteredCommand == null) {
                    TextCommands.Add(textCommand);

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Scope = {
                            Connections = new List<ConnectionModel>() {
                                Connection != null ? Connection.ConnectionModel : null
                            }
                        },
                        Now = new CommandData() {
                            TextCommands = new List<TextCommandModel>() {
                                textCommand
                            }
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.TextCommandRegistered));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.AlreadyExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult TextCommandsUnregister(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var textCommand = parameters["textCommand"].First<TextCommandModel>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var existingRegisteredCommand = TextCommands.Find(existingCommand => existingCommand.PluginGuid == textCommand.PluginGuid && existingCommand.PluginCommand == textCommand.PluginCommand);

                if (existingRegisteredCommand != null) {
                    TextCommands.Remove(existingRegisteredCommand);

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Scope = {
                            Connections = new List<ConnectionModel>() {
                                Connection != null ? Connection.ConnectionModel : null
                            }
                        },
                        Now = new CommandData() {
                            TextCommands = new List<TextCommandModel>() {
                                textCommand
                            }
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.TextCommandUnregistered));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
