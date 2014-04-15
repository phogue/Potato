#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region

using System;
using System.Collections.Generic;
using Potato.Core.Shared.Database;

#endregion

namespace Potato.Core.Shared.Test.Database.Mocks {
    /// <summary>
    ///     A mock model with all types of models
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