using System;
using System.Text;
using System.IO;

namespace Procon.Net.Utils {
    public static class StreamExtensions {

        public static void Write(this Stream s, byte[] b) {
            s.Write(b, 0, b.Length);
        }

        public static void Write(this Stream s, string format = "", params object[] args) {
            byte[] b = Encoding.UTF8.GetBytes(String.Format(format, args));

            s.Write(b, 0, b.Length);
        }

        public static void WriteLine(this Stream s, string format = "", params object[] args) {
            byte[] b = Encoding.UTF8.GetBytes(String.Format(format + "\r\n", args));

            s.Write(b, 0, b.Length);
        }

        public static void CopyTo(this Stream input, Stream output) {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
