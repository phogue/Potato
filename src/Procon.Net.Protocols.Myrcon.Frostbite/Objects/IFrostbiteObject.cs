using System;
using System.Collections.Generic;
using Procon.Net.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {

    // Used or useful?
    public interface IFrostbiteObject {

        NetworkModel Parse(List<String> words);

    }
}
