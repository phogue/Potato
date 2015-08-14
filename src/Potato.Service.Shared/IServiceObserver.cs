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

namespace Potato.Service.Shared {
    /// <summary>
    /// Used to track the state of the service
    /// </summary>
    public interface IServiceObserver {
        /// <summary>
        /// Delegate called when the status is modified
        /// </summary>
        Action<IServiceObserver, ServiceStatusType> StatusChange { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        ServiceStatusType Status { get; set; }

        /// <summary>
        /// When the service was started
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// When the service was stopped
        /// </summary>
        DateTime? StopTime { get; }

        /// <summary>
        /// Fetch the current uptime of the service
        /// </summary>
        /// <returns>How long the service has been started</returns>
        TimeSpan? Uptime();

        /// <summary>
        /// Fetch the current downtime of the service
        /// </summary>
        /// <returns>How long the service has been stopped</returns>
        TimeSpan? Downtime();
    }
}
