using System;
using System.Collections.Generic;

namespace Procon.Service.Shared {
    [Serializable]
    public sealed class ServiceMessage : IDisposable {
        
        /// <summary>
        /// A simple name for this message.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A list of arguments for this message
        /// </summary>
        public Dictionary<String, String> Arguments { get; set; }

        /// <summary>
        /// When this message was created.
        /// </summary>
        public DateTime Stamp { get; set; }

        public ServiceMessage() {
            this.Arguments = new Dictionary<String, String>();
            this.Stamp = DateTime.Now;
        }

        public void Dispose() {
            this.Name = null;

            if (this.Arguments != null) this.Arguments.Clear();
            this.Arguments = null;
        }
    }
}
