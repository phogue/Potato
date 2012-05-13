using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using Procon.Core;
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
            // Start Procon.
            InstanceViewModel tProcon = new InstanceViewModel(new Instance());
            InstanceViewModel.PublicProperties["Procon"].Value = tProcon;
            tProcon.Execute();

            // Load the settings file.
            Settings.Load();

            // Create the root element of the UI.
            Window tRoot = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState           = Settings.Get<WindowState>("WindowState", WindowState.Normal),
                Top                   = Settings.Get<Double>("Top",  Double.NaN),
                Left                  = Settings.Get<Double>("Left", Double.NaN),
                Name       = "Root", Title  = "Procon 2",
                MinWidth   = 1024,   Width  = Settings.Get<Double>("Width",  1024),
                MinHeight  = 768,    Height = Settings.Get<Double>("Height", 768)
            };
            if (File.Exists(Defines.PROCON_ICON))
                tRoot.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));

            // Save window settings before the window is disposed.
            tRoot.Closing += (s, e) => {
                Settings.Set("Width",       tRoot.Width);
                Settings.Set("Height",      tRoot.Height);
                Settings.Set("Top",         tRoot.Top);
                Settings.Set("Left",        tRoot.Left);
                Settings.Set("WindowState", tRoot.WindowState);
            };

            // Load the extensions.
            Settings.ExecuteExtensions(tRoot);

            // Set the Data Context and display the window.
            tRoot.DataContext = tProcon;
            tRoot.ShowDialog();

            // Save the settings file.
            Settings.Save();

            // Shutdown Procon.
            tProcon.Shutdown();
        }
    }
}
