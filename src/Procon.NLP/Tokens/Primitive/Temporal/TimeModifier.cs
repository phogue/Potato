using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.NLP.Tokens.Primitive.Temporal {
    public enum TimeModifier {
        None,
        // in 20 minutes
        Delay,
        // for 20 minutes
        Period,
        // every 20 minutes
        Interval
    }
}
