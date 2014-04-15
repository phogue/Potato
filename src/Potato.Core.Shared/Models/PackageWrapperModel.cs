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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Wraps a package with what is known about the state of the package
    /// </summary>
    [Serializable]
    public class PackageWrapperModel : CoreModel {
        /// <summary>
        /// The id found in this models Installed or Available properties.
        /// </summary>
        public String Id {
            get {
                return this.Available.Id ?? this.Installed.Id;
            }
        }

        /// <summary>
        /// What state this package is in. Is it installed or not,
        /// updateable or is just installed.
        /// </summary>
        public PackageState State { get; set; }

        /// <summary>
        /// The currently installed version of the package.
        /// </summary>
        public PackageModel Installed { get; set; }

        /// <summary>
        /// The available version of the package form the source.
        /// </summary>
        public PackageModel Available { get; set; }

        /// <summary>
        /// Initializes the wrapper with the default values.
        /// </summary>
        public PackageWrapperModel() {
            this.Installed = new PackageModel();
            this.Available = new PackageModel();
        }
    }
}
