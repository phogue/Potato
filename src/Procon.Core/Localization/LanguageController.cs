using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Service.Shared;

namespace Procon.Core.Localization {
    /// <summary>
    /// Handles loading localization files and fetching localization by common key
    /// </summary>
    public class LanguageController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The default language to fall back on
        /// </summary>
        public LanguageConfig Default { get; protected set; }

        /// <summary>
        /// A list of all the languages loaded.
        /// </summary>
        public List<LanguageConfig> LoadedLanguageFiles { get; protected set; }

        [XmlIgnore, JsonIgnore]
        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the language controller with the default values.
        /// </summary>
        public LanguageController() : base() {
            this.Shared = new SharedReferences();
            this.Default = null;
            this.LoadedLanguageFiles = new List<LanguageConfig>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.LanguageLocalize,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "languageCode",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "namespace",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "args",
                                Type = typeof(String),
                                IsList = true,
                                IsConvertable = true
                            }
                        }
                    },
                    new CommandDispatchHandler(this.Localize)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.LanguageLocalize,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "languageCode",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "namespace",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "arg",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SingleParameterLocalize)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.LanguageLocalize,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "languageCode",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "namespace",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "name",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.ParameterlessLocalize)
                }
            });
        }

        /// <summary>
        /// Loads the languages found in the languages directory.
        /// Sets the default language to english before looking at config file.
        /// Executes the commands specified in the configuration file.
        /// Returns a reference back to this object.
        /// </summary>
        public override ICoreController Execute() {
            var languageDirectories = new DirectoryInfo(Defines.PackagesDirectory)
                .GetDirectories(Defines.LocalizationDirectoryName, SearchOption.AllDirectories)
                .SelectMany(localizationDirectory => localizationDirectory.GetDirectories());

            // Loop over each grouped language
            foreach (var groupedLanguageDirectories in languageDirectories.GroupBy(directory => directory.Name)) {
                // Loop over each directory for this language, appending to the build language file.
                LanguageConfig language = null;

                foreach (var languageDirectory in groupedLanguageDirectories) {
                    if (language == null) {
                        language = new LanguageConfig().Load(languageDirectory) as LanguageConfig;
                    }
                    else {
                        language.Combine(new LanguageConfig().Load(languageDirectory) as LanguageConfig);
                    }
                }

                if (language != null && language.LanguageModel.LanguageCode != null) {
                    this.LoadedLanguageFiles.Add(language);
                }
            }

            this.AssignEvents();

            this.Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).Value = "en-UK";

            this.LoadDefaultLanguage();

            return base.Execute();
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        protected void AssignEvents() {
            this.UnassignEvents();

            this.Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(LanguageController_PropertyChanged);

        }
        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(LanguageController_PropertyChanged);
        }

        /// <summary>
        /// Fired whenever the default language variable is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageController_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.LoadDefaultLanguage();
        }
        
        protected void LoadDefaultLanguage() {
            String languageCode = this.Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).ToType("en-UK");

            LanguageConfig language = this.LoadedLanguageFiles.FirstOrDefault(lang => lang.LanguageModel.LanguageCode == languageCode);

            if (language != null) {
                this.Default = language;
            }
            // else - maintain the current default language.
        }

        /// <summary>
        /// Gets a localized string based on a name value specified, limited to a specific namespace,
        /// with additional arguments for formatting if necessary. Uses the default language if the
        /// language code is not specified. Returns an empty string if a localization could not be
        /// found.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs Localize(Command command, Dictionary<String, CommandParameter> parameters) {

            // <param name="languageCode">The ietf language tag.</param>
            // <param name="namespace">The namespace to limit the search for the name to.</param>
            // <param name="name">The name representing the localized string.</param>
            // <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
            String languageCode = parameters["languageCode"].First<String>();
            String @namespace = parameters["namespace"].First<String>();
            String name = parameters["name"].First<String>();
            List<String> args = parameters["args"].All<String>();

            return this.Localize(command, languageCode, @namespace, name, args.Select(arg => (Object)arg).ToArray());
        }

        /// <summary>
        /// Gets a localized string based on a name value specified, limited to a specific namespace,
        /// with additional arguments for formatting if necessary. Uses the default language if the
        /// language code is not specified. Returns an empty string if a localization could not be
        /// found.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="languageCode">The ietf language tag.</param>
        /// <param name="namespace">The namespace to limit the search for the name to.</param>
        /// <param name="name">The name representing the localized string.</param>
        /// <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
        /// <returns></returns>
        public CommandResultArgs Localize(Command command, String languageCode, String @namespace, String name, Object[] args) {
            CommandResultArgs result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                LanguageConfig language = this.LoadedLanguageFiles.FirstOrDefault(lang => lang.LanguageModel.LanguageCode == languageCode);

                if (language != null) {
                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Now = new CommandData() {
                            Content = new List<String>() {
                                language.Localize(@namespace, name, args)
                            }
                        }
                    };
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.DoesNotExists,
                        Message = String.Format(@"Language with the code ""{0}"" does not exist.", languageCode)
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Localizes on a key that requires a single parameter localization.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs SingleParameterLocalize(Command command, Dictionary<String, CommandParameter> parameters) {
            String languageCode = parameters["languageCode"].First<String>();
            String @namespace = parameters["namespace"].First<String>();
            String name = parameters["name"].First<String>();
            String arg = parameters["arg"].First<String>();

            return this.Localize(command, languageCode, @namespace, name, new object[] { arg });
        }

        /// <summary>
        /// Proxy for the localization command, but allows for no parameters to be passed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs ParameterlessLocalize(Command command, Dictionary<String, CommandParameter> parameters) {

            // <param name="languageCode">The ietf language tag.</param>
            // <param name="namespace">The namespace to limit the search for the name to.</param>
            // <param name="name">The name representing the localized string.</param>
            // <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
            String languageCode = parameters["languageCode"].First<String>();
            String @namespace = parameters["namespace"].First<String>();
            String name = parameters["name"].First<String>();

            return this.Localize(command, languageCode, @namespace, name, new object[] { });
        }

        public override void Dispose() {

            this.UnassignEvents();

            foreach (LanguageConfig language in this.LoadedLanguageFiles) {
                language.Dispose();
            }

            this.LoadedLanguageFiles.Clear();
            this.LoadedLanguageFiles = null;

            base.Dispose();
        }
    }
}
