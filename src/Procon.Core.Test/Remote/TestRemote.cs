#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Procon.Core.Connections.Plugins;
using Procon.Core.Remote;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils;
using Procon.Net.Shared.Utils.HTTP;
using Procon.Net.Utils;
using Procon.Net.Utils.HTTP;

#endregion

namespace Procon.Core.Test.Remote {
    [TestFixture]
    public class TestRemote {
        [SetUp]
        protected void Setup() {
            // We could actually validate the certificate in /Certificates directory
            // but for unit testing I find this acceptable enough.

            // If you're reading this, never put this into production (anywhere.)
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        /// <summary>
        ///     Sets up a command server for us to poke at.
        /// </summary>
        protected CommandServerController SetupCommandServer(int listeningPort = 3222) {
            var variables = new VariableController();
            var security = new SecurityController();

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            // We use the TestPlugin to validate various commands can be executed
            var plugins = new CorePluginController().Execute() as CorePluginController;

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            var commandServer = new CommandServerController() {
                TunnelObjects = new List<ICoreController>() {
                    plugins
                },
                Security = security,
                Variables = variables
            };

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    listeningPort
                })
            });

            commandServer.Execute();

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            return commandServer;
        }

        /// <summary>
        ///     Tests variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestRemoteCommandServerDispose() {
            const int listeningPort = 3224;

            CommandServerController commandServer = SetupCommandServer(listeningPort);

            commandServer.Dispose();

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNull(commandServer.TunnelObjects);
        }

        /// <summary>
        ///     Tests that sending a malformed command will return a bad request response.
        /// </summary>
        [Test]
        public void TestRemoteCommandServerMalformedCommandRequest() {
            const int listeningPort = 3226;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = "Lulz, whats up?"
            };

            request.Complete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.BadRequest, request.WebResponse.StatusCode);
            Assert.IsFalse(isSuccess);
        }

        /// <summary>
        ///     Tests that altering the command server enabled/disabled variable
        ///     on an active listener will disable and null the listener.
        /// </summary>
        [Test]
        public void TestRemoteCommandServerVariableDisabled() {
            const int listeningPort = 3225;

            CommandServerController commandServer = SetupCommandServer(listeningPort);

            commandServer.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    false
                })
            });

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.TunnelObjects);
        }

        /// <summary>
        ///     Tests that an incorrect authentication will have a failed success message.
        /// </summary>
        [Test]
        public void TestRemoteSandboxAuthenticationFailed() {
            const int listeningPort = 3222;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = new Command() {
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Parameters = new List<CommandParameter>(),
                    Username = "Phogue",
                    PasswordPlainText = "wrongPassword"
                }.ToXElement().ToString()
            };

            CommandResultArgs result = null;

            request.Complete += sender => {
                isSuccess = true;
                result = XDocument.Parse(sender.GetResponseContent()).Root.FromXElement<CommandResultArgs>();
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(isSuccess);
        }

        /// <summary>
        ///     Tests that a correct authentication with no relvan command will still return a 200 OK status.
        /// </summary>
        [Test]
        public void TestRemoteSandboxAuthenticationSuccess() {
            const int listeningPort = 3221;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = new Command() {
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Parameters = new List<CommandParameter>(),
                    Username = "Phogue",
                    PasswordPlainText = "password"
                }.ToXElement().ToString()
            };

            request.Complete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);
        }

        /// <summary>
        ///     Tests that a html file can be fetched via the command server.
        /// </summary>
        [Test]
        public void TestRemoteSandboxIndexHtml() {
            const int listeningPort = 3223;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = new Command() {
                    Name = "/test/parameters",
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Username = "Phogue",
                    PasswordPlainText = "password",
                    Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                        "Phogue",
                        50
                    })
                }.ToXElement().ToString()
            };

            request.Complete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);

            Assert.AreEqual("Welcome <b>Phogue</b> to the parameter test of this plugin. Your score is: 50", request.GetResponseContent());
        }

        /// <summary>
        ///     Tests that a html file can be fetched via the command server, even when a content type is specified and it is json
        /// </summary>
        [Test]
        public void TestRemoteSandboxJsonRequestIndexHtml() {
            const int listeningPort = 3227;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContentType = Mime.ApplicationJson,
                RequestContent = JsonConvert.SerializeObject(new Command() {
                    Name = "/test/parameters",
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Username = "Phogue",
                    PasswordPlainText = "password",
                    Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                        "Phogue",
                        50
                    })
                })
            };

            request.Complete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);

            Assert.AreEqual("Welcome <b>Phogue</b> to the parameter test of this plugin. Your score is: 50", request.GetResponseContent());
        }

        /// <summary>
        ///     Tests that a command can be initiated with no parameters passed in.
        /// </summary>
        [Test]
        public void TestRemoteSandboxJsonRequestNoParameters() {
            const int listeningPort = 3228;

            var requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            SetupCommandServer(listeningPort);

            var request = new Request(String.Format("https://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContentType = Mime.ApplicationJson,
                RequestContent = JsonConvert.SerializeObject(new Command() {
                    Name = "TestPluginsCommandsZeroParameters",
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Username = "Phogue",
                    PasswordPlainText = "password"
                })
            };

            request.Complete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.Error += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);

            var result = JsonConvert.DeserializeObject<CommandResultArgs>(request.GetResponseContent());

            Assert.AreEqual("TestPluginsCommandsZeroParameters", result.Message);
        }
    }
}