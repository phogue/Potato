using System;

namespace Procon.Database.Shared.Builders {
    /// <summary>
    /// All modifiers that change a sorting order
    /// </summary>
    [Serializable]
    public abstract class SortByModifier : Modifier, ISortByModifier {
    }
}
