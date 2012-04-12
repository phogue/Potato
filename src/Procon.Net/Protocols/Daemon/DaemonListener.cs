using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Daemon {
    // Should listen to and manage connections at a basic level for a daemon
    // Security and packet logic should be handled elsewhere.
    public class Listener {

        // (Private?) List<DaemonClient> ...

        // y: Event: Connection Event - Like Game.
            // x: Event: Connection Accepted
            // x: Event: Connection Dropped

            // The actual clients should fire their own Client Events.
        
            // Perhaps Listener should pass all of the client events on itself?
            // This would mean anything inheriting from this class can be dumb of the inner workings
            // and just know that a client wanted something.
    }
}
