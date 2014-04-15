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
namespace Potato.Core.Shared {
    /// <summary>
    /// Specifies when a dispatch handler will be called in the command execution
    /// </summary>
    public enum CommandAttributeType {
        /// <summary>
        /// We want to preview a command. This allows for commands to be cancelled
        /// by returning any result status other than Continue.
        /// </summary>
        Preview,
        /// <summary>
        /// The actual executable method which will use the command and return a result.
        /// </summary>
        /// <remarks>This is the default used in the CommandAttribute class.</remarks>
        Handler,
        /// <summary>
        /// Executed shows what command has been previewed and passed, then executed
        /// as well as the result of the execution. A way to see what has been modified
        /// elsewhere.
        /// </summary>
        Executed
    }
}
