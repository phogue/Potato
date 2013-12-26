using System.IO;
using Ionic.Zip;

namespace Procon.Core.Utils {
    /// <summary>
    /// Procon extensions to System.IO.DirectoryInfo 
    /// </summary>
    public static class DirectoryInfoExtensions {
        /// <summary>
        /// Zips the contents of the directory.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns>a memory stream to output elsewhere</returns>
        public static MemoryStream Zip(this DirectoryInfo directoryInfo) {
            MemoryStream stream = new MemoryStream();

            using (ZipFile zip = new ZipFile()) {
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                zip.AddDirectory(directoryInfo.FullName);

                zip.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }

        /// <summary>
        /// Removes all files within a directory.
        /// </summary>
        /// <param name="directory"></param>
        public static void Clean(this DirectoryInfo directory) {
            foreach (FileInfo file in directory.GetFiles()) {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) {
                subDirectory.Delete(true);
            }
        }
    }
}
