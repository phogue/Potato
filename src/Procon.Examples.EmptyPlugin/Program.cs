using Procon.Core.Shared.Plugins;

namespace Procon.Examples.EmptyPlugin {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    /// <remarks>
    /// This is deliberately empty as this is the minimal code to run by Procon.
    /// </remarks>
    public class Program : RemotePlugin {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

    }
}
