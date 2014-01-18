using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared {
    /// <summary>
    /// A namespace json config for saving serialized commands 
    /// </summary>
    public interface IConfig {
        /// <summary>
        /// The data loaded or saved for this config
        /// </summary>
        JObject Document { get; set; }

        /// <summary>
        /// The working root for a config of this type.
        /// </summary>
        JArray Root { get; set; }

        /// <summary>
        /// Converts data into a JObject and appends it to the root
        /// </summary>
        /// <returns>this</returns>
        IConfig Append<T>(T data);

        /// <summary>
        /// Combines this configuration file with another configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Union(IConfig config);

        /// <summary>
        /// Loads all the files in the specified directory into this configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Load(DirectoryInfo directory);

        /// <summary>
        /// Loads the specified file into this configuration file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Load(FileInfo file);

        /// <summary>
        /// Initializes this configuration file for the specified object type.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Create<T>();

        /// <summary>
        /// Write this configuration file out to disk using the specified path and name.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Save(FileInfo file);
    }
}
