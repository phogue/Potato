using System;

namespace Procon.Database.Shared.Builders {
    /// <summary>
    /// A database field type
    /// </summary>
    [Serializable]
    public abstract class FieldType : DatabaseObject, IFieldType {
    }
}
