using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Database;
using Procon.Core.Variables;
using Procon.Net.Utils;

namespace Procon.Core.Test.Database {
    [TestFixture]
    public class TestDatabaseController {

        [Test]
        public void TestSqLiteMemoryDatabaseOpened() {
            String configGroupName = StringExtensions.RandomString(10);

            VariableController variables = new VariableController();

            DatabaseController database = new DatabaseController() {
                Variables = variables
            }.Execute() as DatabaseController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(configGroupName, CommonVariableNames.DatabaseDriverName), "SQLite");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(configGroupName, CommonVariableNames.DatabaseMemory), true);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.DatabaseConfigGroups, new List<String>() {
                configGroupName
            });

            Assert.AreEqual(1, database.OpenDrivers.Count);
        }

    }
}
