using Procon.Fuzzy.Tokens.Object;
using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive.Numeric;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference to the Kills property of a player object
    /// </summary>
    public class KillsPropertyReference : INumericPropertyReference {
        public IThingReference ThingReference { get; set; }

        /// <summary>
        /// Initializes with the default player thing reference
        /// </summary>
        public KillsPropertyReference() {
            this.ThingReference = new PlayerThingReference();
        }

        public bool RemoveAll(IThingReference thing, EqualityLogicalOperatorToken comparator, FloatNumericPrimitiveToken value) {
            bool result = false;
            PlayerThingReference playerReference = thing as PlayerThingReference;

            if (playerReference != null) {
                // Think opposite here as we remove all that don't match.
                if (comparator is EqualsEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kills != value.ToFloat());
                }
                else if (comparator is GreaterThanEqualToEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kills < value.ToFloat());
                }
                else if (comparator is GreaterThanEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kills <= value.ToFloat());
                }
                else if (comparator is LessThanEqualToEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kills > value.ToFloat());
                }
                else if (comparator is LessThanEqualityLogicalOperatorToken) {
                    playerReference.Players.RemoveAll(player => player.Kills >= value.ToFloat());
                }

                result = playerReference.Players.Count > 0;
            }

            return result;
        }
    }
}
