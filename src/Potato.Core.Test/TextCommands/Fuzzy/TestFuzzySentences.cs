#region Copyright
// Copyright 2015 Geoff Green.
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
#region

using System.Collections.Generic;
using NUnit.Framework;

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzySentences : TestTextCommandParserBase {
        [Test]
        public void TestSentencesHelloWorldQuoted() {
            AssertCommandSentencesList(CreateTextCommandController(), "test \"Hello World!\"", TextCommandTest, new List<string>() {
                "Hello World!"
            });
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