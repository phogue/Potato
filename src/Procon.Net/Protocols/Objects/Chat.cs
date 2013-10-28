using System;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public sealed class Chat : NetworkAction {

        /// <summary>
        /// Where the message originated from
        /// </summary>
        public ChatOrigin Origin { get; set; }

        public Chat() : base() {
            //this.Author = new Player();
            //this.Subset = new PlayerSubset();
            //this.Text = String.Empty;
            this.ActionType = NetworkActionType.NetworkSay;
        }
    }
}
