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

namespace Potato.Core.Shared.Database.Migrations {
    /// <summary>
    /// An entry into the migration table
    /// </summary>
    public class MigrationModel : DatabaseModel<MigrationModel> {
        /// <summary>
        /// The origin of the migration (Core, Plugin)
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// The name of the stream.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the stream on this entry.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// When this entry was added to the migration table.
        /// </summary>
        public DateTime Stamp { get; set; }
    }
}
