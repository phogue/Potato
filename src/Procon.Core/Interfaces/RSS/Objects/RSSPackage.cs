// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Procon.Core.Interfaces.RSS.Objects {
    using Procon.Core.Utils;

    public class RSSPackage : RSS<RSSPackage>, IPackage {

        #region Properties

        public string Uid { get; set; }
        public PackageType PackageType { get; set; }
        public Version Version { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public string ForumLink { get; set; }
        public string Author { get; set; }
        public string Website { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }

        public List<string> Files { get; set; }

        // From RSS Only
        public int Downloads { get; set; }
        public string Md5 { get; set; }
        public DateTime LastModified { get; set; }
        public int FileSize { get; set; }

        #endregion

        public RSSPackage() {
            this.Tags = new List<string>();
            this.Files = new List<string>();
        }

        public RSSPackage Parse(XElement element) {

            try {

                this.Uid = element.ElementValue("uid"); ;

                this.Version = new Version(element.ElementValue("version"));

                switch (element.ElementValue("type").ToLower()) {
                    case "plugin":
                        this.PackageType = PackageType.Plugin;
                        break;
                    case "application":
                        this.PackageType = PackageType.Application;
                        break;
                    case "language":
                        this.PackageType = PackageType.Language;
                        break;
                    case "mappack":
                        this.PackageType = PackageType.Mappack;
                        break;
                    case "config":
                        this.PackageType = PackageType.Config;
                        break;
                    default:
                        this.PackageType = PackageType.Plugin;
                        break;
                }

                this.Name = element.ElementValue("name");
                this.Image = element.ElementValue("image");
                this.ForumLink = element.ElementValue("forumlink");
                this.Author = element.ElementValue("author");
                this.Website = element.ElementValue("website");
                this.Description = element.ElementValue("description");

                foreach (XElement tag in element.Descendants("tag")) {
                    this.Tags.Add(tag.Value);
                }

                foreach (XElement file in element.Descendants("file")) {
                    if (file.Attribute("path") != null) {
                        this.Files.Add(file.Attribute("path").Value);
                    }
                }
                
                // Below may or may not appear depending on the source of the document.
                string downloadstring = element.ElementValue("downloads");
                int downloads = 0;
                if (int.TryParse(downloadstring, out downloads) == true) {
                    this.Downloads = downloads;
                }

                this.Md5 = element.ElementValue("md5");

                string lastmodifiedstring = element.ElementValue("lastmodified");
                long lastmodified = 0;
                if (long.TryParse(lastmodifiedstring, out lastmodified) == true) {
                    this.LastModified = (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(lastmodified);
                }

                string filesizestring = element.ElementValue("filesize");
                int filesize = 0;
                if (int.TryParse(filesizestring, out filesize) == true) {
                    this.FileSize = filesize;
                }
            }
            catch (Exception) {
            }
            
            return this;
        }
    }
}
