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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Service.Shared;

namespace Potato.Core.Localization {
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

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the language controller with the default values.
        /// </summary>
        public LanguageController() : base() {
            this.Shared = new SharedReferences();
            this.Default = null;
            this.LoadedLanguageFiles = new List<LanguageConfig>();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
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
                    },
                    Handler = this.Localize
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SingleParameterLocalize
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.ParameterlessLocalize
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
            var languageDirectories = new DirectoryInfo(Defines.PackagesDirectory.FullName)
                .GetDirectories(Defines.LocalizationDirectoryName, SearchOption.AllDirectories)
                .Union(new [] {
                    new DirectoryInfo(Defines.LocalizationDirectory.FullName)
                })
                .SelectMany(localizationDirectory => localizationDirectory.GetDirectories());

            // Loop over each grouped language
            foreach (var groupedLanguageDirectories in languageDirectories.GroupBy(directory => directory.Name)) {
                // Loop over each directory for this language, appending to the build language file.
                LanguageConfig language = null;

                foreach (var languageDirectory in groupedLanguageDirectories) {
                    if (language == null) {
                        language = new LanguageConfig();
                        language.Load(languageDirectory);
                    }
                    else {
                        language.Config.Union(new Config().Load(languageDirectory));
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
        public ICommandResult Localize(ICommand command, Dictionary<String, ICommandParameter> parameters) {

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
        public ICommandResult Localize(ICommand command, String languageCode, String @namespace, String name, Object[] args) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                LanguageConfig language = this.LoadedLanguageFiles.FirstOrDefault(lang => lang.LanguageModel.LanguageCode == languageCode);

                if (language != null) {
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Now = new CommandData() {
                            Content = new List<String>() {
                                language.Localize(@namespace, name, args)
                            }
                        }
                    };
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists,
                        Message = String.Format(@"Language with the code ""{0}"" does not exist.", languageCode)
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Localizes on a key that requires a single parameter localization.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SingleParameterLocalize(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
        public ICommandResult ParameterlessLocalize(ICommand command, Dictionary<String, ICommandParameter> parameters) {

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
            
            this.LoadedLanguageFiles.Clear();
            this.LoadedLanguageFiles = null;

            base.Dispose();
        }
    }
}
