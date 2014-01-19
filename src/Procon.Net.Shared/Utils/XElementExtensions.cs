using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Procon.Net.Shared.Utils {
    public static class XElementExtensions {
        public static T FromXElement<T>(this XElement xElement) {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xElement.ToString()))) {
                var xmlSerializer = new XmlSerializer(typeof(T));

                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
}
