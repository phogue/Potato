using System;

namespace Procon.Database.Shared.Builders.Modifiers {
    /// <summary>
    /// Descending attribute. The lack of this attribute implies Ascending.
    /// </summary>
    [Serializable]
    public class Descending : SortByModifier {
    }
}
