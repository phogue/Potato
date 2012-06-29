using System;
using System.Windows;

using Procon.UI.API;

namespace Procon.UI
{
    public class Extension
    {
        // Accessors & Mutators for public variables.
        public IExtension IExtension { get; protected set; }
        public Boolean    IsExecuted { get; protected set; }

        // Constructors.
        public Extension(IExtension extension)
        {
            IExtension = extension;
            IsExecuted = false;
        }
        public Extension(Extension extension)
        {
            this.IExtension = extension.IExtension;
            this.IsExecuted = extension.IsExecuted;
        }

        // Executes the extension.
        public void Execute(Window root)
        {
            try   { IsExecuted = IExtension.Entry(root); }
            catch { IsExecuted = false; }
        }
    }
}
