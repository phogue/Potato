using System;

namespace Procon.Core.Shared.Test.AsynchronousExecutableCommands.Mocks {
    public class MockAsynchronousCoreController : Shared.AsynchronousCoreController {

        public Action ExecuteQueuedCommandsFinished { get; set; }

        public override void ExecuteQueuedCommands() {
            base.ExecuteQueuedCommands();

            if (ExecuteQueuedCommandsFinished != null) {
                this.ExecuteQueuedCommandsFinished();
            }
        }
    }
}
