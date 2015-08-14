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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// The current state of a package (installed, updatable or installed with no update)
    /// </summary>
    [Serializable]
    public enum PackageState {
        /// <summary>
        /// Package is not installed and is available on remote server
        /// </summary>
        NotInstalled,
        /// <summary>
        /// The package is installed and up to date
        /// </summary>
        Installed,
        /// <summary>
        /// Package installed, but version is out of date.
        /// </summary>
        UpdateAvailable
    }
}
