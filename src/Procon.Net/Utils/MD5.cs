// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Procon.Net.Utils {
    public static class MD5 {

        static System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

        public static string File(string path) {
            if (System.IO.File.Exists(path)) {
                return Data(System.IO.File.ReadAllBytes(path));
            } 
            return string.Empty;
        }

        public static string Data(byte[] data) {
            StringBuilder stringifyHash = new StringBuilder();

            byte[] hash = md5Hasher.ComputeHash(data);
            for (int x = 0; x < hash.Length; x++) {
                stringifyHash.Append(hash[x].ToString("x2"));
            }

            return stringifyHash.ToString();
        }

        public static string String(string data) {
            return SHA1.Data(Encoding.ASCII.GetBytes(data));
        }
    }
}
