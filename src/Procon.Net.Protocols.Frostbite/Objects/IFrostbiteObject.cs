using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite.Objects {

    // Used or useful?
    public interface IFrostbiteObject {

        NetworkObject Parse(List<String> words);

    }
}
