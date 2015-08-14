#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Net.Shared.Truths {
    /// <summary>
    /// An implementation of ITruth and base object for the truth tree.
    /// </summary>
    [Serializable]
    public abstract class Truth : List<ITruth>, ITruth {
        public bool BuildAndTest(params ITruth[] branches) {
            var branch = new List<ITruth>() {
                branches.FirstOrDefault()
            };

            for (var offset = branches.Length - 1; offset >= 1; offset--) {
                branches[offset - 1].Add(branches[offset]);
            }

            return Test(branch);
        }

        public bool Test(List<ITruth> branches) {
            var node = branches.FirstOrDefault();
            var truth = false;
            
            if (node != null) {
                foreach (var t in this.Where(t => t.GetType().IsInstanceOfType(node) == true)) {
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
