// Copyright 2011 Geoffrey 'Phogue' Green
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace Procon.Net.Console.Controls {
    public partial class EventPanel : UserControl {

        private Game m_activeGame;
        public Game ActiveGame {
            get {
                return this.m_activeGame;
            }
            set {
                this.m_activeGame = value;

                // Assign events.
                if (this.m_activeGame != null) {
                    this.m_activeGame.GameEvent += new Game.GameEventHandler(m_activeGame_GameEvent);
                    this.m_activeGame.ClientEvent += new Game.ClientEventHandler(m_activeGame_ClientEvent);
                }
            }
        }

        public EventPanel() {
            InitializeComponent();

            System.Timers.Timer listCleanup = new System.Timers.Timer(60000);
            listCleanup.Elapsed += new ElapsedEventHandler(listCleanup_Elapsed);
            listCleanup.Start();
        }

        private void listCleanup_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<object, ElapsedEventArgs>(this.listCleanup_Elapsed), sender, e);
                return;
            }

            this.lsvEventsList.BeginUpdate();

            var expiredList = this.lsvEventsList
                .Items
                .Cast<ListViewItem>()
                .Where(
                    x => ((x.Tag is GameEventArgs) && (x.Tag as GameEventArgs).Stamp < DateTime.Now.AddMinutes(-5)) ||
                         ((x.Tag is ClientEventArgs) && (x.Tag as ClientEventArgs).Stamp < DateTime.Now.AddMinutes(-5))
                ).Select(x => x);

            foreach (ListViewItem eventListItem in expiredList) {
                this.lsvEventsList.Items.Remove(eventListItem);
            }

            foreach (ColumnHeader column in this.lsvEventsList.Columns) {
                column.Width = -2;
            }

            this.lsvEventsList.EndUpdate();
        }

        private void RefreshEventList() {
            this.lsvEventsList.BeginUpdate();

            foreach (ListViewItem eventListItem in this.lsvEventsList.Items) {
                if (eventListItem.Tag is GameEventArgs) {
                    if (eventListItem.SubItems.Count <= 1) {
                        GameEventArgs e = eventListItem.Tag as GameEventArgs;
                        eventListItem.Text = e.Stamp.ToShortTimeString();

                        eventListItem.SubItems.AddRange(
                            new ListViewItem.ListViewSubItem[] {
                                new ListViewItem.ListViewSubItem() {
                                    Text = "GameEventArgs"
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.EventType.ToString()
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.GameState.Variables.ConnectionState.ToString()
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = String.Empty
                                }
                            }
                        );
                    }
                }
                else if (eventListItem.Tag is ClientEventArgs) {
                    if (eventListItem.SubItems.Count <= 1) {
                        ClientEventArgs e = eventListItem.Tag as ClientEventArgs;
                        eventListItem.Text = e.Stamp.ToShortTimeString();

                        eventListItem.SubItems.AddRange(
                            new ListViewItem.ListViewSubItem[] {
                                new ListViewItem.ListViewSubItem() {
                                    Text = "ClientEventArgs"
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.EventType.ToString()
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.ConnectionState.ToString()
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.ConnectionError != null ? e.ConnectionError.Message : String.Empty
                                }
                            }
                        );
                    }
                }
            }

            //foreach (ColumnHeader column in this.lsvEventsList.Columns) {
            //    column.Width = -2;
            //}

            this.lsvEventsList.EndUpdate();
        }

        private void m_activeGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.m_activeGame_ClientEvent), sender, e);
                return;
            }

            if (this.chkPostEvents.Checked == true) {
                this.lsvEventsList.Items.Add(
                    new ListViewItem() {
                        Tag = e
                    }
                );

                this.RefreshEventList();
            }
        }

        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (this.chkPostEvents.Checked == true) {
                this.lsvEventsList.Items.Add(
                    new ListViewItem() {
                        Tag = e
                    }
                );

                this.RefreshEventList();
            }
        }

        private void lsvEventsList_SelectedIndexChanged(object sender, EventArgs e) {

            if (this.lsvEventsList.SelectedItems.Count > 0) {
                this.quickActionsPropertyGrid.SelectedObject = this.lsvEventsList.SelectedItems[0].Tag;
            }

        }
    }
}
