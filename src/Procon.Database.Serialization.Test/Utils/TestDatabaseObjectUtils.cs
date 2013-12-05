using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Values;
using Procon.Database.Serialization.Utils;

namespace Procon.Database.Serialization.Test.Utils {
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

            IEnumerable<StringValue> descendants = query.DescendantsAndSelf<StringValue>();

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

            IEnumerable<StringValue> descendants = value.DescendantsAndSelf<StringValue>();

            Assert.AreEqual(1, descendants.Count());
            Assert.AreEqual(value, descendants.First());
        }
    }
}
