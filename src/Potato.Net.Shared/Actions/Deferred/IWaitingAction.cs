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

namespace Potato.Net.Shared.Actions.Deferred {
    /// <summary>
    /// An action currently pending completion
    /// </summary>
    public interface IWaitingAction {
        /// <summary>
        /// When we should stop waiting for actions and accept the action has timed out.
        /// </summary>
        DateTime Expiration { get; set; }

        /// <summary>
        /// The action taken to generate the requests
        /// </summary>
        INetworkAction Action { get; set; }

        /// <summary>
        /// List of packets waiting for responses
        /// </summary>
        List<IPacket> Requests { get; set; }

        /// <summary>
        /// List of responses to requests we have sent. When Responses.Count == Requests.Count, we have a completed action.
        /// </summary>
        List<IPacket> Responses { get; set; } 
    }
}
