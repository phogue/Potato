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

namespace Procon.Net.Utils {
    
    using System.Linq;
    using System.Text;

    public static class SHA1 {

        public static string Data(byte[] data) {
            System.Security.Cryptography.SHA1 hasher = System.Security.Cryptography.SHA1.Create();

            return hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        public static string String(string data) {
            return SHA1.Data(Encoding.ASCII.GetBytes(data));
        }
    }
}
