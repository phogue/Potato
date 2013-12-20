using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Methods.Data;

namespace Procon.Database.Test.Integration.SqLite {
    [TestFixture]
    public class TestSqLiteIntegrationModify : TestDatabaseIntegrationModify {

        public TestSqLiteIntegrationModify() {
            this.Integration = new SqLiteDatabaseIntegration();
        }

        [Test]
        public override void TestModifyPlayerSetName() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestModifyPlayerSetNameImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 100.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                }
            });
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestModifyPlayerSetNameScoreImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 50.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 50.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 50.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                },
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 50.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                }
            });
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 50.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                },
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                },
                new JObject() {
                    new JProperty("Name", "Duplicate"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                },
                new JObject() {
                    new JProperty("Name", "Duplicate"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                }
            });
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit
            }),
            new Find().Collection("Player"),
            new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 100.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                },
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                },
                new JObject() {
                    new JProperty("Name", "Duplicate"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                },
                new JObject() {
                    new JProperty("Name", "Duplicate"),
                    new JProperty("Score", 1000.00),
                    new JProperty("Rank", 100.0),
                    new JProperty("Kdr", 4.0)
                }
            });
        }
    }
}
