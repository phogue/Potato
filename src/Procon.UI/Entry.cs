using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

using Procon.Core;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;
using Procon.UI.Extensions;

namespace Procon.UI
{
    // Builds UI by loading in extensions at runtime.
    // Manage which extensions get loaded by running the Ui Manager.
    internal class Entry
    {
        [STAThread]
        static void Main(String[] args)
        {
            // Start Procon.
            InstanceViewModel Procon = new InstanceViewModel(new Instance());
            InstanceViewModel.PublicProperties["Procon"].Value = Procon;
            Procon.Execute();

            // Loads the settings file.
            Settings.Load();

            // Create the root element of the UI.
            Window root = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState           = Settings.Get<WindowState>("WindowState", WindowState.Normal),
                Name       = "Root", Title  = "Procon 2",
                MinWidth   = 900,    Width  = Settings.Get<Double>("Width",  1024),
                MinHeight  = 650,    Height = Settings.Get<Double>("Height", 768)
            };
            if (File.Exists(Defines.PROCON_ICON))
                root.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));

            // Save window settings before the window is disposed.
            root.Closing += (s, e) => {
                Settings.Set("Width",       root.Width);
                Settings.Set("Height",      root.Height);
                Settings.Set("WindowState", root.WindowState);
            };

            // Load the extensions into the UI.
            ExtensionController.ReadConfig(Path.Combine(Defines.EXTENSIONS_DIRECTORY, Defines.EXTENSIONS_CONFIG), root);

            // Load some settings related to the state of the program.
            if (!Settings.Get<Boolean>("IsLocalInterface", true))
                foreach (InterfaceViewModel inter in Procon.Interfaces)
                    if (Settings.Get<String>("InterfaceHostname", null) == inter.Hostname && Settings.Get<UInt16>("InterfacePort", UInt16.MaxValue) == inter.Port)
                        InstanceViewModel.PublicProperties["Interface"].Value = inter;

            // Set the Data Context and display the window.
            root.DataContext = Procon;
            root.ShowDialog();

            // Save some settings related to the state of the program.
            Settings.Set("IsLocalInterface",  (InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel).IsLocal);
            Settings.Set("InterfaceHostname", (InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel).Hostname);
            Settings.Set("InterfacePort",     (InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel).Port);
            Settings.Save();

            // Shutdown Procon.
            Procon.Shutdown();
        }
    }
}
