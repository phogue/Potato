using System;
using Procon.Database.Shared.Builders;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// All modifiers that change a sorting order
    /// </summary>
    [Serializable]
    public abstract class SortByModifier : Modifier, ISortByModifier {
    }
}
