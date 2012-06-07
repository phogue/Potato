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
            // Start Procon and load the settings file.
            InstanceViewModel tProcon = new InstanceViewModel(new Instance());
            InstanceViewModel.PublicProperties["Procon"].Value = tProcon;
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
                WindowStartupLocation = WindowStartupLocation.Manual,
            };
            if (File.Exists(Defines.PROCON_ICON))
                tRoot.Icon = new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute));

            // Save window settings before the window closes.
            tRoot.Closing += (s, e) => {
                Settings.Set("Top",         tRoot.Top);
                Settings.Set("Left",        tRoot.Left);
                Settings.Set("Width",       tRoot.Width);
                Settings.Set("Height",      tRoot.Height);
                Settings.Set("WindowState", tRoot.WindowState);
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
