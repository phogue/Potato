using System.Collections.Generic;
using Procon.Net.Actions;

namespace Procon.Net.Test.Mocks.Game {

    public class MockActionChatNullResultGame : MockGame {
        public MockActionChatNullResultGame(string hostName, ushort port)
            : base(hostName, port) {
        }

        protected override List<IPacket> Action(Chat chat) {
            return new List<IPacket>() {
                null
            };
        }
    }
}
