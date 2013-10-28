namespace Procon.Net.Utils {
    using System.Linq;
    using System.Text;

    public static class SHA512 {

        private static readonly System.Security.Cryptography.SHA512 Hasher = System.Security.Cryptography.SHA512Managed.Create();

        public static string Data(byte[] data) {
            return SHA512.Hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        public static string String(string data) {
            return SHA512.Data(Encoding.ASCII.GetBytes(data));
        }
    }
}
