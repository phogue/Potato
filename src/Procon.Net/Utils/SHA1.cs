namespace Procon.Net.Utils {
    using System.Linq;
    using System.Text;

    public static class SHA1 {

        private static readonly System.Security.Cryptography.SHA1 Hasher = System.Security.Cryptography.SHA1Managed.Create();

        public static string Data(byte[] data) {
            return SHA1.Hasher.ComputeHash(data).Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
        }

        public static string String(string data) {
            return SHA1.Data(Encoding.ASCII.GetBytes(data));
        }
    }
}
