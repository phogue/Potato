using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core {
    [Serializable]
    public class Config : IDisposable {

        [NonSerialized]
        private XDocument _document;

        [NonSerialized]
        private XElement _root;

        /// <summary>
        /// The entire document in memory to be flushed to a file.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public XDocument Document {
            get {
                return this._document;
            }
            protected set {
                this._document = value;
            }
        }

        /// <summary>
        /// The root element of the document
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public XElement Root {
            get {
                return this._root;
            }
            protected set {
                this._root = value;
            }
        }

        public Config() {
            this.Document = new XDocument();
        }

        /// <summary>
        /// Combines this configuration file with another configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Add(Config config) {
            if (this.Document != null && config.Document != null) {
                XElement tThisRoot = this.Document.Root;
                XElement tThatRoot = config.Document.Root;

                while (tThisRoot != null && tThatRoot != null) {
                    if (tThisRoot.Name != tThatRoot.Name) {
                        if (tThisRoot.Parent != null) {
                            tThisRoot.Parent.Add(tThatRoot);
                        }
                            
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
            if (mDirectory != null && mDirectory.Exists == true) {
                foreach (FileInfo xmlFile in mDirectory.GetFiles("*.xml")) {
                    if (this.Root == null) {
                        this.LoadFile(xmlFile);
                    }
                    else {
                        this.Add(new Config().LoadFile(xmlFile));
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Loads the specified file into this configuration file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config LoadFile(FileInfo mFile) {
            if (mFile.Exists == true) {
                using (StreamReader sr = new StreamReader(mFile.FullName)) {
                    this.Document = XDocument.Load(sr);

                    this.Root = this.Document.Element("Procon");
                }
            }
            return this;
        }

        /// <summary>
        /// Initializes this configuration file for the specified object type.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Generate(Type mType) {
            this.Document = new XDocument();

            foreach (String mName in mType.FullName.Split('`').First().Split('.')) {
                if (this.Root == null) {
                    this.Document.Add(
                        new XComment("This file is overwritten by Procon."),
                        new XElement(mName)
                    );

                    this.Root = this.Document.Element(mName);
                }
                else {
                    this.Root.Add(new XElement(mName));

                    this.Root = this.Root.Element(mName);
                }
            }

            return this;
        }

        /// <summary>
        /// Write this configuration file out to disk using the specified path and name.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual Config Save(FileInfo mFile) {
            if (File.Exists(mFile.FullName) == false && mFile.Directory != null) {
                Directory.CreateDirectory(mFile.Directory.FullName);

                File.Create(mFile.FullName).Close();
            }

            this.Document.Save(mFile.FullName);

            return this;
        }

        public void Dispose() {
            this.Document = null;
            this.Root = null;
        }
    }
}
