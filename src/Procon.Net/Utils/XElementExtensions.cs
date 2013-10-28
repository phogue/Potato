using System;
using System.Xml.Linq;

namespace Procon.Net.Utils {
    public static class XElementExtensions {
        public static string ElementValue(this XElement element, string xName) {
            XElement xNameElement = element.Element(xName);

            return xNameElement != null ? xNameElement.Value : String.Empty;
        }
    }
}
