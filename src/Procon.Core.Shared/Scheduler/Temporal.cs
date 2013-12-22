using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Core.Shared.Scheduler {

    [Serializable]
    public class Temporal : List<Func<DateTime, Task, bool>> {

        /// <summary>
        /// Checks if any of the stored  functions returns true for the given date time
        /// </summary>
        /// <param name="dt">The date time to check if a predicate matched</param>
        /// <param name="task">The task, which may have additional data to pass into the predicate</param>
        /// <returns>True, a predicate matched; false otherwise.</returns>
        public bool Check(DateTime dt, Task task) {
            return this.Clone().Count(x => x.Invoke(dt, task) == true) == this.Count;
        }

        /// <summary>
        /// Clones the current object
        /// </summary>
        /// <returns>The cloned object</returns>
        public List<Func<DateTime, Task, bool>> Clone() {
            return new List<Func<DateTime, Task, bool>>(this);
        }
    }
}
