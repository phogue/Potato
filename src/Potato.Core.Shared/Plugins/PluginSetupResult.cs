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

namespace Potato.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from the plugin AppDomain back to Potato AppDomain
    /// </summary>
    [Serializable]
    public class PluginSetupResult : IPluginSetupResult {
        public List<string> Commands { get; set; }
        public string Title { get; set; }
    }
}
