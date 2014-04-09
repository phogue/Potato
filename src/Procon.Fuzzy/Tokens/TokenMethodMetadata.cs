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

namespace Procon.Fuzzy.Tokens {
    public class TokenMethodMetadata {
        /// <summary>
        /// The namespace the method exists in.
        /// </summary>
        public String Namespace { get; set; }

        /// <summary>
        /// A list of parameters to pass through to the reduction method.
        /// </summary>
        public List<TokenParameter> Parameters { get; set; }

        /// <summary>
        /// Exact match for each type (not assignable, but exactly of the type)
        /// </summary>
        public bool ExactMatchType { get; set; }

        /// <summary>
        /// Exactly match the order of a signature, not just a combination.
        /// </summary>
        public bool ExactMatchSignature { get; set; }

        /// <summary>
        /// Ensures all tokens are compatible before executing the method. See Token.CompatibleWith
        /// </summary>
        public bool DemandTokenCompatability { get; set; }

        public TokenMethodMetadata() {
            this.Parameters = new List<TokenParameter>();
        }
    }
}