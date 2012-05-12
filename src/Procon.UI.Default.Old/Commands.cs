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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

using Procon.Net.Protocols.Objects;
using Procon.UI.API;
using Procon.UI.API.Commands;
using Procon.UI.API.Enums;
using Procon.UI.API.Objects;
using Procon.UI.API.Utils;
using Procon.UI.API.ViewModels;

namespace Procon.UI.Default
{
    [Extension(
        Alters    = new String[] { },
        Replaces  = new String[] { },
        DependsOn = new String[] { })]
    public class Commands : IExtension
    {
        #region IExtension Properties

        public string Author
        { get { return "Imisnew2"; } }

        public string Link
        { get { return "www.TeamPlayerGaming.com/members/Imisnew2.html"; } }

        public string LinkText
        { get { return "Team Player Gaming"; } }

        public string Name
        { get { return "Default Commands"; } }

        public string Version
        { get { return "1.0.0.0"; } }

        public string Description
        { get { return ""; } }

        #endregion IExtension Properties

        [STAThread]
        public bool Entry(Window root)
        {
            // Setup the commands used by the (default) UI.
            SetupInstanceLevel();
            SetupInterfaceLevel();
            SetupConnectionLevel();

            // We done here broski.
            return true;
        }

        /// <summary>Easy Getter/Setter for [Procon] property.</summary>
        private InstanceViewModel ActiveInstance
        {
            get { return InstanceViewModel.PublicProperties["Procon"].Value as InstanceViewModel; }
            set { InstanceViewModel.PublicProperties["Procon"].Value = value; }
        }
        /// <summary>Easy Getter/Setter for [Interface] property.</summary>
        private InterfaceViewModel ActiveInterface
        {
            get { return InstanceViewModel.PublicProperties["Interface"].Value as InterfaceViewModel; }
            set { InstanceViewModel.PublicProperties["Interface"].Value = value; }
        }
        /// <summary>Easy Getter/Setter for [Connection] property.</summary>
        private ConnectionViewModel ActiveConnection
        {
            get { return InstanceViewModel.PublicProperties["Connection"].Value as ConnectionViewModel; }
            set { InstanceViewModel.PublicProperties["Connection"].Value = value; }
        }

        /// <summary>Sets up the [Instance] commands.</summary>
        private void SetupInstanceLevel()
        {
            InstanceViewModel.PublicCommands["SetPanel"].Value = new RelayCommand<Object>(panelSet);
        }
        /// <summary>Sets up the [Interface] commands.</summary>
        private void SetupInterfaceLevel()
        {
            InstanceViewModel.PublicCommands["Interface"]["Add"].Value    = new RelayCommand<InterfaceViewModel>(interfaceAdd,    interfaceAddCan);
            InstanceViewModel.PublicCommands["Interface"]["Remove"].Value = new RelayCommand<InterfaceViewModel>(interfaceRemove, interfaceRemoveCan);
            InstanceViewModel.PublicCommands["Interface"]["Set"].Value    = new RelayCommand<InterfaceViewModel>(interfaceSet);
        }
        /// <summary>Sets up the [Connection] commands.</summary>
        private void SetupConnectionLevel()
        {
            InstanceViewModel.PublicCommands["Connection"]["Add"].Value    = new RelayCommand<ConnectionViewModel>(connectionAdd,    connectionAddCan);
            InstanceViewModel.PublicCommands["Connection"]["Remove"].Value = new RelayCommand<ConnectionViewModel>(connectionRemove, connectionRemoveCan);
            InstanceViewModel.PublicCommands["Connection"]["Set"].Value = new RelayCommand<ConnectionViewModel>(connectionSet);

            InstanceViewModel.PublicCommands["Connection"]["Filter"]["Chat"].Value = new RelayCommand<Object>(filterChatChanged);
            InstanceViewModel.PublicCommands["Connection"]["Filter"]["Ban"].Value  = new RelayCommand<Object>(filterBanChanged);

            InstanceViewModel.PublicCommands["Connection"]["Action"]["Chat"].Value          = new RelayCommand<Object>(actionChat,     actionChatCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Player"].Value        = new RelayCommand<IList>(actionPlayer,    actionPlayerCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Map"]["Add"].Value    = new RelayCommand<IList>(actionMapAdd,    actionMapCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Map"]["Remove"].Value = new RelayCommand<IList>(actionMapRemove, actionMapCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Map"]["Up"].Value     = new RelayCommand<IList>(actionMapUp,     actionMapCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Map"]["Down"].Value   = new RelayCommand<IList>(actionMapDown,   actionMapCan);
            InstanceViewModel.PublicCommands["Connection"]["Action"]["Ban"].Value           = new RelayCommand<IList>(actionBan,       actionBanCan);   
        }

        #region Instance Level Commands

        private void panelSet(Object panel)
        {
            InstanceViewModel.PublicProperties["Panel"].Value = (panel == null) ? null : panel.GetType();
            if (panel is InterfaceViewModel)
                interfaceSet(panel as InterfaceViewModel);
            if (panel is ConnectionViewModel)
                connectionSet(panel as ConnectionViewModel);
        }

        #endregion Instance Level Commands
        #region Interface Level Commands

        #region Can

        private bool interfaceAddCan(InterfaceViewModel nothing)
        {
            return ActiveInstance != null;
        }
        private bool interfaceRemoveCan(InterfaceViewModel view)
        {
            return ActiveInstance != null && view != null;
        }

        #endregion Can
        #region Action

        private void interfaceAdd(InterfaceViewModel nothing)
        {
            try
            {
                ActiveInstance.AddInterface(
                  (String)InstanceViewModel.PublicProperties["Interface"]["Add"]["Hostname"].Value,
                  UInt16.Parse((String)InstanceViewModel.PublicProperties["Interface"]["Add"]["Port"].Value),
                  (String)InstanceViewModel.PublicProperties["Interface"]["Add"]["Username"].Value,
                  (String)InstanceViewModel.PublicProperties["Interface"]["Add"]["Password"].Value);
            }
            catch (Exception) { }
        }
        private void interfaceRemove(InterfaceViewModel view)
        {
            try
            {
                ActiveInstance.RemoveInterface(
                  view.Hostname,
                  view.Port);
            }
            catch (Exception) { }
        }
        private void interfaceSet(InterfaceViewModel view)
        {
            ActiveInterface = view;
        }

        #endregion Action

        #endregion Interface Level Commands
        #region Connection Level Commands

        #region Can

        private bool connectionAddCan(ConnectionViewModel nothing)
        {
            return ActiveInterface != null;
        }
        private bool connectionRemoveCan(ConnectionViewModel view)
        {
            return ActiveInterface != null && view != null;
        }

        private bool actionChatCan(Object nothing)
        {
            return ActiveConnection != null;
        }
        private bool actionPlayerCan(IList players)
        {
            return ActiveConnection != null && players != null && players.Count > 0;
        }
        private bool actionMapCan(IList maps)
        {
            return ActiveConnection != null && maps != null && maps.Count > 0;
        }
        private bool actionBanCan(IList bans)
        {
            return ActiveConnection != null;
        }

        #endregion Can
        #region Action

        private void connectionAdd(ConnectionViewModel nothing)
        {
            try
            {
                ActiveInterface.AddConnection(
                  (String)InstanceViewModel.PublicProperties["Connection"]["Add"]["Type"].Value.ToString(),
                  (String)InstanceViewModel.PublicProperties["Connection"]["Add"]["Hostname"].Value,
                  UInt16.Parse((String)InstanceViewModel.PublicProperties["Connection"]["Add"]["Port"].Value),
                  (String)InstanceViewModel.PublicProperties["Connection"]["Add"]["Password"].Value,
                  (String)InstanceViewModel.PublicProperties["Connection"]["Add"]["Additional"].Value);
            }
            catch (Exception) { }
        }
        private void connectionRemove(ConnectionViewModel view)
        {
            try
            {
                ActiveInterface.RemoveConnection(
                  view.GameType.ToString(),
                  view.Hostname,
                  view.Port);
            }
            catch (Exception) { }
        }
        private void connectionSet(ConnectionViewModel view)
        {
            ActiveConnection = view;
        }

        private void filterChatChanged(Object collection)
        {
            try
            {
                CollectionViewSource.GetDefaultView(collection).Filter = null;
                CollectionViewSource.GetDefaultView(collection).Filter = new Predicate<Object>(filterChat);
                CollectionViewSource.GetDefaultView(collection).Refresh();
            }
            catch (Exception) { }
        }
        private void filterBanChanged(Object collection)
        {
            try
            {
                CollectionViewSource.GetDefaultView(collection).Filter = null;
                CollectionViewSource.GetDefaultView(collection).Filter = new Predicate<Object>(filterBan);
                CollectionViewSource.GetDefaultView(collection).Refresh();
            }
            catch (Exception) { }
        }
        private bool filterChat(Object item)
        {
            try
            {
                Event           e     = (Event)item;
                String          key   = (String)InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Data"].Value;
                FilterType      type  = (FilterType)InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Type"].Value;
                FilterChatField field = (FilterChatField)InstanceViewModel.PublicProperties["Connection"]["Filter"]["Chat"]["Field"].Value;

                // Add "Additional Filter" support here by doing things like:
                // [Code]
                //   Boolean fSpawn = (Boolean)InstanceViewModel.PublicPropertyies[...]["Spawn"].Value;
                //   Boolean fChat  = ...
                //   ...
                // [End Code]
                // Where each public property represents if the value should be displayed.  E.g, True = display.
                // Then, add second if-statement before the first that evaulates if we want to even check this event type:
                // [Code]
                //   if ((e.EventType == EventType.Spawn && !fSpawn) || (e.EventType == EventType.Chat && !fChat) || ...)
                //      return false;
                // [End Code]

                if (key.Trim().Length > 0)
                    switch (type)
                    {
                        case FilterType.Contains:
                            switch (field)
                            {
                                case FilterChatField.Time:
                                    return e.Time.ToLower().Contains(key.ToLower());
                                case FilterChatField.Type:
                                    return e.Type.ToLower().Contains(key.ToLower());
                                case FilterChatField.Sender:
                                    return e.Sender.ToLower().Contains(key.ToLower());
                                case FilterChatField.Recipient:
                                    return e.Recipient.ToLower().Contains(key.ToLower());
                                case FilterChatField.Data:
                                    return e.Information.ToLower().Contains(key.ToLower());
                            }
                            break;
                        case FilterType.Excludes:
                            switch (field)
                            {
                                case FilterChatField.Time:
                                    return !e.Time.ToLower().Contains(key.ToLower());
                                case FilterChatField.Type:
                                    return !e.Type.ToLower().Contains(key.ToLower());
                                case FilterChatField.Sender:
                                    return !e.Sender.ToLower().Contains(key.ToLower());
                                case FilterChatField.Recipient:
                                    return !e.Recipient.ToLower().Contains(key.ToLower());
                                case FilterChatField.Data:
                                    return !e.Information.ToLower().Contains(key.ToLower());
                            }
                            break;
                        case FilterType.Matches:
                            switch (field)
                            {
                                case FilterChatField.Time:
                                    return e.Time.ToLower() == key.ToLower();
                                case FilterChatField.Type:
                                    return e.Type.ToLower() == key.ToLower();
                                case FilterChatField.Sender:
                                    return e.Sender.ToLower() == key.ToLower();
                                case FilterChatField.Recipient:
                                    return e.Recipient.ToLower() == key.ToLower();
                                case FilterChatField.Data:
                                    return e.Information.ToLower() == key.ToLower();
                            }
                            break;
                    }
                // If any problems, return valid.
                return true;
            }
            catch (Exception) { return true; }
        }
        private bool filterBan(Object item)
        {
            return true;
        }

        private void actionChat(Object nothing)
        {
            try
            {
                ActiveConnection.Action(new Chat()
                {
                    Text           = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value,
                    ChatActionType = (ChatActionType)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Type"].Value,
                    Subset         = new PlayerSubset()
                    {
                        Context = (PlayerSubsetContext)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"].Value,
                        Team    = (Team)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Team"].Value,
                        Squad   = (Squad)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Squad"].Value,
                        Player  = (Player)InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Subset"]["Player"].Value
                    }

                });
                InstanceViewModel.PublicProperties["Connection"]["Action"]["Chat"]["Data"].Value = String.Empty;
            }
            catch (Exception) { }
        }
        private void actionPlayer(IList players)
        {
            try
            {
                switch ((ActionPlayerType)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Type"].Value)
                {
                    // ------- ------- Move Player(s) ------- ------- //
                    case ActionPlayerType.Move:
                        foreach (PlayerViewModel pvm in players)
                            ActiveConnection.Action(new Move()
                            {
                                MoveActionType = MoveActionType.ForceMove,
                                Destination = new PlayerSubset()
                                {
                                    Context = PlayerSubsetContext.Squad,
                                    Team    = (Team)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Team"].Value,
                                    Squad   = (Squad)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Move"]["Squad"].Value
                                },
                                Target = new Player()
                                {
                                    UID    = pvm.Uid,
                                    SlotID = pvm.SlotID,
                                    Name   = pvm.Name,
                                    IP     = pvm.IP
                                },
                                Reason = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
                            });
                        break;
                    // ------- ------- Kill Player(s) ------- ------- //
                    case ActionPlayerType.Kill:
                        foreach (PlayerViewModel pvm in players)
                            ActiveConnection.Action(new Kill()
                            {
                                Target = new Player()
                                {
                                    UID    = pvm.Uid,
                                    SlotID = pvm.SlotID,
                                    Name   = pvm.Name,
                                    IP     = pvm.IP
                                },
                                Reason = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
                            });
                        break;
                    // ------- ------- Kick Player(s) ------- ------- //
                    case ActionPlayerType.Kick:
                        foreach (PlayerViewModel pvm in players)
                            ActiveConnection.Action(new Kick()
                            {
                                Target = new Player()
                                {
                                    UID    = pvm.Uid,
                                    SlotID = pvm.SlotID,
                                    Name   = pvm.Name,
                                    IP     = pvm.IP
                                },
                                Reason = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
                            });
                        break;
                    // ------- ------- Ban Player(s) ------- ------- //
                    case ActionPlayerType.Ban:
                        foreach (PlayerViewModel pvm in players)
                            ActiveConnection.Action(new Ban()
                            {
                                BanActionType = BanActionType.Ban,
                                Time = new TimeSubset()
                                {
                                    Context = (TimeSubsetContext)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Time"].Value,
                                    Length  = TimeSpan.ParseExact(
                                                  (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Ban"]["Length"].Value,
                                                  new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
                                                  null)
                                },
                                Target = new Player()
                                {
                                    UID    = pvm.Uid,
                                    SlotID = pvm.SlotID,
                                    Name   = pvm.Name,
                                    IP     = pvm.IP
                                },
                                Reason = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Player"]["Reason"].Value
                            });
                        break;
                }
            }
            catch (Exception) { }
        }
        private void actionMapAdd(IList maps)
        {
            try
            {
                Int32 rounds = Int32.Parse((String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Map"]["Round"].Value);
                // Create a temp list to sort the maps we want to add.
                List<MapViewModel> sMaps = new List<MapViewModel>();
                foreach (MapViewModel map in maps)
                    sMaps.Add(map);
                sMaps.Sort((x, y) => String.Compare(x.Name, y.Name));
                // Add the maps to the map list.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Name          = map.Name,
                        Rounds        = rounds,
                        MapActionType = MapActionType.Append
                    });
            }
            catch (Exception) { }
        }
        private void actionMapRemove(IList maps)
        {
            try
            {
                // Create a temp list to sort the maps we want to remove.
                List<MapViewModel> sMaps = new List<MapViewModel>();
                foreach (MapViewModel map in maps)
                    sMaps.Add(map);
                sMaps.Sort((x, y) => y.Index - x.Index);
                // Remove the maps from the map list.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Index         = map.Index,
                        MapActionType = MapActionType.RemoveIndex
                    });
            }
            catch (Exception) { }
        }
        private void actionMapUp(IList maps)
        {
            try
            {
                // Create a temp list to sort the maps we want to move up.
                List<MapViewModel> sMaps = new List<MapViewModel>();
                foreach (MapViewModel map in maps)
                    sMaps.Add(map);
                sMaps.Sort((x, y) => y.Index - x.Index);
                // Remove the maps from the map list.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Index         = map.Index,
                        MapActionType = MapActionType.RemoveIndex
                    });
                sMaps.Sort((x, y) => x.Index - y.Index);
                // Add the selected items back 1 index up.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Name          = map.Name,
                        Index         = map.Index - 1,
                        MapActionType = MapActionType.Insert
                    });
            }
            catch (Exception) { }
        }
        private void actionMapDown(IList maps)
        {
            try
            {
                // Create a temp list to sort the maps we want to move down.
                List<MapViewModel> sMaps = new List<MapViewModel>();
                foreach (MapViewModel map in maps)
                    sMaps.Add(map);
                sMaps.Sort((x, y) => y.Index - x.Index);
                // Remove the maps from the map list.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Index         = map.Index,
                        MapActionType = MapActionType.RemoveIndex
                    });
                sMaps.Sort((x, y) => x.Index - y.Index);
                // Add the selected items back 1 index up.
                foreach (MapViewModel map in sMaps)
                    ActiveConnection.Action(new Map()
                    {
                        Name          = map.Name,
                        Index         = map.Index + 1,
                        MapActionType = MapActionType.Insert
                    });
            }
            catch (Exception) { }
        }
        private void actionBan(IList bans)
        {
            try
            {
                // Save a copy just incase the selection changes.
                List<BanViewModel> sBans = new List<BanViewModel>();
                foreach (BanViewModel bvm in bans)
                    sBans.Add(bvm);
                switch ((ActionBanType)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Type"].Value)
                {
                    // ------- ------- Ban Player ------- ------- //
                    case ActionBanType.Ban:
                        ActiveConnection.Action(new Ban()
                        {
                            Target = new Player()
                            {
                                UID  = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value,
                                GUID = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value,
                                Name = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Uid"].Value
                            },
                            BanActionType = BanActionType.Ban,
                            Time          = new TimeSubset()
                            {
                                Context = (TimeSubsetContext)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Time"].Value,
                                Length  = TimeSpan.ParseExact(
                                              (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value,
                                              new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
                                              null)
                            },
                            Reason = (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Reason"].Value
                        });
                        break;
                    // ------- ------- Unban Player(s) ------- ------- //
                    case ActionBanType.Unban:
                        foreach (BanViewModel bvm in sBans)
                            ActiveConnection.Action(new Ban()
                            {
                                Target        = bvm.Target,
                                BanActionType = BanActionType.Unban
                            });
                        break;
                    // ------- ------- Convert Ban(s) to Permanent ------- ------- //
                    case ActionBanType.ToPermanent:
                        foreach (BanViewModel bvm in sBans)
                        {
                            ActiveConnection.Action(new Ban()
                            {
                                Target        = bvm.Target,
                                BanActionType = BanActionType.Unban
                            });
                            ActiveConnection.Action(new Ban()
                            {
                                Target        = bvm.Target,
                                BanActionType = BanActionType.Ban,
                                Time          = new TimeSubset()
                                {
                                    Context = TimeSubsetContext.Permanent
                                },
                                Reason = bvm.Reason
                            });
                        }
                        break;
                    // ------- ------- Convert Ban(s) to Temporary ------- ------- //
                    case ActionBanType.ToTemporary:
                        foreach (BanViewModel bvm in sBans)
                        {
                            ActiveConnection.Action(new Ban()
                            {
                                Target        = bvm.Target,
                                BanActionType = BanActionType.Unban
                            });
                            ActiveConnection.Action(new Ban()
                            {
                                Target        = bvm.Target,
                                BanActionType = BanActionType.Ban,
                                Time          = new TimeSubset()
                                {
                                    Context = TimeSubsetContext.Time,
                                    Length  = TimeSpan.ParseExact(
                                                  (String)InstanceViewModel.PublicProperties["Connection"]["Action"]["Ban"]["Length"].Value,
                                                  new String[] { "%d\\:%h\\:%m", "%h\\:%m", "%m" },
                                                  null)
                                },
                                Reason = bvm.Reason
                            });
                        }
                        break;
                }
            }
            catch (Exception) { }
        }

        #endregion Action

        #endregion Connection Level Commands
    }
}
