using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A localization key-value-pair
    /// </summary>
    public class LanguageEntryModel {
        /// <summary>
        /// The name of this entry
        /// </summary>
        public String Name { get; set; }
        
        /// <summary>
        /// The text attached to this entry
        /// </summary>
        public String Text { get; set; }
    }
}
