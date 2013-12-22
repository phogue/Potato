using System;
using Procon.Database.Shared.Builders;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// Used for comparison between value and field
    /// </summary>
    [Serializable]
    public abstract class Equality : Operator, IEquality {
    }
}
