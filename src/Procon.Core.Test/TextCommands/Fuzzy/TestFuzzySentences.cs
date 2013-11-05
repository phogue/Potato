using System.Collections.Generic;
using NUnit.Framework;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzySentences : TestFuzzyBase {
        [Test]
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
        [Test]
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
        [Test]
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
