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
using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Connections.Plugins {
    /// <summary>
    /// Manages loading and propogating plugin events, as well as callbacks from
    /// a plugin back to Procon.
    /// </summary>
    public interface ICorePluginController : ICoreController {
        /// <summary>
        /// List of plugins loaded in the app domain.
        /// </summary>
        List<PluginModel> LoadedPlugins { get; set; }
    }
}
