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
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestDispatchIdentityCheck {
        /// <summary>
        /// Tests that the command authentication will succeed against itself
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameIsIdentical() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "Phogue");

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that the command authentication will not succeed against another account
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameIsNotIdentical() {
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
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Ike"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "Ike");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the target account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameFailsOnTargetAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "AccountDoesNotExist");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the source account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameFailsOnSourceAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    Username = "AccountDoesNotExist"
                }
            }, "Phogue");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the command authentication will succeed against itself
        /// </summary>
        [Test]
        public void TestCheckByAccountPlayerIsIdentical() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
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
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "ABCDEF"
                }
            }, CommonProtocolType.DiceBattlefield3, "ABCDEF");

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that the command authentication will not succeed against another account
        /// </summary>
        [Test]
        public void TestCheckByAccountPlayerIsNotIdentical() {
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
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Ike"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Ike",
                    CommonProtocolType.DiceBattlefield3,
                    "ZYXWVUT"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "ABCDEF"
                }
            }, CommonProtocolType.DiceBattlefield3, "ZYXWVUT");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the target account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountPlayerFailsOnTargetAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
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
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "ABCDEF"
                }
            }, CommonProtocolType.DiceBattlefield3, "DoesNotExist");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the source account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountPlayerFailsOnSourceAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            // Now add a user.
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
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            ICommandResult result = security.DispatchIdentityCheck(new Command() {
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "DoesNotExist"
                }
            }, CommonProtocolType.DiceBattlefield3, "ABCDEF");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }
    }
}