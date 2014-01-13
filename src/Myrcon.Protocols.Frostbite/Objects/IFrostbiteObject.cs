using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {

    // Used or useful?
    public interface IFrostbiteObject {

        NetworkModel Parse(List<String> words);

    }
}
