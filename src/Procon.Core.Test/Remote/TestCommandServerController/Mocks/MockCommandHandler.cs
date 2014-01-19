using System;
using Procon.Core.Shared;

namespace Procon.Core.Test.Remote.TestCommandServerController.Mocks {
    /// <summary>
    /// Mock command handler to intercept but not process any commands.
    /// </summary>
    public class MockCommandHandler : CoreController {
        public Action<Command> PropogateHandlerCallback { get; set; }

        public override ICommandResult PropogateHandler(Command command, CommandDirection direction) {
            if (this.PropogateHandlerCallback != null) this.PropogateHandlerCallback(command);

            return base.PropogateHandler(command, direction);
        }
    }
}
