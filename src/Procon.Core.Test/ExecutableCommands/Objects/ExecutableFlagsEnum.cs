#region

using System;

#endregion

namespace Procon.Core.Test.ExecutableCommands.Objects {
    [Flags]
    public enum ExecutableFlagsEnum {
        None = 0x00,
        One = 0x01,
        Two,
        Three,
        Four,
        Five
    }
}