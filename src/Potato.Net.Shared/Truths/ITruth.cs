#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;

namespace Potato.Net.Shared.Truths {
    /// <summary>
    /// A truthy object that can be polled to determine if its children has a specific branch.
    /// </summary>
    public interface ITruth : ICollection<ITruth> {
        /// <summary>
        /// Checks if a given branch is within the tree
        /// </summary>
        /// <param name="branches">The branches to check against the tree</param>
        /// <returns>True if the entire branch exists in our tree</returns>
        bool BuildAndTest(params ITruth[] branches);

        /// <summary>
        /// Checks if a given branch is within the tree
        /// </summary>
        /// <param name="branches">The branches to check against the tree</param>
        /// <returns>True if the entire branch exists in our tree</returns>
        bool Test(List<ITruth> branches);
    }
}
