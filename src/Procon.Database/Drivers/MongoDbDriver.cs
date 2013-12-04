using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Drivers {
    public class MongoDbDriver : Driver {

        /// <summary>
        /// The open connection to the database.
        /// </summary>
        protected MongoClient Client { get; set; }

        /// <summary>
        /// The open database object to run queries against
        /// </summary>
        protected MongoDatabase Database { get; set; }

        public override String Name {
            get {
                return "MongoDB";
            }
        }

        public override bool Connect() {
            bool opened = true;
            
            try {
                MongoClientSettings settings = new MongoClientSettings();

                if (this.Settings.Hostname != null && this.Settings.Port.HasValue == true) {
                    settings.Server = new MongoServerAddress(this.Settings.Hostname, (int)this.Settings.Port.Value);
                }
                else if (this.Settings.Hostname != null) {
                    settings.Server = new MongoServerAddress(this.Settings.Hostname);
                }

                if (this.Database != null && this.Settings.Username != null && this.Settings.Password != null) {
                    settings.Credentials = new List<MongoCredential>() {
                        MongoCredential.CreateMongoCRCredential(this.Settings.Database, this.Settings.Username, this.Settings.Password)
                    };
                }
            
                this.Client = new MongoClient(settings);

                this.Database = this.Client.GetServer().GetDatabase(this.Settings.Database);
            }
            catch {
                opened = false;
            }

            return opened;
        }

        /// <summary>
        /// Create table/database query. Creating table ignores all fields except for indexes
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryCreate(ICompiledQuery query, CollectionValue result) {
            if (query.Collections.Any() == true) {
                MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections.FirstOrDefault());

                BsonArray indices = BsonSerializer.Deserialize<BsonArray>(query.Indices.FirstOrDefault());
                
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
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections.FirstOrDefault());

            BsonArray conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());

            foreach (BsonDocument document in collection.Find(new QueryDocument(conditions.First().AsBsonDocument))) {
                DocumentValue row = new DocumentValue();

                foreach (BsonElement value in document.Elements) {
                    var dotNetValue = BsonTypeMapper.MapToDotNetValue(value.Value);

                    if (dotNetValue is ObjectId) {
                        dotNetValue = dotNetValue.ToString();
                    }

                    row.Assignment(value.Name, dotNetValue);
                }

                result.Add(row);
            }
        }

        /// <summary>
        /// Modify all documents that match a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="result"></param>
        protected void QueryModify(ICompiledQuery query, CollectionValue result) {
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections.FirstOrDefault());

            BsonArray conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());
            BsonArray assignments = BsonSerializer.Deserialize<BsonArray>(query.Assignments.FirstOrDefault());

            QueryDocument queryDocument = new QueryDocument(conditions.First().AsBsonDocument);
            UpdateDocument updateDocument = new UpdateDocument(assignments.First().AsBsonDocument);

            WriteConcernResult writeConcernResult = collection.Update(queryDocument, updateDocument, UpdateFlags.Multi);

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Integer = (int)writeConcernResult.DocumentsAffected
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
            MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections.FirstOrDefault());

            BsonArray conditions = BsonSerializer.Deserialize<BsonArray>(query.Conditions.FirstOrDefault());

            WriteConcernResult writeConcernResult = collection.Remove(new QueryDocument(conditions.First().AsBsonDocument));

            result.Add(
                new Affected() {
                    new NumericValue() {
                        Integer = (int)writeConcernResult.DocumentsAffected
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
            if (query.Databases.Any() == true && this.Database.Name == query.Databases.FirstOrDefault()) {
                this.Database.Drop();

                result.Add(
                    new Affected() {
                        new NumericValue() {
                            Integer = 1
                        }
                    }
                );
            }
            else {
                MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections.FirstOrDefault());

                CommandResult commandResult = collection.Drop();

                result.Add(
                    new Affected() {
                        new NumericValue() {
                            Integer = commandResult.Ok == true ? 1 : 0
                        }
                    }
                );
            }
        }

        public override IDatabaseObject Query(IDatabaseObject query) {
            return this.Query(new SerializerMongoDb().Parse(query).Compile());
        }

        protected override IDatabaseObject Query(ICompiledQuery query) {
            CollectionValue results = new CollectionValue();

            if (query.Root is Find) {
                this.QueryFind(query, results);
            }
            else if (query.Root is Modify) {
                this.QueryModify(query, results);
            }
            else if (query.Root is Remove) {
                this.QueryRemove(query, results);
            }
            else if (query.Root is Drop) {
                this.QueryDrop(query, results);
            }
            else if (query.Root is Create) {
                this.QueryCreate(query, results);
            }

            return results;
        }

        public override void Close() {
            if (this.Client != null) {
                this.Client.GetServer().Disconnect();
                this.Client = null;
            }
        }
    }
}
