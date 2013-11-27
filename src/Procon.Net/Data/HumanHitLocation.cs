using System;

namespace Procon.Net.Data {
    [Flags, Serializable]
    public enum HumanHitLocation {
        Head = 0x1,
        Neck = 0x2,
        UpperTorso = 0x4,
        LowerTorso = 0x8,
        UpperLeftLeg = 0x10,
        UpperRightLeg = 0x20,
        LowerLeftLeg = 0x40,
        LowerRightLeg = 0x80,
        LeftFoot = 0x100,
        RightFoot = 0x200,
        UpperLeftArm = 0x400,
        UpperRightArm = 0x800,
        LowerLeftArm = 0x1000,
        LowerRightArm = 0x2000,
        LeftHand = 0x4000,
        RightHand = 0x8000
    }
}
