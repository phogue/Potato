using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Repositories.Objects {
    using Procon.Core.Utils;

    public class PackageFile {

        public String Name { get; protected set; }

        public int Size { get; protected set; }

        public DateTime LastModified { get; protected set; }

        public String RelativePath { get; protected set; }

        public String MD5 { get; protected set; }

        public PackageFile Parse(XElement element) {

            this.Name = element.ElementValue("name");
            this.RelativePath = element.ElementValue("relative_path");
            this.MD5 = element.ElementValue("md5");
            this.LastModified = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(int.Parse(element.ElementValue("date")));
            this.Size = int.Parse(element.ElementValue("size"));

            return this;
        }

    }
}
