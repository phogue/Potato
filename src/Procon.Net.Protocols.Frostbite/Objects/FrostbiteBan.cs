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
    public class FrostbiteBan : Ban {

        public FrostbiteBan()
            : base() {

        }

        public FrostbiteBan ParseBanListItem(List<string> words) {

            if (words.Count == 5) {

                this.Time = new FrostbiteTimeSubset().Parse(words.GetRange(2, 2));

                if (String.Compare(words[0], "name") == 0) {
                    this.Target = new Player() {
                        Name = words[1]
                    };
                }
                // Procon 2.0 considers ip banning as obsolete
                //else if (String.Compare(words[0], "ip") == 0) {
                //    this.IpAddress = words[1];
                //}
                else if (String.Compare(words[0], "guid") == 0) {
                    this.Target = new Player() {
                        GUID = words[1]
                    };
                }

                this.BanActionType = Protocols.Objects.BanActionType.Listed;

                this.Reason = words[4];
            }

            return this;
        }

        public FrostbiteBan ParseBanRemove(List<string> words) {

            if (String.Compare(words[0], "name") == 0) {
                this.Target = new Player() {
                    Name = words[1]
                };
            }
            else if (String.Compare(words[0], "guid") == 0) {
                this.Target = new Player() {
                    GUID = words[1]
                };
            }

            return this;
        }

        public FrostbiteBan ParseBanAdd(List<string> words) {

            if (String.Compare(words[0], "name") == 0) {
                this.Target = new Player() {
                    Name = words[1]
                };
            }
            else if (String.Compare(words[0], "guid") == 0) {
                this.Target = new Player() {
                    GUID = words[1]
                };
            }

            if (words.Count == 3) {
                this.Time = new FrostbiteTimeSubset().Parse(words.GetRange(2, 1));
            }
            else if (words.Count == 4) {
                this.Time = new FrostbiteTimeSubset().Parse(words.GetRange(2, 2));

                // Time has a seconds parameter
                if (this.Time.Context != TimeSubsetContext.Time) {
                    this.Reason = words[3];
                }
            }
            else if (words.Count == 5) {
                this.Time = new FrostbiteTimeSubset().Parse(words.GetRange(2, 2));

                this.Reason = words[4];
            }

            return this;
        }

    }
}
