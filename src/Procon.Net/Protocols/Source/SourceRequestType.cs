using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Source {
    public enum SourceRequestType {
        SERVERDATA_ALLBAD = -1,
        SERVERDATA_EXECCOMMAND = 2,
        SERVERDATA_AUTH = 3,
        None = 127
    }
}
