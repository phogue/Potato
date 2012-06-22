// Copyright 2011 Geoffrey 'Phogue' Green
// Modified by Cameron 'Imisnew2' Gunnin
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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core
{
    [Serializable]
    public class Config
    {
        // Public Properties
        public XDocument Document { get; protected set; }
        public XElement  Root     { get; protected set; }

        
        
        /// <summary>
        /// Combines this configuration file with another configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Add(Config config) {
            if (Document != null && config.Document != null) {
                XElement tThisRoot = Document.Root;
                XElement tThatRoot = config.Document.Root;
                while (tThisRoot != null && tThatRoot != null) {
                    if (tThisRoot.Name != tThatRoot.Name) {
                        if (tThisRoot.Parent != null)
                            tThisRoot.Parent.Add(tThatRoot);
                        break;
                    }
                    tThisRoot = tThisRoot.Descendants().FirstOrDefault();
                    tThatRoot = tThatRoot.Descendants().FirstOrDefault();
                }
            }
            return this;
        }

        /// <summary>
        /// Loads all the files in the specified directory into this configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config LoadDirectory(DirectoryInfo mDirectory) {
            if (mDirectory.Exists)
                foreach (FileInfo xmlFile in mDirectory.GetFiles("*.xml"))
                    if (Root == null)
                        LoadFile(xmlFile);
                    else
                        Add(new Config().LoadFile(xmlFile));

            return this;
        }

        /// <summary>
        /// Loads the specified file into this configuration file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config LoadFile(FileInfo mFile) {
            if (mFile.Exists) {
                using (StreamReader sr = new StreamReader(mFile.FullName)) {
                    Document = XDocument.Load(sr);
                    Root     = Document.Element("procon");
                }
            }
            return this;
        }

        /// <summary>
        /// Initializes this configuration file for the specified object type.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Generate(Type mType) {
            Document = new XDocument();

            foreach (String mName in mType.FullName.ToLower().Split('`').First().Split('.')) {
                if (Root == null) {
                    Document.Add(
                        new XComment("This file is overwritten by Procon."),
                        new XElement(mName)
                    );
                    Root = Document.Element(mName);
                }
                else {
                    Root.Add(new XElement(mName));
                    Root = Root.Element(mName);
                }
            }
            
            return this;
        }

        /// <summary>
        /// Write this configuration file out to disk using the specified path and name.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Save(FileInfo mFile) {
            if (!File.Exists(mFile.FullName)) {
                if (!Directory.Exists(mFile.Directory.FullName))
                    Directory.CreateDirectory(mFile.Directory.FullName);
                File.Create(mFile.FullName).Close();
            }
            Document.Save(mFile.FullName);

            return this;
        }
    }
}
