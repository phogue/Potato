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

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Base model for objects within Procon.Net.*
    /// </summary>
    public interface INetworkModel {
        /// <summary>
        /// When this object was created.
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Where this data originated from.
        /// </summary>
        NetworkOrigin Origin { get; set; }

        /// <summary>
        /// The limiting factor(s) of the action
        /// </summary>
        NetworkModelData Scope { get; set; }

        /// <summary>
        /// Any data that reflects what something looked like before the action was taken
        /// </summary>
        NetworkModelData Then { get; set; }

        /// <summary>
        /// Any data showing the modifications or new data that was introduced with this action.
        /// </summary>
        NetworkModelData Now { get; set; }
    }
}
