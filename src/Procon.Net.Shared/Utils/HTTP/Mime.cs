using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Shared.Utils.HTTP {
    /// <summary>
    /// Mime types
    /// </summary>
    public class Mime {
        /// <summary>
        /// application/json
        /// </summary>
        public const String ApplicationJson = "application/json";

        /// <summary>
        /// application/xml
        /// </summary>
        public const String ApplicationXml = "application/xml";

        /// <summary>
        /// text/html
        /// </summary>
        public const String ApplicationJavascript = "application/javascript";

        /// <summary>
        /// text/html
        /// </summary>
        public const String TextHtml = "text/html";
        
        /// <summary>
        /// text/html
        /// </summary>
        public const String TextCss = "text/css";
        

        /// <summary>
        /// A list of extensions (unique) with the mime type that relates to that file type.
        /// </summary>
        public static Dictionary<String, String> Extensions = new Dictionary<String, String>() {
            { "json", Mime.ApplicationJson },
            { "xml", Mime.ApplicationXml },
            { "js", Mime.ApplicationJavascript },
            { "html", Mime.TextHtml },
            { "htm", Mime.TextHtml },
            { "css", Mime.TextCss }
        };

        /// <summary>
        /// The inverse of the extensions dictionary, but indexed by the mime type with a list of file extensions associated with it.
        /// </summary>
        public static Dictionary<String, List<String>> Types = Mime.Extensions.GroupBy(type => type.Value).Select(groupedTypes => new {
            Keys = groupedTypes.Key,
            Values = groupedTypes.Select(innerGrouping => innerGrouping.Key).ToList()
        }).ToDictionary(keySelector => keySelector.Keys, valueSelector => valueSelector.Values);

        /// <summary>
        /// Converts a mime type or file extension into a mime type.
        /// </summary>
        /// <param name="name">The data to lookup a mime type</param>
        /// <param name="default">The default to use if the mime type was not found.</param>
        /// <returns>The mime type if available, null if nothing exists</returns>
        public static String ToMimeType(String name, String @default = null) {
            String mimeType = @default;

            if (Mime.Types.ContainsKey(name.ToLower()) == true) {
                mimeType = name.ToLower();
            }
            else if (Mime.Extensions.ContainsKey(name.ToLower()) == true) {
                mimeType = Mime.Extensions[name.ToLower()].ToLower();
            }

            return mimeType;
        }
    }
}
