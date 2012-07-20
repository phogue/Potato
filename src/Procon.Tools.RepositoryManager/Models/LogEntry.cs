using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Tools.RepositoryManager.Models {
    public class LogEntry {

        public DateTime Stamp { get; protected set; }

        public String Text { get; protected set; }

        public LogEntry(String format, params object[] args) {
            this.Stamp = DateTime.Now;
            this.Text = String.Format(format, args);
        }
    }
}
