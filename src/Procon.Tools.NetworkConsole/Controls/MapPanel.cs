using System;
using System.Drawing;
using System.Windows.Forms;
using Procon.Net.Actions;

namespace Procon.Tools.NetworkConsole.Controls {
    using Procon.Net;

    public partial class MapPanel : UserControl {

        private Game _activeGame;
        public Game ActiveGame {
            get {
                return this._activeGame;
            }
            set {
                this._activeGame = value;

                // Assign events.
                if (this._activeGame != null) {
                    this._activeGame.GameEvent += m_activeGame_GameEvent;
                    this._activeGame.ClientEvent += m_activeGame_ClientEvent;
                }
            }
        }

        public MapPanel() {
            InitializeComponent();

            this.cboActions.Items.AddRange(
                new Object[] {
                    new Map() {
                        ActionType = NetworkActionType.NetworkMapNext
                    },
                    new Map() {
                        ActionType = NetworkActionType.NetworkMapRoundNext
                    },
                    new Map() {
                        ActionType = NetworkActionType.NetworkMapRestart
                    },
                    new Map() {
                        ActionType = NetworkActionType.NetworkMapRoundRestart
                    }
                }
            );

            this.cboActions.SelectedIndex = 0;
        }


        private void RefreshMapList() {
            this.lsvMapList.BeginUpdate();

            foreach (ListViewItem mapListItem in this.lsvMapList.Items) {

                Map mapObject = mapListItem.Tag as Map;

                if (mapObject != null) {

                    mapListItem.Text = mapObject.Index.ToString();

                    if (mapListItem.SubItems.Count <= 1) {
                        mapListItem.SubItems.AddRange(
                            new ListViewItem.ListViewSubItem[] {
                                new ListViewItem.ListViewSubItem() {
                                    Text = mapObject.Name
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = mapObject.GameMode.FriendlyName
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = mapObject.Rounds.ToString()
                                }
                            }
                        );
                    }
                    else {
                        mapListItem.SubItems[1].Text = mapObject.Name;
                        mapListItem.SubItems[2].Text = mapObject.GameMode.FriendlyName;
                        mapListItem.SubItems[3].Text = mapObject.Rounds.ToString();
                    }
                }
            }

            foreach (ColumnHeader column in this.lsvMapList.Columns) {
                column.Width = -2;
            }

            this.lsvMapList.EndUpdate();
        }

        private void m_activeGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.m_activeGame_ClientEvent), sender, e);
                return;
            }

            if (e.ConnectionState == ConnectionState.ConnectionConnected) {
                this.lsvMapList.Items.Clear();
            }
        }

        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.GameEventType == GameEventType.GameMaplistUpdated) {
                this.lsvMapList.Items.Clear();

                foreach (Map map in this.ActiveGame.State.Maps) {
                    this.lsvMapList.Items.Add(
                        new ListViewItem() {
                            Tag = map
                        }
                    );
                }

                this.RefreshMapList();
            }
        }

        private void RefreshQuickAction() {
            if (this.cboActions.SelectedIndex >= 0) {

                if (this.cboActions.SelectedItem is Map) {
                    this.quickActionsPropertyGrid.SelectedObject = this.cboActions.SelectedItem;
                }

                this.quickActionsPropertyGrid.Refresh();
            }
        }

        private void cboActions_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.cboActions.SelectedIndex >= 0) {
                this.quickActionsPropertyGrid.SelectedObject = this.cboActions.SelectedItem;

                this.RefreshQuickAction();
            }
        }

        private void btnAction_Click(object sender, EventArgs e) {
            if (this.ActiveGame != null && this.cboActions.SelectedIndex >= 0) {
                this.ActiveGame.Action((NetworkAction)this.cboActions.SelectedItem);

                this.RefreshQuickAction();
            }
        }

        private void cboActions_DrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0) {
                if (this.cboActions.Items[e.Index] is Map) {
                    e.Graphics.DrawString(
                        String.Format("Map.{0}", (this.cboActions.Items[e.Index] as Map).ActionType),
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
