using System;

namespace Procon.Core.Scheduler {

    [Serializable]
    public class TickEventArgs : EventArgs {
        public DateTime TickedAt { get; set; }
    }
}
