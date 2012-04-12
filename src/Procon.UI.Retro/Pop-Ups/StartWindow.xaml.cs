using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Procon.UI.Retro.Windows;

namespace Procon.UI.Retro.Pop_Ups
{
    public partial class StartWindow : Window
    {
        public StartWindow() { InitializeComponent(); }
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (start_Local.IsChecked.Value)
                new MainWindow().Show();
            else
                new CreateInterface().Show();
            Close();
        }
    }
}
