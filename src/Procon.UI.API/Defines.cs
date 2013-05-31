using System;
using System.IO;

namespace Procon.UI.API
{
    public static class Defines
    {
        // Ui Images Directory.
        public static readonly String DIR_IMAGES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        // Procon Specific Images.
        public static readonly String PROCON_ICON  = Path.Combine(DIR_IMAGES, "Procon2.ico");
        public static readonly String PROCON_LARGE = Path.Combine(DIR_IMAGES, "Procon2_Large.png");
        public static readonly String PROCON_SMALL = Path.Combine(DIR_IMAGES, "Procon2_Small.png");
        public static readonly String PROCON_TEXT  = Path.Combine(DIR_IMAGES, "Procon2_Text.png");

        // Interface Icons.
        public static readonly String INTERFACE_LOCAL  = Path.Combine(DIR_IMAGES, "Interface_Local.png");
        public static readonly String INTERFACE_REMOTE = Path.Combine(DIR_IMAGES, "Interface_Remote.png");

        // Connection Icons.
        public static readonly String CONNECTION_BF_BC2    = Path.Combine(DIR_IMAGES, "Game_BadCompany2.png");
        public static readonly String CONNECTION_BF_3      = Path.Combine(DIR_IMAGES, "Game_Battlefield3.png");
        public static readonly String CONNECTION_COD_BO    = Path.Combine(DIR_IMAGES, "Game_CallOfDuty_BlackOps.png");
        public static readonly String CONNECTION_HOMEFRONT = Path.Combine(DIR_IMAGES, "Game_Homefront.png");
        public static readonly String CONNECTION_MOH_2010  = Path.Combine(DIR_IMAGES, "Game_MedalOfHonor_2010.png");
        public static readonly String CONNECTION_TF_2      = Path.Combine(DIR_IMAGES, "Game_TeamFortress2.png");
        public static readonly String CONNECTION_UNK       = Path.Combine(DIR_IMAGES, "Game_Unknown.png");

        // Signal Status Icons.
        public static readonly String STATUS_GOOD = Path.Combine(DIR_IMAGES, "Status_Good.png");
        public static readonly String STATUS_FLUX = Path.Combine(DIR_IMAGES, "Status_Flux.png");
        public static readonly String STATUS_BAD  = Path.Combine(DIR_IMAGES, "Status_Bad.png");
        public static readonly String STATUS_UNK  = Path.Combine(DIR_IMAGES, "Status_Off.png");

        // Navigation Icons.
        public static readonly String NAVIGATION_LGRING   = Path.Combine(DIR_IMAGES, "Nav_LgRing.png");
        public static readonly String NAVIGATION_SMRING   = Path.Combine(DIR_IMAGES, "Nav_SmRing.png");
        public static readonly String NAVIGATION_OVERVIEW = Path.Combine(DIR_IMAGES, "Nav_Overview.png");
        public static readonly String NAVIGATION_PLAYERS  = Path.Combine(DIR_IMAGES, "Nav_Players.png");
        public static readonly String NAVIGATION_BANS     = Path.Combine(DIR_IMAGES, "Nav_Bans.png");
        public static readonly String NAVIGATION_MAPS     = Path.Combine(DIR_IMAGES, "Nav_Maps.png");
        public static readonly String NAVIGATION_PLUGINS  = Path.Combine(DIR_IMAGES, "Nav_Plugins.png");
        public static readonly String NAVIGATION_SETTINGS = Path.Combine(DIR_IMAGES, "Nav_Settings.png");
        public static readonly String NAVIGATION_INTERFACES  = Path.Combine(DIR_IMAGES, "19px.png");
        public static readonly String NAVIGATION_CONNECTIONS = Path.Combine(DIR_IMAGES, "19px.png");

        // General Icons.
        public static readonly String GENERAL_ADD    = Path.Combine(DIR_IMAGES, "General_Add.png");
        public static readonly String GENERAL_EDIT   = Path.Combine(DIR_IMAGES, "General_Edit.png");
        public static readonly String GENERAL_REMOVE = Path.Combine(DIR_IMAGES, "General_Remove.png");
        public static readonly String GENERAL_PLAYER = Path.Combine(DIR_IMAGES, "General_Player.png");
        public static readonly String GENERAL_GOOD   = Path.Combine(DIR_IMAGES, "General_Good.png");
        public static readonly String GENERAL_BAD    = Path.Combine(DIR_IMAGES, "General_Bad.png");
        public static readonly String GENERAL_WARN   = Path.Combine(DIR_IMAGES, "General_Warn.png");
        public static readonly String GENERAL_NOTIFY = Path.Combine(DIR_IMAGES, "General_Notify.png");
    }
}
