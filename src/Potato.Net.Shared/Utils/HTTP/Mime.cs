#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Net.Shared.Utils.HTTP {
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

            name = name.Trim('.');

            if (Mime.Types.ContainsKey(name.ToLower()) == true) {
                mimeType = name.ToLower();
            }
            else if (Mime.Extensions.ContainsKey(name.ToLower()) == true) {
                mimeType = Mime.Extensions[name.ToLower()].ToLower();
            }

            return mimeType;
        }

        /// <summary>
        /// Fetch the default mime type to use (if none is specified) given a method
        /// </summary>
        /// <param name="method">The method of the request (GET, POST, etc)</param>
        /// <returns>The default mime type</returns>
        public static String DefaultMimeTypeGivenMethod(String method) {
            return Method.Equals(method, Method.Get) ? Mime.TextHtml : Mime.ApplicationJson;
        }
    }
}
