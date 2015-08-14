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
using System.Text.RegularExpressions;

namespace Potato.Core.Shared {
    /// <summary>
    /// Command to be executed
    /// </summary>
    /// <remarks><para>Called to execute a command.</para></remarks>
    [Serializable]
    public sealed class CommandDispatch : ICommandDispatch {
        public string Name { get; set; }

        public CommandType CommandType {
            get { return _mCommandType; }
            set {
                _mCommandType = value;

                if (_mCommandType != CommandType.None) {
                    Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        public Regex NamePattern { get; set; }

        public CommandAttributeType CommandAttributeType { get; set; }

        public List<CommandParameterType> ParameterTypes { get; set; }

        public Func<ICommand, Dictionary<string, ICommandParameter>, ICommandResult> Handler { get; set; }

        /// <summary>
        /// Initializes the dispatch with default values.
        /// </summary>
        public CommandDispatch() {
            CommandAttributeType = CommandAttributeType.Handler;
        }

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        public ICommandDispatch ParseCommandType(string commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                Name = commandName;
            }

            return this;
        }
        
        public bool CanDispatch(CommandAttributeType attributeType, ICommand command) {
            return CommandAttributeType == attributeType && (NamePattern == null ? Name == command.Name : NamePattern.IsMatch(command.Name));
        }

        public Dictionary<string, ICommandParameter> BuildParameterDictionary(IList<ICommandParameter> parameters) {
            var parameterDictionary = new Dictionary<string, ICommandParameter>();

            // If we're not expecting any parameters
            if (ParameterTypes != null) {
                if (parameters != null && ParameterTypes.Count == parameters.Count) {
                    for (var offset = 0; offset < ParameterTypes.Count && parameterDictionary != null; offset++) {

                        if (ParameterTypes[offset].IsList == true) {
                            if (parameters[offset].HasMany(ParameterTypes[offset].Type, ParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(ParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                        else {
                            if (parameters[offset].HasOne(ParameterTypes[offset].Type, ParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(ParameterTypes[offset].Name, parameters[offset]);
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
    }
}
