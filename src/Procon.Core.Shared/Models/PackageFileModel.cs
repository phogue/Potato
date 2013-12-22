using System;
using System.Xml.Serialization;

namespace Procon.Core.Shared.Models {

    [Serializable, XmlRoot(ElementName = "file")]
    public class PackageFileModel {

        /// <summary>
        /// The basic name of the file (the relative path stripped of the path bit)
        /// </summary>
        [XmlElement(ElementName = "name")]
        public String Name { get; set; }

        /// <summary>
        /// The size of this file in bytes.
        /// </summary>
        [XmlElement(ElementName = "size")]
        public int Size { get; set; }

        /// <summary>
        /// When the file was last modified
        /// </summary>
        [XmlElement(ElementName = "last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The relative path of the file, relative to the base of the path.
        /// </summary>
        [XmlElement(ElementName = "relative_path")]
        public String RelativePath { get; set; }

        /// <summary>
        /// The md5 hash of the file.
        /// </summary>
        [XmlElement(ElementName = "md5")]
        public String Md5 { get; set; }
    }
}
