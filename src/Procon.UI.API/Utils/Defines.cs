using System;
using System.IO;

namespace Procon.UI.API.Utils
{
    public static class Defines
    {
        /// Procon Specific Images.
        public static readonly String PROCON_ICON  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2.ico");
        public static readonly String PROCON_LARGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2Large.png");
        public static readonly String PROCON_SMALL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2Small.png");

        /// Game Icons.
        public static readonly String GAME_BF_BC2    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BadCompany2.png");
        public static readonly String GAME_BF_3      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Battlefield3.png");
        public static readonly String GAME_COD_BO    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\CallOfDuty_BlackOps.png");
        public static readonly String GAME_HOMEFRONT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Homefront.png");
        public static readonly String GAME_MOH_2010  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MedalOfHonor_2010.png");
        public static readonly String GAME_TF_2      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\TeamFortress2.png");

        /// Arrow Icons.
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
