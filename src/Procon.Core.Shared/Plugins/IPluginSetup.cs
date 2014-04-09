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

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from Procon AppDomain to the plugin AppDomain
    /// </summary>
    public interface IPluginSetup {
        /// <summary>
        /// The connection that owns this plugin instance.
        /// </summary>
        String ConnectionGuid { get; }

        /// <summary>
        /// Path to the default config-file of the plugin
        /// </summary>
        String ConfigDirectoryPath { get; }

        /// <summary>
        /// Path to the log-file directory of the plugin
        /// </summary>
        String LogDirectoryPath { get; }
    }
}
