using System;

namespace Procon.Net.Shared.Protocols {

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
