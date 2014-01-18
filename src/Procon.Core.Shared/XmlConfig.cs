using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Shared {
    /// <summary>
    /// A namespace xml config for saving serialized commands 
    /// </summary>
    [Serializable]
    public class XmlConfig : IDisposable {

        [NonSerialized]
        private XDocument _document = new XDocument();

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

        /// <summary>
        /// Combines this configuration file with another configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual XmlConfig Combine(XmlConfig config) {
            if (this.Document != null && config.Document != null) {
                XElement thisRoot = this.Document.Root;
                XElement thatRoot = config.Document.Root;

                while (thisRoot != null && thatRoot != null) {
                    if (thisRoot.Name != thatRoot.Name) {
                        if (thisRoot.Parent != null) {
                            thisRoot.Parent.Add(thatRoot);
                        }
                            
                        break;
                    }

                    thisRoot = thisRoot.Descendants().FirstOrDefault();
                    thatRoot = thatRoot.Descendants().FirstOrDefault();
                }
            }

            return this;
        }

        /// <summary>
        /// Loads all the files in the specified directory into this configuration file.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual XmlConfig Load(DirectoryInfo directory) {
            if (directory != null && directory.Exists == true) {
                foreach (FileInfo file in directory.GetFiles("*.xml")) {
                    if (this.Root == null) {
                        this.Load(file);
                    }
                    else {
                        this.Combine(new XmlConfig().Load(file));
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Loads the specified file into this configuration file using the file's contents.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual XmlConfig Load(FileInfo file) {
            if (file.Exists == true) {
                using (StreamReader reader = new StreamReader(file.FullName)) {
                    this.Document = XDocument.Load(reader);

                    this.Root = this.Document.Element("Procon");
                }
            }
            return this;
        }

        /// <summary>
        /// Initializes this configuration file for the specified object type.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual XmlConfig Create(Type type) {
            this.Document = new XDocument();

            foreach (String name in type.FullName.Split('`').First().Split('.')) {
                if (this.Root == null) {
                    this.Document.Add(
                        new XComment("This file is overwritten by Procon."),
                        new XElement(name)
                    );

                    this.Root = this.Document.Element(name);
                }
                else {
                    this.Root.Add(new XElement(name));

                    this.Root = this.Root.Element(name);
                }
            }

            return this;
        }

        /// <summary>
        /// Write this configuration file out to disk using the specified path and name.
        /// Returns a reference back to this config.
        /// </summary>
        public virtual XmlConfig Save(FileInfo file) {
            if (File.Exists(file.FullName) == false && file.Directory != null) {
                Directory.CreateDirectory(file.Directory.FullName);

                File.Create(file.FullName).Close();
            }

            this.Document.Save(file.FullName);

            return this;
        }

        public void Dispose() {
            this.Document = null;
            this.Root = null;
        }
    }
}
