#region Copyright
// Copyright 2015 Geoff Green.
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
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Potato.Database.Test.Integration.SqLite {
    [TestFixture]
    public class TestSqLiteIntegrationFind : TestDatabaseIntegrationFind {

        public TestSqLiteIntegrationFind() {
            Integration = new SqLiteDatabaseIntegration();
        }

        [Test]
        public override void TestSelectAllFromPlayer() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerImplicit, new JArray() {
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
        public override void TestSelectDistinctAllFromPlayer() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectDistinctAllFromPlayerImplicit, new JArray() {
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
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereNameEqualsPhogueImplicit, new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 100.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31F() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31FImplicit, new JArray() {
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
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31F() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereKdrLessThanEqualTo31FImplicit, new JArray() {
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
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWherePlayerNameEqualsPhogueImplicit, new JArray() {
                new JObject() {
                    new JProperty("Name", "Phogue"),
                    new JProperty("Score", 100.0),
                    new JProperty("Rank", 10.0),
                    new JProperty("Kdr", 1.0)
                }
            });
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectScoreFromPlayerWhereNameEqualsPhogueImplicit, new JArray() {
                new JObject() {
                    new JProperty("Score", 100.0)
                }
            });
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectScoreRankFromPlayerWhereNameEqualsPhogueImplicit, new JArray() {
                new JObject() {
                    new JProperty("Score", 100.0),
                    new JProperty("Rank", 10.0)
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit, new JArray());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit, new JArray() {
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
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit, new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                }
            });
        }

        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit, new JArray() {
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
                }
            });
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerSortByScoreImplicit, new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                },
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
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            Integration.TestFindQuery(TestPlayerTableSetup, TestSelectAllFromPlayerSortByNameThenScoreDescendingImplicit, new JArray() {
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
                },
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
                }
            });
        }
    }
}
