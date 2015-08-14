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

namespace Potato.Core.Shared.Database.Migrations {
    /// <summary>
    /// Holds settings to describe how migrations should be handled and what stream to look into.
    /// </summary>
    public interface IMigrationSettings {
        /// <summary>
        /// The name of the stream to focus on.
        /// </summary>
        MigrationOrigin Origin { get; set; }

        /// <summary>
        /// The name of the stream to focus on.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The current version of the migration for the specified name.
        /// </summary>
        int CurrentVersion { get; set; }
    }
}
