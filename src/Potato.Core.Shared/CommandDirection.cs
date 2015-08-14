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
namespace Potato.Core.Shared {
    /// <summary>
    /// The direction a command should propogate in
    /// </summary>
    public enum CommandDirection {
        /// <summary>
        /// The command should go down the tree of children
        /// </summary>
        Tunnel,
        /// <summary>
        /// The command should go up through the ancestors
        /// </summary>
        Bubble
    }
}
