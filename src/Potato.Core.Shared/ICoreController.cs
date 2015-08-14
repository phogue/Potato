#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using System.Collections.Generic;

namespace Potato.Core.Shared {
    /// <summary>
    /// The implementing object accepts command execution
    /// </summary>
    public interface ICoreController : IDisposable {
        /// <summary>
        /// List of commands this controller will handle/dispatch.
        /// </summary>
        List<ICommandDispatch> CommandDispatchers { get; set; } 

        /// <summary>
        /// All objects to tunnel downwards during execution
        /// </summary>
        List<ICoreController> TunnelObjects { get; set; }

        /// <summary>
        /// All objects to bubble upwards during execution
        /// </summary>
        List<ICoreController> BubbleObjects { get; set; }

        /// <summary>
        /// Run a preview of a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the preview. A handler may have canceled the command.</returns>
        ICommandResult PropogatePreview(ICommand command, CommandDirection direction);

        /// <summary>
        /// Run a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the execution.</returns>
        ICommandResult PropogateHandler(ICommand command, CommandDirection direction);

        /// <summary>
        /// Alert the object that a command has been executed with the following results
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the executed.</returns>
        ICommandResult PropogateExecuted(ICommand command, CommandDirection direction);

        /// <summary>
        /// Execute a command, then tunnel it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandResult Tunnel(ICommand command);

        /// <summary>
        /// Execute a command, then bubble it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandResult Bubble(ICommand command);

        /// <summary>
        /// Call after the constructor is called to setup events with any assigned properties
        /// </summary>
        ICoreController Execute();

        /// <summary>
        /// Appends config items
        /// </summary>
        void WriteConfig(IConfig config, string password = null);

        /// <summary>
        /// A set interval Poke
        /// </summary>
        void Poke();
    }
}
