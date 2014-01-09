#region

using System;
using System.Collections.Generic;
using System.IO;
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
                Shared = {
                    Security = security,
                    Variables = variables
                }
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationJson;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write("Lulz, whats up?");
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);

                            isSuccess = true;
                            requestWait.Set();
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse) e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
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

            commandServer.Shared.Variables.Tunnel(new Command() {
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;
            CommandResultArgs result = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationXml;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write(new Command() {
                            Scope = new CommandScope(),
                            Origin = CommandOrigin.Remote,
                            Parameters = new List<CommandParameter>(),
                            Username = "Phogue",
                            PasswordPlainText = "wrongPassword"
                        }.ToXElement().ToString());
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);
                            
                            using (TextReader reader = new StreamReader(response.GetResponseStream())) {
                                isSuccess = true;
                                result = XDocument.Parse(reader.ReadToEnd()).Root.FromXElement<CommandResultArgs>();
                                requestWait.Set();
                            }
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;
            CommandResultArgs result = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationXml;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write(new Command() {
                            Scope = new CommandScope(),
                            Origin = CommandOrigin.Remote,
                            Parameters = new List<CommandParameter>(),
                            Username = "Phogue",
                            PasswordPlainText = "password"
                        }.ToXElement().ToString());
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);

                            using (TextReader reader = new StreamReader(response.GetResponseStream())) {
                                isSuccess = true;
                                result = XDocument.Parse(reader.ReadToEnd()).Root.FromXElement<CommandResultArgs>();
                                requestWait.Set();
                            }
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;
            String result = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationXml;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write(
                            new Command() {
                                Name = "/test/parameters",
                                Scope = new CommandScope(),
                                Origin = CommandOrigin.Remote,
                                Username = "Phogue",
                                PasswordPlainText = "password",
                                Parameters = TestHelpers.ObjectListToContentList(
                                    new List<Object>() {
                                    "Phogue",
                                    50
                                })
                            }.ToXElement().ToString()
                        );
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);

                            using (TextReader reader = new StreamReader(response.GetResponseStream())) {
                                isSuccess = true;
                                result = reader.ReadToEnd();
                                requestWait.Set();
                            }
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(isSuccess);

            Assert.AreEqual("Welcome <b>Phogue</b> to the parameter test of this plugin. Your score is: 50", result);
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;
            String result = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationXml;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write(
                            new Command() {
                                Name = "/test/parameters",
                                Scope = new CommandScope(),
                                Origin = CommandOrigin.Remote,
                                Username = "Phogue",
                                PasswordPlainText = "password",
                                Parameters = TestHelpers.ObjectListToContentList(
                                    new List<Object>() {
                                    "Phogue",
                                    50
                                })
                            }.ToXElement().ToString()
                        );
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);

                            using (TextReader reader = new StreamReader(response.GetResponseStream())) {
                                isSuccess = true;
                                result = reader.ReadToEnd();
                                requestWait.Set();
                            }
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(isSuccess);

            Assert.AreEqual("Welcome <b>Phogue</b> to the parameter test of this plugin. Your score is: 50", result);
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

            WebRequest request = WebRequest.Create(String.Format("https://127.0.0.1:{0}/", listeningPort));
            HttpWebResponse response = null;
            CommandResultArgs result = null;

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = Mime.ApplicationJson;
            request.Proxy = null;

            request.BeginGetRequestStream(streamAsyncResult => {
                try {
                    using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                        writer.Write(
                            JsonConvert.SerializeObject(new Command() {
                                Name = "TestPluginsCommandsZeroParameters",
                                Scope = new CommandScope(),
                                Origin = CommandOrigin.Remote,
                                Username = "Phogue",
                                PasswordPlainText = "password"
                            })    
                        );
                    }

                    request.BeginGetResponse(responseAsyncResult => {
                        try {
                            response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult);

                            using (TextReader reader = new StreamReader(response.GetResponseStream())) {
                                isSuccess = true;
                                result = JsonConvert.DeserializeObject<CommandResultArgs>(reader.ReadToEnd());
                                requestWait.Set();
                            }
                        }
                        catch (WebException e) {
                            response = (HttpWebResponse)e.Response;
                            isSuccess = false;
                            requestWait.Set();
                        }
                    }, null);
                }
                catch {
                    isSuccess = false;
                    requestWait.Set();
                }
            }, null);

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(isSuccess);

            Assert.AreEqual("TestPluginsCommandsZeroParameters", result.Message);
        }
    }
}