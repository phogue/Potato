using System;
using System.IO;
using NUnit.Framework;

namespace Procon.Core.Shared.Test.TestConfig {
    public abstract class TestConfigBase {
        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFileA = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete.json"));

        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFileB = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Procon.Core.Shared.Test.TestConfig.Mocks.AlternativeName.json"));

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
