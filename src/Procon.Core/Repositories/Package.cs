using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Repositories {

    [Serializable, XmlRoot(ElementName = "package")]
    public class Package {
        /// <summary>
        /// This is the unique identifier of the package.
        /// </summary>
        [XmlElement(ElementName = "uid")]
        public String Uid { get; set; }

        /// <summary>
        /// The full friendly name of the package
        /// </summary>
        [XmlElement(ElementName = "name")]
        public String Name { get; set; }

        /// <summary>
        ///  The type of package (Application, Plugin etc)
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public PackageType PackageType { get; set; }

        /// <summary>
        /// The latest version found for this package
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public PackageVersion LatestVersion {
            get { return this.Versions.OrderByDescending(version => version.Version.SystemVersion).FirstOrDefault(); }
        }

        /// <summary>
        /// List of all available versions of this package
        /// </summary>
        [XmlArray(ElementName = "package_versions")]
        [XmlArrayItem(ElementName = "package_version")]
        public List<PackageVersion> Versions { get; set; }
        
        public Package() {
            this.Versions = new List<PackageVersion>();
        }

        /// <summary>
        /// Copies the contents of another package object to this object.
        /// 
        /// This will maintain references to this object but allow it to be updated
        /// easily with outside data.
        /// </summary>
        /// <param name="other">The package to clone into this package</param>
        /// <returns>itself</returns>
        public virtual Package Copy(Package other) {
            this.Name = other.Name;
            this.Uid = other.Uid;
            this.Versions = other.Versions;
            this.PackageType = other.PackageType;
            
            return this;
        }
    }
}
