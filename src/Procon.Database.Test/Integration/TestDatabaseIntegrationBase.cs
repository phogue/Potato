using System.Collections.Generic;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods;

namespace Procon.Database.Test.Integration {
    public abstract class TestDatabaseIntegrationBase {

        protected IDatabaseIntegration Integration { get; set; }

        /// <summary>
        /// Creates a player table, populating it with four items for us to test with.
        /// </summary>
        protected List<IDatabaseObject> TestPlayerTableSetup = new List<IDatabaseObject>() {
            new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType()),
            new Save().Collection("Player").Assignment("Name", "Phogue").Assignment("Score", 100).Assignment("Rank", 10).Assignment("Kdr", 1),
            new Save().Collection("Player").Assignment("Name", "Zaeed").Assignment("Score", 15).Assignment("Rank", 20).Assignment("Kdr", 2),
            new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4),
            new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4)
        };
    }
}
