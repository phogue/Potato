#region

using System;
using Procon.Core.Shared.Database;

#endregion

namespace Procon.Core.Test.Database.Mocks {
    /// <summary>
    ///     A mock model with two simple fields
    /// </summary>
    public class MockSimpleModel : DatabaseModel<MockSimpleModel> {
        public int Id { get; set; }

        public String Name { get; set; }
    }
}