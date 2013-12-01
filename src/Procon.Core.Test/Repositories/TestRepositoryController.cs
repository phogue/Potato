using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Repositories;

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestRepositoryController {

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Repository.xml");

        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a config can be written in a specific format.
        /// </summary>
        [Test]
        public void TestRepositoryControllerWriteConfig() {
            RepositoryController repository = new RepositoryController();

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "http://localhost/"
                            }
                        }
                    }
                }
            });

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "localhost"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "packageUid"
                            }
                        }
                    }
                }
            });

            // Save a config of the language controller
            Config saveConfig = new Config();
            saveConfig.Create(typeof(RepositoryController));
            repository.WriteConfig(saveConfig);
            saveConfig.Save(TestRepositoryController.ConfigFileInfo);

            // Load the config in a new config.
            Config loadConfig = new Config();
            loadConfig.Load(TestRepositoryController.ConfigFileInfo);

            var commands = loadConfig.Root.Descendants("RepositoryController").Elements("Command").ToList();

            Assert.AreEqual("PackagesAddRemoteRepository", commands[0].Attribute("name").Value);
            Assert.AreEqual("http://localhost/", commands[0].Element("url").Value);
            
            Assert.AreEqual("PackagesIngoreAutomaticUpdateOnPackage", commands[1].Attribute("name").Value);
            Assert.AreEqual("localhost", commands[1].Element("urlSlug").Value);
            Assert.AreEqual("packageUid", commands[1].Element("packageUid").Value);
        }
    }
}
