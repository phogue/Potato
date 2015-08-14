#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Fuzzy.Models {
    /// <summary>
    /// A matching object from the jobject document
    /// </summary>
    public class MatchModel {
        public string Name { get; set; }

        public string Text { get; set; }

        public Regex CompiledRegex { get; set; }

        public string Regex {
            get { return _regex; }
            set { 
                _regex = value;
                CompiledRegex = new Regex(_regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }
        private string _regex;

        public string Value { get; set; }
    }
}
