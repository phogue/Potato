using System;

namespace Procon.Database.Shared.Builders {
    /// <summary>
    /// Base implementation for index modififiers
    /// </summary>
    [Serializable]
    public abstract class IndexModifier : DatabaseObject, IIndexModifier {
    }
}
