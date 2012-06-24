using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Procon.Net.Protocols;
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
        { get { return "Default Commands And Properties"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        // Set up the extension.
        [STAThread]
        public bool Entry(Window root)
        {
            // Setup Active Interface.
            if (ExtensionApi.Procon != null
                && ExtensionApi.Settings["InterfaceType"].Value is Boolean
                && ExtensionApi.Settings["InterfaceHost"].Value is String
                && ExtensionApi.Settings["InterfacePort"].Value is UInt16)
                ExtensionApi.Interface = ExtensionApi.Procon.Interfaces.FirstOrDefault(x =>
                    x.IsLocal  == (Boolean)ExtensionApi.Settings["InterfaceType"].Value &&
                    x.Hostname == (String) ExtensionApi.Settings["InterfaceHost"].Value &&
                    x.Port     == (UInt16) ExtensionApi.Settings["InterfacePort"].Value);
            // Setup Active Connection.
            if (ExtensionApi.Interface != null
                && ExtensionApi.Settings["ConnectionType"].Value is GameType
                && ExtensionApi.Settings["ConnectionHost"].Value is String
                && ExtensionApi.Settings["ConnectionPort"].Value is UInt16)
                ExtensionApi.Connection = ExtensionApi.Interface.Connections.FirstOrDefault(x =>
                    x.GameType == (GameType)ExtensionApi.Settings["ConnectionType"].Value &&
                    x.Hostname == (String)  ExtensionApi.Settings["ConnectionHost"].Value &&
                    x.Port     == (UInt16)  ExtensionApi.Settings["ConnectionPort"].Value);



            // [Interface] Level Commands.
            ViewModelBase.PublicCommands["Interface"]["Add"].Value    = new RelayCommand<Object[]>(interfaceAdd, interfaceAddCan);
            ViewModelBase.PublicCommands["Interface"]["Remove"].Value = new RelayCommand<Object[]>(interfaceRemove, interfaceRemoveCan);
            ViewModelBase.PublicCommands["Interface"]["Set"].Value    = new RelayCommand<InterfaceViewModel>(interfaceSet);
            // [Connection] Level Commands.
            ViewModelBase.PublicCommands["Connection"]["Add"].Value    = new RelayCommand<Object[]>(connectionAdd, connectionAddCan);
            ViewModelBase.PublicCommands["Connection"]["Remove"].Value = new RelayCommand<Object[]>(connectionRemove, connectionRemoveCan);
            ViewModelBase.PublicCommands["Connection"]["Set"].Value    = new RelayCommand<ConnectionViewModel>(connectionSet);



            // Types.
            ViewModelBase.PublicProperties["Types"]["Connection"].Value = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);

            // Images.
            ViewModelBase.PublicProperties["Images"]["Empty"].Value = new BitmapImage();

            // [Procon] - Procon images.
            ViewModelBase.PublicProperties["Images"]["Procon"]["Icon"].Value  = (File.Exists(Defines.PROCON_ICON))  ? new BitmapImage(new Uri(Defines.PROCON_ICON,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Procon"]["Large"].Value = (File.Exists(Defines.PROCON_LARGE)) ? new BitmapImage(new Uri(Defines.PROCON_LARGE, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Procon"]["Small"].Value = (File.Exists(Defines.PROCON_SMALL)) ? new BitmapImage(new Uri(Defines.PROCON_SMALL, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [Interfaces] - Interface types.
            ViewModelBase.PublicProperties["Images"]["Interfaces"]["Local"].Value  = (File.Exists(Defines.INTERFACE_LOCAL))  ? new BitmapImage(new Uri(Defines.INTERFACE_LOCAL,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Interfaces"]["Remote"].Value = (File.Exists(Defines.INTERFACE_REMOTE)) ? new BitmapImage(new Uri(Defines.INTERFACE_REMOTE, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [Connections] - Game types.
            ViewModelBase.PublicProperties["Images"]["Connections"]["BF_3"].Value      = (File.Exists(Defines.CONNECTION_BF_3))      ? new BitmapImage(new Uri(Defines.CONNECTION_BF_3,      UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["BF_BC2"].Value    = (File.Exists(Defines.CONNECTION_BF_BC2))    ? new BitmapImage(new Uri(Defines.CONNECTION_BF_BC2,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["COD_BO"].Value    = (File.Exists(Defines.CONNECTION_COD_BO))    ? new BitmapImage(new Uri(Defines.CONNECTION_COD_BO,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["HOMEFRONT"].Value = (File.Exists(Defines.CONNECTION_HOMEFRONT)) ? new BitmapImage(new Uri(Defines.CONNECTION_HOMEFRONT, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["MOH_2010"].Value  = (File.Exists(Defines.CONNECTION_MOH_2010))  ? new BitmapImage(new Uri(Defines.CONNECTION_MOH_2010,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["TF_2"].Value      = (File.Exists(Defines.CONNECTION_TF_2))      ? new BitmapImage(new Uri(Defines.CONNECTION_TF_2,      UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connections"]["Unkown"].Value    = (File.Exists(Defines.CONNECTION_UNKNOWN))   ? new BitmapImage(new Uri(Defines.CONNECTION_UNKNOWN,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [Status] - Connection status.
            ViewModelBase.PublicProperties["Images"]["Status"]["LoggedIn"].Value      = (File.Exists(Defines.STATUS_GOOD)) ? new BitmapImage(new Uri(Defines.STATUS_GOOD, UriKind.RelativeOrAbsolute))  : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Status"]["Connecting"].Value    = (File.Exists(Defines.STATUS_FLUX)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX, UriKind.RelativeOrAbsolute))  : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Status"]["Connected"].Value     = (File.Exists(Defines.STATUS_FLUX)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX, UriKind.RelativeOrAbsolute))  : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Status"]["Ready"].Value         = (File.Exists(Defines.STATUS_FLUX)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX, UriKind.RelativeOrAbsolute))  : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Status"]["Disconnecting"].Value = (File.Exists(Defines.STATUS_BAD))  ? new BitmapImage(new Uri(Defines.STATUS_BAD,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Status"]["Disconnected"].Value  = (File.Exists(Defines.STATUS_BAD))  ? new BitmapImage(new Uri(Defines.STATUS_BAD,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [Countries] - The country flags.
            #region Country Flags

            ViewModelBase.PublicProperties["Images"]["Countries"]["AD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ad.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ae.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/af.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ag.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ai.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/al.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/am.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/an.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ao.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ar.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/as.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/at.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/au.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/aw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ax.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["AZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/az.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ba.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bb.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bd.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/be.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bi.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bj.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bo.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/br.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bs.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/by.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["BZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/bz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ca.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CAT"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/catalonia.png",     UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cd.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ch.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ci.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ck.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/co.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cs.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cx.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cy.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["CZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/cz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/de.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dj.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/do.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["DZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/dz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["EC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ec.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["EE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ee.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["EG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/eg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["EH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/eh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ENG"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/england.png",       UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ER"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/er.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ES"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/es.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ET"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/et.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["EUR"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/europeanunion.png", UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FAM"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fam.png",           UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fi.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fj.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fo.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["FR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/fr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ga.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gb.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gd.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ge.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gi.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gp.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gq.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gs.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["GY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/gy.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ht.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["HU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/hu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ID"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/id.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ie.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/il.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/in.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/io.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/iq.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ir.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/is.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["IT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/it.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["JM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["JO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jo.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["JP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/jp.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ke.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ki.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/km.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kp.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ky.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["KZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/kz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/la.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lb.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/li.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ls.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/lv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["LY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ly.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ma.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/md.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ME"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/me.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ML"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ml.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mo.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mp.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MQ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mq.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ms.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MX"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mx.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/my.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["MZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/mz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/na.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ne.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ng.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ni.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/no.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NP"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/np.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["NZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/nz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["OM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/om.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pa.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pe.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ph.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ps.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/pw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["PY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/py.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["QA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/qa.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["RE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/re.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["RO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ro.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["RS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/rs.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["RU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ru.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["RW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/rw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sa.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SB"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sb.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SCO"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/scotland.png",      UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sd.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/se.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sh.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/si.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sj.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/so.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ST"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/st.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sy.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["SZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/sz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TD"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/td.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TH"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/th.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TJ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tj.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TK"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tk.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TL"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tl.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TO"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/to.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TR"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tr.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TV"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tv.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tw.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["TZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/tz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ua.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ug.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/um.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UNK"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/unknown.png",       UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["US"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/us.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UY"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/uy.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["UZ"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/uz.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/va.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VC"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vc.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ve.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VG"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vg.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VI"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vi.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VN"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vn.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["VU"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/vu.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["WAL"].Value = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/wales.png",         UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["WF"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/wf.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["WS"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ws.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["YE"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/ye.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["YT"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/yt.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ZA"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/za.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ZM"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/zm.png",            UriKind.RelativeOrAbsolute));
            ViewModelBase.PublicProperties["Images"]["Countries"]["ZW"].Value  = new BitmapImage(new Uri("pack://application:,,,/Procon.UI;component/Images/Countries/zw.png",            UriKind.RelativeOrAbsolute));

            #endregion

            // [Content] - Various images that represent tabs.
            //   [Players] - Represents the players per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Default"].Value  = (File.Exists(Defines.PLAYERS_DEFAULT))  ? new BitmapImage(new Uri(Defines.PLAYERS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Hover"].Value    = (File.Exists(Defines.PLAYERS_HOVER))    ? new BitmapImage(new Uri(Defines.PLAYERS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Press"].Value    = (File.Exists(Defines.PLAYERS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLAYERS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Active"].Value   = (File.Exists(Defines.PLAYERS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLAYERS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Disabled"].Value = (File.Exists(Defines.PLAYERS_DISABLED)) ? new BitmapImage(new Uri(Defines.PLAYERS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            //   [Maps] - Represents the maps per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Default"].Value  = (File.Exists(Defines.MAPS_DEFAULT))  ? new BitmapImage(new Uri(Defines.MAPS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Hover"].Value    = (File.Exists(Defines.MAPS_HOVER))    ? new BitmapImage(new Uri(Defines.MAPS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Press"].Value    = (File.Exists(Defines.MAPS_ACTIVE))   ? new BitmapImage(new Uri(Defines.MAPS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Active"].Value   = (File.Exists(Defines.MAPS_ACTIVE))   ? new BitmapImage(new Uri(Defines.MAPS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Disabled"].Value = (File.Exists(Defines.MAPS_DISABLED)) ? new BitmapImage(new Uri(Defines.MAPS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            //   [Bans] - Represents the bans per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Default"].Value  = (File.Exists(Defines.BANS_DEFAULT))  ? new BitmapImage(new Uri(Defines.BANS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Hover"].Value    = (File.Exists(Defines.BANS_HOVER))    ? new BitmapImage(new Uri(Defines.BANS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Press"].Value    = (File.Exists(Defines.BANS_ACTIVE))   ? new BitmapImage(new Uri(Defines.BANS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Active"].Value   = (File.Exists(Defines.BANS_ACTIVE))   ? new BitmapImage(new Uri(Defines.BANS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Disabled"].Value = (File.Exists(Defines.BANS_DISABLED)) ? new BitmapImage(new Uri(Defines.BANS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            //   [Plugins] - Represents the plugins per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Default"].Value   = (File.Exists(Defines.PLUGINS_DEFAULT))  ? new BitmapImage(new Uri(Defines.PLUGINS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Hover"].Value     = (File.Exists(Defines.PLUGINS_HOVER))    ? new BitmapImage(new Uri(Defines.PLUGINS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Press"].Value     = (File.Exists(Defines.PLUGINS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLUGINS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Active"].Value    = (File.Exists(Defines.PLUGINS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLUGINS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Disabled"].Value  = (File.Exists(Defines.PLUGINS_DISABLED)) ? new BitmapImage(new Uri(Defines.PLUGINS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            //   [Settings] - Represents the connection level settings.
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Default"].Value  = (File.Exists(Defines.SETTINGS_DEFAULT))  ? new BitmapImage(new Uri(Defines.SETTINGS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Hover"].Value    = (File.Exists(Defines.SETTINGS_HOVER))    ? new BitmapImage(new Uri(Defines.SETTINGS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Press"].Value    = (File.Exists(Defines.SETTINGS_ACTIVE))   ? new BitmapImage(new Uri(Defines.SETTINGS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Active"].Value   = (File.Exists(Defines.SETTINGS_ACTIVE))   ? new BitmapImage(new Uri(Defines.SETTINGS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Disabled"].Value = (File.Exists(Defines.SETTINGS_DISABLED)) ? new BitmapImage(new Uri(Defines.SETTINGS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            //   [Options] - Represents the interface level options.
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Default"].Value  = (File.Exists(Defines.OPTIONS_DEFAULT))  ? new BitmapImage(new Uri(Defines.OPTIONS_DEFAULT,  UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Hover"].Value    = (File.Exists(Defines.OPTIONS_HOVER))    ? new BitmapImage(new Uri(Defines.OPTIONS_HOVER,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Press"].Value    = (File.Exists(Defines.OPTIONS_ACTIVE))   ? new BitmapImage(new Uri(Defines.OPTIONS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Active"].Value   = (File.Exists(Defines.OPTIONS_ACTIVE))   ? new BitmapImage(new Uri(Defines.OPTIONS_ACTIVE,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Disabled"].Value = (File.Exists(Defines.OPTIONS_DISABLED)) ? new BitmapImage(new Uri(Defines.OPTIONS_DISABLED, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [General] - Images used across the program.
            ViewModelBase.PublicProperties["Images"]["General"]["Player"].Value = (File.Exists(Defines.GENERAL_PLAYER)) ? new BitmapImage(new Uri(Defines.GENERAL_PLAYER, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["General"]["Good"].Value   = (File.Exists(Defines.GENERAL_GOOD))   ? new BitmapImage(new Uri(Defines.GENERAL_GOOD,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["General"]["Bad"].Value    = (File.Exists(Defines.GENERAL_BAD))    ? new BitmapImage(new Uri(Defines.GENERAL_BAD,    UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["General"]["Warn"].Value   = (File.Exists(Defines.GENERAL_WARN))   ? new BitmapImage(new Uri(Defines.GENERAL_WARN,   UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["General"]["Notify"].Value = (File.Exists(Defines.GENERAL_NOTIFY)) ? new BitmapImage(new Uri(Defines.GENERAL_NOTIFY, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // [Connection] - Images associated with managing connections.
            ViewModelBase.PublicProperties["Images"]["Connection"]["Swap"].Value = (File.Exists(Defines.CONNECTION_SWAP)) ? new BitmapImage(new Uri(Defines.CONNECTION_SWAP, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;
            ViewModelBase.PublicProperties["Images"]["Connection"]["Info"].Value = (File.Exists(Defines.CONNECTION_INFO)) ? new BitmapImage(new Uri(Defines.CONNECTION_INFO, UriKind.RelativeOrAbsolute)) : ViewModelBase.PublicProperties["Images"]["Empty"].Value;

            // We done here broski.
            return true;
        }



        // -- [Interface][Add]
        private void interfaceAdd(Object[] parameters)
        {
            ExtensionApi.Procon.CreateInterface(
                (String)parameters[0],
                UInt16.Parse((String)parameters[1]),
                (String)parameters[2],
                (String)parameters[3]);
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
                (String)parameters[0],
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
                (String)parameters[0],
                (String)parameters[1],
                UInt16.Parse((String)parameters[2]),
                (String)parameters[3],
                (String)parameters[4]);
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
                (String)parameters[0],
                (String)parameters[1],
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

        


        //ViewModelBase.PublicCommands["Connection"]["Filter"]["Chat"].Value = new RelayCommand<Object>(filterChatChanged);
        //ViewModelBase.PublicCommands["Connection"]["Filter"]["Ban"].Value  = new RelayCommand<Object>(filterBanChanged);

        //ViewModelBase.PublicCommands["Connection"]["Action"]["Chat"].Value          = new RelayCommand<Object>(actionChat,     actionChatCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Player"].Value        = new RelayCommand<IList>(actionPlayer,    actionPlayerCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Map"]["Add"].Value    = new RelayCommand<IList>(actionMapAdd,    actionMapCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Map"]["Remove"].Value = new RelayCommand<IList>(actionMapRemove, actionMapCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Map"]["Up"].Value     = new RelayCommand<IList>(actionMapUp,     actionMapCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Map"]["Down"].Value   = new RelayCommand<IList>(actionMapDown,   actionMapCan);
        //ViewModelBase.PublicCommands["Connection"]["Action"]["Ban"].Value           = new RelayCommand<IList>(actionBan,       actionBanCan);   

        //private bool actionChatCan(Object nothing)
        //{
        //    return ActiveConnection != null;
        //}
        //private bool actionPlayerCan(IList players)
        //{
        //    return ActiveConnection != null && players != null && players.Count > 0;
        //}
        //private bool actionMapCan(IList maps)
        //{
        //    return ActiveConnection != null && maps != null && maps.Count > 0;
        //}
        //private bool actionBanCan(IList bans)
        //{
        //    return ActiveConnection != null;
        //}


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
        //        String          key   = (String)ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Data"].Value;
        //        FilterType      type  = (FilterType)ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Type"].Value;
        //        FilterChatField field = (FilterChatField)ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Field"].Value;

        //        // Add "Additional Filter" support here by doing things like:
        //        // [Code]
        //        //   Boolean fSpawn = (Boolean)ViewModelBase.PublicPropertyies[...]["Spawn"].Value;
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
        //            Text           = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value,
        //            ChatActionType = (ChatActionType)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Type"].Value,
        //            Subset         = new PlayerSubset()
        //            {
        //                Context = (PlayerSubsetContext)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"].Value,
        //                Team    = (Team)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value,
        //                Squad   = (Squad)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value,
        //                Player  = (Player)ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value
        //            }

        //        });
        //        ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value = String.Empty;
        //    }
        //    catch (Exception) { }
        //}
        //private void actionPlayer(IList players)
        //{
        //    try
        //    {
        //        switch ((ActionPlayerType)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Type"].Value)
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
        //                            Team    = (Team)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Team"].Value,
        //                            Squad   = (Squad)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value
        //                        },
        //                        Target = new Player()
        //                        {
        //                            UID    = pvm.Uid,
        //                            SlotID = pvm.SlotID,
        //                            Name   = pvm.Name,
        //                            IP     = pvm.IP
        //                        },
        //                        Reason = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
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
        //                        Reason = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
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
        //                        Reason = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
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
        //                            Context = (TimeSubsetContext)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value,
        //                            Length  = TimeSpan.ParseExact(
        //                                          (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value,
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
        //                        Reason = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
        //                    });
        //                break;
        //        }
        //    }
        //    catch (Exception) { }
        //}
        //private void actionMapAdd(IList maps)
        //{
        //    try
        //    {
        //        Int32 rounds = Int32.Parse((String)ViewModelBase.PublicProperties["Connection"]["Action"]["Map"]["Round"].Value);
        //        // Create a temp list to sort the maps we want to add.
        //        List<MapViewModel> sMaps = new List<MapViewModel>();
        //        foreach (MapViewModel map in maps)
        //            sMaps.Add(map);
        //        sMaps.Sort((x, y) => String.Compare(x.Name, y.Name));
        //        // Add the maps to the map list.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Name          = map.Name,
        //                Rounds        = rounds,
        //                MapActionType = MapActionType.Append
        //            });
        //    }
        //    catch (Exception) { }
        //}
        //private void actionMapRemove(IList maps)
        //{
        //    try
        //    {
        //        // Create a temp list to sort the maps we want to remove.
        //        List<MapViewModel> sMaps = new List<MapViewModel>();
        //        foreach (MapViewModel map in maps)
        //            sMaps.Add(map);
        //        sMaps.Sort((x, y) => y.Index - x.Index);
        //        // Remove the maps from the map list.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Index         = map.Index,
        //                MapActionType = MapActionType.RemoveIndex
        //            });
        //    }
        //    catch (Exception) { }
        //}
        //private void actionMapUp(IList maps)
        //{
        //    try
        //    {
        //        // Create a temp list to sort the maps we want to move up.
        //        List<MapViewModel> sMaps = new List<MapViewModel>();
        //        foreach (MapViewModel map in maps)
        //            sMaps.Add(map);
        //        sMaps.Sort((x, y) => y.Index - x.Index);
        //        // Remove the maps from the map list.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Index         = map.Index,
        //                MapActionType = MapActionType.RemoveIndex
        //            });
        //        sMaps.Sort((x, y) => x.Index - y.Index);
        //        // Add the selected items back 1 index up.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Name          = map.Name,
        //                Index         = map.Index - 1,
        //                MapActionType = MapActionType.Insert
        //            });
        //    }
        //    catch (Exception) { }
        //}
        //private void actionMapDown(IList maps)
        //{
        //    try
        //    {
        //        // Create a temp list to sort the maps we want to move down.
        //        List<MapViewModel> sMaps = new List<MapViewModel>();
        //        foreach (MapViewModel map in maps)
        //            sMaps.Add(map);
        //        sMaps.Sort((x, y) => y.Index - x.Index);
        //        // Remove the maps from the map list.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Index         = map.Index,
        //                MapActionType = MapActionType.RemoveIndex
        //            });
        //        sMaps.Sort((x, y) => x.Index - y.Index);
        //        // Add the selected items back 1 index up.
        //        foreach (MapViewModel map in sMaps)
        //            ActiveConnection.Action(new Map()
        //            {
        //                Name          = map.Name,
        //                Index         = map.Index + 1,
        //                MapActionType = MapActionType.Insert
        //            });
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
        //        switch ((ActionBanType)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Type"].Value)
        //        {
        //            // ------- ------- Ban Player ------- ------- //
        //            case ActionBanType.Ban:
        //                ActiveConnection.Action(new Ban()
        //                {
        //                    Target = new Player()
        //                    {
        //                        UID  = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value,
        //                        GUID = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value,
        //                        Name = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value
        //                    },
        //                    BanActionType = BanActionType.Ban,
        //                    Time          = new TimeSubset()
        //                    {
        //                        Context = (TimeSubsetContext)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Time"].Value,
        //                        Length  = TimeSpan.ParseExact(
        //                                      (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value,
        //                                      new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
        //                                      null)
        //                    },
        //                    Reason = (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Reason"].Value
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
        //                                          (String)ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value,
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
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Data"].Value  = String.Empty;
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Type"].Value  = FilterType.Contains;
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Field"].Value = FilterChatField.Data;

        ///* [Filter][Ban] - Contains information necessary to filter through bans.
        // *   [Data]  - The text to filter by.
        // *   [Type]  - The method used to filter with.
        // *   [Field] - The data to filter on. */
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Ban"]["Data"].Value  = String.Empty;
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Ban"]["Type"].Value  = FilterType.Contains;
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Ban"]["Field"].Value = FilterBanField.Id;

        ///* [Action][Chat] - Contains information necessary to send a message to a game server.
        // *   [Type]     - How to display the text.
        // *   [Subset]   - Who to display the text to.
        // *     [Team]   - Which team to display the text to.
        // *     [Squad]  - Which squad to display the text to.
        // *     [Player] - Which player to display the text to.
        // *   [Data]     - The text to send. */
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Type"].Value             = ChatActionType.Say;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"].Value           = PlayerSubsetContext.All;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value   = Team.Team1;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value  = Squad.None;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value = null;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value             = String.Empty;

        ///* [Action][Player] - Contains information necessary to perform player administrative actions.
        // *   [Type]        - The type of player action to perform.
        // *   [Move][Team]  - If moving player, the team to move them to.
        // *   [Move][Squad] - If moving player, the squad to move them to.
        // *   [Ban][Time]   - If banning player, the time context to ban them for.
        // *   [Ban][Length] - If banning player, the time length to ban them for.
        // *   [Reason]      - Why the action is being performed. */
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Type"].Value          = ActionPlayerType.Kill;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Team"].Value  = Team.Team1;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value = Squad.Squad1;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value = "1:00";
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value        = String.Empty;

        ///* [Action][Map] - Contains the information necessary to perform map administrative actions.
        // *   [Mode]  - UNSURE AS OF YET.
        // *   [Round] - The number of rounds a map should be added for. */
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Map"]["Mode"].Value  = String.Empty;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Map"]["Round"].Value = "2";

        ///* [Action][Ban] - Contains information necessary to perform ban administrative actions.
        // *   [Type]   - The type of ban action to perform.
        // *   [Uid]    - If banning player, the unique identifier of the player to ban.
        // *   [Time]   - If banning, the time context to ban for.
        // *   [Length] - If banning or to temp., the time length to ban them for.
        // *   [Reason] - If banning, why the action is being performed. */
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Type"].Value   = ActionBanType.Ban;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value    = String.Empty;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Time"].Value   = TimeSubsetContext.Permanent;
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value = "1:00";
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Reason"].Value = String.Empty;




        //// TYPES - Enumerations used for various reasons within the UI. //
        //// ------------------------------------------------------------ //
        //// Valid Game Types of connections that can be created.
        //ViewModelBase.PublicProperties["Connection"]["Add"]["Types"].Value  = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);

        //// Valid Filter Methods and Chat Fields that can be used to filter and filter on, respectively.
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Chat"]["Fields"].Value = Enum.GetValues(typeof(FilterChatField)).Cast<FilterChatField>().Where(x => true);

        //// Valid Filter Methods and Ban Fields that can be used to filter and filter on, respectively.
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Ban"]["Types"].Value  = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Filter"]["Ban"]["Fields"].Value = Enum.GetValues(typeof(FilterBanField)).Cast<FilterChatField>().Where(x => true);

        //// Valid Methods to display a chat message and subsets to send a chat message to.
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Types"].Value   = Enum.GetValues(typeof(ActionChatType)).Cast<ActionChatType>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Subsets"].Value = Enum.GetValues(typeof(PlayerSubsetContext)).Cast<PlayerSubsetContext>().Where(x => (x != PlayerSubsetContext.Server));
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Teams"].Value   = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Chat"]["Squads"].Value  = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);

        //// Valid Player Actions to take, and selections for various player actions.
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Types"].Value          = Enum.GetValues(typeof(ActionPlayerType)).Cast<ActionPlayerType>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Teams"].Value  = Enum.GetValues(typeof(Team)).Cast<Team>().Where(x => (x != Team.None));
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squads"].Value = Enum.GetValues(typeof(Squad)).Cast<Squad>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Times"].Value   = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None));

        //// Valid Ban Time Contexts for banning players.
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Types"].Value = Enum.GetValues(typeof(ActionBanType)).Cast<ActionBanType>().Where(x => true);
        //ViewModelBase.PublicProperties["Connection"]["Action"]["Ban"]["Times"].Value = Enum.GetValues(typeof(TimeSubsetContext)).Cast<TimeSubsetContext>().Where(x => (x != TimeSubsetContext.None) && (x != TimeSubsetContext.Round));
    }
}
