using System;
using System.Collections.Generic;
using Procon.Core.Database;

namespace Procon.Core.Test.Database.Mocks {
    /// <summary>
    /// A mock model with all types of models
    /// </summary>
    public class MockComplexModel : DatabaseModel<MockComplexModel> {
        public int IntegerValue { get; set; }

        public long LongValue { get; set; }

        public double DoubleValue { get; set; }

        public String StringValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public MockSimpleModel SingleMockSimpleModel { get; set; }

        public List<MockSimpleModel> MultipleMockSimpleModel { get; set; }
    }
}
