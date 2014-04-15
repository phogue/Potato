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
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Plugins;
using Potato.Database.Shared.Builders.Methods.Data;

namespace Potato.Examples.Plugins.Database {
    /// <summary>
    /// This plugin shows how to handle database migrations and CRUD on a database.
    /// </summary>
    /// <remarks>
    /// <para>If you only use the query builder and keep your schematics extremely simple then
    /// you'll never need to worry about the underlying type of database being used.</para>
    /// <para>Think Key-Value-Store instead of full fledged database and you'll be golden.</para>
    /// </remarks>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

        public Program() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "SaveOneUser",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.SaveOneUser
                },
                new CommandDispatch() {
                    Name = "FindOneUser",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = this.FindOneUser
                }
            });
        }

        protected ICommandResult SaveOneUser(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            command.Result.CommandResultType = CommandResultType.Success;
            command.Result.Success = true;

            this.Bubble(
                CommandBuilder.DatabaseQuery(
                    new UserModel() {
                        Name = "Phogue",
                        Age = 29
                    }
                    .ToSaveQuery()
                    .Collection("Potato_Example_Database_Users")
                )
            );

            return command.Result;
        }

        /// <summary>
        /// Fetch the first user from the table of users and return their name in the message of the result
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected ICommandResult FindOneUser(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            // Grab the first result from the table.
            ICommandResult fetchResult = this.Bubble(
                CommandBuilder.DatabaseQuery(
                    new Find()
                    .Collection("Potato_Example_Database_Users")
                    .Limit(1)
                )
            );

            UserModel model = UserModel.FirstFromQuery(fetchResult.Now.Queries.FirstOrDefault());

            if (model != null) {
                command.Result.CommandResultType = CommandResultType.Success;
                command.Result.Success = true;
                command.Result.Message = model.Name;
            }

            return command.Result;
        }

        /// <summary>
        /// See the Potato.Examples.Events project for details on why this event handler exists.
        /// </summary>
        /// <param name="e"></param>
        public override void GenericEvent(GenericEvent e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsEnabled) {
                // You don't need to store a reference to this object as it'll only be executed
                // the once then forgotten.
                new Migrations() {
                    BubbleObjects = {
                        // Tell the controller to bubble commands to this object, which will
                        // then eventually pass the commands onto Potato.
                        this
                    }
                }.Execute();

                // That's it. Your migration/tables/collection should be all updated!
            }
        }
    }
}
