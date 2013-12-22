using System;
using System.Linq;
using System.Windows.Forms;
using System.Timers;

namespace Procon.Tools.NetworkConsole.Controls {
    using Procon.Net;
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
                    this.m_activeGame.GameEvent += m_activeGame_GameEvent;
                    this.m_activeGame.ClientEvent += m_activeGame_ClientEvent;
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
                                    Text = e.GameEventType.ToString()
                                },
                                new ListViewItem.ListViewSubItem() {
                                    Text = e.GameState.Settings.Current.ConnectionState.ToString()
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
                                    Text = e.Now.Exceptions != null && e.Now.Exceptions.Any() == true ? e.Now.Exceptions.FirstOrDefault().Message : String.Empty
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

        private void m_activeGame_ClientEvent(IGame sender, ClientEventArgs e) {
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

        private void m_activeGame_GameEvent(IGame sender, GameEventArgs e) {
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
