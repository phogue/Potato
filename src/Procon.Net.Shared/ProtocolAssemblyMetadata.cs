using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Procon.Net.Shared.Serialization;

namespace Procon.Net.Shared {
    /// <summary>
    /// Wraps an assembly reference and supported protocol types.
    /// </summary>
    public class ProtocolAssemblyMetadata : IProtocolAssemblyMetadata {
        public String Name { get; set; }

        public FileInfo Assembly { get; set; }

        public FileInfo Meta { get; set; }

        public DirectoryInfo Directory { get; set; }

        public List<IProtocolType> ProtocolTypes { get; set; }

        /// <summary>
        /// Initalizes meta data with default values.
        /// </summary>
        public ProtocolAssemblyMetadata() {
            this.ProtocolTypes = new List<IProtocolType>();
        }

        /// <summary>
        /// Loads directory into a new assembly meta data object
        /// </summary>
        /// <param name="path">The directory path to load the meta data from.</param>
        /// <returns>True if the meta data loaded, false if an error occured or metadata/assembly is missing</returns>
        public bool Load(String path) {
            bool loaded = true;

            this.Directory = new DirectoryInfo(path);
            this.Name = this.Directory.Name;

            this.Assembly = new FileInfo(Path.Combine(this.Directory.FullName, this.Name + ".dll"));
            this.Meta = new FileInfo(Path.Combine(this.Directory.FullName, this.Name + ".json"));

            if (this.Assembly.Exists == true && this.Meta.Exists == true) {
                try {
                    using (var stream = new StreamReader(this.Meta.FullName)) {
                        using (JsonReader reader = new JsonTextReader(stream)) {
                            JsonSerialization.Minimal.Populate(reader, this);
                        }
                    }
                }
                catch {
                    loaded = false;
                }
            }
            else {
                loaded = false;
            }

            return loaded;
        }
    }
}
