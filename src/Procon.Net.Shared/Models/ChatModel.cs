using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A line of text between servers, procon or players.
    /// </summary>
    [Serializable]
    public sealed class ChatModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public ChatModel() : base() {
            this.Now.Content = new List<String>();
        }
    }
}
