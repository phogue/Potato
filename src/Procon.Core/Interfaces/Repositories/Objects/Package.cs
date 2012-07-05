using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Core.Utils;

    public class Package {

        /// <summary>
        /// This is the unique identifier of the package.
        /// </summary>
        public String Uid { get; set; }

        /// <summary>
        ///  The type of package (Application, Plugin etc)
        /// </summary>
        PackageType PackageType { get; set; }

        /// <summary>
        /// The latest version found for this package
        /// </summary>
        public PackageVersion LatestVersion { get; protected set; }

        /// <summary>
        /// List of all available versions of this package
        /// </summary>
        public List<PackageVersion> Versions { get; set; }

        /// <summary>
        /// The full friendly name of the package
        /// </summary>
        public String Name { get; protected set; }

        public XElement SerializedElement { get; protected set; }

        protected PackageVersion GetLatestVersion() {
            return this.Versions.OrderByDescending(x => x.Version).Select(x => x).First();
        }

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
        public T Copy<T>(T other) where T : Package {
            foreach (PropertyInfo info in typeof(T)
                                              .GetProperties(BindingFlags.Instance
                                                           | BindingFlags.Public)) {
                info.SetValue(this, info.GetValue(other, null), null);
            }
            
            return (T)this;
        }

        public Package Parse(XElement element) {

            this.SerializedElement = element;

            this.Name = element.ElementValue("name");
            this.Uid = element.ElementValue("uid");

            foreach (XElement packageVersionElement in element.Descendants("package_version")) {
                this.Versions.Add(new PackageVersion().Parse(packageVersionElement));
            }

            this.LatestVersion = this.GetLatestVersion();

            return this;
        }
    }
}
