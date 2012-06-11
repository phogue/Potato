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

        // Connection.
        public static readonly String CONNECTION_SWAP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ConnectionSwap.png");
        public static readonly String CONNECTION_INFO = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ConnectionInfo.png");

        // Backgrounds.
        public static readonly String BACKGROUND_NAVIGATION = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BackgroundNavigation.png");

        // Game Icons.
        public static readonly String GAME_BF_BC2    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BadCompany2.png");
        public static readonly String GAME_BF_3      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Battlefield3.png");
        public static readonly String GAME_COD_BO    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\CallOfDuty_BlackOps.png");
        public static readonly String GAME_HOMEFRONT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Homefront.png");
        public static readonly String GAME_MOH_2010  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MedalOfHonor_2010.png");
        public static readonly String GAME_TF_2      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\TeamFortress2.png");

        // Arrow Icons.
        public static readonly String ARROW_DOWN         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowDown.png");
        public static readonly String ARROW_LEFT         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowLeft.png");
        public static readonly String ARROW_LEFT_DOUBLE  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowLeftDouble.png");
        public static readonly String ARROW_RIGHT        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowRight.png");
        public static readonly String ARROW_RIGHT_DOUBLE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowRightDouble.png");
        public static readonly String ARROW_UP           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowUp.png");

        // Media Icons.
        public static readonly String MEDIA_PLAY         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaPlay.png");
        public static readonly String MEDIA_PAUSE        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaPause.png");
        public static readonly String MEDIA_STOP         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaStop.png");
        public static readonly String MEDIA_NEXT         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaNext.png");
        public static readonly String MEDIA_PREVIOUS     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaPrevious.png");
        public static readonly String MEDIA_REFRESH      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaRefresh.png");
        public static readonly String MEDIA_REPEAT       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaRepeat.png");
        public static readonly String MEDIA_RECORD       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MediaRecord.png");
    }
}
