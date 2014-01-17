using System.Collections.Generic;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Test.Mocks.Game {

    public class MockActionChatNullResultGame : MockGame {
        public MockActionChatNullResultGame(string hostName, ushort port)
            : base(hostName, port) {
        }

        protected override List<IPacketWrapper> DispatchAction(NetworkAction action) {
            return new List<IPacketWrapper>() {
                null
            };
        }
    }
}
