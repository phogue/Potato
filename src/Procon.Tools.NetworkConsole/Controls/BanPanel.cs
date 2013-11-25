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

namespace Procon.Tools.NetworkConsole.Controls {
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public partial class BanPanel : UserControl {

        private Game activeGame;
        public Game ActiveGame {
            get {
                return this.activeGame;
            }
            set {
                this.activeGame = value;

                // Assign events.
                if (this.activeGame != null) {
                    this.activeGame.GameEvent += new Game.GameEventHandler(m_activeGame_GameEvent);
                    this.activeGame.ClientEvent += new Game.ClientEventHandler(m_activeGame_ClientEvent);
                }
            }
        }

        public BanPanel() {
            InitializeComponent();

            this.cboActions.Items.AddRange(
                new Object[] {
                    new Ban() {
                        ActionType = NetworkActionType.NetworkUnban,
                        Time = new TimeSubset() {
                            Context = TimeSubsetContext.None
                        }
                    }
                }
            );

            this.cboActions.SelectedIndex = 0;
        }

        private string BanId(Ban ban) {
            List<string> text = new List<string>() {
                ban.Scope.Players.First().Name,
                ban.Scope.Players.First().Ip,
                ban.Scope.Players.First().Uid
            };

            text.RemoveAll(x => String.IsNullOrEmpty(x) == true);

            return String.Join(",", text.ToArray());
        }

        private void RefreshBanList() {
            this.lsvBanList.BeginUpdate();

            foreach (ListViewItem banListItem in this.lsvBanList.Items) {

                Ban banObject = banListItem.Tag as Ban;

                if (banObject != null) {

                    banListItem.Text = this.BanId(banObject);

                    if (banListItem.SubItems.Count <= 1) {
                        banListItem.SubItems.AddRange(
                            new ListViewItem.ListViewSubItem[] {
                                new ListViewItem.ListViewSubItem() {
                                    Text = banObject.Time != null ? banObject.Time.Context.ToString() : String.Empty
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = banObject.Scope.Content != null ? banObject.Scope.Content.FirstOrDefault() : String.Empty
                                }
                            }
                        );
                    }
                    else {
                        banListItem.SubItems[1].Text = banObject.Time != null ? banObject.Time.Context.ToString() : String.Empty;
                        banListItem.SubItems[2].Text = banObject.Scope.Content != null ? banObject.Scope.Content.FirstOrDefault() : String.Empty;
                    }
                }
            }

            foreach (ColumnHeader column in this.lsvBanList.Columns) {
                column.Width = -2;
            }

            this.lsvBanList.EndUpdate();
        }

        private void m_activeGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.m_activeGame_ClientEvent), sender, e);
                return;
            }

            if (e.ConnectionState == ConnectionState.ConnectionConnected) {
                this.lsvBanList.Items.Clear();
            }
        }


        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.GameEventType == GameEventType.GameBanlistUpdated) {

                foreach (Ban ban in e.GameState.BanList) {

                    ListViewItem listBan = this.lsvBanList
                                               .Items
                                               .Cast<ListViewItem>()
                                               .Where(x => x.Tag == ban)
                                               .FirstOrDefault();

                    if (listBan == null) {
                        this.lsvBanList.Items.Add(
                            new ListViewItem() {
                                Tag = ban
                            }
                        );
                    }
                }

                this.RefreshBanList();
            }
            else if (e.GameEventType == GameEventType.GamePlayerUnbanned) {

                ListViewItem listBan = (
                    from x in this.lsvBanList.Items.Cast<ListViewItem>()
                    let unbannedId = this.BanId(e.Now.Bans.First()) 
                    where this.BanId(x.Tag as Ban) == unbannedId
                    select x
                ).FirstOrDefault();

                if (listBan != null) {
                    this.lsvBanList.Items.Remove(listBan);
                }

                this.RefreshBanList();
            }
            else if (e.GameEventType == GameEventType.GamePlayerBanned) {

                this.lsvBanList.Items.Add(
                    new ListViewItem() {
                        Tag = e.Now.Bans.First()
                    }
                );

                this.RefreshBanList();
            }
        }


        #region Quick Actions

        private void RefreshQuickAction() {
            if (this.cboActions.SelectedIndex >= 0 && this.lsvBanList.SelectedItems.Count > 0) {

                if (this.cboActions.SelectedItem is Ban) {
                    Ban unban = this.cboActions.SelectedItem as Ban;

                    unban.Scope.Players = new List<Player>() {
                        (this.lsvBanList.SelectedItems[0].Tag as Ban).Scope.Players.First()
                    };
                }

                this.quickActionsPropertyGrid.Refresh();
            }
        }

        private void lsvPlayerlist_SelectedIndexChanged(object sender, EventArgs e) {
            this.grpQuick.Enabled = (this.lsvBanList.SelectedItems.Count > 0);

            this.RefreshQuickAction();
        }

        private void btnAction_Click(object sender, EventArgs e) {
            if (this.ActiveGame != null && this.cboActions.SelectedIndex >= 0 && this.lsvBanList.SelectedItems.Count > 0) {
                this.ActiveGame.Action((NetworkAction)this.cboActions.SelectedItem);

                this.RefreshQuickAction();
            }
        }

        private void cboActions_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.cboActions.SelectedIndex >= 0) {
                this.quickActionsPropertyGrid.SelectedObject = this.cboActions.SelectedItem;

                this.RefreshQuickAction();
            }
        }

        #endregion

        private void cboActions_DrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0) {
                if (this.cboActions.Items[e.Index] is Ban) {
                    e.Graphics.DrawString(
                        String.Format("Ban.{0}", (this.cboActions.Items[e.Index] as Ban).ActionType),
                        this.cboActions.Font,
                        SystemBrushes.ControlText,
                        e.Bounds.Left + 5,
                        e.Bounds.Top
                    );
                }
            }
        }
    }
}
