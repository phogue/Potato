using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Repositories.Serialization {

    [Serializable]
    public class SerializableVersion {
        private int _major;
        private int _minor;
        private int _build;
        private int _revision;

        [XmlElement(ElementName = "major")]
        public int Major {
            get { return this._major; }
            set {
                this._major = value;

                this.SystemVersion = new Version(this.Major, this.Minor, this.Build, this.Revision);
            }
        }

        [XmlElement(ElementName = "minor")]
        public int Minor {
            get { return this._minor; }
            set {
                this._minor = value;

                this.SystemVersion = new Version(this.Major, this.Minor, this.Build, this.Revision);
            }
        }

        [XmlElement(ElementName = "build")]
        public int Build {
            get { return this._build; }
            set {
                this._build = value;

                this.SystemVersion = new Version(this.Major, this.Minor, this.Build, this.Revision);
            }
        }

        [XmlElement(ElementName = "revision")]
        public int Revision {
            get { return this._revision; }
            set {
                this._revision = value;

                this.SystemVersion = new Version(this.Major, this.Minor, this.Build, this.Revision);
            }
        }

        /// <summary>
        /// The underlying SystemVersion object. Use this for comparison. It seems like
        /// double work implementing everything this sealed class does.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Version SystemVersion { get; protected set; }

        public SerializableVersion() : base() {
            this.SystemVersion = new Version();
        }

        public override string ToString() {
            return this.SystemVersion.ToString();
        }
    }
}
