using System;
using System.IO;

namespace Procon.UI.API.Utils
{
    public static class Defines
    {
        // Images Directory.
        public static readonly String DIR_IMAGES    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images");
        public static readonly String DIR_LIGHT_128 = Path.Combine(DIR_IMAGES, @"Light\128px");
        public static readonly String DIR_LIGHT_24  = Path.Combine(DIR_IMAGES, @"Light\24px");
        public static readonly String DIR_DARK_128  = Path.Combine(DIR_IMAGES, @"Dark\128px");
        public static readonly String DIR_DARK_24   = Path.Combine(DIR_IMAGES, @"Dark\24px");


        // Procon Specific Images.
        public static readonly String PROCON_ICON  = Path.Combine(DIR_IMAGES, "Procon2.ico");
        public static readonly String PROCON_LARGE = Path.Combine(DIR_IMAGES, "Procon2Large.png");
        public static readonly String PROCON_SMALL = Path.Combine(DIR_IMAGES, "Procon2Small.png");

        // Interface Icons.
        public static readonly String INTERFACE_LOCAL  = Path.Combine(DIR_IMAGES, "InterfaceLocal.png");
        public static readonly String INTERFACE_REMOTE = Path.Combine(DIR_IMAGES, "InterfaceRemote.png");

        // Connection Icons.
        public static readonly String CONNECTION_BF_BC2    = Path.Combine(DIR_IMAGES, "GameBadCompany2.png");
        public static readonly String CONNECTION_BF_3      = Path.Combine(DIR_IMAGES, "GameBattlefield3.png");
        public static readonly String CONNECTION_COD_BO    = Path.Combine(DIR_IMAGES, "GameCallOfDuty_BlackOps.png");
        public static readonly String CONNECTION_HOMEFRONT = Path.Combine(DIR_IMAGES, "GameHomefront.png");
        public static readonly String CONNECTION_MOH_2010  = Path.Combine(DIR_IMAGES, "GameMedalOfHonor_2010.png");
        public static readonly String CONNECTION_TF_2      = Path.Combine(DIR_IMAGES, "GameTeamFortress2.png");
        public static readonly String CONNECTION_UNKNOWN   = Path.Combine(DIR_IMAGES, "Unknown.png");

        // Connection Status Icons.
        public static readonly String STATUS_GOOD_L24 = Path.Combine(DIR_LIGHT_24, "cstatus3.png");
        public static readonly String STATUS_FLUX_L24 = Path.Combine(DIR_LIGHT_24, "cstatus2.png");
        public static readonly String STATUS_BAD_L24  = Path.Combine(DIR_LIGHT_24, "cstatus1.png");
        public static readonly String STATUS_UNK_L24  = Path.Combine(DIR_LIGHT_24, "cstatus0.png");
        public static readonly String STATUS_GOOD_D24 = Path.Combine(DIR_DARK_24,  "cstatus3.png");
        public static readonly String STATUS_FLUX_D24 = Path.Combine(DIR_DARK_24,  "cstatus2.png");
        public static readonly String STATUS_BAD_D24  = Path.Combine(DIR_DARK_24,  "cstatus1.png");
        public static readonly String STATUS_UNK_D24  = Path.Combine(DIR_DARK_24,  "cstatus0.png");

        // Header Icons.
        public static readonly String HEADER_OVERVIEW_L24 = Path.Combine(DIR_LIGHT_24, "overview.png");
        public static readonly String HEADER_OVERVIEW_D24 = Path.Combine(DIR_DARK_24,  "overview.png");

        // Content Icons.
        public static readonly String NAVIGATION_PLAYERS  = Path.Combine(DIR_IMAGES, "nplayer_32.png");
        public static readonly String NAVIGATION_MAPS     = Path.Combine(DIR_IMAGES, "nmap_32.png");
        public static readonly String NAVIGATION_BANS     = Path.Combine(DIR_IMAGES, "nban_32.png");
        public static readonly String NAVIGATION_PLUGINS  = Path.Combine(DIR_IMAGES, "nplugin_32.png");
        public static readonly String NAVIGATION_SETTINGS = Path.Combine(DIR_IMAGES, "nsetting_32.png");
        public static readonly String NAVIGATION_OPTIONS  = Path.Combine(DIR_IMAGES, "nconfig_32.png");

        // General Icons.
        public static readonly String GENERAL_PLAYER = Path.Combine(DIR_IMAGES, "GeneralPlayer.png");
        public static readonly String GENERAL_GOOD   = Path.Combine(DIR_IMAGES, "GeneralGood.png");
        public static readonly String GENERAL_BAD    = Path.Combine(DIR_IMAGES, "GeneralBad.png");
        public static readonly String GENERAL_WARN   = Path.Combine(DIR_IMAGES, "GeneralWarn.png");
        public static readonly String GENERAL_NOTIFY = Path.Combine(DIR_IMAGES, "GeneralNotify.png");

        // Connection.
        public static readonly String CONNECTION_SWAP = Path.Combine(DIR_IMAGES, @"ConnectionSwap.png");
        public static readonly String CONNECTION_INFO = Path.Combine(DIR_IMAGES, @"ConnectionInfo.png");
    }
}
