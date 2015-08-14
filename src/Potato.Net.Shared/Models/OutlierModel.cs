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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// An outlier attached to an object
    /// </summary>
    [Serializable]
    public sealed class OutlierModel : NetworkModel {
        /// <summary>
        /// The name of the field outlier, or describing factor of the data e.g Kills, Ping etc.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The value as an outlier
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// The mean value of this field over all samples 
        /// </summary>
        public float Mean { get; set; }

        /// <summary>
        /// The standard deviation of this field over all samples.
        /// </summary>
        public float StandardDeviation { get; set; }

        /// <summary>
        /// The number of deviations above the mean (Grubbs test)
        /// </summary>
        public float Deviations { get; set; }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public OutlierModel() {
            Field = string.Empty;
        }
    }
}
