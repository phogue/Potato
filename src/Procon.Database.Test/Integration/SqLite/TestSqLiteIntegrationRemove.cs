using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Builders.Methods.Data;
using Procon.Database.Shared;

namespace Procon.Database.Test.Integration.SqLite {
    [TestFixture]
    public class TestSqLiteIntegrationRemove : TestDatabaseIntegrationRemove {

        public TestSqLiteIntegrationRemove() {
            this.Integration = new SqLiteDatabaseIntegration();
        }

        [Test]
        public override void TestRemoveAllFromPlayer() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerImplicit
            }),
            new Find().Collection("Player"),
            new JArray());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
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
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
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
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit
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

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit
            }),
            new Find().Collection("Player"),
            new JArray() {
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
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit
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
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            this.Integration.TestFindQuery(this.TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit
            }),
            new Find().Collection("Player"),
            new JArray() {
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
