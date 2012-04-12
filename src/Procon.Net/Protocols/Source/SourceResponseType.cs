using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Source {
    public enum SourceResponseType {
        SERVERDATA_RESPONSE_VALUE = 0,
        SERVERDATA_AUTH_RESPONSE = 2,
        None = 127
    }
}
