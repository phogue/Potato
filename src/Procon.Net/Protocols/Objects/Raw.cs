using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Objects {
    public class Raw : Action {

        public RawActionType RawActionType { get; set; }

        public String PacketText { get; set; }

        public Raw() : base() {
            this.PacketText = String.Empty;
        }

        public Raw(String format, params object[] args) : base() {
            this.PacketText = String.Format(format, args);
        }
    }
}
