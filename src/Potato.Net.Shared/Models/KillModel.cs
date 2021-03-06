﻿#region Copyright
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
using System.Collections.Generic;

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A description of a kill between server, Potato or player(s)
    /// </summary>
    [Serializable]
    public sealed class KillModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public KillModel() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
            this.Scope.Items = new List<ItemModel>();
            this.Scope.HumanHitLocations = new List<HumanHitLocation>();

            // Killer
            this.Now.Players = new List<PlayerModel>();
            this.Now.Items = new List<ItemModel>();
        }
    }
}
