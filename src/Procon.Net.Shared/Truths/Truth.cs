using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Shared.Truths {
    /// <summary>
    /// An implementation of ITruth and base object for the truth tree.
    /// </summary>
    [Serializable]
    public abstract class Truth : List<ITruth>, ITruth {
        public bool BuildAndTest(params ITruth[] branches) {
            List<ITruth> branch = new List<ITruth>() {
                branches.FirstOrDefault()
            };

            for (var offset = branches.Length - 1; offset >= 1; offset--) {
                branches[offset - 1].Add(branches[offset]);
            }

            return this.Test(branch);
        }

        public bool Test(List<ITruth> branches) {
            ITruth node = branches.FirstOrDefault();
            bool truth = false;
            
            if (node != null) {
                foreach (ITruth t in this.Where(t => t.GetType().IsInstanceOfType(node) == true)) {
                    truth = t.Test(new List<ITruth>(node));
                }
            }
            else {
                truth = true;
            }

            return truth;
        }
    }
}
