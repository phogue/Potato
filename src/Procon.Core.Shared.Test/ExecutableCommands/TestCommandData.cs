#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandData {
        /// <summary>
        ///     Tests that all references are removed by emptying and nulling the lists.
        /// </summary>
        [Test]
        public void TestCommandDataDispose() {
            var data = new CommandData() {
                Content = new List<String>(),
                Connections = new List<ConnectionModel>(),
                Groups = new List<GroupModel>(),
                Accounts = new List<AccountModel>(),
                Permissions = new List<PermissionModel>(),
                AccountPlayers = new List<AccountPlayerModel>(),
                Variables = new List<VariableModel>(),
                Languages = new List<LanguageModel>(),
                TextCommands = new List<TextCommandModel>(),
                TextCommandMatches = new List<TextCommandMatchModel>(),
                Chats = new List<ChatModel>(),
                Players = new List<PlayerModel>(),
                Kills = new List<KillModel>(),
                Spawns = new List<SpawnModel>(),
                Kicks = new List<KickModel>(),
                Bans = new List<BanModel>()
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