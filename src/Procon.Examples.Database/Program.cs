using Procon.Core.Connections.Plugins;
using Procon.Core.Events;

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

        /// <summary>
        /// See the Procon.Examples.Events project for details on why this event handler exists.
        /// </summary>
        /// <param name="e"></param>
        public override void GenericEvent(Core.Events.GenericEventArgs e) {
            base.GenericEvent(e);

            if (e.GenericEventType == GenericEventType.PluginsPluginEnabled) {
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
