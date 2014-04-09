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
