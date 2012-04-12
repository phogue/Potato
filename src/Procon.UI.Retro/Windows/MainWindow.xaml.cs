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
using Procon.Net.Protocols.Objects;
using System.Windows.Controls.Primitives;

namespace Procon.UI.Retro.Windows
{
    public partial class MainWindow : Window
    {
        public List<Player> Players { get; set; }
        public List<Map> CurMaps { get; set; }
        public List<Map> AvaMaps { get; set; }
        public List<Ban> Bans { get; set; }

        public MainWindow()
        {
            Players = new List<Player>()
            {
                new Player() { Name = "Killamanjaro", Score = 1000 },
                new Player() { Name = "Bob", Score = 20 },
                new Player() { Name = "Thoraxe The All Mighty", Score = 200 },
                new Player() { Name = "Newblet", Score = -540 },
                new Player() { Name = "Weeeeee", Score = 650 },
                new Player() { Name = "H4x0r", Score = 9001 },
                new Player() { Name = "Three Little Pigs", Score = 333 }
            };
            CurMaps = new List<Map>()
            {
                new Map() { FriendlyName = "Map 1!", Rounds = 3 },
                new Map() { FriendlyName = "Map 3!", Rounds = 1 },
                new Map() { FriendlyName = "Map 5!", Rounds = 6 }
            };
            AvaMaps = new List<Map>()
            {
                new Map() { FriendlyName = "Map 1!", Rounds = 2 },
                new Map() { FriendlyName = "Map 2!", Rounds = 2 },
                new Map() { FriendlyName = "Map 3!", Rounds = 2 },
                new Map() { FriendlyName = "Map 4!", Rounds = 2 },
                new Map() { FriendlyName = "Map 5!", Rounds = 2 },
                new Map() { FriendlyName = "Map 6!", Rounds = 2 }
            };
            Bans = new List<Ban>()
            {
                new Ban() { Target = Players[0], Reason = "I don't like your face", Time = new TimeSubset() { Context = TimeSubsetContext.Permanent, Length = TimeSpan.FromHours(10) } },
                new Ban() { Target = Players[5], Reason = "A Hacker", Time = new TimeSubset() { Context = TimeSubsetContext.Permanent, Length = TimeSpan.FromHours(20) } },
                new Ban() { Target = Players[4], Reason = "Stupid Name", Time = new TimeSubset() { Context = TimeSubsetContext.Permanent, Length = TimeSpan.FromHours(30) } }
            };

            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as ToggleButton).Width = Double.NaN;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as ToggleButton).Width = 18;
        }
    }
}
