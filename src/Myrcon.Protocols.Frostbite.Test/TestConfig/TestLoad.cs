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
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Test.TestConfig {
    [TestFixture]
    public class TestLoad {
        public static String ConfigDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/Protocols/Myrcon");

        /// <summary>
        /// Checks all configs can be loaded (no json errors)
        /// </summary>
        [Test]
        public void TestJsonIntegrity() {
            foreach (var file in Directory.EnumerateFiles(TestLoad.ConfigDirectory, "*.json", SearchOption.AllDirectories)) {
                var config = ProtocolConfigLoader.Load<ProtocolConfigModel>(file);

                Assert.IsNotNull(config, "Failed to load config {0}. Validate json structure.", Path.GetFileName(file));
            }
        }

        /// <summary>
        /// Loads all game configs and ensures all maps have a game mode.
        /// </summary>
        [Test]
        public void TestMapGamemodeIntegrity() {
            foreach (var file in Directory.EnumerateFiles(TestLoad.ConfigDirectory, "*.json", SearchOption.AllDirectories)) {
                var config = ProtocolConfigLoader.Load<ProtocolConfigModel>(file);

                foreach (var map in config.MapPool) {
                    Assert.IsNotNull(map.GameMode, "Missing game mode for map {0} {1}", map.Name, map.FriendlyName);
                }
            }
        }
    }
}
