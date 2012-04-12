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

namespace Procon.Net.Console.Controls {
    using Procon.Net.Protocols.Objects;

    public partial class BanPanel : UserControl {

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

        public BanPanel() {
            InitializeComponent();

            this.cboActions.Items.AddRange(
                new Object[] {
                    new Ban() {
                        BanActionType = BanActionType.Unban,
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
                ban.Target.Name,
                ban.Target.IP,
                ban.Target.GUID
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
                                    Text = banObject.Reason
                                }
                            }
                        );
                    }
                    else {
                        banListItem.SubItems[1].Text = banObject.Time != null ? banObject.Time.Context.ToString() : String.Empty;
                        banListItem.SubItems[2].Text = banObject.Reason;
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

            if (e.ConnectionState == ConnectionState.Connected) {
                this.lsvBanList.Items.Clear();
            }
        }


        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.EventType == GameEventType.BanlistUpdated) {

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
            else if (e.EventType == GameEventType.PlayerUnbanned) {

                ListViewItem listBan = (
                    from x in this.lsvBanList.Items.Cast<ListViewItem>()
                    let unbannedId = this.BanId(e.Ban) 
                    where this.BanId(x.Tag as Ban) == unbannedId
                    select x
                ).FirstOrDefault();

                if (listBan != null) {
                    this.lsvBanList.Items.Remove(listBan);
                }

                this.RefreshBanList();
            }
            else if (e.EventType == GameEventType.PlayerBanned) {

                this.lsvBanList.Items.Add(
                    new ListViewItem() {
                        Tag = e.Ban
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

                    unban.Target = (this.lsvBanList.SelectedItems[0].Tag as Ban).Target;
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
                this.ActiveGame.Action((ProtocolObject)this.cboActions.SelectedItem);

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
                        String.Format("Ban.{0}", (this.cboActions.Items[e.Index] as Ban).BanActionType),
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
