namespace Procon.Net.Test.Mocks {
    public class MockPacketDispatcher : PacketDispatcher {

        public override void Dispatch(IPacketWrapper wrapper) {
            base.Dispatch(wrapper);

            this.Dispatch(new PacketDispatch() {
                Name = wrapper.Packet.Words[0],
                Origin = wrapper.Packet.Origin
            }, wrapper, wrapper);
        }
    }
}
