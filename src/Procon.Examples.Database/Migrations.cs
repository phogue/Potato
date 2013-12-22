using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Database.Migrations;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods.Schema;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Utils;

namespace Procon.Examples.Database {
    /// <summary>
    /// We are only wrapping the MigrationController here in another executable object
    /// so we can keep Program.cs cleaner. All of this can be done inside Program.cs
    /// if you want.
    /// </summary>
    public class Migrations : ExecutableBase {

        protected MigrationController MigrationController { get; set; }

        public override ExecutableBase Execute() {
            // 1. Create our migration controller 
            this.MigrationController = new MigrationController() {
                // 2. Tell the migration controller where it should send it's commands.
                BubbleObjects = {
                    // Pipe all commands to this controller, which is in turn passed onto Program.cs
                    // and eventually to Procon Database controller.
                    this
                },
                // 3. Setup a stream for us to track changes on.
                Settings = new MigrationSettings() {
                    Origin = MigrationOrigin.Plugin,
                    // This name should be unique to your plugin. You could even use your plugin GUID,
                    // but you might want to combine your guid with the connection guid, if you require
                    // migrations on a per-plugin, per-connection basis (a store for each connection)
                    Name = "Procon.Examples.Database"
                },
                // 4. Migrations are run in order in this list. You should append database changes onto
                // this chain *whenever* you make changes.
                // If you always ADD to this list then every release you have will automatically update
                // the database schematic, if the database being used requires a schematic.
                // A single migration is just executable code, so you can do any number of commands within
                // this migration. You could even do file structure changes in here if you wanted.
                Migrations = {
                    // Version 1.0.0.0
                    new Migration() {
                        // Up, moving upstream in changes.
                        Up = () => {
                            CommandResultArgs result = this.Bubble(
                                CommandBuilder.DatabaseQuery(
                                    new Create()
                                    // You should namespace/prefix your collections/tables to avoid clashes with other plugins
                                    .Collection("Procon_Example_Database_Users")
                                    .Modifier(new IfNotExists())
                                    .Field("Name", 255)
                                    .Field("Stamp", new DateTimeType())
                                )
                            );

                            return result.Now.Queries.First().DescendantsAndSelf<Error>().Any() == false;
                        },
                        // Down, moving downstream in changes (do the opposite of Up)
                        Down = () => {
                            // Including Down is not critical when moving upstream, but it's good
                            // practice to include Down. We may in the future include plugin uninstalling
                            // and down migrations would allow the database to be uninstalled.
                            CommandResultArgs result = this.Bubble(
                                CommandBuilder.DatabaseQuery(
                                    new Drop()
                                    .Collection("Procon_Example_Database_Users")
                                )
                            );

                            return result.Now.Queries.First().DescendantsAndSelf<Error>().Any() == false;
                        }
                    },
                    // Pretend you released 1.0.0.0, then released an update later
                    // Version 1.0.0.1
                    new Migration() {
                        Up = () => {
                            // Add another field to our example
                            CommandResultArgs result = this.Bubble(
                                CommandBuilder.DatabaseQuery(
                                    new Alter()
                                    .Collection("Procon_Example_Database_Users")
                                    .Method(
                                        new Create()
                                        .Field("Age", new IntegerType())
                                    )
                                )
                            );

                            return result.Now.Queries.First().DescendantsAndSelf<Error>().Any() == false;
                        },
                        Down = () => {
                            // Drop the field, so people can revert this migration or uninstall can go
                            // through the motions.
                            CommandResultArgs result = this.Bubble(
                                CommandBuilder.DatabaseQuery(
                                    new Alter()
                                    .Collection("Procon_Example_Database_Users")
                                    .Method(
                                        new Drop()
                                        .Field("Age", new IntegerType())
                                    )
                                )
                            );

                            return result.Now.Queries.First().DescendantsAndSelf<Error>().Any() == false;
                        }
                    }
                }
            };

            // 5. This will setup the basic migration tables if they do not exists already (your plugin
            // might be first to use migrations). You should always call Execute on a newly formed ExecutableBase anyway.
            this.MigrationController.Execute();

            // 6. Now run through the migrations until we are at the latest version.
            this.MigrationController.Up();

            return base.Execute();
        }
    }
}
