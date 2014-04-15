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
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Actions {
    /// <summary>
    /// An action for the network layer to execute
    /// </summary>
    [Serializable]
    public class NetworkAction : NetworkModel, INetworkAction {
        public String Name {
            get { return this._name; }
            set {
                if (this._name != value) {
                    this._name = value;

                    NetworkActionType parsed = NetworkActionType.None;
                    
                    if (Enum.TryParse(this._name, true, out parsed) == true) {
                        this.ActionType = parsed;
                    }
                }
            }
        }
        private String _name;

        public NetworkActionType ActionType {
            get { return this._actionType; }
            set {
                if (this._actionType != value) {
                    this._actionType = value;
                    this.Name = this._actionType.ToString();
                }
            }
        }
        private NetworkActionType _actionType;

        public Guid Uid { get; set; }

        /// <summary>
        /// Initializes the network action with a new Guid
        /// </summary>
        public NetworkAction(): base() {
            this.Uid = Guid.NewGuid();
        }
    }
}
