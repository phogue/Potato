using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Variables;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandData {

        /// <summary>
        /// Tests that all references are removed by emptying and nulling the lists.
        /// </summary>
        [Test]
        public void TestCommandDataDispose() {
            CommandData data = new CommandData() {
                Content = new List<String>(),
                Connections = new List<Connection>(),
                Groups = new List<Group>(),
                Accounts = new List<Account>(),
                Permissions = new List<Permission>(),
                AccountPlayers = new List<AccountPlayer>(),
                Variables = new List<Variable>(),
                Languages = new List<Language>(),
                TextCommands = new List<TextCommand>(),
                TextCommandMatches = new List<TextCommandMatch>(),
                Chats = new List<Chat>(),
                Players = new List<Player>(),
                Kills = new List<Kill>(),
                Spawns = new List<Spawn>(),
                Kicks = new List<Kick>(),
                Bans = new List<Ban>()
            };

            data.Dispose();

            Assert.IsNull(data.Content);
            Assert.IsNull(data.Connections);
            Assert.IsNull(data.Groups);
            Assert.IsNull(data.Accounts);
            Assert.IsNull(data.Permissions);
            Assert.IsNull(data.AccountPlayers);
            Assert.IsNull(data.Variables);
            Assert.IsNull(data.Languages);
            Assert.IsNull(data.TextCommands);
            Assert.IsNull(data.TextCommandMatches);
            Assert.IsNull(data.Chats);
            Assert.IsNull(data.Players);
            Assert.IsNull(data.Kills);
            Assert.IsNull(data.Spawns);
            Assert.IsNull(data.Kicks);
            Assert.IsNull(data.Bans);
        }
    }
}
