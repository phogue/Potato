using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace Procon.Net.Shared.Utils {
    public static class XElementExtensions {
        public static string ElementValue(this XElement element, string xName) {
            XElement xNameElement = element.Element(xName);

            return xNameElement != null ? xNameElement.Value : String.Empty;
        }

        /// <summary>
        /// Queries all _Reference_ xml tags with associated Select attributes
        /// </summary>
        /// <param name="root">The element to lookup and replace all instances of Reference</param>
        /// <returns>The element passed in</returns>
        public static XElement QuerySelectReferences(this XElement root) {
            // Generate all reference replacements.
            Dictionary<XElement, XElement> replacements = root.DescendantsAndSelf("Reference").ToDictionary(element => element, element => {
                XElement value = null;
                var select = element.Attribute("select");

                if (select != null) {
                    try {
                        value = root.XPathSelectElement(select.Value);
                    }
                    catch {
                        value = null;
                    }
                }

                return value;
            });

            // Now replace all elements where the replacement value is not null.
            foreach (var replacement in replacements) {
                if (replacement.Value != null) {
                    replacement.Key.ReplaceWith(replacement.Value);
                }
                else {
                    replacement.Key.Remove();
                }
            }

            return root;
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

        /// <summary>
        /// Converts an object to an XElement then saves it to a textWriter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="textWriter"></param>
        public static void WriteXElement<T>(this T obj, TextWriter textWriter) {
            using (var memoryStream = new MemoryStream()) {
                using (TextWriter streamWriter = new StreamWriter(memoryStream)) {
                    var xmlSerializer = new XmlSerializer(typeof(T));

                    xmlSerializer.Serialize(streamWriter, obj);
                    
                    XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray())).Save(textWriter);
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
