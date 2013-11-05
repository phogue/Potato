using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzySentences : TestFuzzyBase {
        [TestMethod]
        public void TestSentencesHelloWorldQuoted() {
            TestFuzzyBase.AssertCommandSentencesList(
                this.CreateTextCommandController(), 
                "test \"Hello World!\"",
                TestFuzzyBase.TextCommandTest,
                new List<string>() {
                    "Hello World!"
                }
            );
        }

        /*
        [TestMethod]
        public void TestSentencesHelloWorldUnquoted() {
            this.AssertCommandSentencesList(
                "test Hello World!",
                TestNlpBase.TextCommandTest,
                new List<string>() {
                    "Hello World!"
                }
            );
        }
        */

        /*
        [TestMethod]
        public void TestSentencesKickPhogueForTeamKilling() {
            CommandResult args = this.ExecuteTextCommand("kick phogue for team killing");

            this.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>()
            );

            this.AssertExecutedCommandAgainstSentencesList(
                args,
                TestNlpBase.TextCommandKick,
                new List<string>() {
                    "team killing"
                }
            );
        }
        */
    }
}
