using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Connections.TextCommands.Parsers;
using Procon.Core.Localization;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Models;

namespace Procon.Core.Connections.TextCommands {
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
        public ConnectionController Connection { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Creates new controller with the default attributes set
        /// </summary>
        public TextCommandController() {
            this.Shared = new SharedReferences();
            this.TextCommands = new List<TextCommandModel>();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsExecute,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "text",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.ExecuteTextCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsPreview,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "text",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.PreviewTextCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsRegister,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = this.RegisterTextCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.TextCommandsUnregister,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "textCommand",
                            Type = typeof(TextCommandModel)
                        }
                    },
                    Handler = this.UnregisterTextCommand
                }
            });
        }

        /// <summary>
        /// Does nothing.  Information about this object is handled via it's parent interface.
        /// </summary>
        public override void Dispose() {
            foreach (TextCommandModel textCommand in this.TextCommands) {
                textCommand.Dispose();
            }

            this.TextCommands.Clear();
            this.TextCommands = null;

            this.Connection = null;

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

            if (speakerAccount != null && speakerAccount.PreferredLanguageCode != String.Empty) {
                selectedLanguage = this.Shared.Languages.LoadedLanguageFiles.Find(language => language.LanguageModel.LanguageCode == speakerAccount.PreferredLanguageCode);
            }
            else {
                selectedLanguage = this.Shared.Languages.Default;
            }

            return new FuzzyParser() {
                Connection = this.Connection,
                TextCommands = this.TextCommands.Where(textCommand => textCommand.Parser == TextCommandParserType.Any || textCommand.Parser == TextCommandParserType.Fuzzy).ToList(),
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
                Connection = this.Connection,
                TextCommands = this.TextCommands.Where(textCommand => textCommand.Parser == TextCommandParserType.Any || textCommand.Parser == TextCommandParserType.Route).ToList(),
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
        protected ICommandResult Parse(PlayerModel speakerNetworkPlayer, AccountModel speakerAccount, String prefix, String text) {
            List<ITextCommandParser> parsers = new List<ITextCommandParser>() {
                this.BuildFuzzyParser(speakerNetworkPlayer, speakerAccount),
                this.BuildRouteParser(speakerNetworkPlayer, speakerAccount)
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
            PlayerModel player = this.Connection.ProtocolState.Players.FirstOrDefault(p => p.Uid == command.Authentication.Uid);

            if (speaker != null) {
                AccountPlayerModel accountPlayer = speaker.Players.FirstOrDefault(p => p.GameType == this.Connection.ConnectionModel.ProtocolType.Type);

                if (accountPlayer != null) {
                    player = this.Connection.ProtocolState.Players.FirstOrDefault(p => p.Uid == accountPlayer.Uid);
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
        public ICommandResult ExecuteTextCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String text = parameters["text"].First<String>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel speakerAccount = this.Shared.Security.GetAccount(command);

                // @todo pull the prefix from the text?

                String prefix = this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPublicPrefix);

                result = this.Parse(this.GetAccountNetworkPlayer(command, speakerAccount), speakerAccount, prefix, text) ?? command.Result;

                // todo fire event? GenericEventType.TextCommandExecuted
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
        public ICommandResult PreviewTextCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String text = parameters["text"].First<String>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel speakerAccount = this.Shared.Security.GetAccount(command);

                // @todo pull the prefix from the text?

                String prefix = this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPublicPrefix);

                result = this.Parse(this.GetAccountNetworkPlayer(command, speakerAccount), speakerAccount, prefix, text) ?? command.Result;

                // todo fire event? GenericEventType.TextCommandPreviewed
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
        public ICommandResult RegisterTextCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            TextCommandModel textCommand = parameters["textCommand"].First<TextCommandModel>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                TextCommandModel existingRegisteredCommand = this.TextCommands.Find(existingCommand => existingCommand.PluginUid == textCommand.PluginUid && existingCommand.PluginCommand == textCommand.PluginCommand);

                if (existingRegisteredCommand == null) {
                    this.TextCommands.Add(textCommand);

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Scope = {
                            Connections = new List<ConnectionModel>() {
                                this.Connection != null ? this.Connection.ConnectionModel : null
                            }
                        },
                        Now = new CommandData() {
                            TextCommands = new List<TextCommandModel>() {
                                textCommand
                            }
                        }
                    };

                    if (this.Shared.Events != null) {
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.TextCommandRegistered));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.AlreadyExists
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
        public ICommandResult UnregisterTextCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            TextCommandModel textCommand = parameters["textCommand"].First<TextCommandModel>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                TextCommandModel existingRegisteredCommand = this.TextCommands.Find(existingCommand => existingCommand.PluginUid == textCommand.PluginUid && existingCommand.PluginCommand == textCommand.PluginCommand);

                if (existingRegisteredCommand != null) {
                    this.TextCommands.Remove(existingRegisteredCommand);

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Scope = {
                            Connections = new List<ConnectionModel>() {
                                this.Connection != null ? this.Connection.ConnectionModel : null
                            }
                        },
                        Now = new CommandData() {
                            TextCommands = new List<TextCommandModel>() {
                                textCommand
                            }
                        }
                    };

                    if (this.Shared.Events != null) {
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.TextCommandUnregistered));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Checks if a prefix is an allowed prefix
        /// </summary>
        /// <param name="prefix">The prefix to check (e.g !, @ etc.)</param>
        /// <returns>The parameter prefix, or null if the prefix is invalid</returns>
        public String GetValidTextCommandPrefix(String prefix) {
            String result = null;

            if (prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPublicPrefix) ||
                prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandProtectedPrefix) ||
                prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPrivatePrefix)) {
                result = prefix;
            }

            return result;
        }
    }
}
