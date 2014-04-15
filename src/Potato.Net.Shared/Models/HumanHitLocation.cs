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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A hit location for a humanoid
    /// </summary>
    [Flags, Serializable]
    public enum HumanHitLocation {
        /// <summary>
        /// No hit indicated
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Head was hit
        /// </summary>
        Head = 0x1,
        /// <summary>
        /// Neck was hit
        /// </summary>
        Neck = 0x2,
        /// <summary>
        /// Upper torso was hit
        /// </summary>
        UpperTorso = 0x4,
        /// <summary>
        /// Lower torso was hit
        /// </summary>
        LowerTorso = 0x8,
        /// <summary>
        /// Upper left leg was hit
        /// </summary>
        UpperLeftLeg = 0x10,
        /// <summary>
        /// Upper right leg was hit
        /// </summary>
        UpperRightLeg = 0x20,
        /// <summary>
        /// Lower left left was hit
        /// </summary>
        LowerLeftLeg = 0x40,
        /// <summary>
        /// Lower right leg was hit
        /// </summary>
        LowerRightLeg = 0x80,
        /// <summary>
        /// Left foot was hit
        /// </summary>
        LeftFoot = 0x100,
        /// <summary>
        /// Right foot was hit
        /// </summary>
        RightFoot = 0x200,
        /// <summary>
        /// Upper left arm was hit
        /// </summary>
        UpperLeftArm = 0x400,
        /// <summary>
        /// Upper right arm was hit
        /// </summary>
        UpperRightArm = 0x800,
        /// <summary>
        /// Lower left arm was hit
        /// </summary>
        LowerLeftArm = 0x1000,
        /// <summary>
        /// Lower right arm was hit
        /// </summary>
        LowerRightArm = 0x2000,
        /// <summary>
        /// Left hand was hit
        /// </summary>
        LeftHand = 0x4000,
        /// <summary>
        /// Right hand was hit
        /// </summary>
        RightHand = 0x8000
    }
}
