using System;
using System.IO;

namespace Procon.UI.API.Utils
{
    public static class Defines
    {
        // Procon Specific Images.
        public static readonly String PROCON_ICON  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2.ico");
        public static readonly String PROCON_LARGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2Large.png");
        public static readonly String PROCON_SMALL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2Small.png");

        // Interface Icons.
        public static readonly String INTERFACE_LOCAL  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\InterfaceLocal.png");
        public static readonly String INTERFACE_REMOTE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\InterfaceRemote.png");

        // Connection Icons.
        public static readonly String CONNECTION_BF_BC2    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameBadCompany2.png");
        public static readonly String CONNECTION_BF_3      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameBattlefield3.png");
        public static readonly String CONNECTION_COD_BO    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameCallOfDuty_BlackOps.png");
        public static readonly String CONNECTION_HOMEFRONT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameHomefront.png");
        public static readonly String CONNECTION_MOH_2010  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameMedalOfHonor_2010.png");
        public static readonly String CONNECTION_TF_2      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GameTeamFortress2.png");
        public static readonly String CONNECTION_UNKNOWN   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Unknown.png");

        // Connection Status Icons.
        public static readonly String STATUS_GOOD = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusGood.png");
        public static readonly String STATUS_FLUX = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusFlux.png");
        public static readonly String STATUS_BAD  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusBad.png");

        // Content Icons.
        public static readonly String PLAYERS_DEFAULT   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PlayersDefault.png");
        public static readonly String PLAYERS_HOVER     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PlayersHover.png");
        public static readonly String PLAYERS_ACTIVE    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PlayersActive.png");
        public static readonly String PLAYERS_DISABLED  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PlayersDisabled.png");
        public static readonly String MAPS_DEFAULT      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MapsDefault.png");
        public static readonly String MAPS_HOVER        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MapsHover.png");
        public static readonly String MAPS_ACTIVE       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MapsActive.png");
        public static readonly String MAPS_DISABLED     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MapsDisabled.png");
        public static readonly String BANS_DEFAULT      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BansDefault.png");
        public static readonly String BANS_HOVER        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BansHover.png");
        public static readonly String BANS_ACTIVE       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BansActive.png");
        public static readonly String BANS_DISABLED     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BansDisabled.png");
        public static readonly String PLUGINS_DEFAULT   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PluginsDefault.png");
        public static readonly String PLUGINS_HOVER     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PluginsHover.png");
        public static readonly String PLUGINS_ACTIVE    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PluginsActive.png");
        public static readonly String PLUGINS_DISABLED  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\PluginsDisabled.png");
        public static readonly String SETTINGS_DEFAULT  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SettingsDefault.png");
        public static readonly String SETTINGS_HOVER    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SettingsHover.png");
        public static readonly String SETTINGS_ACTIVE   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SettingsActive.png");
        public static readonly String SETTINGS_DISABLED = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SettingsDisabled.png");
        public static readonly String OPTIONS_DEFAULT   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\OptionsDefault.png");
        public static readonly String OPTIONS_HOVER     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\OptionsHover.png");
        public static readonly String OPTIONS_ACTIVE    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\OptionsActive.png");
        public static readonly String OPTIONS_DISABLED  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\OptionsDisabled.png");

        // General Icons.
        public static readonly String GENERAL_PLAYER = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GeneralPlayer.png");
        public static readonly String GENERAL_GOOD   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GeneralGood.png");
        public static readonly String GENERAL_BAD    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GeneralBad.png");
        public static readonly String GENERAL_WARN   = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GeneralWarn.png");
        public static readonly String GENERAL_NOTIFY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\GeneralNotify.png");

        // Connection.
        public static readonly String CONNECTION_SWAP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ConnectionSwap.png");
        public static readonly String CONNECTION_INFO = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ConnectionInfo.png");

        // Backgrounds.
        public static readonly String BACKGROUND_NAVIGATION = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BackgroundNavigation.png");
    }
}
