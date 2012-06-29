using System;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandArgs
    {
        public Object Sender    { get; private set; }
        public Object Args      { get; private set; }
        public Object Parameter { get; private set; }

        // Wraps sender, event args, and an optional parameter for attached commands.
        public AttachedCommandArgs(Object sender, Object args, Object param)
        {
            Sender    = sender;
            Args      = args;
            Parameter = param;
        }
    }
}
