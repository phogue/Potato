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
using Procon.Fuzzy.Tokens.Object;
using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive.Numeric;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference to the Kdr property of a player object
    /// </summary>
    public class KdrPropertyReference : INumericPropertyReference {
        public IThingReference ThingReference { get; set; }

        /// <summary>
        /// Initializes with the default player thing reference
        /// </summary>
        public KdrPropertyReference() {
            this.ThingReference = new PlayerThingReference();
        }

        public bool RemoveAll(IThingReference thing, EqualityLogicalOperatorToken comparator, FloatNumericPrimitiveToken value) {
            bool result = false;
            PlayerThingReference playerReference = thing as PlayerThingReference;

            if (playerReference != null) {
                // Think opposite here as we remove all that don't match.
                if (comparator is EqualsEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kdr != value.ToFloat());
                }
                else if (comparator is GreaterThanEqualToEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kdr < value.ToFloat());
                }
                else if (comparator is GreaterThanEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kdr <= value.ToFloat());
                }
                else if (comparator is LessThanEqualToEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kdr > value.ToFloat());
                }
                else if (comparator is LessThanEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kdr >= value.ToFloat());
                }

                result = playerReference.Players.Count > 0;
            }

            return result;
        }
    }
}
