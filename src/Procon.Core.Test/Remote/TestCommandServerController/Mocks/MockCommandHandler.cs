using System;
using Procon.Core.Shared;

namespace Procon.Core.Test.Remote.TestCommandServerController.Mocks {
    /// <summary>
    /// Mock command handler to intercept but not process any commands.
    /// </summary>
    public class MockCommandHandler : CoreController {
        public Action<ICommand> PropogateHandlerCallback { get; set; }

        public override ICommandResult PropogateHandler(ICommand command, CommandDirection direction) {
            if (this.PropogateHandlerCallback != null) this.PropogateHandlerCallback(command);

            return base.PropogateHandler(command, direction);
        }
    }
}
