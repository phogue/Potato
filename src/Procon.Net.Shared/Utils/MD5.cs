using System;
using System.Linq;
using System.Text;

namespace Procon.Net.Shared.Utils {
    /// <summary>
    /// Helpers for generating MD5 hashes
    /// </summary>
    public static class MD5 {
        private static readonly System.Security.Cryptography.MD5 Hasher = System.Security.Cryptography.MD5.Create();

        /// <summary>
        /// Generates the md5 hash of a file
        /// </summary>
        public static String File(String path) {
            return System.IO.File.Exists(path) ? Data(System.IO.File.ReadAllBytes(path)) : string.Empty;
        }

        /// <summary>
        /// Generates the md5 hash of a byte array
        /// </summary>
        public static String Data(byte[] data) {
            return MD5.Hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        /// <summary>
        /// Generates the md5 hash of a string
        /// </summary>
        public static String String(String data) {
            return MD5.Data(Encoding.ASCII.GetBytes(data));
        }

        /// <summary>
        /// Generates a GUID based on the md5 data of a string
        /// </summary>
        public static Guid Guid(String data) {
            return new Guid(Hasher.ComputeHash(Encoding.ASCII.GetBytes(data)));
        }
    }
}
