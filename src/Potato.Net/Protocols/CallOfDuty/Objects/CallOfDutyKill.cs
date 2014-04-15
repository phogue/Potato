using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Potato.Net.Protocols.CallOfDuty.Objects {
    using Potato.Net.Protocols.Objects;

    [Serializable]
    public class CallOfDutyKill : NetworkObject, ICallOfDutyObject {

        public NetworkObject Parse(Match match) {
            Kill kill = new Kill {
                Scope = {
                    Players = new List<Player>() {
                        new Player() {
                            SlotId = uint.Parse(match.Groups["V_ID"].Value),
                            Name = match.Groups["V_Name"].Value,
                            Uid = match.Groups["V_GUID"].Value
                        }
                    },
                    Items = new List<Item>() {
                        new Item() {
                            Name = match.Groups["Weapon"].Value
                        }
                    }
                },
                Now = {
                    Players = new List<Player>() {
                        new Player() {
                            SlotId = uint.Parse(match.Groups["V_ID"].Value),
                            Name = match.Groups["V_Name"].Value,
                            Uid = match.Groups["V_GUID"].Value
                        }
                    }
                }
            };

            switch (match.Groups["HitLocation"].Value.ToLower()) {
                // Body
                case "head":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.Head;
                    break;
                case "neck":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.Neck;
                    break;
                case "torso_upper":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.UpperTorso;
                    break;
                case "torso_lower":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LowerTorso;
                    break;

                // Left arm
                case "left_arm_lower":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LowerLeftArm;
                    break;
                case "left_arm_upper":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.UpperLeftArm;
                    break;

                // Right arm
                case "right_arm_lower":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LowerLeftArm;
                    break;
                case "right_arm_upper":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.UpperLeftArm;
                    break;

                // Left Leg
                case "left_leg_lower":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LowerLeftLeg;
                    break;
                case "left_leg_upper":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.UpperLeftLeg;
                    break;
                case "left_foot":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LeftFoot;
                    break;

                // Right Leg
                case "right_leg_lower":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.LowerRightLeg;
                    break;
                case "right_leg_upper":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.UpperRightLeg;
                    break;
                case "right_foot":
                    kill.HumanHitLocation = Protocols.Objects.HumanHitLocation.RightFoot;
                    break;
            }

            return kill;
        }
    }
}
