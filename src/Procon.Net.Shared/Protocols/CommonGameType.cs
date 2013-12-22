using System;

namespace Procon.Net.Shared.Protocols {

    /// <summary>
    /// The purpose of this over "Connection is BFBC2" is so the game has a
    /// unified way of describing itself across namespaces and serialization.
    /// </summary>
    [Serializable]
    public static class CommonGameType {
        public const String None = "None";

        public const String BF_BC2 = "BF_BC2";
        public const String BF_3 = "BF_3";
        public const String BF_4 = "BF_4";
        public const String COD_BO = "COD_BO";
        public const String HOMEFRONT = "HOMEFRONT";
        public const String MOH_2010 = "MOH_2010";
    }
}
