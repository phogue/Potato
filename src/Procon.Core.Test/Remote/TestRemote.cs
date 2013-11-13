using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Procon.Core.Connections.Plugins;
using Procon.Core.Security;
using Procon.Net.Utils;
using Procon.Core.Variables;
using Procon.Net.Utils;
using Procon.Net.Utils.HTTP;

namespace Procon.Core.Test.Remote {
    using Procon.Core.Remote;

    [TestFixture]
    public class TestRemote {
        
        // Daemon runs, can authenticate and call function in test plugin

        // Daemon runs, denied authentication (account does not exist)

        // Daemon runs, maximum requests are handled
        
        // Daemon runs, handles several thousand calls in a second.

        /// <summary>
        /// Sets up a daemon for us to poke at.
        /// </summary>
        protected DaemonController SetupDaemon(int listeningPort = 3222) {
            VariableController variables = new VariableController();
            SecurityController security = new SecurityController();

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.SecurityAccountAuthenticate, 1 }) });

            // We use the TestPlugin to validate various commands can be executed
            PluginController plugins = new PluginController().Execute() as PluginController;

            plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            DaemonController daemon = new DaemonController() {
                ExecutableObjects = new List<IExecutableBase>() {
                    plugins
                },
                Security = security,
                Variables = variables
            };

            variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.DaemonListenerPort,
                    listeningPort
                })
            });

            daemon.Execute();

            variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.DaemonEnabled,
                    true
                })
            });

            return daemon;
        }

        /// <summary>
        /// Tests that a correct authentication with no relvan command will still return a 200 OK status.
        /// </summary>
        [Test]
        public void TestRemoteSandboxAuthenticationSuccess() {
            const int listeningPort = 3221;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = new Command() {
                    Scope = new CommandScope(),
                    Origin = CommandOrigin.Remote,
                    Parameters = new List<CommandParameter>(),
                    Username = "Phogue",
                    PasswordPlainText = "password"
                }.ToXElement().ToString()
            };

            request.RequestComplete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.RequestError += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);
        }

        /// <summary>
        /// Tests that an incorrect authentication will have a failed success message.
        /// </summary>
        [Test]
        public void TestRemoteSandboxAuthenticationFailed() {
            const int listeningPort = 3222;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
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

            request.RequestComplete += sender => {
                isSuccess = true;
                result = XDocument.Parse(sender.GetResponseContent()).Root.FromXElement<CommandResultArgs>();
                requestWait.Set();
            };

            request.RequestError += sender => {
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
        /// Tests that a html file can be fetched via the daemon.
        /// </summary>
        [Test]
        public void TestRemoteSandboxIndexHtml() {
            const int listeningPort = 3223;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
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

            request.RequestComplete += sender => {
                isSuccess = true;
                requestWait.Set();
            };
             
            request.RequestError += sender => {
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
        /// Tests variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestRemoteDaemonDispose() {
            const int listeningPort = 3224;

            DaemonController daemon = this.SetupDaemon(listeningPort);

            daemon.Dispose();

            Assert.IsNull(daemon.DaemonListener);
            Assert.IsNull(daemon.ExecutableObjects);
        }

        /// <summary>
        /// Tests that altering the daemon enabled/disabled variable
        /// on an active listener will disable and null the listener.
        /// </summary>
        [Test]
        public void TestRemoteDaemonVariableDisabled() {
            const int listeningPort = 3225;

            DaemonController daemon = this.SetupDaemon(listeningPort);

            daemon.Variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.DaemonEnabled,
                    false
                })
            });

            Assert.IsNull(daemon.DaemonListener);
            Assert.IsNotNull(daemon.ExecutableObjects);
        }

        /// <summary>
        /// Tests that sending a malformed command will return a bad request response.
        /// </summary>
        [Test]
        public void TestRemoteDaemonMalformedCommandRequest() {
            const int listeningPort = 3226;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
                Method = "POST",
                RequestContent = "Lulz, whats up?"
            };

            request.RequestComplete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.RequestError += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.BadRequest, request.WebResponse.StatusCode);
            Assert.IsFalse(isSuccess);
        }

        /// <summary>
        /// Tests that a html file can be fetched via the daemon, even when a content type is specified and it is json
        /// </summary>
        [Test]
        public void TestRemoteSandboxJsonRequestIndexHtml() {
            const int listeningPort = 3227;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
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

            request.RequestComplete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.RequestError += sender => {
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
        /// Tests that a command can be initiated with no parameters passed in.
        /// </summary>
        [Test]
        public void TestRemoteSandboxJsonRequestNoParameters() {
            const int listeningPort = 3228;

            AutoResetEvent requestWait = new AutoResetEvent(false);
            bool isSuccess = false;

            this.SetupDaemon(listeningPort);

            Request request = new Request(String.Format("http://127.0.0.1:{0}/", listeningPort)) {
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

            request.RequestComplete += sender => {
                isSuccess = true;
                requestWait.Set();
            };

            request.RequestError += sender => {
                isSuccess = false;
                requestWait.Set();
            };

            request.BeginRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, request.WebResponse.StatusCode);
            Assert.IsTrue(isSuccess);

            CommandResultArgs result = JsonConvert.DeserializeObject<CommandResultArgs>(request.GetResponseContent());

            Assert.AreEqual("TestPluginsCommandsZeroParameters", result.Message);
        }

        /*
        [Test]
        public void TestRemoteSandbox() {
            Executable.MasterSecurity.Dispose();
            Executable.MasterSecurity.Execute();
            Executable.MasterSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            Executable.MasterSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            Executable.MasterSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });
            Executable.MasterSecurity.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.SecurityAccountAuthenticate, 1 }) });

            DaemonController daemon = new DaemonController();

            Executable.MasterVariables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.DaemonListenerPort,
                    3222
                }
            });

            daemon.Execute();

            Executable.MasterVariables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.DaemonEnabled,
                    true
                }
            });

            System.Threading.Thread.Sleep(10000000);
        }
        */
    }
}
