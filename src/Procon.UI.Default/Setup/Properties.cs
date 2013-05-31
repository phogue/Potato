using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Procon.UI.Default.Setup
{
    using Procon.Net.Protocols;
    using Procon.UI.API;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Properties : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Properties"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

        #endregion IExtension Properties

        // Set up the extension.
        [STAThread]
        public bool Entry(Window root)
        {
            // [Procon] - Procon.
            ExtensionApi.Properties["Images"]["Procon"]["Icon"].Value  = GetImage(Defines.PROCON_ICON);
            ExtensionApi.Properties["Images"]["Procon"]["Large"].Value = GetImage(Defines.PROCON_LARGE);
            ExtensionApi.Properties["Images"]["Procon"]["Small"].Value = GetImage(Defines.PROCON_SMALL);
            ExtensionApi.Properties["Images"]["Procon"]["Text"].Value  = GetImage(Defines.PROCON_TEXT);

            // [Interface] - Interface types.
            ExtensionApi.Properties["Images"]["Interface"]["Local"].Value  = GetImage(Defines.INTERFACE_LOCAL);
            ExtensionApi.Properties["Images"]["Interface"]["Remote"].Value = GetImage(Defines.INTERFACE_REMOTE);

            // [Connection] - Connection types.
            ExtensionApi.Properties["Images"]["Connection"]["BF_3"].Value      = GetImage(Defines.CONNECTION_BF_3);
            ExtensionApi.Properties["Images"]["Connection"]["BF_BC2"].Value    = GetImage(Defines.CONNECTION_BF_BC2);
            ExtensionApi.Properties["Images"]["Connection"]["COD_BO"].Value    = GetImage(Defines.CONNECTION_COD_BO);
            ExtensionApi.Properties["Images"]["Connection"]["HOMEFRONT"].Value = GetImage(Defines.CONNECTION_HOMEFRONT);
            ExtensionApi.Properties["Images"]["Connection"]["MOH_2010"].Value  = GetImage(Defines.CONNECTION_MOH_2010);
            ExtensionApi.Properties["Images"]["Connection"]["TF_2"].Value      = GetImage(Defines.CONNECTION_TF_2);
            ExtensionApi.Properties["Images"]["Connection"]["UNK"].Value       = GetImage(Defines.CONNECTION_UNK);

            // [Status] - Connection status.
            ExtensionApi.Properties["Images"]["Status"]["LoggedIn"].Value      = GetImage(Defines.STATUS_GOOD);
            ExtensionApi.Properties["Images"]["Status"]["Connecting"].Value    = GetImage(Defines.STATUS_FLUX);
            ExtensionApi.Properties["Images"]["Status"]["Connected"].Value     = GetImage(Defines.STATUS_FLUX);
            ExtensionApi.Properties["Images"]["Status"]["Ready"].Value         = GetImage(Defines.STATUS_FLUX);
            ExtensionApi.Properties["Images"]["Status"]["Disconnecting"].Value = GetImage(Defines.STATUS_BAD);
            ExtensionApi.Properties["Images"]["Status"]["Disconnected"].Value  = GetImage(Defines.STATUS_BAD);
            ExtensionApi.Properties["Images"]["Status"]["Unknown"].Value       = GetImage(Defines.STATUS_UNK);

            // [Navigation] - Various images that represent tabs.
            ExtensionApi.Properties["Images"]["Navigation"]["LgRing"].Value   = GetImage(Defines.NAVIGATION_LGRING);
            ExtensionApi.Properties["Images"]["Navigation"]["SmRing"].Value   = GetImage(Defines.NAVIGATION_SMRING);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"].Value = GetImage(Defines.NAVIGATION_OVERVIEW);
            ExtensionApi.Properties["Images"]["Navigation"]["Players"].Value  = GetImage(Defines.NAVIGATION_PLAYERS);
            ExtensionApi.Properties["Images"]["Navigation"]["Maps"].Value     = GetImage(Defines.NAVIGATION_MAPS);
            ExtensionApi.Properties["Images"]["Navigation"]["Bans"].Value     = GetImage(Defines.NAVIGATION_BANS);
            ExtensionApi.Properties["Images"]["Navigation"]["Plugins"].Value  = GetImage(Defines.NAVIGATION_PLUGINS);
            ExtensionApi.Properties["Images"]["Navigation"]["Settings"].Value = GetImage(Defines.NAVIGATION_SETTINGS);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"]["Interfaces"].Value  = GetImage(Defines.NAVIGATION_INTERFACES);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"]["Connections"].Value = GetImage(Defines.NAVIGATION_CONNECTIONS);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"]["Interfaces"]["Accounts"].Value = GetImage(Defines.NAVIGATION_INTERFACES);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"]["Interfaces"]["Groups"].Value   = GetImage(Defines.NAVIGATION_INTERFACES);
            ExtensionApi.Properties["Images"]["Navigation"]["Overview"]["Interfaces"]["Layer"].Value    = GetImage(Defines.NAVIGATION_INTERFACES);

            // [General] - Images used across the program.
            ExtensionApi.Properties["Images"]["General"]["Add"].Value    = GetImage(Defines.GENERAL_ADD);
            ExtensionApi.Properties["Images"]["General"]["Edit"].Value   = GetImage(Defines.GENERAL_EDIT);
            ExtensionApi.Properties["Images"]["General"]["Remove"].Value = GetImage(Defines.GENERAL_REMOVE);
            ExtensionApi.Properties["Images"]["General"]["Player"].Value = GetImage(Defines.GENERAL_PLAYER);
            ExtensionApi.Properties["Images"]["General"]["Good"].Value   = GetImage(Defines.GENERAL_GOOD);
            ExtensionApi.Properties["Images"]["General"]["Bad"].Value    = GetImage(Defines.GENERAL_BAD);
            ExtensionApi.Properties["Images"]["General"]["Warn"].Value   = GetImage(Defines.GENERAL_WARN);
            ExtensionApi.Properties["Images"]["General"]["Notify"].Value = GetImage(Defines.GENERAL_NOTIFY);

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



            // Setup Enum Types.
            ExtensionApi.Properties["Types"]["Connection"].Value = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.None);
            
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



        // -- Omg why didn't I do this earlier, saves so much typing...
        private BitmapImage GetImage(String image)
        {
            return File.Exists(image)
                ? new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute))
                : new BitmapImage();
        }
    }
}
