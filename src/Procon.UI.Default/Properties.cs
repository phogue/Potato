// Copyright 2011 Cameron 'Imisnew2' Gunnin
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Enums;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Properties : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Default Properties"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Setup the properties used by the (default) UI.
            SetupInstanceLevel();
            SetupInterfaceLevel();
            SetupConnectionLevel();
            SetupImages();

            // We done here broski.
            return true;
        }

        /// <summary>Easy Getter/Setter for [Procon] property.</summary>
        private InstanceViewModel ActiveInstance
        {
            get { return InstanceViewModel.PublicProperties["Procon"].Value as InstanceViewModel; }
            set { InstanceViewModel.PublicProperties["Procon"].Value = value; }
        }
        /// <summary>Easy Getter/Setter for [Interface] property.</summary>
        private InterfaceViewModel ActiveInterface
        {
            get { return InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel; }
            set { InstanceViewModel.PublicProperties["Interface"].Value = value; }
        }
        /// <summary>Easy Getter/Setter for [Connection] property.</summary>
        private ConnectionViewModel ActiveConnection
        {
            get { return InstanceViewModel.PublicProperties["Connection"].Value as ConnectionViewModel; }
            set { InstanceViewModel.PublicProperties["Connection"].Value = value; }
        }

        /// <summary>Sets up the [Instance] properties.</summary>
        private void SetupInstanceLevel()
        {
            /* [Panel] - The type of panel to display to the user (null = dashboard). */
            InstanceViewModel.PublicProperties["Panel"].Value = null;
        }
        /// <summary>Sets up the [Interface] properties.</summary>
        private void SetupInterfaceLevel()
        {
            /* [Interface] - The currently active interface (used to perform interface actions). */
            ActiveInterface = ActiveInstance.Interfaces.FirstOrDefault(x => x.IsLocal);

            /* [Add] - Contains information necessary to connect to a layer.
             *   [Hostname] - The Hostname/IP of the layer.
             *   [Port]     - The Port of the layer.
             *   [Username] - The user to login as.
             *   [Password] - The password for the user specified. */
            InstanceViewModel.PublicProperties["Interface"]["Add"]["Hostname"].Value = String.Empty;
            InstanceViewModel.PublicProperties["Interface"]["Add"]["Port"].Value     = String.Empty;
            InstanceViewModel.PublicProperties["Interface"]["Add"]["Username"].Value = String.Empty;
            InstanceViewModel.PublicProperties["Interface"]["Add"]["Password"].Value = String.Empty;
        }
        /// <summary>Sets up the [Connection] properties.</summary>
        private void SetupConnectionLevel()
        {
            /* [Connection] - The currently active connection (used to perform connection actions). */
            ActiveConnection = ActiveInterface.Connections.FirstOrDefault(x => true);

            /* [Add] - Contains information necessary to connect to a game server via rcon.
             *   [Type]       - The Type of game server (BFBC2, TF2, etc).
             *   [Hostname]   - The Hostname/IP of the game server.
             *   [Port]       - The Port of the game server.
             *   [Password]   - The rcon password for the game server.
             *   [Additional] - *Optional* Other information required for a connection. */
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Type"].Value       = GameType.BF_BC2;
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Hostname"].Value   = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Port"].Value       = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Password"].Value   = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Additional"].Value = String.Empty;

            /* [Filter][Chat] - Contains information necessary to filter chat/event messages.
             *   [Data]  - The text to filter by.
             *   [Type]  - The method used to filter with.
             *   [Field] - The data to filter on. */
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Data"].Value  = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Type"].Value  = FilterType.Contains;
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Field"].Value = FilterChatField.Data;

            /* [Filter][Ban] - Contains information necessary to filter through bans.
             *   [Data]  - The text to filter by.
             *   [Type]  - The method used to filter with.
             *   [Field] - The data to filter on. */
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Ban"]["Data"].Value  = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Ban"]["Type"].Value  = FilterType.Contains;
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Ban"]["Field"].Value = FilterBanField.Id;

            /* [Action][Chat] - Contains information necessary to send a message to a game server.
             *   [Type]     - How to display the text.
             *   [Subset]   - Who to display the text to.
             *     [Team]   - Which team to display the text to.
             *     [Squad]  - Which squad to display the text to.
             *     [Player] - Which player to display the text to.
             *   [Data]     - The text to send. */
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Type"].Value             = ChatActionType.Say;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"].Value           = PlayerSubsetContext.All;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value   = Team.Team1;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value  = Squad.None;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value = null;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value             = String.Empty;

            /* [Action][Player] - Contains information necessary to perform player administrative actions.
             *   [Type]        - The type of player action to perform.
             *   [Move][Team]  - If moving player, the team to move them to.
             *   [Move][Squad] - If moving player, the squad to move them to.
             *   [Ban][Time]   - If banning player, the time context to ban them for.
             *   [Ban][Length] - If banning player, the time length to ban them for.
             *   [Reason]      - Why the action is being performed. */
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Type"].Value          = ActionPlayerType.Kill;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Team"].Value  = Team.Team1;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value = Squad.Squad1;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value = "1:00";
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value        = String.Empty;

            /* [Action][Map] - Contains the information necessary to perform map administrative actions.
             *   [Mode]  - UNSURE AS OF YET.
             *   [Round] - The number of rounds a map should be added for. */
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Map"]["Mode"].Value  = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Map"]["Round"].Value = "2";

            /* [Action][Ban] - Contains information necessary to perform ban administrative actions.
             *   [Type]   - The type of ban action to perform.
             *   [Uid]    - If banning player, the unique identifier of the player to ban.
             *   [Time]   - If banning, the time context to ban for.
             *   [Length] - If banning or to temp., the time length to ban them for.
             *   [Reason] - If banning, why the action is being performed. */
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Type"].Value   = ActionBanType.Ban;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value    = String.Empty;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value = "1:00";
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Reason"].Value = String.Empty;




            // TYPES - Enumerations used for various reasons within the UI. //
            // ------------------------------------------------------------ //
            // Valid Game Types of connections that can be created.
            InstanceViewModel.PublicProperties["Connection"]["Add"]["Types"].Value  = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);

            // Valid Filter Methods and Chat Fields that can be used to filter and filter on, respectively.
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Fields"].Value = Enum.GetValues(typeof(FilterChatField)).Cast<FilterChatField>().Where(x => true);

            // Valid Filter Methods and Ban Fields that can be used to filter and filter on, respectively.
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Ban"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Filter"]["Ban"]["Fields"].Value = Enum.GetValues(typeof(FilterBanField)).Cast<FilterChatField>().Where(x => true);

            // Valid Methods to display a chat message and subsets to send a chat message to.
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Types"].Value   = Enum.GetValues(typeof(ActionChatType)).Cast<ActionChatType>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subsets"].Value = Enum.GetValues(typeof(PlayerSubsetContext)).Cast<PlayerSubsetContext>().Where(x => (x != PlayerSubsetContext.Server));
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Teams"].Value   = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Squads"].Value  = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);

            // Valid Player Actions to take, and selections for various player actions.
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Types"].Value          = Enum.GetValues(typeof(ActionPlayerType)).Cast<ActionPlayerType>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Teams"].Value  = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squads"].Value = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Times"].Value   = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None));

            // Valid Ban Time Contexts for banning players.
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Types"].Value = Enum.GetValues(typeof(ActionBanType)).Cast<ActionBanType>().Where(x => true);
            InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Times"].Value = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None) && (x != TimeSubsetContext.Round));
        }
        /// <summary>Sets up the [Images] properties.</summary>
        private void SetupImages()
        {
            /* [Procon] - Procon images.
             *   [Icon] - The Procon Image by itself.
             *   [Logo] - The Procon Image with "Procon 2" text attached. */
            InstanceViewModel.PublicProperties["Images"]["Procon"]["Icon"].Value = (File.Exists(Defines.PROCON_ICON)) ? new BitmapImage(new Uri(Defines.PROCON_ICON, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Procon"]["Logo"].Value = (File.Exists(Defines.PROCON_LOGO)) ? new BitmapImage(new Uri(Defines.PROCON_LOGO, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            /* [Games] - Represents a game.
             *   [BF_BC2]    - Battlefield: Bad Company 2. 
             *   [BF_3]      - Battlefield 3.
             *   [COD_BO]    - Call of Duty: Black Ops.
             *   [HOMEFRONT] - Homefront.
             *   [MOH_2010]  - Medal of Honor: 2010.
             *   [TF_2]      - Team Fortress 2. */
            InstanceViewModel.PublicProperties["Images"]["Games"]["BF_BC2"].Value    = (File.Exists(Defines.GAME_BF_BC2))    ? new BitmapImage(new Uri(Defines.GAME_BF_BC2, UriKind.RelativeOrAbsolute))    : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Games"]["BF_3"].Value      = (File.Exists(Defines.GAME_BF_3))      ? new BitmapImage(new Uri(Defines.GAME_BF_3, UriKind.RelativeOrAbsolute))      : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Games"]["COD_BO"].Value    = (File.Exists(Defines.GAME_COD_BO))    ? new BitmapImage(new Uri(Defines.GAME_COD_BO, UriKind.RelativeOrAbsolute))    : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Games"]["HOMEFRONT"].Value = (File.Exists(Defines.GAME_HOMEFRONT)) ? new BitmapImage(new Uri(Defines.GAME_HOMEFRONT, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Games"]["MOH_2010"].Value  = (File.Exists(Defines.GAME_MOH_2010))  ? new BitmapImage(new Uri(Defines.GAME_MOH_2010, UriKind.RelativeOrAbsolute))  : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Games"]["TF_2"].Value      = (File.Exists(Defines.GAME_TF_2))      ? new BitmapImage(new Uri(Defines.GAME_TF_2, UriKind.RelativeOrAbsolute))      : new BitmapImage();

            /* [Connection] - Represents an Action or State dealing with Connections.
             *   [Add]    - Create/Add a connection.
             *   [Edit]   - Change/Edit a connection.
             *   [Remove] - Delete/Remove a connection.
             *   [Good] - The connection is connected.
             *   [Flux] - The connection is connecting.
             *   [Bad]  - The connection is disconnected. */
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Add"].Value    = (File.Exists(Defines.CONN_ADD))    ? new BitmapImage(new Uri(Defines.CONN_ADD, UriKind.RelativeOrAbsolute))    : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Edit"].Value   = (File.Exists(Defines.CONN_EDIT))   ? new BitmapImage(new Uri(Defines.CONN_EDIT, UriKind.RelativeOrAbsolute))   : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Remove"].Value = (File.Exists(Defines.CONN_REMOVE)) ? new BitmapImage(new Uri(Defines.CONN_REMOVE, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Good"].Value   = (File.Exists(Defines.CONN_GOOD)) ? new BitmapImage(new Uri(Defines.CONN_GOOD, UriKind.RelativeOrAbsolute))     : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Flux"].Value   = (File.Exists(Defines.CONN_FLUX)) ? new BitmapImage(new Uri(Defines.CONN_FLUX, UriKind.RelativeOrAbsolute))     : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Connection"]["Bad"].Value    = (File.Exists(Defines.CONN_BAD))  ? new BitmapImage(new Uri(Defines.CONN_BAD, UriKind.RelativeOrAbsolute))      : new BitmapImage();

            /* [Menu] - Represents Actions specific to the UI.
             *   [Home]     - View the home page. 
             *   [Settings] - View the settings page. */
            InstanceViewModel.PublicProperties["Images"]["Menu"]["Home"].Value     = (File.Exists(Defines.MENU_HOME))     ? new BitmapImage(new Uri(Defines.MENU_HOME, UriKind.RelativeOrAbsolute))     : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Menu"]["Settings"].Value = (File.Exists(Defines.MENU_SETTINGS)) ? new BitmapImage(new Uri(Defines.MENU_SETTINGS, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            /* [Info] - Represents various pieces of information related to server information.
             *   [Players]         - Players currently on the server. 
             *   [General]         - Information such as the server name, ip, and port.
             *   [Current]         - Current map / game mode.
             *   [Setting]         - Various server settings. 
             *   [Ranked]          - The server is streaming to a ranking system.
             *   [Secure]          - The server is running Anti-cheat software.
             *   [Passworded]      - The server is password protected.
             *   [AutoBalanced]    - The server is running Auto-balance software.
             *   [NotRanked]       - The server is NOT streaming to a ranking system.
             *   [NotSecure]       - The server is NOT running Anti-cheat software. 
             *   [NotPassworded]   - The server is NOT password protected.
             *   [NotAutoBalanced] - The server is NOT running Auto-balance software. */
            InstanceViewModel.PublicProperties["Images"]["Info"]["Players"].Value         = (File.Exists(Defines.INFO_PLAYERS))           ? new BitmapImage(new Uri(Defines.INFO_PLAYERS, UriKind.RelativeOrAbsolute))           : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["General"].Value         = (File.Exists(Defines.INFO_GENERAL))           ? new BitmapImage(new Uri(Defines.INFO_GENERAL, UriKind.RelativeOrAbsolute))           : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["Current"].Value         = (File.Exists(Defines.INFO_CURRENT))           ? new BitmapImage(new Uri(Defines.INFO_CURRENT, UriKind.RelativeOrAbsolute))           : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["Setting"].Value         = (File.Exists(Defines.INFO_SETTING))           ? new BitmapImage(new Uri(Defines.INFO_SETTING, UriKind.RelativeOrAbsolute))           : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["Ranked"].Value          = (File.Exists(Defines.INFO_RANKED))            ? new BitmapImage(new Uri(Defines.INFO_RANKED, UriKind.RelativeOrAbsolute))            : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["Secure"].Value          = (File.Exists(Defines.INFO_SECURE))            ? new BitmapImage(new Uri(Defines.INFO_SECURE, UriKind.RelativeOrAbsolute))            : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["Passworded"].Value      = (File.Exists(Defines.INFO_PASSWORDED))        ? new BitmapImage(new Uri(Defines.INFO_PASSWORDED, UriKind.RelativeOrAbsolute))        : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["AutoBalanced"].Value    = (File.Exists(Defines.INFO_AUTO_BALANCED))     ? new BitmapImage(new Uri(Defines.INFO_AUTO_BALANCED, UriKind.RelativeOrAbsolute))     : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["NotRanked"].Value       = (File.Exists(Defines.INFO_NOT_RANKED))        ? new BitmapImage(new Uri(Defines.INFO_NOT_RANKED, UriKind.RelativeOrAbsolute))        : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["NotSecure"].Value       = (File.Exists(Defines.INFO_NOT_SECURE))        ? new BitmapImage(new Uri(Defines.INFO_NOT_SECURE, UriKind.RelativeOrAbsolute))        : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["NotPassworded"].Value   = (File.Exists(Defines.INFO_NOT_PASSWORDED))    ? new BitmapImage(new Uri(Defines.INFO_NOT_PASSWORDED, UriKind.RelativeOrAbsolute))    : new BitmapImage();
            InstanceViewModel.PublicProperties["Images"]["Info"]["NotAutoBalanced"].Value = (File.Exists(Defines.INFO_NOT_AUTO_BALANCED)) ? new BitmapImage(new Uri(Defines.INFO_NOT_AUTO_BALANCED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
        }
    }
}
