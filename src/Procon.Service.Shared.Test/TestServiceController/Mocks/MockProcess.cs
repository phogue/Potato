namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
    public class MockProcess : IProcess {
        public bool OnKill { get; set; }

        public void Kill() {
            this.OnKill = true;
        }
    }
}
