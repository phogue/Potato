using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Procon.Setup {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            var mainWindow = new MainWindow();
            MainWindowViewModel context = new MainWindowViewModel();

            if (context.CreateInstance() == true) {
                context.RefreshInstance();
            }

            mainWindow.DataContext = context;
            mainWindow.Show();
        }
    }
}
