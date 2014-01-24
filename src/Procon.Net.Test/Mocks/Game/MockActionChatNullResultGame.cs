using System.Collections.Generic;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Test.Mocks.Game {

    public class MockActionChatNullResultGame : MockGame {
        protected override List<IPacketWrapper> DispatchAction(INetworkAction action) {
            return new List<IPacketWrapper>() {
                null
            };
        }
    }
}
