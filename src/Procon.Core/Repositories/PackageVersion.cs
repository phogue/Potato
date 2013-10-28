using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Procon.Core.Repositories.Serialization;

namespace Procon.Core.Repositories {
    using Procon.Net.Utils;

    [Serializable, XmlRoot(ElementName = "package_version")]
    public class PackageVersion {

        /// <summary>
        /// The version of this package
        /// </summary>
        [XmlElement(ElementName = "version")]
        public SerializableVersion Version { get; set; }

        /// <summary>
        /// The list of files attached to this version of the package
        /// </summary>
        [XmlArray(ElementName = "files")]
        [XmlArrayItem(ElementName = "file")]
        public List<PackageFile> Files { get; set; }

        public PackageVersion() {
            this.Files = new List<PackageFile>();
        }

        public IEnumerable<PackageFile> ModifiedFilesAt(String path) {
            return this.Files.Where(file => String.Compare(file.Md5, MD5.File(Path.Combine(path, file.RelativePath)), StringComparison.OrdinalIgnoreCase) != 0);
        }
    }
}
