using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Shared.Truths {
    /// <summary>
    /// An implementation of ITruth and base object for the truth tree.
    /// </summary>
    public abstract class Truth : List<ITruth>, ITruth {

        public bool Exists(params ITruth[] branches) {
            return this.Exists(branches.ToList());
        }

        public bool Exists(List<ITruth> branches) {
            ITruth node = branches.FirstOrDefault();
            bool truth = false;
            
            if (node != null) {
                foreach (ITruth t in this.Where(t => t.GetType().IsInstanceOfType(node) == true)) {
                    truth = t.Exists(branches.Skip(1).ToList());
                }
            }
            else {
                truth = true;
            }

            return truth;
        }
    }
}
