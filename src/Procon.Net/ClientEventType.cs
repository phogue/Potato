using System;

namespace Procon.Net {

    [Serializable]
    public enum ClientEventType {
        ClientConnectionStateChange,
        ClientConnectionFailure,
        ClientSocketException,
        ClientPacketSent,
        ClientPacketReceived
    }
}
