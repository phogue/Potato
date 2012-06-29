using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Procon.Core;
using Procon.UI.API;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI
{
    // Builds UI by loading in extensions at runtime.
    // Manage which extensions get loaded by running the Ui Manager.
    internal class Entry
    {
        [STAThread]
        static void Main(String[] args)
        {
            // Start Procon and load the settings file.
            InstanceViewModel tProcon = new InstanceViewModel(new Instance());
            ExtensionApi.Properties["Procon"].Value = tProcon;
            tProcon.Execute();
            Settings.Load();

            // Create the root element of the UI.
            Window tRoot = new Window()
            {
                Name  = "Root",
                Title = "Procon 2",
                Top    = Settings.Get<Double>("Top", Double.NaN),
                Left   = Settings.Get<Double>("Left", Double.NaN),
                Width  = Settings.Get<Double>("Width",  1024),
                Height = Settings.Get<Double>("Height", 768),
                MinWidth    = 1024,
                MinHeight   = 768,
                DataContext = tProcon,
                WindowState           = Settings.Get<WindowState>("WindowState", WindowState.Normal),
                WindowStartupLocation = WindowStartupLocation.Manual
            };
            if (File.Exists(Defines.PROCON_ICON))
                tRoot.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));
            TextOptions.SetTextFormattingMode(tRoot, TextFormattingMode.Display);

            // Save window settings before the window closes.
            tRoot.Closing += (s, e) => {
                Settings.Set("Top",         tRoot.Top);
                Settings.Set("Left",        tRoot.Left);
                Settings.Set("Width",       tRoot.Width);
                Settings.Set("Height",      tRoot.Height);
                Settings.Set("WindowState", tRoot.WindowState);
            };

            // Setup some setting management.
            ExtensionApi.Properties["Interface"].PropertyChanged +=
            (s, e) => {
                ExtensionApi.Settings["InterfaceType"].Value = (ExtensionApi.Interface != null) ? (Object)ExtensionApi.Interface.IsLocal  : null;
                ExtensionApi.Settings["InterfaceHost"].Value = (ExtensionApi.Interface != null) ? (Object)ExtensionApi.Interface.Hostname : null;
                ExtensionApi.Settings["InterfacePort"].Value = (ExtensionApi.Interface != null) ? (Object)ExtensionApi.Interface.Port     : null;
            };
            ExtensionApi.Properties["Connection"].PropertyChanged +=
            (s, e) => {
                ExtensionApi.Settings["ConnectionType"].Value = (ExtensionApi.Connection != null) ? (Object)ExtensionApi.Connection.GameType : null;
                ExtensionApi.Settings["ConnectionHost"].Value = (ExtensionApi.Connection != null) ? (Object)ExtensionApi.Connection.Hostname : null;
                ExtensionApi.Settings["ConnectionPort"].Value = (ExtensionApi.Connection != null) ? (Object)ExtensionApi.Connection.Port     : null;
            };

            // Load the extensions, show the window, then save the settings.
            Settings.ExecuteExtensions(tRoot);
            tRoot.ShowDialog();
            Settings.Save();

            // Shutdown Procon.
            tProcon.Shutdown();
        }
    }
}
