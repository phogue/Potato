using System;
using System.Windows;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandArgs
    {
        public Object          Sender    { get; private set; }
        public RoutedEventArgs Args      { get; private set; }
        public Object          Parameter { get; private set; }

        // Wraps sender, event args, and an optional parameter for attached commands.
        public AttachedCommandArgs(Object sender, RoutedEventArgs args, Object param)
        {
            Sender    = sender;
            Args      = args;
            Parameter = param;
        }
    }
}
