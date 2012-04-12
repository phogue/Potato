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

namespace Procon.Core.Interfaces.Layer.Objects {
    using System.Security.Cryptography;

    /// <summary>
    /// This entire class is subject to change due to optimization
    /// and checks to see what libraries are available on various devices.  The goal
    /// is to maximize the security and coverage as easily as possible.
    /// </summary>
    public class Encryption {

        public static byte[] Encrypt(byte[] plain, string password, string salt) {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = RijndaelManaged.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plain, 0, plain.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] cipher, string password, string salt) {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = RijndaelManaged.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipher, 0, cipher.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }
    }
}
