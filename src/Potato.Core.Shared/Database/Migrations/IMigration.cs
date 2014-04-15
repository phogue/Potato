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
    /// What actions should be taken in a migration up/down stream.
    /// </summary>
    public interface IMigration {
        /// <summary>
        /// A handler to execute when migrating upwards.
        /// </summary>
        /// <returns>True if the migration was successful, false if an error occured</returns>
        Func<bool> Up { get; set; }

        /// <summary>
        /// A handler to execute when migrating downwards.
        /// </summary>
        /// <returns>True if the migration was successful, false if an error occured</returns>
        Func<bool> Down { get; set; }
    }
}
