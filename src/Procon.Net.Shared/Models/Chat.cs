using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {

    [Serializable]
    public sealed class Chat : NetworkModel {

        public Chat() : base() {
            this.Now.Content = new List<String>();
        }
    }
}
