using System;
using Procon.Database.Shared.Builders;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// A method to execute on the database (update, delete, find etc.)
    /// </summary>
    [Serializable]
    public abstract class Method : DatabaseObject, IMethod {
    }
}
