using System;
using System.Xml;
using System.Xml.Linq;

namespace Procon.Core.Utils {
    public static class XElementValidator {

        public static bool TryParse(String xml, out XElement element) {
            bool parsed = false;

            try {
                element = XElement.Parse(xml);

                parsed = true;
            }
            catch (XmlException) {
                element = null;
            }

            return parsed;
        }
    }
}
