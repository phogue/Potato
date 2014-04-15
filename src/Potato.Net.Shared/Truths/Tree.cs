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
using System.Linq;

namespace Potato.Net.Shared.Truths {
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
