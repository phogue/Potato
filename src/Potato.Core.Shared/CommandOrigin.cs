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
    /// Where the command originated from (within Potato, over the command server or from a plugin)
    /// </summary>
    [Serializable]
    public enum CommandOrigin {
        /// <summary>
        /// Unknown location for where the command came from. This will always fail security checks.
        /// </summary>
        None,
        /// <summary>
        /// The command came from a plugin or was generated internally
        /// </summary>
        Local,
        /// <summary>
        /// The command came from a remote client
        /// </summary>
        Remote,
        /// <summary>
        /// The command came from a plugin
        /// </summary>
        Plugin
    }
}
