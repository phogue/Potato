// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbiteTimeSubset : TimeSubset {

        public FrostbiteTimeSubset Parse(List<string> words) {
            this.Context = TimeSubsetContext.None;
            int seconds = 0;

            if (String.Compare(words[0], "perm") == 0) {
                this.Context = TimeSubsetContext.Permanent;
            }
            else if (String.Compare(words[0], "round") == 0) {
                this.Context = TimeSubsetContext.Round;
            }
            else if (words.Count == 2 && String.Compare(words[0], "seconds") == 0 && int.TryParse(words[1], out seconds) == true) {
                this.Context = TimeSubsetContext.Time;

                this.Length = TimeSpan.FromSeconds(seconds);
            }

            return this;
        }

    }
}
