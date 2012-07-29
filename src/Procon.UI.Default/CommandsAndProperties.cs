using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class CommandsAndProperties : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Commands And Properties"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // Set up the extension.
        [STAThread]
        public bool Entry(Window root)
        {
            // [Interface] Level Commands.
            ExtensionApi.Commands["Interface"]["Add"].Value    = new RelayCommand<Object[]>(interfaceAdd, interfaceAddCan);
            ExtensionApi.Commands["Interface"]["Remove"].Value = new RelayCommand<Object[]>(interfaceRemove, interfaceRemoveCan);
            ExtensionApi.Commands["Interface"]["Set"].Value    = new RelayCommand<InterfaceViewModel>(interfaceSet);

            // [Connection] Level Commands.
            ExtensionApi.Commands["Connection"]["Add"].Value    = new RelayCommand<Object[]>(connectionAdd, connectionAddCan);
            ExtensionApi.Commands["Connection"]["Remove"].Value = new RelayCommand<Object[]>(connectionRemove, connectionRemoveCan);
            ExtensionApi.Commands["Connection"]["Set"].Value    = new RelayCommand<ConnectionViewModel>(connectionSet);

            // [Player] Level Commands.
            ExtensionApi.Commands["Player"]["Move"].Value = new RelayCommand<Object[]>(playerMove, playerMoveCan);
            ExtensionApi.Commands["Player"]["Kick"].Value = new RelayCommand<Object[]>(playerKick, playerKickCan);
            ExtensionApi.Commands["Player"]["Ban"].Value  = new RelayCommand<Object[]>(playerBan,  playerBanCan);

            // [Map] Level Commands.
            ExtensionApi.Commands["Map"]["NextMap"].Value      = new RelayCommand<Object[]>(mapNextMap,      mapNextMapCan);
            ExtensionApi.Commands["Map"]["NextRound"].Value    = new RelayCommand<Object[]>(mapNextRound,    mapNextRoundCan);
            ExtensionApi.Commands["Map"]["RestartMap"].Value   = new RelayCommand<Object[]>(mapRestartMap,   mapRestartMapCan);
            ExtensionApi.Commands["Map"]["RestartRound"].Value = new RelayCommand<Object[]>(mapRestartRound, mapRestartRoundCan);
            ExtensionApi.Commands["Map"]["Insert"].Value       = new RelayCommand<Object[]>(mapInsert,       mapInsertCan);
            ExtensionApi.Commands["Map"]["Remove"].Value       = new RelayCommand<Object[]>(mapRemove,       mapRemoveCan);
            ExtensionApi.Commands["Map"]["Move"].Value         = new RelayCommand<Object[]>(mapMove,         mapMoveCan);



            // [Connection] - Game Types.
            ExtensionApi.Properties["Types"]["Connection"].Value = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);

            // [Empty] - A null image.
            ExtensionApi.Properties["Images"]["Empty"].Value = new BitmapImage();

            // [Procon] - Procon images.
            ExtensionApi.Properties["Images"]["Procon"]["Icon"].Value  = (File.Exists(Defines.PROCON_ICON))  ? new BitmapImage(new Uri(Defines.PROCON_ICON,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Procon"]["Large"].Value = (File.Exists(Defines.PROCON_LARGE)) ? new BitmapImage(new Uri(Defines.PROCON_LARGE, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Procon"]["Small"].Value = (File.Exists(Defines.PROCON_SMALL)) ? new BitmapImage(new Uri(Defines.PROCON_SMALL, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Interfaces] - Interface types.
            ExtensionApi.Properties["Images"]["Interfaces"]["Local"].Value  = (File.Exists(Defines.INTERFACE_LOCAL))  ? new BitmapImage(new Uri(Defines.INTERFACE_LOCAL,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Interfaces"]["Remote"].Value = (File.Exists(Defines.INTERFACE_REMOTE)) ? new BitmapImage(new Uri(Defines.INTERFACE_REMOTE, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Connections] - Game types.
            ExtensionApi.Properties["Images"]["Connections"]["BF_3"].Value      = (File.Exists(Defines.CONNECTION_BF_3))      ? new BitmapImage(new Uri(Defines.CONNECTION_BF_3,      UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["BF_BC2"].Value    = (File.Exists(Defines.CONNECTION_BF_BC2))    ? new BitmapImage(new Uri(Defines.CONNECTION_BF_BC2,    UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["COD_BO"].Value    = (File.Exists(Defines.CONNECTION_COD_BO))    ? new BitmapImage(new Uri(Defines.CONNECTION_COD_BO,    UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["HOMEFRONT"].Value = (File.Exists(Defines.CONNECTION_HOMEFRONT)) ? new BitmapImage(new Uri(Defines.CONNECTION_HOMEFRONT, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["MOH_2010"].Value  = (File.Exists(Defines.CONNECTION_MOH_2010))  ? new BitmapImage(new Uri(Defines.CONNECTION_MOH_2010,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["TF_2"].Value      = (File.Exists(Defines.CONNECTION_TF_2))      ? new BitmapImage(new Uri(Defines.CONNECTION_TF_2,      UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Connections"]["Unknown"].Value   = (File.Exists(Defines.CONNECTION_UNKNOWN))   ? new BitmapImage(new Uri(Defines.CONNECTION_UNKNOWN,   UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Status] - Connection status.
            ExtensionApi.Properties["Images"]["Status"]["Light"]["LoggedIn"].Value      = (File.Exists(Defines.STATUS_GOOD_L24)) ? new BitmapImage(new Uri(Defines.STATUS_GOOD_L24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Connecting"].Value    = (File.Exists(Defines.STATUS_FLUX_L24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_L24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Connected"].Value     = (File.Exists(Defines.STATUS_FLUX_L24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_L24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Ready"].Value         = (File.Exists(Defines.STATUS_FLUX_L24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_L24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Disconnecting"].Value = (File.Exists(Defines.STATUS_BAD_L24))  ? new BitmapImage(new Uri(Defines.STATUS_BAD_L24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Disconnected"].Value  = (File.Exists(Defines.STATUS_BAD_L24))  ? new BitmapImage(new Uri(Defines.STATUS_BAD_L24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Light"]["Unknown"].Value       = (File.Exists(Defines.STATUS_UNK_L24))  ? new BitmapImage(new Uri(Defines.STATUS_UNK_L24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["LoggedIn"].Value       = (File.Exists(Defines.STATUS_GOOD_D24)) ? new BitmapImage(new Uri(Defines.STATUS_GOOD_D24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Connecting"].Value     = (File.Exists(Defines.STATUS_FLUX_D24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_D24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Connected"].Value      = (File.Exists(Defines.STATUS_FLUX_D24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_D24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Ready"].Value          = (File.Exists(Defines.STATUS_FLUX_D24)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX_D24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Disconnecting"].Value  = (File.Exists(Defines.STATUS_BAD_D24))  ? new BitmapImage(new Uri(Defines.STATUS_BAD_D24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Disconnected"].Value   = (File.Exists(Defines.STATUS_BAD_D24))  ? new BitmapImage(new Uri(Defines.STATUS_BAD_D24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Status"]["Dark"]["Unknown"].Value        = (File.Exists(Defines.STATUS_UNK_D24))  ? new BitmapImage(new Uri(Defines.STATUS_UNK_D24,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Header] - Icons used in the header.
            ExtensionApi.Properties["Images"]["Header"]["Light"]["Overview"].Value = (File.Exists(Defines.HEADER_OVERVIEW_L24)) ? new BitmapImage(new Uri(Defines.HEADER_OVERVIEW_L24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Header"]["Dark"]["Overview"].Value  = (File.Exists(Defines.HEADER_OVERVIEW_D24)) ? new BitmapImage(new Uri(Defines.HEADER_OVERVIEW_D24, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [General] - Images used across the program.
            ExtensionApi.Properties["Images"]["General"]["Player"].Value = (File.Exists(Defines.GENERAL_PLAYER)) ? new BitmapImage(new Uri(Defines.GENERAL_PLAYER, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["General"]["Good"].Value   = (File.Exists(Defines.GENERAL_GOOD))   ? new BitmapImage(new Uri(Defines.GENERAL_GOOD,   UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["General"]["Bad"].Value    = (File.Exists(Defines.GENERAL_BAD))    ? new BitmapImage(new Uri(Defines.GENERAL_BAD,    UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["General"]["Warn"].Value   = (File.Exists(Defines.GENERAL_WARN))   ? new BitmapImage(new Uri(Defines.GENERAL_WARN,   UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["General"]["Notify"].Value = (File.Exists(Defines.GENERAL_NOTIFY)) ? new BitmapImage(new Uri(Defines.GENERAL_NOTIFY, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Countries] - The country flags.
            #region Country Flags

            ExtensionApi.Properties["Images"]["Countries"]["AD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ad.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ae.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/af.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ag.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ai.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/al.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/am.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/an.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ao.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ar.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/as.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/at.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/au.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/aw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ax.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["AZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/az.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ba.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bb.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bd.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/be.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bi.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bj.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bo.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/br.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bs.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/by.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["BZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ca.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CAT"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/catalonia.png",     UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cd.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ch.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ci.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ck.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/co.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cs.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cx.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cy.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["CZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/de.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dj.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/do.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["DZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["EC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ec.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["EE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ee.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["EG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/eg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["EH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/eh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ENG"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/england.png",       UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ER"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/er.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ES"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/es.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ET"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/et.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["EUR"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/europeanunion.png", UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FAM"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fam.png",           UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fi.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fj.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fo.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["FR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ga.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gb.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gd.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ge.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gi.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gp.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gq.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gs.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["GY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gy.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ht.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["HU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ID"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/id.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ie.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/il.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/in.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/io.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/iq.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ir.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/is.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["IT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/it.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["JM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["JO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jo.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["JP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jp.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ke.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ki.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/km.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kp.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ky.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["KZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/la.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lb.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/li.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ls.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["LY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ly.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ma.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/md.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ME"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/me.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ML"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ml.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mo.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mp.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mq.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ms.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mx.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/my.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["MZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/na.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ne.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ng.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ni.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/no.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/np.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["NZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["OM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/om.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pa.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pe.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ph.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ps.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["PY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/py.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["QA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/qa.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["RE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/re.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["RO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ro.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["RS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/rs.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["RU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ru.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["RW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/rw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sa.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sb.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SCO"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/scotland.png",      UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sd.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/se.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sh.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/si.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sj.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/so.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ST"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/st.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sy.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["SZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/td.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/th.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tj.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tk.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tl.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/to.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tr.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tv.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tw.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["TZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ua.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ug.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/um.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UNK"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/unknown.png",       UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["US"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/us.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/uy.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["UZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/uz.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/va.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vc.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ve.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vg.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vi.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vn.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["VU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vu.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["WAL"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/wales.png",         UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["WF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/wf.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["WS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ws.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["YE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ye.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["YT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/yt.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ZA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/za.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ZM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/zm.png",            UriKind.RelativeOrAbsolute));
            ExtensionApi.Properties["Images"]["Countries"]["ZW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/zw.png",            UriKind.RelativeOrAbsolute));

            #endregion

            // [Navigation] - Various images that represent tabs.
            ExtensionApi.Properties["Images"]["Navigation"]["Players"].Value  = (File.Exists(Defines.NAVIGATION_PLAYERS))  ? new BitmapImage(new Uri(Defines.NAVIGATION_PLAYERS,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Navigation"]["Maps"].Value     = (File.Exists(Defines.NAVIGATION_MAPS))     ? new BitmapImage(new Uri(Defines.NAVIGATION_MAPS,     UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Navigation"]["Bans"].Value     = (File.Exists(Defines.NAVIGATION_BANS))     ? new BitmapImage(new Uri(Defines.NAVIGATION_BANS,     UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Navigation"]["Plugins"].Value  = (File.Exists(Defines.NAVIGATION_PLUGINS))  ? new BitmapImage(new Uri(Defines.NAVIGATION_PLUGINS,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Navigation"]["Settings"].Value = (File.Exists(Defines.NAVIGATION_SETTINGS)) ? new BitmapImage(new Uri(Defines.NAVIGATION_SETTINGS, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;
            ExtensionApi.Properties["Images"]["Navigation"]["Options"].Value  = (File.Exists(Defines.NAVIGATION_OPTIONS))  ? new BitmapImage(new Uri(Defines.NAVIGATION_OPTIONS,  UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;

            // [Connection] - Images associated with managing connections.
            ExtensionApi.Properties["Images"]["Connection"]["Info"].Value = (File.Exists(Defines.CONNECTION_INFO)) ? new BitmapImage(new Uri(Defines.CONNECTION_INFO, UriKind.RelativeOrAbsolute)) : ExtensionApi.Properties["Images"]["Empty"].Value;


            
            // Setup Active Interface.
            if (ExtensionApi.Procon != null
                && ExtensionApi.Settings["InterfaceType"].Value is Boolean
                && ExtensionApi.Settings["InterfaceHost"].Value is String
                && ExtensionApi.Settings["InterfacePort"].Value is UInt16)
                ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.FirstOrDefault(x =>
                    x.IsLocal  == (Boolean)ExtensionApi.Settings["InterfaceType"].Value &&
                    x.Hostname == (String) ExtensionApi.Settings["InterfaceHost"].Value &&
                    x.Port     == (UInt16) ExtensionApi.Settings["InterfacePort"].Value);
            if (ExtensionApi.Interface == null)
                ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.FirstOrDefault();

            // Setup Active Connection.
            if (ExtensionApi.Interface != null
                && ExtensionApi.Settings["ConnectionType"].Value is GameType
                && ExtensionApi.Settings["ConnectionHost"].Value is String
                && ExtensionApi.Settings["ConnectionPort"].Value is UInt16)
                ExtensionApi.Connection = ExtensionApi.Interface.Connections.FirstOrDefault(x =>
                    x.GameType == (GameType)ExtensionApi.Settings["ConnectionType"].Value &&
                    x.Hostname == (String)  ExtensionApi.Settings["ConnectionHost"].Value &&
                    x.Port     == (UInt16)  ExtensionApi.Settings["ConnectionPort"].Value);
            if (ExtensionApi.Connection == null)
                ExtensionApi.Connection = ExtensionApi.Interface.Connections.FirstOrDefault();



            // We done here broski.
            return true;
        }



        // -- [Interface][Add]
        private void interfaceAdd(Object[] parameters)
        {
            ExtensionApi.Procon.CreateInterface(
                ((String)parameters[0]).Trim(),
                UInt16.Parse(((String)parameters[1]).Trim()),
                ((String)parameters[2]).Trim(),
                ((String)parameters[3]).Trim());
        }
        private bool interfaceAddCan(Object[] parameters)
        {
            String tString;
            UInt16 tUInt16;
            return
                ExtensionApi.Procon != null && parameters.Length >= 4
                && (tString = parameters[0] as String) != null && tString.Trim() != String.Empty
                && UInt16.TryParse(parameters[1] as String, out tUInt16) && tUInt16 != 0
                && (tString = parameters[2] as String) != null && tString.Trim() != String.Empty
                && (tString = parameters[3] as String) != null && tString.Trim() != String.Empty;
        }
        // -- [Interface][Remove]
        private void interfaceRemove(Object[] parameters)
        {
            ExtensionApi.Procon.DestroyInterface(
                ((String)parameters[0]).Trim(),
                UInt16.Parse((String)parameters[1]));
        }
        private bool interfaceRemoveCan(Object[] parameters)
        {
            String tString;
            UInt16 tUInt16;
            return ExtensionApi.Procon != null && parameters.Length >= 2 
                   && (tString = parameters[0] as String) != null && tString.Trim() != String.Empty
                   && UInt16.TryParse(parameters[1] as String, out tUInt16) && tUInt16 != 0;
        }
        // -- [Interface][Set]
        private void interfaceSet(InterfaceViewModel view)
        {
            ExtensionApi.Interface = view;
        }

        // -- [Connection][Add]
        private void connectionAdd(Object[] parameters)
        {
            ExtensionApi.Interface.AddConnection(
                ((String)parameters[0]).Trim(),
                ((String)parameters[1]).Trim(),
                UInt16.Parse((String)parameters[2]),
                ((String)parameters[3]).Trim(),
                ((String)parameters[4]).Trim());
        }
        private bool connectionAddCan(Object[] parameters)
        {
            String tString;
            UInt16 tUInt16;
            return
                ExtensionApi.Interface != null && parameters.Length >= 5
                && (tString = parameters[0] as String) != null && tString.Trim() != String.Empty
                && (tString = parameters[1] as String) != null && tString.Trim() != String.Empty
                && UInt16.TryParse(parameters[2] as String, out tUInt16) && tUInt16 != 0
                && (tString = parameters[3] as String) != null && tString.Trim() != String.Empty
                && (((String)parameters[0] == "COD_BO") ? ((tString = parameters[4] as String) != null && tString.Trim() != String.Empty) : true);
        }
        // -- [Connection][Remove]
        private void connectionRemove(Object[] parameters)
        {
            ExtensionApi.Interface.RemoveConnection(
                ((String)parameters[0]).Trim(),
                ((String)parameters[1]).Trim(),
                UInt16.Parse((String)parameters[2]));
        }
        private bool connectionRemoveCan(Object[] parameters)
        {
            String tString;
            UInt16 tUInt16;
            return
                ExtensionApi.Interface != null && parameters.Length >= 3
                && (tString = parameters[0] as String) != null && tString.Trim() != String.Empty
                && (tString = parameters[1] as String) != null && tString.Trim() != String.Empty
                && UInt16.TryParse(parameters[2] as String, out tUInt16) && tUInt16 != 0;
        }
        // -- [Connection][Set]
        private void connectionSet(ConnectionViewModel view)
        {
            ExtensionApi.Connection = view;
        }
        
        // -- [Player][Move]
        private void playerMove(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Move() {
                    MoveActionType = MoveActionType.ForceRotate,
                    Target         = ((Player)parameters[0]),
                    Destination    = ((PlayerSubset)parameters[1]),
                    Reason         = ((String)parameters[2]).Trim()
                });
        }
        private bool playerMoveCan(Object[] parameters)
        {
            Player       tPlayer;
            PlayerSubset tSubset;
            String       tString;
            return
                ExtensionApi.Connection != null && parameters.Length >= 3
                && (tPlayer = parameters[0] as Player)       != null && tPlayer.UID    != String.Empty
                && (tSubset = parameters[1] as PlayerSubset) != null && (tSubset.Team  != Team.None || tSubset.Squad != Squad.None)
                && (tString = parameters[1] as String)       != null && tString.Trim() != String.Empty;
        }
        // -- [Player][Kick]
        private void playerKick(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Kick() {
                    Target = ((Player)parameters[0]),
                    Reason = ((String)parameters[1]).Trim()
                });
        }
        private bool playerKickCan(Object[] parameters)
        {
            Player tPlayer;
            String tString;
            return
                ExtensionApi.Connection != null && parameters.Length >= 2
                && (tPlayer = parameters[0] as Player) != null && tPlayer.UID    != String.Empty
                && (tString = parameters[1] as String) != null && tString.Trim() != String.Empty;
        }
        // -- [Player][Ban]
        private void playerBan(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Ban() {
                    BanActionType = BanActionType.Ban,
                    Target        = ((Player)parameters[0]),
                    Time          = ((TimeSubset)parameters[1]),
                    Reason        = ((String)parameters[2]).Trim()
                });
        }
        private bool playerBanCan(Object[] parameters)
        {
            Player     tPlayer;
            TimeSubset tSubset;
            String     tString;
            return
                ExtensionApi.Connection != null && parameters.Length >= 3
                && (tPlayer = parameters[0] as Player)     != null && tPlayer.UID      != String.Empty
                && (tSubset = parameters[1] as TimeSubset) != null && (tSubset.Context != TimeSubsetContext.Time ? tSubset.Context != TimeSubsetContext.None : tSubset.Length.HasValue)
                && (tString = parameters[2] as String)     != null && tString.Trim()   != String.Empty;
        }

        // -- [Map][NextMap]
        private void mapNextMap(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.NextMap
                });
        }
        private bool mapNextMapCan(Object parameters)
        {
            return ExtensionApi.Connection != null;
        }
        // -- [Map][NextRound]
        private void mapNextRound(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.NextRound
                });
        }
        private bool mapNextRoundCan(Object parameters)
        {
            return ExtensionApi.Connection != null;
        }
        // -- [Map][RestartMap]
        private void mapRestartMap(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.RestartMap
                });
        }
        private bool mapRestartMapCan(Object parameters)
        {
            return ExtensionApi.Connection != null;
        }
        // -- [Map][RestartRound]
        private void mapRestartRound(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.RestartRound
                });
        }
        private bool mapRestartRoundCan(Object parameters)
        {
            return ExtensionApi.Connection != null;
        }

        // -- [Map][Add]
        private void mapInsert(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.Insert,
                    Index         = Int32.Parse((String)parameters[0]),
                    Name          = ((String)parameters[1]),
                    GameMode      = (GameMode)parameters[2],
                    Rounds        = Int32.Parse((String)parameters[3])
                });
        }
        private bool mapInsertCan(Object[] parameters)
        {
            GameMode tMode;
            String   tString;
            Int32    tInt32;
            return
                ExtensionApi.Connection != null && parameters.Length >= 4
                && Int32.TryParse(parameters[0] as String, out tInt32) && tInt32 >= 0
                && (tString = parameters[1]     as String)   != null && tString.Trim()    != String.Empty
                && (tMode   = parameters[2]     as GameMode) != null && tMode.Name.Trim() != String.Empty
                && Int32.TryParse(parameters[3] as String, out tInt32) && tInt32 > 0;
        }
        // -- [Map][Remove]
        private void mapRemove(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.RemoveIndex,
                    Index = Int32.Parse((String)parameters[0])
                });
        }
        private bool mapRemoveCan(Object[] parameters)
        {
            Int32  tInt32;
            return
                ExtensionApi.Connection != null && parameters.Length >= 1
                && Int32.TryParse(parameters[0] as String, out tInt32) && tInt32 >= 0;
        }
        // -- [Map][Move]
        private void mapMove(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.RemoveIndex,
                    Index         = Int32.Parse((String)parameters[0])
                });
            ExtensionApi.Connection.Action(
                new Map() {
                    MapActionType = MapActionType.Insert,
                    Index         = Int32.Parse((String)parameters[1]),
                    Name          = (String)parameters[2],
                    GameMode      = (GameMode)parameters[3],
                    Rounds        = Int32.Parse((String)parameters[4])
                });
        }
        private bool mapMoveCan(Object[] parameters)
        {
            GameMode tMode;
            String   tString;
            Int32    tInt32;
            return
                ExtensionApi.Connection != null && parameters.Length >= 5
                && Int32.TryParse(parameters[0] as String, out tInt32) && tInt32 >= 0
                && Int32.TryParse(parameters[1] as String, out tInt32) && tInt32 >= 0
                && (tString = parameters[2]     as String)   != null && tString.Trim()    != String.Empty
                && (tMode   = parameters[3]     as GameMode) != null && tMode.Name.Trim() != String.Empty
                && Int32.TryParse(parameters[4] as String, out tInt32) && tInt32 > 0;
        }


        //ExtensionApi.Commands["Connection"]["Filter"]["Chat"].Value = new RelayCommand<Object>(filterChatChanged);
        //ExtensionApi.Commands["Connection"]["Filter"]["Ban"].Value  = new RelayCommand<Object>(filterBanChanged);

        //ExtensionApi.Commands["Connection"]["Action"]["Chat"].Value          = new RelayCommand<Object>(actionChat,     actionChatCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Player"].Value        = new RelayCommand<IList>(actionPlayer,    actionPlayerCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Map"]["Add"].Value    = new RelayCommand<IList>(actionMapAdd,    actionMapCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Map"]["Remove"].Value = new RelayCommand<IList>(actionMapRemove, actionMapCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Map"]["Up"].Value     = new RelayCommand<IList>(actionMapUp,     actionMapCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Map"]["Down"].Value   = new RelayCommand<IList>(actionMapDown,   actionMapCan);
        //ExtensionApi.Commands["Connection"]["Action"]["Ban"].Value           = new RelayCommand<IList>(actionBan,       actionBanCan);   



        //private void filterChatChanged(Object collection)
        //{
        //    try
        //    {
        //        CollectionViewSource.GetDefaultView(collection).Filter = null;
        //        CollectionViewSource.GetDefaultView(collection).Filter = new Predicate<Object>(filterChat);
        //        CollectionViewSource.GetDefaultView(collection).Refresh();
        //    }
        //    catch (Exception) { }
        //}
        //private void filterBanChanged(Object collection)
        //{
        //    try
        //    {
        //        CollectionViewSource.GetDefaultView(collection).Filter = null;
        //        CollectionViewSource.GetDefaultView(collection).Filter = new Predicate<Object>(filterBan);
        //        CollectionViewSource.GetDefaultView(collection).Refresh();
        //    }
        //    catch (Exception) { }
        //}
        //private bool filterChat(Object item)
        //{
        //    try
        //    {
        //        Event           e     = (Event)item;
        //        String          key   = (String)ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Data"].Value;
        //        FilterType      type  = (FilterType)ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Type"].Value;
        //        FilterChatField field = (FilterChatField)ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Field"].Value;

        //        // Add "Additional Filter" support here by doing things like:
        //        // [Code]
        //        //   Boolean fSpawn = (Boolean)ExtensionApi.Propertyies[...]["Spawn"].Value;
        //        //   Boolean fChat  = ...
        //        //   ...
        //        // [End Code]
        //        // Where each public property represents if the value should be displayed.  E.g, True = display.
        //        // Then, add second if-statement before the first that evaulates if we want to even check this event type:
        //        // [Code]
        //        //   if ((e.EventType == EventType.Spawn && !fSpawn) || (e.EventType == EventType.Chat && !fChat) || ...)
        //        //      return false;
        //        // [End Code]

        //        if (key.Trim().Length > 0)
        //            switch (type)
        //            {
        //                case FilterType.Contains:
        //                    switch (field)
        //                    {
        //                        case FilterChatField.Time:
        //                            return e.Time.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Type:
        //                            return e.Type.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Sender:
        //                            return e.Sender.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Recipient:
        //                            return e.Recipient.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Data:
        //                            return e.Information.ToLower().Contains(key.ToLower());
        //                    }
        //                    break;
        //                case FilterType.Excludes:
        //                    switch (field)
        //                    {
        //                        case FilterChatField.Time:
        //                            return !e.Time.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Type:
        //                            return !e.Type.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Sender:
        //                            return !e.Sender.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Recipient:
        //                            return !e.Recipient.ToLower().Contains(key.ToLower());
        //                        case FilterChatField.Data:
        //                            return !e.Information.ToLower().Contains(key.ToLower());
        //                    }
        //                    break;
        //                case FilterType.Matches:
        //                    switch (field)
        //                    {
        //                        case FilterChatField.Time:
        //                            return e.Time.ToLower() == key.ToLower();
        //                        case FilterChatField.Type:
        //                            return e.Type.ToLower() == key.ToLower();
        //                        case FilterChatField.Sender:
        //                            return e.Sender.ToLower() == key.ToLower();
        //                        case FilterChatField.Recipient:
        //                            return e.Recipient.ToLower() == key.ToLower();
        //                        case FilterChatField.Data:
        //                            return e.Information.ToLower() == key.ToLower();
        //                    }
        //                    break;
        //            }
        //        // If any problems, return valid.
        //        return true;
        //    }
        //    catch (Exception) { return true; }
        //}
        //private bool filterBan(Object item)
        //{
        //    return true;
        //}

        //private void actionChat(Object nothing)
        //{
        //    try
        //    {
        //        ActiveConnection.Action(new Chat()
        //        {
        //            Text           = (String)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Data"].Value,
        //            ChatActionType = (ChatActionType)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Type"].Value,
        //            Subset         = new PlayerSubset()
        //            {
        //                Context = (PlayerSubsetContext)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"].Value,
        //                Team    = (Team)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value,
        //                Squad   = (Squad)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value,
        //                Player  = (Player)ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value
        //            }

        //        });
        //        ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Data"].Value = String.Empty;
        //    }
        //    catch (Exception) { }
        //}
        //private void actionPlayer(IList players)
        //{
        //    try
        //    {
        //        switch ((ActionPlayerType)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Type"].Value)
        //        {
        //            // ------- ------- Move Player(s) ------- ------- //
        //            case ActionPlayerType.Move:
        //                foreach (PlayerViewModel pvm in players)
        //                    ActiveConnection.Action(new Move()
        //                    {
        //                        MoveActionType = MoveActionType.ForceMove,
        //                        Destination = new PlayerSubset()
        //                        {
        //                            Context = PlayerSubsetContext.Squad,
        //                            Team    = (Team)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Team"].Value,
        //                            Squad   = (Squad)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value
        //                        },
        //                        Target = new Player()
        //                        {
        //                            UID    = pvm.Uid,
        //                            SlotID = pvm.SlotID,
        //                            Name   = pvm.Name,
        //                            IP     = pvm.IP
        //                        },
        //                        Reason = (String)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Reason"].Value
        //                    });
        //                break;
        //            // ------- ------- Kill Player(s) ------- ------- //
        //            case ActionPlayerType.Kill:
        //                foreach (PlayerViewModel pvm in players)
        //                    ActiveConnection.Action(new Kill()
        //                    {
        //                        Target = new Player()
        //                        {
        //                            UID    = pvm.Uid,
        //                            SlotID = pvm.SlotID,
        //                            Name   = pvm.Name,
        //                            IP     = pvm.IP
        //                        },
        //                        Reason = (String)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Reason"].Value
        //                    });
        //                break;
        //            // ------- ------- Kick Player(s) ------- ------- //
        //            case ActionPlayerType.Kick:
        //                foreach (PlayerViewModel pvm in players)
        //                    ActiveConnection.Action(new Kick()
        //                    {
        //                        Target = new Player()
        //                        {
        //                            UID    = pvm.Uid,
        //                            SlotID = pvm.SlotID,
        //                            Name   = pvm.Name,
        //                            IP     = pvm.IP
        //                        },
        //                        Reason = (String)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Reason"].Value
        //                    });
        //                break;
        //            // ------- ------- Ban Player(s) ------- ------- //
        //            case ActionPlayerType.Ban:
        //                foreach (PlayerViewModel pvm in players)
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        BanActionType = BanActionType.Ban,
        //                        Time = new TimeSubset()
        //                        {
        //                            Context = (TimeSubsetContext)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value,
        //                            Length  = TimeSpan.ParseExact(
        //                                          (String)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value,
        //                                          new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
        //                                          null)
        //                        },
        //                        Target = new Player()
        //                        {
        //                            UID    = pvm.Uid,
        //                            SlotID = pvm.SlotID,
        //                            Name   = pvm.Name,
        //                            IP     = pvm.IP
        //                        },
        //                        Reason = (String)ExtensionApi.Properties["Connection"]["Action"]["Player"]["Reason"].Value
        //                    });
        //                break;
        //        }
        //    }
        //    catch (Exception) { }
        //}
        //private void actionBan(IList bans)
        //{
        //    try
        //    {
        //        // Save a copy just incase the selection changes.
        //        List<BanViewModel> sBans = new List<BanViewModel>();
        //        foreach (BanViewModel bvm in bans)
        //            sBans.Add(bvm);
        //        switch ((ActionBanType)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Type"].Value)
        //        {
        //            // ------- ------- Ban Player ------- ------- //
        //            case ActionBanType.Ban:
        //                ActiveConnection.Action(new Ban()
        //                {
        //                    Target = new Player()
        //                    {
        //                        UID  = (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Uid"].Value,
        //                        GUID = (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Uid"].Value,
        //                        Name = (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Uid"].Value
        //                    },
        //                    BanActionType = BanActionType.Ban,
        //                    Time          = new TimeSubset()
        //                    {
        //                        Context = (TimeSubsetContext)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Time"].Value,
        //                        Length  = TimeSpan.ParseExact(
        //                                      (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Length"].Value,
        //                                      new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
        //                                      null)
        //                    },
        //                    Reason = (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Reason"].Value
        //                });
        //                break;
        //            // ------- ------- Unban Player(s) ------- ------- //
        //            case ActionBanType.Unban:
        //                foreach (BanViewModel bvm in sBans)
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        Target        = bvm.Target,
        //                        BanActionType = BanActionType.Unban
        //                    });
        //                break;
        //            // ------- ------- Convert Ban(s) to Permanent ------- ------- //
        //            case ActionBanType.ToPermanent:
        //                foreach (BanViewModel bvm in sBans)
        //                {
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        Target        = bvm.Target,
        //                        BanActionType = BanActionType.Unban
        //                    });
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        Target        = bvm.Target,
        //                        BanActionType = BanActionType.Ban,
        //                        Time          = new TimeSubset()
        //                        {
        //                            Context = TimeSubsetContext.Permanent
        //                        },
        //                        Reason = bvm.Reason
        //                    });
        //                }
        //                break;
        //            // ------- ------- Convert Ban(s) to Temporary ------- ------- //
        //            case ActionBanType.ToTemporary:
        //                foreach (BanViewModel bvm in sBans)
        //                {
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        Target        = bvm.Target,
        //                        BanActionType = BanActionType.Unban
        //                    });
        //                    ActiveConnection.Action(new Ban()
        //                    {
        //                        Target        = bvm.Target,
        //                        BanActionType = BanActionType.Ban,
        //                        Time          = new TimeSubset()
        //                        {
        //                            Context = TimeSubsetContext.Time,
        //                            Length  = TimeSpan.ParseExact(
        //                                          (String)ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Length"].Value,
        //                                          new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
        //                                          null)
        //                        },
        //                        Reason = bvm.Reason
        //                    });
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception) { }
        //}




        ///* [Filter][Chat] - Contains information necessary to filter chat/event messages.
        // *   [Data]  - The text to filter by.
        // *   [Type]  - The method used to filter with.
        // *   [Field] - The data to filter on. */
        //ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Data"].Value  = String.Empty;
        //ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Type"].Value  = FilterType.Contains;
        //ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Field"].Value = FilterChatField.Data;

        ///* [Filter][Ban] - Contains information necessary to filter through bans.
        // *   [Data]  - The text to filter by.
        // *   [Type]  - The method used to filter with.
        // *   [Field] - The data to filter on. */
        //ExtensionApi.Properties["Connection"]["Filter"]["Ban"]["Data"].Value  = String.Empty;
        //ExtensionApi.Properties["Connection"]["Filter"]["Ban"]["Type"].Value  = FilterType.Contains;
        //ExtensionApi.Properties["Connection"]["Filter"]["Ban"]["Field"].Value = FilterBanField.Id;

        ///* [Action][Chat] - Contains information necessary to send a message to a game server.
        // *   [Type]     - How to display the text.
        // *   [Subset]   - Who to display the text to.
        // *     [Team]   - Which team to display the text to.
        // *     [Squad]  - Which squad to display the text to.
        // *     [Player] - Which player to display the text to.
        // *   [Data]     - The text to send. */
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Type"].Value             = ChatActionType.Say;
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"].Value           = PlayerSubsetContext.All;
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value   = Team.Team1;
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value  = Squad.None;
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value = null;
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Data"].Value             = String.Empty;

        ///* [Action][Player] - Contains information necessary to perform player administrative actions.
        // *   [Type]        - The type of player action to perform.
        // *   [Move][Team]  - If moving player, the team to move them to.
        // *   [Move][Squad] - If moving player, the squad to move them to.
        // *   [Ban][Time]   - If banning player, the time context to ban them for.
        // *   [Ban][Length] - If banning player, the time length to ban them for.
        // *   [Reason]      - Why the action is being performed. */
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Type"].Value          = ActionPlayerType.Kill;
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Team"].Value  = Team.Team1;
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value = Squad.Squad1;
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value = "1:00";
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Reason"].Value        = String.Empty;

        ///* [Action][Map] - Contains the information necessary to perform map administrative actions.
        // *   [Mode]  - UNSURE AS OF YET.
        // *   [Round] - The number of rounds a map should be added for. */
        //ExtensionApi.Properties["Connection"]["Action"]["Map"]["Mode"].Value  = String.Empty;
        //ExtensionApi.Properties["Connection"]["Action"]["Map"]["Round"].Value = "2";

        ///* [Action][Ban] - Contains information necessary to perform ban administrative actions.
        // *   [Type]   - The type of ban action to perform.
        // *   [Uid]    - If banning player, the unique identifier of the player to ban.
        // *   [Time]   - If banning, the time context to ban for.
        // *   [Length] - If banning or to temp., the time length to ban them for.
        // *   [Reason] - If banning, why the action is being performed. */
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Type"].Value   = ActionBanType.Ban;
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Uid"].Value    = String.Empty;
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Length"].Value = "1:00";
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Reason"].Value = String.Empty;




        //// TYPES - Enumerations used for various reasons within the UI. //
        //// ------------------------------------------------------------ //
        //// Valid Game Types of connections that can be created.
        //ExtensionApi.Properties["Connection"]["Add"]["Types"].Value  = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);

        //// Valid Filter Methods and Chat Fields that can be used to filter and filter on, respectively.
        //ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Filter"]["Chat"]["Fields"].Value = Enum.GetValues(typeof(FilterChatField)).Cast<FilterChatField>().Where(x => true);

        //// Valid Filter Methods and Ban Fields that can be used to filter and filter on, respectively.
        //ExtensionApi.Properties["Connection"]["Filter"]["Ban"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Filter"]["Ban"]["Fields"].Value = Enum.GetValues(typeof(FilterBanField)).Cast<FilterChatField>().Where(x => true);

        //// Valid Methods to display a chat message and subsets to send a chat message to.
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Types"].Value   = Enum.GetValues(typeof(ActionChatType)).Cast<ActionChatType>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Subsets"].Value = Enum.GetValues(typeof(PlayerSubsetContext)).Cast<PlayerSubsetContext>().Where(x => (x != PlayerSubsetContext.Server));
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Teams"].Value   = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
        //ExtensionApi.Properties["Connection"]["Action"]["Chat"]["Squads"].Value  = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);

        //// Valid Player Actions to take, and selections for various player actions.
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Types"].Value          = Enum.GetValues(typeof(ActionPlayerType)).Cast<ActionPlayerType>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Teams"].Value  = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Move"]["Squads"].Value = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Action"]["Player"]["Ban"]["Times"].Value   = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None));

        //// Valid Ban Time Contexts for banning players.
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Types"].Value = Enum.GetValues(typeof(ActionBanType)).Cast<ActionBanType>().Where(x => true);
        //ExtensionApi.Properties["Connection"]["Action"]["Ban"]["Times"].Value = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None) && (x != TimeSubsetContext.Round));
    }
}
