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
#region

using System;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands.Objects {
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