using System;

namespace Procon.Net.Protocols.Frostbite.Objects {

    [Serializable]
    public static class FrostbiteConverter {

        public static string BoolToString(bool b) {
            return b == true ? "true" : "false";
        }
    }
}
