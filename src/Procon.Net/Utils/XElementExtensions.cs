using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Procon.Net.Utils {
    public static class XElementExtensions {
        public static string ElementValue(this XElement element, string xName) {
            XElement xNameElement = element.Element(xName);

            return xNameElement != null ? xNameElement.Value : String.Empty;
        }

        public static XElement ToXElement<T>(this T obj) {
            using (var memoryStream = new MemoryStream()) {
                using (TextWriter streamWriter = new StreamWriter(memoryStream)) {
                    var xmlSerializer = new XmlSerializer(typeof(T));

                    xmlSerializer.Serialize(streamWriter, obj);

                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        public static T FromXElement<T>(this XElement xElement) {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xElement.ToString()))) {
                var xmlSerializer = new XmlSerializer(typeof(T));

                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
}
