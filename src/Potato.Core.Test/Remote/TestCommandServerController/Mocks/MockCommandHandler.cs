#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using Potato.Core.Shared;

namespace Potato.Core.Test.Remote.TestCommandServerController.Mocks {
    /// <summary>
    /// Mock command handler to intercept but not process any commands.
    /// </summary>
    public class MockCommandHandler : CoreController {
        public Action<ICommand> PropogateHandlerCallback { get; set; }

        public override ICommandResult PropogateHandler(ICommand command, CommandDirection direction) {
            if (PropogateHandlerCallback != null) PropogateHandlerCallback(command);

            return base.PropogateHandler(command, direction);
        }
    }
}
