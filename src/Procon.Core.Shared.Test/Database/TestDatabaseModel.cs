#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared.Test.Database.Mocks;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Values;

#endregion

namespace Procon.Core.Shared.Test.Database {
    /// <summary>
    ///     Tests serialization of a database model
    /// </summary>
    [TestFixture]
    public class TestDatabaseModel {
        /// <summary>
        ///     Tests serialization to a single complex database model
        /// </summary>
        [Test]
        public void TestComplexFromQuerySingle() {
            var item = new CollectionValue() {
                new DocumentValue().Set("IntegerValue", 1).Set("LongValue", 2).Set("DoubleValue", 3.5).Set("StringValue", "Phogue").Set("DateTimeValue", new DateTime(2013, 12, 19, 13, 38, 0)).Set("SingleMockSimpleModel", new Dictionary<String, Object>() {
                    {"Id", 10},
                    {"Name", "Zaeed"}
                }).Set("MultipleMockSimpleModel", new List<Object>() {
                    new Dictionary<String, Object>() {
                        {"Id", 20},
                        {"Name", "Ike"}
                    },
                    new Dictionary<String, Object>() {
                        {"Id", 30},
                        {"Name", "Phil"}
                    }
                })
            };

            var models = MockComplexModel.FromQuery(item);

            Assert.AreEqual(1, models.First().IntegerValue);
            Assert.AreEqual(2, models.First().LongValue);
            Assert.AreEqual(3.5D, models.First().DoubleValue);
            Assert.AreEqual("Phogue", models.First().StringValue);
            Assert.AreEqual(new DateTime(2013, 12, 19, 13, 38, 0), models.First().DateTimeValue);
            Assert.AreEqual(10, models.First().SingleMockSimpleModel.Id);
            Assert.AreEqual("Zaeed", models.First().SingleMockSimpleModel.Name);
            Assert.AreEqual(20, models.First().MultipleMockSimpleModel.First().Id);
            Assert.AreEqual("Ike", models.First().MultipleMockSimpleModel.First().Name);
            Assert.AreEqual(30, models.First().MultipleMockSimpleModel.Last().Id);
            Assert.AreEqual("Phil", models.First().MultipleMockSimpleModel.Last().Name);
        }

        /// <summary>
        ///     Tests a complex model can be serialized to a Save query
        /// </summary>
        [Test]
        public void TestComplexToSaveQuery() {
            Save save = new MockComplexModel() {
                IntegerValue = 1,
                LongValue = 2,
                DoubleValue = 3.5D,
                StringValue = "Phogue",
                DateTimeValue = new DateTime(2013, 12, 19, 14, 14, 0),
                SingleMockSimpleModel = new MockSimpleModel() {
                    Id = 10,
                    Name = "Zaeed"
                },
                MultipleMockSimpleModel = new List<MockSimpleModel>() {
                    new MockSimpleModel() {
                        Id = 20,
                        Name = "Ike"
                    },
                    new MockSimpleModel() {
                        Id = 30,
                        Name = "Phil"
                    }
                }
            }.ToSaveQuery();

            var integerValue = save.ElementAt(0).First(value => value is NumericValue) as NumericValue;
            var longValue = save.ElementAt(1).First(value => value is NumericValue) as NumericValue;
            var doubleValue = save.ElementAt(2).First(value => value is NumericValue) as NumericValue;
            var stringValue = save.ElementAt(3).First(value => value is StringValue) as StringValue;
            var dateTimeValue = save.ElementAt(4).First(value => value is DateTimeValue) as DateTimeValue;

            var documentValue = save.ElementAt(5).First(value => value is DocumentValue) as DocumentValue;
            var documentValueDoubleValue = documentValue.ElementAt(0).First(value => value is NumericValue) as NumericValue;
            var documentValueStringValue = documentValue.ElementAt(1).First(value => value is StringValue) as StringValue;

            var collectionValue = save.ElementAt(6).First(value => value is CollectionValue) as CollectionValue;
            var collectionValueFirstDocumentValueDoubleValue = collectionValue.ElementAt(0).ElementAt(0).First(value => value is NumericValue) as NumericValue;
            var collectionValueFirstDocumentValueStringValue = collectionValue.ElementAt(0).ElementAt(1).First(value => value is StringValue) as StringValue;
            var collectionValueLastDocumentValueDoubleValue = collectionValue.ElementAt(1).ElementAt(0).First(value => value is NumericValue) as NumericValue;
            var collectionValueLastDocumentValueStringValue = collectionValue.ElementAt(1).ElementAt(1).First(value => value is StringValue) as StringValue;

            Assert.AreEqual(1, integerValue.Long);
            Assert.AreEqual(2, longValue.Long);
            Assert.AreEqual(3.5D, doubleValue.Double);
            Assert.AreEqual("Phogue", stringValue.Data);
            Assert.AreEqual(new DateTime(2013, 12, 19, 14, 14, 0), dateTimeValue.Data);

            Assert.AreEqual(10.0D, documentValueDoubleValue.Double);
            Assert.AreEqual("Zaeed", documentValueStringValue.Data);

            Assert.AreEqual(20.0D, collectionValueFirstDocumentValueDoubleValue.Double);
            Assert.AreEqual("Ike", collectionValueFirstDocumentValueStringValue.Data);
            Assert.AreEqual(30.0D, collectionValueLastDocumentValueDoubleValue.Double);
            Assert.AreEqual("Phil", collectionValueLastDocumentValueStringValue.Data);
        }

        /// <summary>
        ///     Tests serialization of multiple database model
        /// </summary>
        [Test]
        public void TestSimpleFromQueryMultiple() {
            var item = new CollectionValue() {
                new DocumentValue().Set("Id", 50).Set("Name", "Phogue"),
                new DocumentValue().Set("Id", 100).Set("Name", "Zaeed")
            };

            var models = MockSimpleModel.FromQuery(item);

            Assert.AreEqual(50, models.First().Id);
            Assert.AreEqual("Phogue", models.First().Name);

            Assert.AreEqual(100, models.Last().Id);
            Assert.AreEqual("Zaeed", models.Last().Name);
        }

        /// <summary>
        ///     Tests serialization to a single database model
        /// </summary>
        [Test]
        public void TestSimpleFromQuerySingle() {
            var item = new CollectionValue() {
                new DocumentValue().Set("Id", 50).Set("Name", "Phogue")
            };

            var models = MockSimpleModel.FromQuery(item);

            Assert.AreEqual(50, models.First().Id);
            Assert.AreEqual("Phogue", models.First().Name);
        }

        /// <summary>
        ///     Tests a simple model can be serialized to a Modify query
        /// </summary>
        [Test]
        public void TestSimpleToModifyQuery() {
            Modify modify = new MockSimpleModel() {
                Name = "Phogue",
                Id = 10
            }.ToModifyQuery();

            var playerId = modify.First().First(value => value is NumericValue) as NumericValue;
            var name = modify.Last().First(value => value is StringValue) as StringValue;


            Assert.AreEqual(10, playerId.Long);
            Assert.AreEqual("Phogue", name.Data);
        }

        /// <summary>
        ///     Tests a simple model can be serialized to a Save query
        /// </summary>
        [Test]
        public void TestSimpleToSaveQuery() {
            Save save = new MockSimpleModel() {
                Name = "Phogue",
                Id = 10
            }.ToSaveQuery();

            var playerId = save.First().First(value => value is NumericValue) as NumericValue;
            var name = save.Last().First(value => value is StringValue) as StringValue;


            Assert.AreEqual(10, playerId.Long);
            Assert.AreEqual("Phogue", name.Data);
        }
    }
}