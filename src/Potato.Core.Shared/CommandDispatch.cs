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

namespace Potato.Core.Shared {
    /// <summary>
    /// Command to be executed
    /// </summary>
    /// <remarks><para>Called to execute a command.</para></remarks>
    [Serializable]
    public sealed class CommandDispatch : ICommandDispatch {
        public String Name { get; set; }

        public CommandType CommandType {
            get { return this._mCommandType; }
            set {
                this._mCommandType = value;

                if (this._mCommandType != CommandType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        public CommandAttributeType CommandAttributeType { get; set; }

        public List<CommandParameterType> ParameterTypes { get; set; }

        public Func<ICommand, Dictionary<String, ICommandParameter>, ICommandResult> Handler { get; set; }

        /// <summary>
        /// Initializes the dispatch with default values.
        /// </summary>
        public CommandDispatch() {
            this.CommandAttributeType = CommandAttributeType.Handler;
        }

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        public ICommandDispatch ParseCommandType(String commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                this.CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                this.Name = commandName;
            }

            return this;
        }
        
        public bool CanDispatch(CommandAttributeType attributeType, ICommand command) {
            return this.CommandAttributeType == attributeType && this.Name == command.Name;
        }

        public Dictionary<String, ICommandParameter> BuildParameterDictionary(IList<ICommandParameter> parameters) {
            Dictionary<String, ICommandParameter> parameterDictionary = new Dictionary<String, ICommandParameter>();

            // If we're not expecting any parameters
            if (this.ParameterTypes != null) {
                if (parameters != null && this.ParameterTypes.Count == parameters.Count) {
                    for (int offset = 0; offset < this.ParameterTypes.Count && parameterDictionary != null; offset++) {

                        if (this.ParameterTypes[offset].IsList == true) {
                            if (parameters[offset].HasMany(this.ParameterTypes[offset].Type, this.ParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(this.ParameterTypes[offset].Name, parameters[offset]);
                            }
                            else {
                                // Parameter type mismatch. Return null.
                                parameterDictionary = null;
                            }
                        }
                        else {
                            if (parameters[offset].HasOne(this.ParameterTypes[offset].Type, this.ParameterTypes[offset].IsConvertable) == true) {
                                parameterDictionary.Add(this.ParameterTypes[offset].Name, parameters[offset]);
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
