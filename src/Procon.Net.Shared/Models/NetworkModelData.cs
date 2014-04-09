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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Underlying data group for a model
    /// </summary>
    [Serializable]
    public class NetworkModelData {
        /// <summary>
        /// List of players that have an effect with this action.
        /// </summary>
        public List<PlayerModel> Players { get; set; }

        /// <summary>
        /// A list of strings attached to this network action. A reason associated with the action.
        /// Why the action is being taken or why it was taken, or text used in the action.
        /// </summary>
        public List<String> Content { get; set; }

        /// <summary>
        /// The groups attached to this action.
        /// </summary>
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this action, if any.
        /// </summary>
        public List<Point3DModel> Points { get; set; }

        /// <summary>
        /// List of items attached to this action, if any.
        /// </summary>
        public List<ItemModel> Items { get; set; }

        /// <summary>
        /// List of maps attached to this action, if any.
        /// </summary>
        public List<MapModel> Maps { get; set; }

        /// <summary>
        /// List of time subsets attached to this action, if any.
        /// </summary>
        public List<TimeSubsetModel> Times { get; set; }

        /// <summary>
        /// List of human hit location attached to this data, if any.
        /// </summary>
        public List<HumanHitLocation> HumanHitLocations { get; set; }

        /// <summary>
        /// List of packets attached to this action, if any.
        /// </summary>
        public List<IPacket> Packets { get; set; }
    }
}
