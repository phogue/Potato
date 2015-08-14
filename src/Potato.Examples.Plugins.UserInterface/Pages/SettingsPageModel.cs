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
using Potato.Net.Shared.Models;

namespace Potato.Examples.Plugins.UserInterface.Pages {
    public partial class SettingsPageView {

        /// <summary>
        /// Just a string to output in the template. Shows how to use variables within the template.
        /// </summary>
        public string MyStringyVariable { get; set; }

        /// <summary>
        /// A list of players to output, showing how to loop within a template (therefore how to use conditions and what not as well)
        /// </summary>
        public IList<PlayerModel> MyListOfPlayers { get; set; }

        public SettingsPageView() : base() {
            MyStringyVariable = "Empty";

            MyListOfPlayers = new List<PlayerModel>();
        }
    }
}
