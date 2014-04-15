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
namespace Potato.Service.Shared {
    /// <summary>
    /// The current state of the Potato server running.
    /// </summary>
    public enum ServiceStatusType {
        /// <summary>
        /// Default value, nothing has been said about the status yet.
        /// </summary>
        None,
        /// <summary>
        /// Service is currently stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// Service is in the process of stopping
        /// </summary>
        Stopping,
        /// <summary>
        /// Service is in the process of starting
        /// </summary>
        Starting,
        /// <summary>
        /// Service is currently running.
        /// </summary>
        Started
    }
}
