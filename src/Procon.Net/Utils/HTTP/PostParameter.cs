using System;
using System.IO;
using Procon.Net.Shared.Utils;

namespace Procon.Net.Utils.HTTP {
    public class PostParameter {

        /// <summary>
        /// The name of the parameter
        /// </summary>
        private String Name;

        /// <summary>
        /// The string value to assign to the param
        /// </summary>
        private String Value;

        /// <summary>
        /// The filename of the file to send
        /// </summary>
        private String FileName;

        /// <summary>
        /// The mimetype of the file to send
        /// </summary>
        private String MimeType;

        /// <summary>
        /// (optional) Stream to the file to send
        /// </summary>
        private Stream Stream;

        /// <summary>
        /// (optional) The byte value of the file to send
        /// </summary>
        private byte[] ByteValue;

        /// <summary>
        /// The type of parameter
        /// </summary>
        public PostParameterType Type { get; protected set; }

        public PostParameter(string name, string value, PostParameterType type) {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public PostParameter(string name, string fileName, string value, PostParameterType type) {
            this.Name = name;
            this.Value = value;
            this.FileName = fileName;
            this.Type = type;
        }

        public PostParameter(string name, string fileName, Stream stream, String mimeType, PostParameterType type) {
            this.Name = name;
            this.Stream = stream;
            this.FileName = fileName;
            this.MimeType = mimeType;
            this.Type = type;
        }

        public PostParameter(string name, string fileName, byte[] byteValue, String mimeType, PostParameterType type) {
            this.Name = name;
            this.ByteValue = byteValue;
            this.FileName = fileName;
            this.MimeType = mimeType;
            this.Type = type;
        }

        public byte[] BuildHeader() {

            MemoryStream stream = new MemoryStream();

            if (this.Type == PostParameterType.File) {
                if (this.Value != null) {
                    stream.WriteLine("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\";", this.Name, this.FileName);
                    stream.WriteLine("Content-Type: text/plain");
                    stream.WriteLine();
                    stream.WriteLine(this.Value);
                }
                else if (this.Stream != null) {
                    stream.WriteLine("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\";", this.Name, this.FileName);
                    stream.WriteLine("Content-Type: {0}", this.MimeType);
                    stream.WriteLine();
                    this.Stream.CopyTo(stream);
                    stream.WriteLine();
                }
                else if (this.ByteValue != null) {
                    stream.WriteLine("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\";", this.Name, this.FileName);
                    stream.WriteLine("Content-Type: {0}", this.MimeType);
                    stream.WriteLine();
                    stream.Write(this.ByteValue);
                    stream.WriteLine();
                }
            }
            else if (this.Type == PostParameterType.Field) {
                stream.WriteLine("Content-Disposition: form-data; name=\"{0}\"", this.Name);
                stream.WriteLine();
                stream.WriteLine(this.Value);
            }

            return stream.ToArray();
        }
    }
}
