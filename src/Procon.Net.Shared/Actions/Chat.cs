using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Actions {

    [Serializable]
    public sealed class Chat : NetworkAction {

        /// <summary>
        /// Where the message originated from
        /// </summary>
        public ChatOrigin Origin { get; set; }

        public Chat() : base() {
            this.Now.Content = new List<String>();
            this.ActionType = NetworkActionType.NetworkSay;
        }
    }
}
