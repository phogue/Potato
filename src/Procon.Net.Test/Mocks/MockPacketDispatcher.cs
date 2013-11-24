namespace Procon.Net.Test.Mocks {
    public class MockPacketDispatcher : PacketDispatcher {

        public override void Dispatch(Packet packet) {
            base.Dispatch(packet);

            this.Dispatch(new PacketDispatch() {
                Name = packet.Words[0],
                Origin = packet.Origin
            }, packet, packet);
        }
    }
}
