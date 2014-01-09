using System.IO;

namespace Procon.Core.Utils {
    /// <summary>
    /// Procon extensions to System.IO.DirectoryInfo 
    /// </summary>
    public static class DirectoryInfoExtensions {

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
