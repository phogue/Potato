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

            // Commands.
            // [Interface] Level Commands.
            ViewModelBase.PublicCommands["Interface"]["Add"].Value    = new RelayCommand<Object[]>(interfaceAdd, interfaceAddCan);
            ViewModelBase.PublicCommands["Interface"]["Remove"].Value = new RelayCommand<Object[]>(interfaceRemove, interfaceRemoveCan);
            ViewModelBase.PublicCommands["Interface"]["Set"].Value    = new RelayCommand<InterfaceViewModel>(interfaceSet);
            // [Connection] Level Commands.
            ViewModelBase.PublicCommands["Connection"]["Add"].Value    = new RelayCommand<Object[]>(connectionAdd, connectionAddCan);
            ViewModelBase.PublicCommands["Connection"]["Remove"].Value = new RelayCommand<Object[]>(connectionRemove, connectionRemoveCan);
            ViewModelBase.PublicCommands["Connection"]["Set"].Value    = new RelayCommand<ConnectionViewModel>(connectionSet);

            // Properties.
            // [Procon] - Procon images.
            ViewModelBase.PublicProperties["Images"]["Procon"]["Icon"].Value  = (File.Exists(Defines.PROCON_ICON))  ? new BitmapImage(new Uri(Defines.PROCON_ICON,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Procon"]["Large"].Value = (File.Exists(Defines.PROCON_LARGE)) ? new BitmapImage(new Uri(Defines.PROCON_LARGE, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Procon"]["Small"].Value = (File.Exists(Defines.PROCON_SMALL)) ? new BitmapImage(new Uri(Defines.PROCON_SMALL, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // Connection Types.
            // [Interfaces] - Interface types.
            ViewModelBase.PublicProperties["Images"]["Interfaces"]["Local"].Value  = (File.Exists(Defines.INTERFACE_LOCAL))  ? new BitmapImage(new Uri(Defines.INTERFACE_LOCAL,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Interfaces"]["Remote"].Value = (File.Exists(Defines.INTERFACE_REMOTE)) ? new BitmapImage(new Uri(Defines.INTERFACE_REMOTE, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            // [Connections] - Game types.
            ViewModelBase.PublicProperties["Images"]["Connections"]["BF_3"].Value      = (File.Exists(Defines.CONNECTION_BF_3))      ? new BitmapImage(new Uri(Defines.CONNECTION_BF_3,      UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connections"]["BF_BC2"].Value    = (File.Exists(Defines.CONNECTION_BF_BC2))    ? new BitmapImage(new Uri(Defines.CONNECTION_BF_BC2,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connections"]["COD_BO"].Value    = (File.Exists(Defines.CONNECTION_COD_BO))    ? new BitmapImage(new Uri(Defines.CONNECTION_COD_BO,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connections"]["HOMEFRONT"].Value = (File.Exists(Defines.CONNECTION_HOMEFRONT)) ? new BitmapImage(new Uri(Defines.CONNECTION_HOMEFRONT, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connections"]["MOH_2010"].Value  = (File.Exists(Defines.CONNECTION_MOH_2010))  ? new BitmapImage(new Uri(Defines.CONNECTION_MOH_2010,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connections"]["TF_2"].Value      = (File.Exists(Defines.CONNECTION_TF_2))      ? new BitmapImage(new Uri(Defines.CONNECTION_TF_2,      UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // Connection Status.
            // [Status] - Connection status.
            ViewModelBase.PublicProperties["Images"]["Status"]["Good"].Value = (File.Exists(Defines.STATUS_GOOD)) ? new BitmapImage(new Uri(Defines.STATUS_GOOD, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Status"]["Flux"].Value = (File.Exists(Defines.STATUS_FLUX)) ? new BitmapImage(new Uri(Defines.STATUS_FLUX, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Status"]["Bad"].Value  = (File.Exists(Defines.STATUS_BAD))  ? new BitmapImage(new Uri(Defines.STATUS_BAD,  UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // Content.
            // [Content] - Various images that represent tabs.
            //   [Players] - Represents the players per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Default"].Value  = (File.Exists(Defines.PLAYERS_DEFAULT))  ? new BitmapImage(new Uri(Defines.PLAYERS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Hover"].Value    = (File.Exists(Defines.PLAYERS_HOVER))    ? new BitmapImage(new Uri(Defines.PLAYERS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Press"].Value    = (File.Exists(Defines.PLAYERS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLAYERS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Active"].Value   = (File.Exists(Defines.PLAYERS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLAYERS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Players"]["Disabled"].Value = (File.Exists(Defines.PLAYERS_DISABLED)) ? new BitmapImage(new Uri(Defines.PLAYERS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            //   [Maps] - Represents the maps per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Default"].Value  = (File.Exists(Defines.MAPS_DEFAULT))  ? new BitmapImage(new Uri(Defines.MAPS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Hover"].Value    = (File.Exists(Defines.MAPS_HOVER))    ? new BitmapImage(new Uri(Defines.MAPS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Press"].Value    = (File.Exists(Defines.MAPS_ACTIVE))   ? new BitmapImage(new Uri(Defines.MAPS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Active"].Value   = (File.Exists(Defines.MAPS_ACTIVE))   ? new BitmapImage(new Uri(Defines.MAPS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Maps"]["Disabled"].Value = (File.Exists(Defines.MAPS_DISABLED)) ? new BitmapImage(new Uri(Defines.MAPS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            //   [Bans] - Represents the bans per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Default"].Value  = (File.Exists(Defines.BANS_DEFAULT))  ? new BitmapImage(new Uri(Defines.BANS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Hover"].Value    = (File.Exists(Defines.BANS_HOVER))    ? new BitmapImage(new Uri(Defines.BANS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Press"].Value    = (File.Exists(Defines.BANS_ACTIVE))   ? new BitmapImage(new Uri(Defines.BANS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Active"].Value   = (File.Exists(Defines.BANS_ACTIVE))   ? new BitmapImage(new Uri(Defines.BANS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Bans"]["Disabled"].Value = (File.Exists(Defines.BANS_DISABLED)) ? new BitmapImage(new Uri(Defines.BANS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            //   [Plugins] - Represents the plugins per connection.
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Default"].Value   = (File.Exists(Defines.PLUGINS_DEFAULT))  ? new BitmapImage(new Uri(Defines.PLUGINS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Hover"].Value     = (File.Exists(Defines.PLUGINS_HOVER))    ? new BitmapImage(new Uri(Defines.PLUGINS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Press"].Value     = (File.Exists(Defines.PLUGINS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLUGINS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Active"].Value    = (File.Exists(Defines.PLUGINS_ACTIVE))   ? new BitmapImage(new Uri(Defines.PLUGINS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Plugins"]["Disabled"].Value  = (File.Exists(Defines.PLUGINS_DISABLED)) ? new BitmapImage(new Uri(Defines.PLUGINS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            //   [Settings] - Represents the connection level settings.
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Default"].Value  = (File.Exists(Defines.SETTINGS_DEFAULT))  ? new BitmapImage(new Uri(Defines.SETTINGS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Hover"].Value    = (File.Exists(Defines.SETTINGS_HOVER))    ? new BitmapImage(new Uri(Defines.SETTINGS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Press"].Value    = (File.Exists(Defines.SETTINGS_ACTIVE))   ? new BitmapImage(new Uri(Defines.SETTINGS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Active"].Value   = (File.Exists(Defines.SETTINGS_ACTIVE))   ? new BitmapImage(new Uri(Defines.SETTINGS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Settings"]["Disabled"].Value = (File.Exists(Defines.SETTINGS_DISABLED)) ? new BitmapImage(new Uri(Defines.SETTINGS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            //   [Options] - Represents the interface level options.
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Default"].Value  = (File.Exists(Defines.OPTIONS_DEFAULT))  ? new BitmapImage(new Uri(Defines.OPTIONS_DEFAULT,  UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Hover"].Value    = (File.Exists(Defines.OPTIONS_HOVER))    ? new BitmapImage(new Uri(Defines.OPTIONS_HOVER,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Press"].Value    = (File.Exists(Defines.OPTIONS_ACTIVE))   ? new BitmapImage(new Uri(Defines.OPTIONS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Active"].Value   = (File.Exists(Defines.OPTIONS_ACTIVE))   ? new BitmapImage(new Uri(Defines.OPTIONS_ACTIVE,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Content"]["Options"]["Disabled"].Value = (File.Exists(Defines.OPTIONS_DISABLED)) ? new BitmapImage(new Uri(Defines.OPTIONS_DISABLED, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // [General] - Images used across the program.
            ViewModelBase.PublicProperties["Images"]["General"]["Player"].Value = (File.Exists(Defines.GENERAL_PLAYER)) ? new BitmapImage(new Uri(Defines.GENERAL_PLAYER, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["General"]["Good"].Value   = (File.Exists(Defines.GENERAL_GOOD))   ? new BitmapImage(new Uri(Defines.GENERAL_GOOD,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["General"]["Bad"].Value    = (File.Exists(Defines.GENERAL_BAD))    ? new BitmapImage(new Uri(Defines.GENERAL_BAD,    UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["General"]["Warn"].Value   = (File.Exists(Defines.GENERAL_WARN))   ? new BitmapImage(new Uri(Defines.GENERAL_WARN,   UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["General"]["Notify"].Value = (File.Exists(Defines.GENERAL_NOTIFY)) ? new BitmapImage(new Uri(Defines.GENERAL_NOTIFY, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // [Connection] - Images associated with managing connections.
            ViewModelBase.PublicProperties["Images"]["Connection"]["Swap"].Value = (File.Exists(Defines.CONNECTION_SWAP)) ? new BitmapImage(new Uri(Defines.CONNECTION_SWAP, UriKind.RelativeOrAbsolute)) : new BitmapImage();
            ViewModelBase.PublicProperties["Images"]["Connection"]["Info"].Value = (File.Exists(Defines.CONNECTION_INFO)) ? new BitmapImage(new Uri(Defines.CONNECTION_INFO, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // [Background] - Background images.
            //   [Navigation] - Navigation bg image.
            ViewModelBase.PublicProperties["Images"]["Background"]["Navigation"].Value = (File.Exists(Defines.BACKGROUND_NAVIGATION)) ? new BitmapImage(new Uri(Defines.BACKGROUND_NAVIGATION, UriKind.RelativeOrAbsolute)) : new BitmapImage();

            // We done here broski.
            return true;
        }

        // [Interface] Level Commands
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

        // [Connection] Level Commands
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
                && (tString = parameters[4] as String) != null && tString.Trim() != String.Empty;
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
                ExtensionApi.Interface != null && parameters.Length >= 5
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
