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
using System.Collections.Generic;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;

namespace Potato.Database.Test.Integration {
    public abstract class TestDatabaseIntegrationBase {

        protected IDatabaseIntegration Integration { get; set; }

        /// <summary>
        /// Creates a player table, populating it with four items for us to test with.
        /// </summary>
        protected List<IDatabaseObject> TestPlayerTableSetup = new List<IDatabaseObject>() {
            new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType()),
            new Save().Collection("Player").Set("Name", "Phogue").Set("Score", 100).Set("Rank", 10).Set("Kdr", 1),
            new Save().Collection("Player").Set("Name", "Zaeed").Set("Score", 15).Set("Rank", 20).Set("Kdr", 2),
            new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4),
            new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4)
        };
    }
}
