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
        public LanguageConfig Default { get; set; }

        /// <summary>
        /// A list of all the languages loaded.
        /// </summary>
        public List<LanguageConfig> LoadedLanguageFiles { get; protected set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the language controller with the default values.
        /// </summary>
        public LanguageController() : base() {
            Shared = new SharedReferences();
            Default = null;
            LoadedLanguageFiles = new List<LanguageConfig>();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.LanguageLocalize,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "languageCode",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "namespace",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "args",
                            Type = typeof(string),
                            IsList = true,
                            IsConvertable = true
                        }
                    },
                    Handler = Localize
                },
                new CommandDispatch() {
                    CommandType = CommandType.LanguageLocalize,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "languageCode",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "namespace",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "arg",
                            Type = typeof(string)
                        }
                    },
                    Handler = SingleParameterLocalize
                },
                new CommandDispatch() {
                    CommandType = CommandType.LanguageLocalize,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "languageCode",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "namespace",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        }
                    },
                    Handler = ParameterlessLocalize
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
                    LoadedLanguageFiles.Add(language);
                }
            }

            AssignEvents();

            Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).Value = "en-GB";

            LoadDefaultLanguage();

            return base.Execute();
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        protected void AssignEvents() {
            UnassignEvents();

            Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(LanguageController_PropertyChanged);

        }
        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(LanguageController_PropertyChanged);
        }

        /// <summary>
        /// Fired whenever the default language variable is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageController_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            LoadDefaultLanguage();
        }
        
        protected void LoadDefaultLanguage() {
            var languageCode = Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).ToType("en-GB");

            var language = LoadedLanguageFiles.FirstOrDefault(lang => lang.LanguageModel.LanguageCode == languageCode);

            if (language != null) {
                Default = language;
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
        public ICommandResult Localize(ICommand command, Dictionary<string, ICommandParameter> parameters) {

            // <param name="languageCode">The ietf language tag.</param>
            // <param name="namespace">The namespace to limit the search for the name to.</param>
            // <param name="name">The name representing the localized string.</param>
            // <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
            var languageCode = parameters["languageCode"].First<string>();
            var @namespace = parameters["namespace"].First<string>();
            var name = parameters["name"].First<string>();
            var args = parameters["args"].All<string>();

            return Localize(command, languageCode, @namespace, name, args.Select(arg => (object)arg).ToArray());
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
        public ICommandResult Localize(ICommand command, string languageCode, string @namespace, string name, object[] args) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local || Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var language = LoadedLanguageFiles.FirstOrDefault(lang => lang.LanguageModel.LanguageCode == languageCode) ?? Default;

                if (language != null) {
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Now = new CommandData() {
                            Content = new List<string>() {
                                language.Localize(@namespace, name, args)
                            }
                        }
                    };
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists,
                        Message = string.Format(@"Language with the code ""{0}"" does not exist and no default language specified.", languageCode)
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
        public ICommandResult SingleParameterLocalize(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var languageCode = parameters["languageCode"].First<string>();
            var @namespace = parameters["namespace"].First<string>();
            var name = parameters["name"].First<string>();
            var arg = parameters["arg"].First<string>();

            return Localize(command, languageCode, @namespace, name, new object[] { arg });
        }

        /// <summary>
        /// Proxy for the localization command, but allows for no parameters to be passed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult ParameterlessLocalize(ICommand command, Dictionary<string, ICommandParameter> parameters) {

            // <param name="languageCode">The ietf language tag.</param>
            // <param name="namespace">The namespace to limit the search for the name to.</param>
            // <param name="name">The name representing the localized string.</param>
            // <param name="args">Arguments to use in String.Format() for the value obtained by name.</param>
            var languageCode = parameters["languageCode"].First<string>();
            var @namespace = parameters["namespace"].First<string>();
            var name = parameters["name"].First<string>();

            return Localize(command, languageCode, @namespace, name, new object[] { });
        }

        /// <summary>
        /// Finds the best matching (exact) language config.
        /// </summary>
        /// <param name="languageCode">The language code of the config to find</param>
        /// <returns>The found language config, or default if no match can be found</returns>
        public LanguageConfig FindOptimalLanguageConfig(string languageCode) {
            var found = LoadedLanguageFiles.Find(config => config.LanguageModel.LanguageCode.ToLowerInvariant() == languageCode.ToLowerInvariant());
            
            // todo this should be expanded to find "en_us" given "en", "en-us" given "en-gb" and no en-us available.
            // I'd prefer the LanguageModel implemented a method to split the languageCode
            // named them different like "IsoXXLanguageCode" and "IsoXXCountryCode"

            return found ?? Default;
        }

        public override void Dispose() {

            UnassignEvents();
            
            LoadedLanguageFiles.Clear();
            LoadedLanguageFiles = null;

            base.Dispose();
        }
    }
}
