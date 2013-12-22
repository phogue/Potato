﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Procon.Net.Actions;
using Procon.Net.Models;

namespace Procon.Tools.NetworkConsole.Controls {
    using Procon.Net;

    public partial class PlayerPanel : UserControl {

        private Game m_activeGame;
        public Game ActiveGame {
            get {
                return this.m_activeGame;
            }
            set {
                this.m_activeGame = value;

                // Assign events.
                if (this.m_activeGame != null) {
                    this.m_activeGame.GameEvent += m_activeGame_GameEvent;
                    this.m_activeGame.ClientEvent += m_activeGame_ClientEvent;
                }
            }
        }

        public PlayerPanel() {
            InitializeComponent();

            this.cboActions.Items.AddRange(
                new Object[] {
                    new Kill(),
                    new Kick(),
                    new Ban() {
                        ActionType = NetworkActionType.NetworkBan,
                        Time = new TimeSubset() {
                            Context = TimeSubsetContext.Permanent
                        }
                    },
                    new Move() {
                        ActionType = NetworkActionType.NetworkPlayerRotate
                    }
                    // new Move()
                }
            );

            this.cboActions.SelectedIndex = 0;
        }

        private void RefreshPlayerlist() {
            this.lsvPlayerlist.BeginUpdate();

            foreach (ListViewItem player in this.lsvPlayerlist.Items) {

                Player playerObject = player.Tag as Player;

                if (playerObject != null) {
                    if (playerObject.ClanTag.Length > 0) {
                        player.Text = String.Format("[{0}]{1}", playerObject.ClanTag, playerObject.Name);
                    }
                    else {
                        player.Text = playerObject.Name;
                    }

                    if (player.SubItems.Count <= 1) {

                        Grouping teamGroup = playerObject.Groups.FirstOrDefault(group => group.Type == Grouping.Team);

                        player.SubItems.AddRange(
                            new[] {
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Uid
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Score.ToString(CultureInfo.InvariantCulture)
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Kills.ToString(CultureInfo.InvariantCulture)
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Deaths.ToString(CultureInfo.InvariantCulture)
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Kdr.ToString(CultureInfo.InvariantCulture)
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = playerObject.Ping.ToString(CultureInfo.InvariantCulture)
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = teamGroup != null ? teamGroup.Uid.ToString(CultureInfo.InvariantCulture) : String.Empty
                                }
                            }
                        );
                    }
                    else {
                        Grouping teamGroup = playerObject.Groups.FirstOrDefault(group => group.Type == Grouping.Team);

                        player.SubItems[1].Text = playerObject.Uid;
                        player.SubItems[2].Text = playerObject.Score.ToString(CultureInfo.InvariantCulture);
                        player.SubItems[3].Text = playerObject.Kills.ToString(CultureInfo.InvariantCulture);
                        player.SubItems[4].Text = playerObject.Deaths.ToString(CultureInfo.InvariantCulture);
                        player.SubItems[5].Text = playerObject.Kdr.ToString(CultureInfo.InvariantCulture);
                        player.SubItems[6].Text = playerObject.Ping.ToString(CultureInfo.InvariantCulture);
                        player.SubItems[7].Text = teamGroup != null ? teamGroup.Uid.ToString(CultureInfo.InvariantCulture) : String.Empty;
                    }
                }
            }

            foreach (ColumnHeader column in this.lsvPlayerlist.Columns) {
                column.Width = -2;
            }

            this.lsvPlayerlist.EndUpdate();
        }

        private void m_activeGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.m_activeGame_ClientEvent), sender, e);
                return;
            }

            if (e.ConnectionState == ConnectionState.ConnectionConnected) {
                this.lsvPlayerlist.Items.Clear();
            }
        }

        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.GameEventType == GameEventType.GamePlayerlistUpdated) {

                foreach (Player player in e.GameState.Players) {

                    ListViewItem currentPlayer = this.lsvPlayerlist
                                                     .Items
                                                     .Cast<ListViewItem>()
                                                     .Where(x => x.Tag == player)
                                                     .FirstOrDefault();

                    if (currentPlayer == null) {
                        this.lsvPlayerlist.Items.Add(
                            new ListViewItem() {
                                Tag = player
                            }
                        );
                    }
                }

                this.RefreshPlayerlist();
            }
            else if (e.GameEventType == GameEventType.GamePlayerJoin) {
                this.lsvPlayerlist.Items.Add(
                    new ListViewItem() {
                        Tag = e.Now.Players.First()
                    }
                );

                this.RefreshPlayerlist();
            }
            else if (e.GameEventType == GameEventType.GamePlayerLeave) {
                ListViewItem player = this.lsvPlayerlist
                                          .Items
                                          .Cast<ListViewItem>()
                                          .Where(x => x.Tag == e.Now.Players.First())
                                          .FirstOrDefault();

                if (player != null) {
                    this.lsvPlayerlist.Items.Remove(player);
                }
            }
            else if (e.GameEventType == GameEventType.GamePlayerMoved) {
                this.RefreshPlayerlist();
            }
        }

        #region Quick Actions

        private void RefreshQuickAction() {
            if (this.cboActions.SelectedIndex >= 0 && this.lsvPlayerlist.SelectedItems.Count > 0) {

                if (this.cboActions.SelectedItem is Kill) {
                    Kill kill = this.cboActions.SelectedItem as Kill;

                    kill.Scope = new NetworkActionData() {
                        Players = new List<Player>() {
                            this.lsvPlayerlist.SelectedItems[0].Tag as Player
                        },
                        Content = new List<String>() {
                            this.txtReason.Text
                        }
                    };
                }
                else if (this.cboActions.SelectedItem is Kick) {
                    Kick kick = this.cboActions.SelectedItem as Kick;

                    kick.Scope = new NetworkActionData() {
                        Players = new List<Player>() {
                            this.lsvPlayerlist.SelectedItems[0].Tag as Player
                        },
                        Content = new List<String>() {
                            this.txtReason.Text
                        }
                    };
                }
                else if (this.cboActions.SelectedItem is Move) {
                    Move move = this.cboActions.SelectedItem as Move;
                    move.Scope.Players.Add(this.lsvPlayerlist.SelectedItems[0].Tag as Player);

                    move.Scope = new NetworkActionData() {
                        Content = new List<String>() {
                            this.txtReason.Text
                        }
                    };
                }
                else if (this.cboActions.SelectedItem is Ban) {
                    Ban ban = this.cboActions.SelectedItem as Ban;

                    ban.Scope = new NetworkActionData() {
                        Content = new List<String>() {
                            this.txtReason.Text
                        },
                        Players = new List<Player>() {
                            this.lsvPlayerlist.SelectedItems[0].Tag as Player
                        }
                    };
                }

                this.quickActionsPropertyGrid.Refresh();
            }
        }

        private void lsvPlayerlist_SelectedIndexChanged(object sender, EventArgs e) {
            this.grpQuick.Enabled = (this.lsvPlayerlist.SelectedItems.Count > 0);

            this.RefreshQuickAction();
        }

        private void btnAction_Click(object sender, EventArgs e) {
            if (this.ActiveGame != null && this.cboActions.SelectedIndex >= 0 && this.lsvPlayerlist.SelectedItems.Count > 0) {
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

        private void txtReason_TextChanged(object sender, EventArgs e) {
            this.RefreshQuickAction();
        }

        #endregion

        private void cboActions_DrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0) {
                if (this.cboActions.Items[e.Index] is Kill) {
                    e.Graphics.DrawString(
                        String.Format("Kill"),
                        this.cboActions.Font,
                        SystemBrushes.ControlText,
                        e.Bounds.Left + 5,
                        e.Bounds.Top
                    );
                }
                else if (this.cboActions.Items[e.Index] is Kick) {
                    e.Graphics.DrawString(
                        String.Format("Kick"),
                        this.cboActions.Font,
                        SystemBrushes.ControlText,
                        e.Bounds.Left + 5,
                        e.Bounds.Top
                    );
                }
                else if (this.cboActions.Items[e.Index] is Move) {
                    Move move = (this.cboActions.Items[e.Index] as Move);

                    e.Graphics.DrawString(
                        String.Format("Move.{0}", move.ActionType),
                        this.cboActions.Font,
                        SystemBrushes.ControlText,
                        e.Bounds.Left + 5,
                        e.Bounds.Top
                    );
                }
                else if (this.cboActions.Items[e.Index] is Ban) {
                    Ban ban = (this.cboActions.Items[e.Index] as Ban);

                    e.Graphics.DrawString(
                        String.Format("Ban.{0}.{1}", ban.ActionType, ban.Time != null ? ban.Time.Context : TimeSubsetContext.None),
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
