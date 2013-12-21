using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core;
using Procon.Core.Connections.Plugins;
using Procon.Core.Events;
using Procon.Database.Serialization.Builders.Methods.Data;

namespace Procon.Examples.Database {
    /// <summary>
    /// This plugin shows how to handle database migrations and CRUD on a database.
    /// </summary>
    /// <remarks>
    /// <para>If you only use the query builder and keep your schematics extremely simple then
    /// you'll never need to worry about the underlying type of database being used.</para>
    /// <para>Think Key-Value-Store instead of full fledged database and you'll be golden.</para>
    /// </remarks>
    public class Program : RemotePlugin {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "InsertOneUser",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.SaveOneUser)
                },
                {
                    new CommandAttribute() {
                        Name = "FindOneUser",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.FindOneUser)
                }
            });
        }

        protected CommandResultArgs SaveOneUser(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result.Status = CommandResultType.Success;
            command.Result.Success = true;

            this.Bubble(
                CommandBuilder.DatabaseQuery(
                    new UserModel() {
                        Name = "Phogue",
                        Age = 29
                    }
                    .ToSaveQuery()
                    .Collection("Procon_Example_Database_Users")
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
        protected CommandResultArgs FindOneUser(Command command, Dictionary<String, CommandParameter> parameters) {
            // Grab the first result from the table.
            CommandResultArgs fetchResult = this.Bubble(
                CommandBuilder.DatabaseQuery(
                    new Find()
                    .Collection("Procon_Example_Database_Users")
                    .Limit(1)
                )
            );

            UserModel model = UserModel.FirstFromQuery(fetchResult.Now.Queries.FirstOrDefault());

            if (model != null) {
                command.Result.Message = model.Name;
            }

            return command.Result;
        }

        /// <summary>
        /// See the Procon.Examples.Events project for details on why this event handler exists.
        /// </summary>
        /// <param name="e"></param>
        public override void GenericEvent(Core.Events.GenericEventArgs e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsPluginEnabled) {
                var y = AppDomain.CurrentDomain;

                // You don't need to store a reference to this object as it'll only be executed
                // the once then forgotten.
                new Migrations() {
                    BubbleObjects = {
                        // Tell the controller to bubble commands to this object, which will
                        // then eventually pass the commands onto Procon.
                        this
                    }
                }.Execute();

                // That's it. Your migration/tables/collection should be all updated!
            }
        }
    }
}
