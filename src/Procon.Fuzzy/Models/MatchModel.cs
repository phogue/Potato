using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Fuzzy.Models {
    /// <summary>
    /// A matching object from the jobject document
    /// </summary>
    public class MatchModel {
        public String Name { get; set; }

        public String Text { get; set; }

        public Regex CompiledRegex { get; set; }

        public String Regex {
            get { return this._regex; }
            set { 
                _regex = value;
                this.CompiledRegex = new Regex(this._regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }
        private string _regex;

        public String Value { get; set; }
    }
}
