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
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Potato.Core.Shared {
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
        /// Finds the root of a specific type.
        /// </summary>
        /// <returns>this</returns>
        JArray RootOf<T>();

        /// <summary>
        /// Finds the root of a specific type.
        /// </summary>
        /// <returns>this</returns>
        JArray RootOf(Type type);

        /// <summary>
        /// Finds the root of a specific type.
        /// </summary>
        /// <returns>this</returns>
        JArray RootOf(String @namespace);

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
        /// Initializes this configuration file for the specified object type.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Create(Type type);

        /// <summary>
        /// Write this configuration file out to disk using the specified path and name.
        /// Returns a reference back to this config.
        /// </summary>
        /// <returns>this</returns>
        IConfig Save(FileInfo file);
    }
}
