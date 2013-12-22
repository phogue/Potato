using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Shared.Utils.HTTP {
    public class Mime {

        public const String ApplicationJson = "application/json";
        public const String ApplicationXml = "application/xml";
        public const String TextHtml = "text/html";

        /// <summary>
        /// A list of extensions (unique) with the mime type that relates to that file type.
        /// </summary>
        public static Dictionary<String, String> Extensions = new Dictionary<String, String>() {
            { "json", Mime.ApplicationJson },
            { "xml", Mime.ApplicationXml },
            { "html", Mime.TextHtml },
            { "htm", Mime.TextHtml }
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
