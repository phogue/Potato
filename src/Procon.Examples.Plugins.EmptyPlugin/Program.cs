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
using Procon.Core.Shared.Plugins;

namespace Procon.Examples.Plugins.EmptyPlugin {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    /// <remarks>
    /// This is deliberately empty as this is the minimal code to run by Procon.
    /// </remarks>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

    }
}
