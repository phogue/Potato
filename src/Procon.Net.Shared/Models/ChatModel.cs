using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {

    [Serializable]
    public sealed class ChatModel : NetworkModel {

        public ChatModel() : base() {
            this.Now.Content = new List<String>();
        }
    }
}
