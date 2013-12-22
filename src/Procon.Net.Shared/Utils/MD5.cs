using System;
using System.Linq;
using System.Text;

namespace Procon.Net.Shared.Utils {
    public static class MD5 {

        static readonly System.Security.Cryptography.MD5 Hasher = System.Security.Cryptography.MD5.Create();

        public static String File(String path) {
            return System.IO.File.Exists(path) ? Data(System.IO.File.ReadAllBytes(path)) : string.Empty;
        }

        public static String Data(byte[] data) {
            return MD5.Hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        public static String String(String data) {
            return MD5.Data(Encoding.ASCII.GetBytes(data));
        }

        public static Guid Guid(String data) {
            return new Guid(Hasher.ComputeHash(Encoding.ASCII.GetBytes(data)));
        }
    }
}
