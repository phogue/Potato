using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Methods;
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

        public override IDatabaseObject Query(IDatabaseObject query) {
            return this.Query(new SerializerMongoDb().Parse(query).Compile());
        }

        protected override IDatabaseObject Query(ICompiledQuery query) {
            CollectionValue result = new CollectionValue();

            if (query.Root is Find) {
                MongoCollection<BsonDocument> collection = this.Database.GetCollection(query.Collections);

                foreach (BsonDocument document in collection.Find(new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query.Conditions)))) {
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
            else {
                //this.Execute(query, result);
            }

            return result;
        }

        public override void Close() {
            if (this.Client != null) {
                this.Client.GetServer().Disconnect();
                this.Client = null;
            }
        }
    }
}
