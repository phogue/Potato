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

namespace Potato.Net.Shared.Protocols {

    /// <summary>
    /// The purpose of this over "Connection is BFBC2" is so the game has a
    /// unified way of describing itself across namespaces, serialization without needing
    /// to know this type.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The game type does not need to be defined in this class to be used, it's just
    ///         a way to maintain game type strings over tests and code without the risk
    ///         of a typo.
    ///     </para>
    /// </remarks>
    [Serializable]
    public static class CommonProtocolType {
        /// <summary>
        /// No game type specified.
        /// </summary>
        public const String None = "None";

        /// <summary>
        /// The game type is unknown. Even though it is not defined in this list though
        /// you can still define it on your type.
        /// </summary>
        public const String Unknown = "Unknown";

        /// <summary>
        /// Bad company 2
        /// </summary>
        public const String DiceBattlefieldBadCompany2 = "DiceBattlefieldBadCompany2";

        /// <summary>
        /// Battlefield 3
        /// </summary>
        public const String DiceBattlefield3 = "DiceBattlefield3";

        /// <summary>
        /// Battlefield 4
        /// </summary>
        public const String DiceBattlefield4 = "DiceBattlefield4";

        /// <summary>
        /// Battlefield 5
        /// </summary>
        public const String TreyarchCallOfDutyBlackOps = "TreyarchCallOfDutyBlackOps";

        /// <summary>
        /// Medal of Honor 2010
        /// </summary>
        public const String DiceMedalOfHonor2010 = "DiceMedalOfHonor2010";
    }
}
