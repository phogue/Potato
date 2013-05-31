using System;
using System.Windows;

namespace Procon.UI.Default
{
    using Procon.Net.Protocols.Objects;
    using Procon.UI.API;
    using Procon.UI.API.Commands;
    using Procon.UI.API.ViewModels;

    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Commands : IExtension
    {
        #region IExtension Properties

        public String Author
        { get { return "Imisnew2"; } }

        public Uri Link
        { get { return new Uri("www.TeamPlayerGaming.com/members/Imisnew2.html"); } }

        public String LinkText
        { get { return "Team Player Gaming"; } }

        public String Name
        { get { return "Commands"; } }

        public String Description
        { get { return ""; } }

        public Version Version
        { get { return new Version(1, 0, 0, 0); } }

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

            // [Chat] Level Commands.
            ExtensionApi.Commands["Chat"]["Send"].Value = new RelayCommand<Object[]>(chatSend, chatSendCan);

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



        // -- [Chat][Send]
        private void chatSend(Object[] parameters)
        {
            ExtensionApi.Connection.Action(
                new Chat() {
                    Text           = ((String)parameters[0]),
                    ChatActionType = ((ChatActionType)parameters[1]),
                    Subset         = ((PlayerSubset)parameters[2])
                });
        }
        private bool chatSendCan(Object[] parameters)
        {
            String       tString;
            PlayerSubset tSubset;
            return
                ExtensionApi.Connection != null && parameters.Length >= 3
                && (tString = parameters[0] as String)       != null && tString.Trim()  != String.Empty
                && parameters[1] is ChatActionType
                && (tSubset = parameters[2] as PlayerSubset) != null && tSubset.Context != PlayerSubsetContext.Server;
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
