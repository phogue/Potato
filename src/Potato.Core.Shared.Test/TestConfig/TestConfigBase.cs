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
using System.IO;
using NUnit.Framework;

namespace Potato.Core.Shared.Test.TestConfig {
    public abstract class TestConfigBase {
        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFileA = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Potato.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete.json"));

        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFileB = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Potato.Core.Shared.Test.TestConfig.Mocks.AlternativeName.json"));

        [SetUp]
        public void ClearConfigDirectory() {
            this.ConfigFileA.Refresh();

            if (this.ConfigFileA.Directory != null) {
                if (this.ConfigFileA.Directory.Exists == true) {
                    this.ConfigFileA.Directory.Delete(true);
                }

                this.ConfigFileA.Directory.Create();
            }

            this.ConfigFileB.Refresh();

            if (this.ConfigFileB.Directory != null) {
                if (this.ConfigFileB.Directory.Exists == true) {
                    this.ConfigFileB.Directory.Delete(true);
                }

                this.ConfigFileB.Directory.Create();
            }
        }
    }
}
