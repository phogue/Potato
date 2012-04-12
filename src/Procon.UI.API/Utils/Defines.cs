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

namespace Procon.UI.API.Utils
{
    public static class Defines
    {
        /// The Extensions Config File / Config Directory.
        public static readonly String EXTENSIONS_CONFIG    = Path.Combine(@"Procon.Extension.cfg");
        public static readonly String EXTENSIONS_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Extensions");

        /// Procon Specific Images.
        public static readonly String PROCON_ICON = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2.ico");
        public static readonly String PROCON_LOGO = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Procon2.png");

        /// Game Type Icons.
        public static readonly String GAME_BF_BC2    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\BadCompany2.png");
        public static readonly String GAME_BF_3      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Battlefield3.png");
        public static readonly String GAME_COD_BO    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\CallOfDuty_BlackOps.png");
        public static readonly String GAME_HOMEFRONT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Homefront.png");
        public static readonly String GAME_MOH_2010  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MedalOfHonor_2010.png");
        public static readonly String GAME_TF_2      = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\TeamFortress2.png");

        /// Connection Add/Edit/Remove & Status Icons.
        public static readonly String CONN_ADD        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MathPlus.png");
        public static readonly String CONN_EDIT       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MathPencil.png");
        public static readonly String CONN_REMOVE     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\MathMinus.png");
        public static readonly String CONN_GOOD       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Wireless_Good.png");
        public static readonly String CONN_FLUX       = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Wireless_Flux.png");
        public static readonly String CONN_BAD        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Wireless_Down.png");

        /// Menu Bar Icons
        public static readonly String MENU_HOME     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Home.png");
        public static readonly String MENU_SETTINGS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Gear.png");

        /// Info Bar Icons
        public static readonly String INFO_PLAYERS           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\IconPerson.png");
        public static readonly String INFO_GENERAL           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusInformation.png");
        public static readonly String INFO_CURRENT           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\IconLocation.png");
        public static readonly String INFO_SETTING           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ActionPage.png");
        public static readonly String INFO_RANKED            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusFlag.png");
        public static readonly String INFO_SECURE            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SecurityLockClosed.png");
        public static readonly String INFO_PASSWORDED        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SecurityKey.png");
        public static readonly String INFO_AUTO_BALANCED     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\IconPeople.png");
        public static readonly String INFO_NOT_RANKED        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\StatusNoFlag.png");
        public static readonly String INFO_NOT_SECURE        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SecurityLockOpen.png");
        public static readonly String INFO_NOT_PASSWORDED    = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\SecurityNoKey.png");
        public static readonly String INFO_NOT_AUTO_BALANCED = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\IconNoPeople.png");

        /// Various Arrows / Media Icons
        public static readonly String ARROW_DOWN         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowDown.png");
        public static readonly String ARROW_LEFT         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowLeft.png");
        public static readonly String ARROW_LEFT_DOUBLE  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowLeftDouble.png");
        public static readonly String ARROW_RIGHT        = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowRight.png");
        public static readonly String ARROW_RIGHT_DOUBLE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowRightDouble.png");
        public static readonly String ARROW_UP           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\ArrowUp.png");
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
