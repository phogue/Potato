using System;
using System.Collections.Generic;
using Procon.Net.Data;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {

    // Used or useful?
    public interface IFrostbiteObject {

        NetworkObject Parse(List<String> words);

    }
}
