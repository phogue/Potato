using System.Collections.Generic;

namespace Procon.Net.Shared.Truths {
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
