using System;

namespace Procon.Core.Shared.Scheduler {

    [Serializable]
    public class TickEventArgs : EventArgs {
        public DateTime TickedAt { get; set; }
    }
}
