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
            new Save().Collection("Player").Set("Name", "Phogue").Set("Score", 100).Set("Rank", 10).Set("Kdr", 1),
            new Save().Collection("Player").Set("Name", "Zaeed").Set("Score", 15).Set("Rank", 20).Set("Kdr", 2),
            new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4),
            new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4)
        };
    }
}
