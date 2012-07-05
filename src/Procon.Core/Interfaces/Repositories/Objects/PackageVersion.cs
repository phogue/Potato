using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Core.Utils;

    public class PackageVersion {
        public Version Version { get; protected set; }

        public List<PackageFile> Files { get; protected set; }

        public PackageVersion() {
            this.Files = new List<PackageFile>();
        }

        protected void ParseVersion(XElement element) {
            if (element != null) {
                this.Version = new Version(
                    int.Parse(element.ElementValue("major")),
                    int.Parse(element.ElementValue("minor")),
                    int.Parse(element.ElementValue("build")),
                    int.Parse(element.ElementValue("revision"))
                );
            }
        }

        public PackageVersion Parse(XElement element) {
            this.ParseVersion(element.Descendants("version").FirstOrDefault());

            this.Files.Clear();
            foreach (XElement fileElement in element.Descendants("file")) {
                this.Files.Add(new PackageFile().Parse(fileElement));
            }

            return this;
        }
    }
}
