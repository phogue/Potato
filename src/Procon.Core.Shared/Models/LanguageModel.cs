using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class LanguageModel : CoreModel {
        /// <summary>
        /// IETF language tag for the LanguageCode string.
        /// http://en.wikipedia.org/wiki/IETF_language_tag
        /// http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// http://en.wikipedia.org/wiki/ISO_3166-1
        /// </summary>
        public String LanguageCode { get; set; }
    }
}
