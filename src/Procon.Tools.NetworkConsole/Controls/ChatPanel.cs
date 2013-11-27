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
using Procon.Net.Actions;

namespace Procon.Tools.NetworkConsole.Controls {
    using Procon.Net;

    public partial class ChatPanel : UserControl {

        // Console
        private LinkedList<string> m_chatHistory;
        private LinkedListNode<string> m_chatHistoryCurrentNode;

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
                }
            }
        }

        public ChatPanel() {
            InitializeComponent();

            this.m_chatHistory = new LinkedList<string>();

            System.Timers.Timer listCleanup = new System.Timers.Timer(60000);
            listCleanup.Elapsed += new ElapsedEventHandler(listCleanup_Elapsed);
            listCleanup.Start();
        }

        private void listCleanup_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<object, ElapsedEventArgs>(this.listCleanup_Elapsed), sender, e);
                return;
            }

            this.lsvChatObjects.BeginUpdate();

            var expiredList = this.lsvChatObjects
                .Items
                .Cast<ListViewItem>()
                .Where(
                    x => ((x.Tag is NetworkObject) && (x.Tag as NetworkObject).Created < DateTime.Now.AddMinutes(-5))
                ).Select(x => x);

            foreach (ListViewItem eventListItem in expiredList) {
                this.lsvChatObjects.Items.Remove(eventListItem);
            }

            foreach (ColumnHeader column in this.lsvChatObjects.Columns) {
                column.Width = -2;
            }

            this.lsvChatObjects.EndUpdate();
        }

        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.GameEventType == GameEventType.GameChat) {
                Chat chat = e.Now.Chats.First();

                if (chat != null) {

                    #region Chat Object

                    ListViewItem item = new ListViewItem() {
                        Text = chat.Created.ToShortTimeString(),
                        Tag = chat
                    };

                    item.SubItems.AddRange(
                        new ListViewItem.ListViewSubItem[] {
                            new ListViewItem.ListViewSubItem() {
                                Text = chat.Origin.ToString()
                            },
                            new ListViewItem.ListViewSubItem() {
                                Text = chat.Now.Players.First().Name
                            },
                            new ListViewItem.ListViewSubItem() {
                                Text = chat.Now.Content.First()
                            }
                        }
                    );

                    this.lsvChatObjects.Items.Add(item);

                    #endregion

                    #region Faux Chat Box

                    if (chat.Origin == ChatOrigin.Player) {
                        this.rtbChat.AppendText(String.Format("^0[{0}] ^2{1}: {2}{3}", chat.Created.ToShortTimeString(), chat.Now.Players.First().Name, chat.Now.Content.First(), Environment.NewLine));
                    }
                    else if (chat.Origin == ChatOrigin.Reflected) {
                        this.rtbChat.AppendText(String.Format("^0[{0}]: ^4{1}{2}", chat.Created.ToShortTimeString(), chat.Now.Content.First(), Environment.NewLine));
                    }

                    this.rtbChat.ScrollToCaret();

                    this.rtbChat.ReadOnly = false;
                    int consoleBoxLines = this.rtbChat.Lines.Length;

                    if (consoleBoxLines > 100 && this.rtbChat.Focused == false) {
                        for (int i = 0; i < consoleBoxLines - 100; i++) {
                            this.rtbChat.Select(0, this.rtbChat.Lines[0].Length + 1);
                            this.rtbChat.SelectedText = String.Empty;
                        }
                    }

                    this.rtbChat.ReadOnly = true;

                    #endregion
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            this.chatPropertyGrid.SelectedObject = new Chat() {

            };
        }

        private void lsvChatObjects_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.lsvChatObjects.SelectedItems.Count > 0) {
                this.chatPropertyGrid.SelectedObject = this.lsvChatObjects.SelectedItems[0].Tag;
            }
        }

        private void btnPost_Click(object sender, EventArgs e) {
            if (this.ActiveGame != null && this.chatPropertyGrid.SelectedObject != null && this.chatPropertyGrid.SelectedObject is Chat) {
                this.ActiveGame.Action(this.chatPropertyGrid.SelectedObject as Chat);
            }
        }

        #region Quick Announce

        private void btnQuickAnnounce_Click(object sender, EventArgs e) {

            if (this.ActiveGame != null) {
                this.ActiveGame.Action(
                    new Chat() {
                        Now = new NetworkActionData() {
                            Content = new List<string>() {
                                this.txtQuickAnnounce.Text
                            }
                        }
                    }
                );

                this.m_chatHistory.AddFirst(this.txtQuickAnnounce.Text);
                if (this.m_chatHistory.Count > 20) {
                    this.m_chatHistory.RemoveLast();
                }
                this.m_chatHistoryCurrentNode = null;

                this.txtQuickAnnounce.Clear();
                this.txtQuickAnnounce.Focus();
            }
        }

        private void txtQuickAnnounce_TextChanged(object sender, EventArgs e) {
            this.btnQuickAnnounce.Enabled = (this.txtQuickAnnounce.Text.Length > 0);
        }

        private void txtQuickAnnounce_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {

                this.btnQuickAnnounce_Click(null, null);

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Up) {
                e.SuppressKeyPress = true;

                if (this.m_chatHistoryCurrentNode == null && this.m_chatHistory.First != null) {
                    this.m_chatHistoryCurrentNode = this.m_chatHistory.First;
                    this.txtQuickAnnounce.Text = this.m_chatHistoryCurrentNode.Value;

                    this.txtQuickAnnounce.Select(this.txtQuickAnnounce.Text.Length, 0);
                }
                else if (this.m_chatHistoryCurrentNode != null && this.m_chatHistoryCurrentNode.Next != null) {
                    this.m_chatHistoryCurrentNode = this.m_chatHistoryCurrentNode.Next;
                    this.txtQuickAnnounce.Text = this.m_chatHistoryCurrentNode.Value;

                    this.txtQuickAnnounce.Select(this.txtQuickAnnounce.Text.Length, 0);
                }
            }
            else if (e.KeyData == Keys.Down) {

                if (this.m_chatHistoryCurrentNode != null && this.m_chatHistoryCurrentNode.Previous != null) {
                    this.m_chatHistoryCurrentNode = this.m_chatHistoryCurrentNode.Previous;
                    this.txtQuickAnnounce.Text = this.m_chatHistoryCurrentNode.Value;

                    this.txtQuickAnnounce.Select(this.txtQuickAnnounce.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        #endregion
    }
}
