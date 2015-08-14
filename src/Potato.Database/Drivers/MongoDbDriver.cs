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
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Values;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Drivers {
    /// <summary>
    /// Driver support for MonogDb
    /// </summary>
    public class MongoDbDriver : Driver {

        /// <summary>
        /// The open connection to the database.
        /// </summary>
        protected MongoClient Client { get; set; }

        /// <summary>
        /// The open database object to run queries against
        /// </summary>
        protected MongoDatabase Database { get; set; }

        public override string Name {
            get {
                return "MongoDB";
            }
        }

        public override bool Connect() {
            var opened = true;

            if (Client == null) {
                try {
                    var settings = new MongoClientSettings();

                    if (Settings.Hostname != null && Settings.Port.HasValue == true) {
                        settings.Server = new MongoServerAddress(Settings.Hostname, (int)Settings.Port.Value);
                    }
                    else if (Settings.Hostname != null) {
                        settings.Server = new MongoServerAddress(Settings.Hostname);
                    }

                    if (Database != null && Settings.Username != null && Settings.Password != null) {
                        settings.Credentials = new List<MongoCredential>() {
                        MongoCredential.CreateMongoCRCredential(Settings.Database, Settings.Username, Settings.Password)
                    };
                    }

                    Client = new MongoClient(settings);

                    Database = Client.GetServer().GetDatabase(Settings.Database);
                }
                catch {
                    opened = false;
                }
            }

            return opened;
        }

        /// <summary>
        /// Converts a BsonDocument to a DocumentValue (known to Potato)
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        protected DocumentValue ToDocument(BsonDocument document) {
            var row = new DocumentValue();

            foreach (var value in document.Elements) {
                var dotNetValue = BsonTypeMapper.MapToDotNetValue(value.Value);

                if (dotNetValue is ObjectId) {
                    dotNetValue = dotNetValue.ToString();
                }

                row.Set(value.Name, dotNetValue);
            }

            return row;
        }

        protected void QueryCreateIndex(ICompiledQuery query, CollectionValue result) {
            if (query.Collections.Any() == true) {
                var collection = Database.GetCollection(query.Collections.FirstOrDefault());

                var indices = BsonSerializer.Deserialize<BsonArray>(query.Indices.FirstOrDefault());

                if (indices.Count > 1) {
                    collection.EnsureIndex(new IndexKeysDocument(indices.First().AsBsonDocument), new IndexOptionsDocument(indices.Last().AsBsonDocument));
                }
                else {
                    collection.EnsureIndex(new IndexKeysDocument(indices.First().AsBsonDocument));
                }
            }
        }

        /// <summary>
        /// Select query on the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryFind(ICompiledQuery query, CollectionValue result) {
            var collection = Database.GetCollection(query.Collections.FirstOrDefault());

            var conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());

            var cursor = collection.Find(new QueryDocument(conditions.First().AsBsonDocument));

            if (query.Limit != null) {
                cursor.SetLimit((int)query.Limit.Value);
            }

            if (query.Skip != null) {
                cursor.SetSkip((int)query.Skip.Value);
            }

            result.AddRange(cursor.Select(ToDocument));
        }

        /// <summary>
        /// Modify all documents that match a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryModify(ICompiledQuery query, CollectionValue result) {
            var collection = Database.GetCollection(query.Collections.FirstOrDefault());

            var conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());
            var assignments = BsonSerializer.Deserialize<BsonArray>(query.Assignments.FirstOrDefault());

            var queryDocument = new QueryDocument(conditions.First().AsBsonDocument);
            var updateDocument = new UpdateDocument(assignments.First().AsBsonDocument);

            var writeConcernResult = collection.Update(queryDocument, updateDocument, UpdateFlags.Multi);

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Long = (int)writeConcernResult.DocumentsAffected
                    }
                }
            );
        }

        /// <summary>
        /// Query to remove documents from a collection.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryRemove(ICompiledQuery query, CollectionValue result) {
            var collection = Database.GetCollection(query.Collections.FirstOrDefault());

            var conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());

            var writeConcernResult = collection.Remove(new QueryDocument(conditions.First().AsBsonDocument));

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Long = (int)writeConcernResult.DocumentsAffected
                    }
                }
            );
        }

        /// <summary>
        /// Drops a database or collection
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryDrop(ICompiledQuery query, CollectionValue result) {
            if (query.Databases.Any() == true && Database.Name == query.Databases.FirstOrDefault()) {
                Database.Drop();

                result.Add(
                    new Affected() {
                        new NumericValue() {
                            Long = 1
                        }
                    }
                );
            }
            else {
                var collection = Database.GetCollection(query.Collections.FirstOrDefault());

                var commandResult = collection.Drop();

                result.Add(
                    new Affected() {
                        new NumericValue() {
                            Long = commandResult.Ok == true ? 1 : 0
                        }
                    }
                );
            }
        }

        /// <summary>
        /// find and update, with upsert.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryMerge(ICompiledQuery query, CollectionValue result) {
            var collection = Database.GetCollection(query.Collections.FirstOrDefault());

            var save = query.Children.FirstOrDefault(child => child.Root is Save);
            var modify = query.Children.FirstOrDefault(child => child.Root is Modify);

            if (save != null && modify != null) {
                var conditions = BsonSerializer.Deserialize<BsonArray>(modify.Conditions.FirstOrDefault());
                var assignments = BsonSerializer.Deserialize<BsonArray>(save.Assignments.FirstOrDefault());
                var sortings = BsonSerializer.Deserialize<BsonArray>(modify.Sortings.FirstOrDefault());

                var queryDocument = new QueryDocument(conditions.First().AsBsonDocument);
                IMongoSortBy sortByDocument = new SortByDocument(sortings.First().AsBsonDocument);
                var updateDocument = new UpdateDocument(assignments.First().AsBsonDocument);

                var findAndModifyResult = collection.FindAndModify(queryDocument, sortByDocument, updateDocument, true, true);

                result.Add(ToDocument(findAndModifyResult.ModifiedDocument));
            }
        }

        public override List<IDatabaseObject> Query(IDatabaseObject query) {
            Connect();

            return Query(new SerializerMongoDb().Parse(EscapeStringValues(query)).Compile());
        }

        public override IDatabaseObject EscapeStringValues(IDatabaseObject query) {
            // No escaping is required
            return query;
        }

        protected override List<IDatabaseObject> Query(ICompiledQuery query) {
            var results = new List<IDatabaseObject>();
            var result = new CollectionValue();

            try {
                if (query.Root is Find) {
                    QueryFind(query, result);
                }
                else if (query.Root is Modify) {
                    QueryModify(query, result);
                }
                else if (query.Root is Remove) {
                    QueryRemove(query, result);
                }
                else if (query.Root is Drop) {
                    QueryDrop(query, result);
                }
                // Essentially ignore the create command and process the index children instead.
                //else if (query.Root is Create) {
                //    this.QueryCreate(query, results);
                //}
                else if (query.Root is Merge) {
                    QueryMerge(query, result);
                }

                results.Add(result);
            }
            catch (Exception exception) {
                results.Add(new Error() {
                    new StringValue() {
                        Data = exception.Message
                    }
                });
            }

            foreach (var child in query.Children) {
                results.AddRange(Query(child));    
            }

            return results;
        }

        public override void Close() {
            if (Client != null) {
                Client.GetServer().Disconnect();
                Client = null;
            }
        }
    }
}
