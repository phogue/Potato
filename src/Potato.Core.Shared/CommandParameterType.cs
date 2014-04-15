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

namespace Potato.Core.Shared {
    /// <summary>
    /// Specifies what type of parameter the command is expecting
    /// </summary>
    public sealed class CommandParameterType {
        /// <summary>
        /// The name of parameter.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The singular type of parameter List-String- would just assign typeof(String)
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// True if this parameter requires a list of Type, or just a single Type
        /// </summary>
        public bool IsList { get; set; }

        /// <summary>
        /// Allow the conversion of the type, default is true.
        /// </summary>
        public bool IsConvertable { get; set; }

        /// <summary>
        /// Initializes the parameter type with default values.
        /// </summary>
        public CommandParameterType() {
            this.IsConvertable = true;
            this.IsList = false;
        }
    }
}
