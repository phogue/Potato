using System;
using System.Windows.Input;

using Procon.UI.API.Classes;

namespace Procon.UI.API.ViewModels
{
    // The basics of what a view model should have.
    public abstract class ViewModelBase
    {
        // Public properties and commands available to the UI.
        public static InfinityDictionary<String, Object>   PublicProperties { get; set; }
        public static InfinityDictionary<String, ICommand> PublicCommands   { get; set; }
        
        // Initializes the PublicProperties and PublicCommands.
        static ViewModelBase()
        {
            if (PublicProperties == null)
                PublicProperties = new InfinityDictionary<String, Object>();
            if (PublicCommands == null)
                PublicCommands   = new InfinityDictionary<String, ICommand>();
        }
    }
}
