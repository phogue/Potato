using System;
using System.Linq;

namespace Procon.Net.Shared.Truths {
    /// <summary>
    /// The root element of the truth tree
    /// </summary>
    [Serializable]
    public class Tree : Truth {

        /// <summary>
        /// Merges a list of trees into one single tree
        /// </summary>
        /// <param name="trees">The list of trees to merge together</param>
        /// <returns>A new tree with all branches merged together</returns>
        public static Tree Union(params Tree[] trees) {
            Tree tree = new Tree();

            foreach (Tree branch in trees) {
                Tree.Merge(tree, branch);
            }

            return tree;
        }

        protected static ITruth Merge(ITruth source, ITruth other) {
            // Only merge if the roots match types.
            if (source.GetType() == other.GetType()) {
                foreach (ITruth otherNode in other) {
                    ITruth existingSourceNode = source.FirstOrDefault(c => c.GetType().IsInstanceOfType(otherNode));

                    if (existingSourceNode != null) {
                        Tree.Merge(existingSourceNode, otherNode);
                    }
                    else {
                        source.Add(otherNode);
                    }
                }
            }

            return source;
        }
    }
}
