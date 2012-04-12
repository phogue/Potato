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
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.CallOfDuty.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class CallOfDutyKill : Kill, ICallOfDutyObject {

        public CallOfDutyKill() : base() {

        }

        public ICallOfDutyObject Parse(Match match) {

            this.Killer = new CallOfDutyPlayer() {
                SlotID = uint.Parse(match.Groups["K_ID"].Value),
                Name = match.Groups["K_Name"].Value,
                GUID = match.Groups["K_GUID"].Value
            };

            this.Target = new CallOfDutyPlayer() {
                SlotID = uint.Parse(match.Groups["V_ID"].Value),
                Name = match.Groups["V_Name"].Value,
                GUID = match.Groups["V_GUID"].Value
            };

            this.DamageType = new Item() {
                Name = match.Groups["Weapon"].Value
            };

            switch (match.Groups["HitLocation"].Value.ToLower()) {
                // Body
                case "head":
                    this.HitLocation = Protocols.Objects.HitLocation.Head;
                    break;
                case "neck":
                    this.HitLocation = Protocols.Objects.HitLocation.Neck;
                    break;
                case "torso_upper":
                    this.HitLocation = Protocols.Objects.HitLocation.UpperTorso;
                    break;
                case "torso_lower":
                    this.HitLocation = Protocols.Objects.HitLocation.LowerTorso;
                    break;

                // Left arm
                case "left_arm_lower":
                    this.HitLocation = Protocols.Objects.HitLocation.LowerLeftArm;
                    break;
                case "left_arm_upper":
                    this.HitLocation = Protocols.Objects.HitLocation.UpperLeftArm;
                    break;

                // Right arm
                case "right_arm_lower":
                    this.HitLocation = Protocols.Objects.HitLocation.LowerLeftArm;
                    break;
                case "right_arm_upper":
                    this.HitLocation = Protocols.Objects.HitLocation.UpperLeftArm;
                    break;

                // Left Leg
                case "left_leg_lower":
                    this.HitLocation = Protocols.Objects.HitLocation.LowerLeftLeg;
                    break;
                case "left_leg_upper":
                    this.HitLocation = Protocols.Objects.HitLocation.UpperLeftLeg;
                    break;
                case "left_foot":
                    this.HitLocation = Protocols.Objects.HitLocation.LeftFoot;
                    break;

                // Right Leg
                case "right_leg_lower":
                    this.HitLocation = Protocols.Objects.HitLocation.LowerRightLeg;
                    break;
                case "right_leg_upper":
                    this.HitLocation = Protocols.Objects.HitLocation.UpperRightLeg;
                    break;
                case "right_foot":
                    this.HitLocation = Protocols.Objects.HitLocation.RightFoot;
                    break;
            }

            return this;
        }
    }
}
