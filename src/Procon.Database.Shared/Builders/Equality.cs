using System;

namespace Procon.Database.Shared.Builders {
    /// <summary>
    /// Used for comparison between value and field
    /// </summary>
    [Serializable]
    public abstract class Equality : Operator, IEquality {
    }
}
