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
using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Methods.Schema;
using Procon.Database.Shared.Builders.Values;
using Procon.Database.Shared.Utils;

namespace Procon.Database.Shared.Test.Utils {
    [TestFixture]
    public class TestDatabaseObjectUtils {
        /// <summary>
        /// Tests all descendants of a type can be extracted from a tree.
        /// </summary>
        [Test]
        public void TestDescendantsAndSelfQueryTree() {
            List<StringValue> values = new List<StringValue>() {
                new StringValue() {
                    Data = "0"
                },
                new StringValue() {
                    Data = "1"
                },
                new StringValue() {
                    Data = "2"
                },
                new StringValue() {
                    Data = "3"
                }
            };

            // Create a tree of values. We will want to flatten and find each value after.
            IDatabaseObject query = new Find()
                .Method(
                    values[0]
                )
                .Method(
                    values[1]
                )
                .Method(
                    new Drop()
                    .Field(
                        values[2]
                        .Method(
                            new Save()
                            .Field(
                                values[3]
                            )
                        )
                    )
                );

            List<StringValue> descendants = query.DescendantsAndSelf<StringValue>().ToList();

            Assert.AreEqual(4, descendants.Count());
            
            foreach (StringValue item in descendants) {
                Assert.IsTrue(values.Contains(item));    
            }
        }

        /// <summary>
        /// Tests that the "self" part of DescendantsAndSelf will be returned if it matches the type.
        /// </summary>
        [Test]
        public void TestDescendantsAndSelfSingleValue() {
            StringValue value = new StringValue() {
                Data = "0"
            };

            List<StringValue> descendants = value.DescendantsAndSelf<StringValue>().ToList();

            Assert.AreEqual(1, descendants.Count());
            Assert.AreEqual(value, descendants.First());
        }
    }
}
