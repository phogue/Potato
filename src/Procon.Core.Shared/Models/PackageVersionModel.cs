using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Procon.Core.Shared.Models.Serialization;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

namespace Procon.Core.Shared.Models {
    [Serializable, XmlRoot(ElementName = "package_version")]
    public class PackageVersionModel : CoreModel {

        /// <summary>
        /// The version of this package
        /// </summary>
        [XmlElement(ElementName = "version")]
        public SerializableVersionModel Version { get; set; }

        /// <summary>
        /// The list of files attached to this version of the package
        /// </summary>
        [XmlArray(ElementName = "files")]
        [XmlArrayItem(ElementName = "file")]
        public List<PackageFileModel> Files { get; set; }

        public PackageVersionModel() {
            this.Files = new List<PackageFileModel>();
        }

        public IEnumerable<PackageFileModel> ModifiedFilesAt(String path) {
            return this.Files.Where(file => String.Compare(file.Md5, MD5.File(Path.Combine(path, file.RelativePath)), StringComparison.OrdinalIgnoreCase) != 0);
        }
    }
}
