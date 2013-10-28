using System;

namespace Procon.Net.Utils.PunkBuster.Objects {
    [Serializable]
    public class PunkBusterPlayer : PunkBusterObject {

        public uint SlotID { get; set; }
        public String IP { get; set; }
        public String Name { get; set; }
        public String GUID { get; set; }

    }
}
