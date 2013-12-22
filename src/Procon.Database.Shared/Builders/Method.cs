using System;

namespace Procon.Database.Shared.Builders {
    /// <summary>
    /// A method to execute on the database (update, delete, find etc.)
    /// </summary>
    [Serializable]
    public abstract class Method : DatabaseObject, IMethod {
    }
}
