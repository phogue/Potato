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
using System.Linq;
using System.Text;

namespace Potato.Net.Shared.Utils {
    /// <summary>
    /// Helpers for generating MD5 hashes
    /// </summary>
    public static class MD5 {
        private static readonly System.Security.Cryptography.MD5 Hasher = System.Security.Cryptography.MD5.Create();

        /// <summary>
        /// Generates the md5 hash of a file
        /// </summary>
        public static string File(string path) {
            return System.IO.File.Exists(path) ? Data(System.IO.File.ReadAllBytes(path)) : string.Empty;
        }

        /// <summary>
        /// Generates the md5 hash of a byte array
        /// </summary>
        public static string Data(byte[] data) {
            return Hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        /// <summary>
        /// Generates the md5 hash of a string
        /// </summary>
        public static string String(string data) {
            return Data(Encoding.ASCII.GetBytes(data));
        }

        /// <summary>
        /// Generates a GUID based on the md5 data of a string
        /// </summary>
        public static Guid Guid(string data) {
            return new Guid(Hasher.ComputeHash(Encoding.ASCII.GetBytes(data)));
        }
    }
}
