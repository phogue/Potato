﻿#region Copyright
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

namespace Procon.Database {
    /// <summary>
    /// The connection details for a driver to use when connecting/authenticating
    /// </summary>
    public class DriverSettings : IDriverSettings {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        public String Hostname { get; set; }

        /// <summary>
        /// The port to connect over.
        /// </summary>
        public uint? Port { get; set; }

        /// <summary>
        /// The username for authentication.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The password for authentication.
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// The name of the database to select.
        /// </summary>
        public String Database { get; set; }

        /// <summary>
        /// If the database should exists in memory only, not on a file system.
        /// </summary>
        public bool Memory { get; set; }
    }
}
