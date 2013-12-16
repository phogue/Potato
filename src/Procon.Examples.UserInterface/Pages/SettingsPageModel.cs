using System;
using System.Collections.Generic;
using Procon.Net.Models;

namespace Procon.Examples.UserInterface.Pages {
    public partial class SettingsPageView {

        /// <summary>
        /// Just a string to output in the template. Shows how to use variables within the template.
        /// </summary>
        public String MyStringyVariable { get; set; }

        /// <summary>
        /// A list of players to output, showing how to loop within a template (therefore how to use conditions and what not as well)
        /// </summary>
        public IList<Player> MyListOfPlayers { get; set; }

        public SettingsPageView() : base() {
            this.MyStringyVariable = "Empty";

            this.MyListOfPlayers = new List<Player>();
        }
    }
}
