using System;
using Procon.Database.Shared.Builders;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// A database field type
    /// </summary>
    [Serializable]
    public abstract class FieldType : DatabaseObject, IFieldType {
    }
}
