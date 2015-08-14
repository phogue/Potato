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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using NUnit.Framework;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Methods.Data;

namespace Potato.Database.Test.Integration.SqLite {
    [TestFixture]
    public class TestSqLiteIntegrationModify : TestDatabaseIntegrationModify {

        public TestSqLiteIntegrationModify() {
            Integration = new SqLiteDatabaseIntegration();
        }

        [Test]
        public override void TestModifyPlayerSetName() {
            Integration.TestFindQuery(TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                TestModifyPlayerSetNameImplicit
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
            Integration.TestFindQuery(TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                TestModifyPlayerSetNameScoreImplicit
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
            Integration.TestFindQuery(TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit
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
            Integration.TestFindQuery(TestPlayerTableSetup.Union(new List<IDatabaseObject>() {
                TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit
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
