using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Procon.Core.Utils {
    public static class XElementExtensions {

        /*
         * Not used now.
        public static string ToXPath(this XElement element) {
            List<string> xpath = new List<string>() {
                element.Name.ToString()
            };

            XElement parent = element.Parent;

            while (parent != null) {
                xpath.Add(parent.Name.LocalName);

                parent = parent.Parent;
            }

            xpath.Reverse();
            return "/" + String.Join("/", xpath.ToArray());
        }
        */

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
