using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Procon.Net.Shared.Test.TestProtocolAssemblyMetadata {
    [TestFixture]
    public class TestLoad {

        public DirectoryInfo TestPath = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Procon.Net.Shared.Test.ProtocolAssemblyMetadata", "Myrcon.Protocols.Frostbite"));

        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteTestPathDirectory() {
            this.TestPath.Refresh();

            if (this.TestPath.Exists == true) {
                this.TestPath.Delete(true);
            }

            this.TestPath.Create();
        }

        /// <summary>
        /// Tests the meta file path is set correctly
        /// </summary>
        [Test]
        public void TestMetaFileSetOnObject() {
            var meta = new ProtocolAssemblyMetadata();

            meta.Load(this.TestPath.FullName);

            Assert.AreEqual(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), meta.Meta.FullName);
        }

        /// <summary>
        /// Tests the assembly file path is set correctly
        /// </summary>
        [Test]
        public void TestAssemblyFileSetOnObject() {
            var assembly = new ProtocolAssemblyMetadata();

            assembly.Load(this.TestPath.FullName);

            Assert.AreEqual(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), assembly.Assembly.FullName);
        }

        /// <summary>
        /// Tests the load will fail when no dll is found
        /// </summary>
        [Test]
        public void TestFailureWhenDllFileNotFound() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), "{}");

            var result = new ProtocolAssemblyMetadata().Load(this.TestPath.FullName);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests the load will fail when no json is found
        /// </summary>
        [Test]
        public void TestFailureWhenJsonFileNotFound() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), "binary");

            var result = new ProtocolAssemblyMetadata().Load(this.TestPath.FullName);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test success when both files are present and json has basic object notation
        /// </summary>
        [Test]
        public void TestSuccessWhenBothFilesPresent() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), "binary");
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), "{}");

            var result = new ProtocolAssemblyMetadata().Load(this.TestPath.FullName);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test failure when json file has incorrect format
        /// </summary>
        [Test]
        public void TestFailureWhenIncorrectFormatJsonFile() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), "binary");
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), "not json data at all!");

            var result = new ProtocolAssemblyMetadata().Load(this.TestPath.FullName);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test any protocol types are loaded successfully
        /// </summary>
        [Test]
        public void TestProtocolTypesLoadedSuccessfully() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), "binary");
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), @"{ ""ProtocolTypes"": [ { ""Provider"": ""Myrcon"",""Name"": ""Battlefield 4"",""Type"": ""DiceBattlefield4"" } ] }");

            var meta = new ProtocolAssemblyMetadata();

            meta.Load(this.TestPath.FullName);

            Assert.IsNotEmpty(meta.ProtocolTypes);
        }

        /// <summary>
        /// Test the data within the loaded protcol types was loaded successfully
        /// </summary>
        [Test]
        public void TestProtocolTypesDataLoadedSuccessfully() {
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.dll"), "binary");
            File.WriteAllText(Path.Combine(this.TestPath.FullName, "Myrcon.Protocols.Frostbite.json"), @"{ ""ProtocolTypes"": [ { ""Provider"": ""Myrcon"",""Name"": ""Battlefield 4"",""Type"": ""DiceBattlefield4"" } ] }");

            var meta = new ProtocolAssemblyMetadata();

            meta.Load(this.TestPath.FullName);

            Assert.AreEqual("Battlefield 4", meta.ProtocolTypes.First().Name);
            Assert.AreEqual("Myrcon", meta.ProtocolTypes.First().Provider);
            Assert.AreEqual("DiceBattlefield4", meta.ProtocolTypes.First().Type);
        }
    }
}
