using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared {
    /// <summary>
    /// Config for saving JSON data
    /// </summary>
    public class JsonConfig : IConfig {
        /// <summary>
        /// The serializer to use for saving/loading data to/from files.
        /// </summary>
        public JsonSerializer Serializer { get; set; }

        public JObject Document { get; set; }

        public JArray Root { get; set; }

        /// <summary>
        /// Initializes the serializer
        /// </summary>
        public JsonConfig() {
            this.Serializer = new JsonSerializer() {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            this.Document = new JObject();
        }

        public IConfig Append<T>(T data) {
            if (Equals(data, default(T))) throw new ArgumentNullException("data");

            this.Root.Add(JObject.FromObject(data, this.Serializer));

            return this;
        }

        public JArray RootOf<T>() {
            return this.RootOf(typeof(T));
        }

        public JArray RootOf(Type type) {
            if (type == null) throw new ArgumentNullException("type");

            return this.RootOf(String.Format("{0}.{1}", type.Namespace, type.Name));
        }

        public JArray RootOf(string @namespace) {
            if (@namespace == null) throw new ArgumentNullException("namespace");

            return this.Document[@namespace] != null ? this.Document[@namespace].Value<JArray>() : new JArray();
        }

        public IConfig Union(IConfig config) {
            if (config == null) throw new ArgumentNullException("config");

            foreach (var property in config.Document.Children<JProperty>()) {
                if (this.Document[property.Name] == null) {
                    this.Document.Add(property);
                }
                else {
                    this.Document[property.Name].Value<JArray>().Add(property.Values());
                }
            }

            return this;
        }

        public IConfig Load(DirectoryInfo directory) {
            if (directory == null) throw new ArgumentNullException("directory");

            directory.Refresh();

            if (directory.Exists == true) {
                foreach (FileInfo file in directory.GetFiles("*.json").OrderBy(file => file.Name)) {
                    if (this.Root == null) {
                        this.Load(file);
                    }
                    else {
                        this.Union(new JsonConfig().Load(file));
                    }
                }
            }

            return this;
        }

        public IConfig Load(FileInfo file) {
            if (file == null) throw new ArgumentNullException("file");

            file.Refresh();

            if (file.Exists == true) {
                using (var stream = new StreamReader(file.FullName)) {
                    using (JsonReader reader = new JsonTextReader(stream)) {
                        try {
                            this.Document = JObject.Load(reader);

                            String name = file.Name.Replace(".json", "");

                            this.Root = this.Document[name] != null ? this.Document.Value<JArray>(name) : this.Document.First.First.Value<JArray>();
                        }
                        catch {
                            this.Root = new JArray();
                            this.Document = new JObject() {
                            new JProperty("Empty", this.Root)
                        };
                        }
                    }
                }
            }

            return this;
        }

        public IConfig Create<T>() {
            return this.Create(typeof(T));
        }

        public IConfig Create(Type type) {
            // Clear out all old data
            this.Root = new JArray();
            this.Document = new JObject() {
                new JProperty(String.Format("{0}.{1}", type.Namespace, type.Name), this.Root)
            };

            return this;
        }

        public IConfig Save(FileInfo file) {
            if (file == null) throw new ArgumentNullException("file");

            using (TextWriter writer = new StreamWriter(file.FullName, false)) {
                this.Serializer.Serialize(writer, this.Document);
            }

            return this;
        }
    }
}
