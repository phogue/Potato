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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Packages;
using Procon.Core.Interfaces.Security.Objects;
using Procon.Core.Interfaces.Variables;
using Procon.Net;
using Procon.Net.Protocols;
using Procon.Net.Utils;
using Procon.UI.Old.Classes;

namespace Procon.UI.Old.Windows
{
    // TODO LIST:
    // - Listen to event for when account is created to set the password.
    // - Make Layer UI bound to the Layers values.
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Maintains a reference to all Game Windows currently open.  This is useful for two reasons:
        /// 1- Information such as chat logs are not lost when the user closes the game window.  The
        ///   user can simply re-open the game window and all information tracked in the meantime will
        ///   be displayed.
        /// 2- When shutting down procon, hidden windows can be closed so that procon can finish shutting down.
        /// </summary>
        private Dictionary<Connection, GameWindow> mGameWindows = new Dictionary<Connection, GameWindow>();
        /// <summary>
        /// Remembers what password the user entered when creating a new account so the password can
        /// be applied once the account has been added.  This is used because with remote layers, the
        /// account is not guaranteed to exist until the account added event is fired.
        /// </summary>
        private Dictionary<String, String> mNewAccounts = new Dictionary<String, String>();

        /// <summary>
        /// Gets/Sets the name of the layer so that controls in the UI can bind to the layername directly.
        /// </summary>
        public String LayerName
        {
            get
            {
                if (combobox_Interfaces.SelectedItem is LocalInterface)
                    return (combobox_Interfaces.SelectedItem as Interface).Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.LayerName);
                return String.Empty;
            }
            set
            {
                if (combobox_Interfaces.SelectedItem is LocalInterface)
                {
                    (combobox_Interfaces.SelectedItem as Interface).Variables.Set(CommandInitiator.Local, CommonVariableNames.LayerName, value);
                    (combobox_Interfaces.SelectedItem as Interface).Layer.Hostname = (combobox_Interfaces.SelectedItem as Interface).Layer.Hostname;
                }

            }
        }

        /// <summary>
        /// Creates an instance of Procon and constructs the UI to aid the user
        /// in completing actions such as connecting to procon layers, managing game
        /// connections, adding/removing accounts and groups, managing interface settings,
        /// and managing their local layer.
        /// </summary>
        public MainWindow()
        {
            // Setup the heart of procon.
            Instance i = new Instance();
            i.Execute();
            DataContext = i;

            // Initialize UI.
            InitializeComponent();
            if (combobox_Interfaces.Items.Count > 0)
                combobox_Interfaces.SelectedIndex = 0;
            foreach (Interface inter in i.Interfaces)
                inter.Security.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);

            #region Localization

            #region Tab Items

            tabitem_Connections.Header      = Local.Loc.Loc(null, "Procon.UI.MainWindow.Connections", "Title");
            tabitem_Groups.Header           = Local.Loc.Loc(null, "Procon.UI.MainWindow.Groups",      "Title");
            tabitem_Accounts.Header         = Local.Loc.Loc(null, "Procon.UI.MainWindow.Accounts",    "Title");
            tabitem_Packages.Header         = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages",    "Title");
            tabitem_SettingsAndLayer.Header = Local.Loc.Loc(null, "Procon.UI.MainWindow.Settings",    "Title");

            #region Packages Tab

            button_PackageInstall.Content           = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "Install");
            label_PackageAuthor.Content             = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "Author");
            label_PackageDiscussion.Content         = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "Discussion");
            label_PackageDiscussionLinkText.Content = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "DiscussionText");
            label_PackageDescription.Content        = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "Description");
            label_PackageDownloads.Content          = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "Downloads");
            label_PackageLastModified.Content       = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "LastModified");
            label_PackageSize.Content               = Local.Loc.Loc(null, "Procon.UI.MainWindow.Packages", "FileSize");

            #endregion
            #region Settings Tab

            groupbox_Settings.Header = Local.Loc.Loc(null, "Procon.UI.MainWindow.Settings", "Title");

            #endregion
            #region Layer Tab

            groupbox_Layer.Header   = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "Title");
            label_LayerName.Content = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "Name");
            label_LayerHost.Content = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "Host");
            label_LayerPort.Content = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "Port");
            textbox_LayerHostPopup.Text = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "ErrorHost");
            textbox_LayerPortPopup.Text = Local.Loc.Loc(null, "Procon.UI.MainWindow.Layer", "ErrorPort");

            #endregion

            #endregion
            #region Popups
            
            #region Add Interface Popup

            label_pInterfaceTitle.Content    = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddTitle");
            textblock_pInterfaceMessage.Text = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddMsg");
            label_pInterfaceHost.Content     = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddHost");
            label_pInterfacePort.Content     = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddPort");
            label_pInterfaceUser.Content     = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddUser");
            label_pInterfacePass.Content     = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddPass");
            button_pInterfaceOk.Content      = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddOK");
            button_pInterfaceCancel.Content  = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddCancel");
            textbox_pInterfaceHostPopup.Text = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddErrorHost");
            textbox_pInterfacePortPopup.Text = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddErrorPort");
            textbox_pInterfaceUserPopup.Text = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddErrorUser");
            textbox_pInterfacePassPopup.Text = Local.Loc.Loc(null, "Procon.UI.Interfaces", "AddErrorPass");

            #endregion
            #region Add Connection Popup

            label_pConnectionTitle.Content    = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddTitle");
            textblock_pConnectionMessage.Text = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddMsg");
            label_pConnectionType.Content     = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddType");
            label_pConnectionHost.Content     = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddHost");
            label_pConnectionPort.Content     = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddPort");
            label_pConnectionPass.Content     = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddPass");
            label_pConnectionMisc.Content     = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddMisc");
            button_pConnectionOk.Content      = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddOK");
            button_pConnectionCancel.Content  = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddCancel");
            textbox_pConnectionHostPopup.Text = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddErrorHost");
            textbox_pConnectionPortPopup.Text = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddErrorPort");
            textbox_pConnectionPassPopup.Text = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddErrorPass");
            textbox_pConnectionMiscPopup.Text = Local.Loc.Loc(null, "Proocn.UI.Connections", "AddErrorMisc");

            #endregion
            #region Add Group Popup

            label_pGroupTitle.Content    = Local.Loc.Loc(null, "Proocn.UI.Groups", "AddTitle");
            textblock_pGroupMessage.Text = Local.Loc.Loc(null, "Proocn.UI.Groups", "AddMsg");
            button_pGroupOk.Content      = Local.Loc.Loc(null, "Proocn.UI.Groups", "AddOK");
            button_pGroupCancel.Content  = Local.Loc.Loc(null, "Proocn.UI.Groups", "AddCancel");
            textbox_pGroupNamePopup.Text = Local.Loc.Loc(null, "Proocn.UI.Groups", "AddErrorName");

            #endregion
            #region Add Account Popup

            label_pAccountTitle.Content    = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddTitle");
            textblock_pAccountMessage.Text = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddMsg");
            label_pAccountName.Content     = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddName");
            label_pAccountPass.Content     = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddPass");
            button_pAccountOk.Content      = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddOK");
            button_pAccountCancel.Content  = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddCancel");
            textbox_pAccountNamePopup.Text = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddErrorName");
            textbox_pAccountPassPopup.Text = Local.Loc.Loc(null, "Proocn.UI.Accounts", "AddErrorPass");

            #endregion

            #endregion

            #endregion
            #region Setup Comboboxes

            List<Enum> mEnumerations = new List<Enum>();
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
                if (gameType != GameType.None)
                    mEnumerations.Add(gameType);
            combobox_pConnectionType.ItemsSource  = new List<Enum>(mEnumerations);
            combobox_pConnectionType.SelectedItem = GameType.BF_BC2;

            #endregion
        }

        void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ObservableCollectionList<Account> accounts = (sender as ObservableCollectionList<Account>);
                foreach (Account account in accounts)
                    if (mNewAccounts.ContainsKey(account.Username))
                    {
                        account.SetPassword(CommandInitiator.Local, account.Username, mNewAccounts[account.Username]);
                        mNewAccounts.Remove(account.Username);
                    }
            }
        }

        /// <summary>
        /// Occurs whenever a button is clicked either on the main form,
        /// or a popup window used to server additional functions.
        /// </summary>
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            #region Add Interface

            // Simply focuses on the first control in the popup.
            if (sender == togglebutton_AddInterface)
                textbox_pInterfaceHost.Focus();
            // Attempt to add the interface.  Closes popup if successful.
            else if (sender == button_pInterfaceOk)
                addInterface();
            // Close the popup.
            else if (sender == button_pInterfaceCancel)
                closeAddInterface();

            #endregion
            #region Remove Interface

            // Remove the interface and shut it down.
            if (sender == button_RemoveInterface)
            {
                Instance  ins = (DataContext as Instance);
                Interface i   = (combobox_Interfaces.SelectedItem as Interface);
                ins.Interfaces.Remove(i);
                i.Shutdown();
            }

            #endregion

            #region Add Connection

            // Simply focuses on the first control in the popup.
            if (sender == togglebutton_AddConnection)
                combobox_pConnectionType.Focus();
            // Attempt to add the connection.  Closes popup if successful.
            else if (sender == button_pConnectionOk)
                addConnection();
            // Close the popup.
            else if (sender == button_pConnectionCancel)
                closeAddConnection();

            #endregion
            #region Remove Connection

            // Remove the connection and shut it down.
            if (sender == button_RemoveConnection)
            {
                Interface  i = (combobox_Interfaces.SelectedItem as Interface);
                Connection c = (listbox_Connections.SelectedItem as Connection);
                i.RemoveConnection(CommandInitiator.Local, c.GameType.ToString(), c.Hostname, c.Port);
            }

            #endregion
            #region View Connections

            // View the connection window.
            if (sender == button_ViewConnection)
            {
                Interface  i = (combobox_Interfaces.SelectedItem as Interface);
                Connection c = (listbox_Connections.SelectedItem as Connection);
                if (!mGameWindows.ContainsKey(c))
                    mGameWindows.Add(c, new GameWindow(c));
                mGameWindows[c].Show();
                mGameWindows[c].Focus();
            }

            #endregion

            #region Add Group

            // Simply focuses on the first control in the popup.
            if (sender == togglebutton_AddGroup)
                textbox_pGroupName.Focus();
            // Attempt to add the group.  Closes popup if successful.
            else if (sender == button_pGroupOk)
                addGroup();
            // Close the popup.
            else if (sender == button_pGroupCancel)
                closeAddGroup();

            #endregion
            #region Remove Group

            // Remove the group.
            if (sender == button_RemoveGroup)
            {
                Interface i = (combobox_Interfaces.SelectedItem as Interface);
                Group g     = (listbox_Groups.SelectedItem as Group);
                i.Security.RemoveGroup(CommandInitiator.Local, g.Name);
            }

            #endregion

            #region Add Account

            // Simply focuses on the first control in the popup.
            if (sender == togglebutton_AddAccount)
                textbox_pAccountName.Focus();
            // Attempt to add the account.  Closes popup if successful.
            else if (sender == button_pAccountOk)
                addAccount();
            // Close the popup.
            else if (sender == button_pAccountCancel)
                closeAddAccount();

            #endregion
            #region Remove Account

            // Remove the group.
            if (sender == button_RemoveAccount)
            {
                Interface i = (combobox_Interfaces.SelectedItem as Interface);
                Account a   = (listbox_Accounts.SelectedItem as Account);
                i.Security.RemoveAccount(CommandInitiator.Local, a.Username);
            }

            #endregion

            #region Install Package

            // Installs a package.
            if (sender == button_PackageInstall)
            {
                Interface i = (combobox_Interfaces.SelectedItem as Interface);
                i.Packages.InstallPackage(
                    CommandInitiator.Local,
                    (listbox_Packages.SelectedItem as Package).Uid);
            }

            #endregion

            #region Manage Layer

            // Toggle whether the layer is on or off.
            if (sender == button_Layer)
            {
                Interface i = combobox_Interfaces.SelectedItem as Interface;

                // Turn on the layer.
                if (i.Layer.ConnectionState == ConnectionState.Disconnected)
                {
                    // Clear old errors.
                    popup_LayerHost.IsOpen = false;
                    popup_LayerPort.IsOpen = false;

                    // Get info from UI.
                    String host = textbox_LayerHost.Text.Trim();
                    String port = textbox_LayerPort.Text.Trim();

                    // Error check info.
                    if (host == String.Empty)
                        popup_LayerHost.IsOpen = true;
                    UInt16 tempPort = 0;
                    if (port == String.Empty || !UInt16.TryParse(port, out tempPort))
                        popup_LayerPort.IsOpen = true;

                    // Start layer if no errors.
                    if (!popup_LayerHost.IsOpen && !popup_LayerPort.IsOpen)
                        i.Layer.Begin();
                }
                // Turn off the layer.
                else
                    i.Layer.Shutdown();
            }

            #endregion
        }

        /// <summary>
        /// Manages being able to press return/esc instead of clicking
        /// OK or Cancel for a popup window.
        /// </summary>
        private void TextBox_KeyPressed(object sender, KeyEventArgs e)
        {
            // Add Interface Popup.
            if (sender == passwordbox_pInterfacePass)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pInterfaceOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pInterfaceCancel, null);
            }  
            // Add Connection Popup.
            if (sender == passwordbox_pConnectionPass || sender == textbox_pConnectionMisc)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pConnectionOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pConnectionCancel, null);
            }
            // Add Group Popup.
            else if (sender == textbox_pGroupName)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pGroupOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pGroupCancel, null);
            }
            // Add Account Popup.
            else if (sender == textbox_pAccountPass)
            {
                if (e.Key == Key.Return)
                    Button_Clicked(button_pAccountOk, null);
                else if (e.Key == Key.Escape)
                    Button_Clicked(button_pAccountCancel, null);
            }

        }

        /// <summary>
        /// Occurs whenever a hyperlink is clicked.
        /// Hyperlinks are currently on the Packages tab.
        /// </summary>
        private void Web_Request(object sender, RequestNavigateEventArgs e)
        {
            // Make sure the uri is not bad stuff, like Format C:\.
            if (!String.IsNullOrEmpty(e.Uri.OriginalString))
                if (e.Uri.OriginalString.StartsWith("http"))
                    Process.Start(e.Uri.OriginalString);
                else
                    Process.Start("http://" + e.Uri.OriginalString);
        }


        /// <summary>
        /// Error checks the user's input to see if it at least kinda matches
        /// what needs to be entered (mostly checks to see if the input fields are
        /// empty).  Displays popup boxes containing error messages if there are errors,
        /// otherwise it attempts to connect to the procon layer and closes the popup.
        /// </summary>
        private void addInterface()
        {
            // Clear old errors.
            popup_pInterfaceHost.IsOpen = false;
            popup_pInterfacePort.IsOpen = false;
            popup_pInterfaceUser.IsOpen = false;
            popup_pInterfacePass.IsOpen = false;

            // Get info from UI.
            String host = textbox_pInterfaceHost.Text.Trim();
            String port = textbox_pInterfacePort.Text.Trim();
            String user = textbox_pInterfaceUser.Text.Trim();
            String pass = passwordbox_pInterfacePass.Password.Trim();

            // Error check info.
            if (host == String.Empty)
                popup_pInterfaceHost.IsOpen = true;
            UInt16 tempPort = 0;
            if (port == String.Empty || !UInt16.TryParse(port, out tempPort))
                popup_pInterfacePort.IsOpen = true;
            if (user == String.Empty)
                popup_pInterfaceUser.IsOpen = true;
            if (pass == String.Empty)
                popup_pInterfacePass.IsOpen = true;

            // Add connection if no errors.
            if (!popup_pInterfaceHost.IsOpen && !popup_pInterfacePort.IsOpen &&
                !popup_pInterfaceUser.IsOpen && !popup_pInterfacePass.IsOpen)
            {
                (DataContext as Instance).CreateRemoteInterface(CommandInitiator.Local, host, tempPort, user, pass).Execute();
                closeAddInterface();
            }
        }
        /// <summary>
        /// Closes the Add Connection popup window.
        /// </summary>
        private void closeAddInterface()
        {
            textbox_pInterfaceHost.Text = String.Empty;
            textbox_pInterfacePort.Text = String.Empty;
            textbox_pInterfaceUser.Text = String.Empty;
            passwordbox_pInterfacePass.Password = String.Empty;
            togglebutton_AddInterface.IsChecked = false;
            togglebutton_AddInterface.Focus();
        }


        /// <summary>
        /// Error checks the user's input to see if it at least kinda matches
        /// what needs to be entered (mostly checks to see if the input fields are
        /// empty).  Displays popup boxes containing error messages if there are errors,
        /// otherwise it attempts to connect to the game server and closes the popup.
        /// </summary>
        private void addConnection()
        {
            // Clear old errors.
            popup_pConnectionHost.IsOpen = false;
            popup_pConnectionPort.IsOpen = false;
            popup_pConnectionPass.IsOpen = false;
            popup_pConnectionMisc.IsOpen = false;

            // Get info from UI.
            String type = combobox_pConnectionType.SelectedItem.ToString();
            String host = textbox_pConnectionHost.Text.Trim();
            String port = textbox_pConnectionPort.Text.Trim();
            String pass = passwordbox_pConnectionPass.Password.Trim();
            String misc = textbox_pConnectionMisc.Text.Trim();

            // Error check info.
            if (host == String.Empty)
                popup_pConnectionHost.IsOpen = true;
            UInt16 tempPort = 0;
            if (port == String.Empty || !UInt16.TryParse(port, out tempPort))
                popup_pConnectionPort.IsOpen = true;
            if (pass == String.Empty)
                popup_pConnectionPass.IsOpen = true;
            if (misc == String.Empty && label_pConnectionMisc.IsVisible)
                popup_pConnectionMisc.IsOpen = true;

            // Add connection if no errors.
            if (!popup_pConnectionHost.IsOpen && !popup_pConnectionPort.IsOpen &&
                !popup_pConnectionPass.IsOpen && !popup_pConnectionMisc.IsOpen)
            {
                Interface i = (combobox_Interfaces.SelectedItem as Interface);
                i.AddConnection(CommandInitiator.Local, type, host, tempPort, pass, misc);
                closeAddConnection();
            }
        }
        /// <summary>
        /// Closes the Add Connection popup window.
        /// </summary>
        private void closeAddConnection()
        {
            combobox_pConnectionType.SelectedIndex = 0;
            textbox_pConnectionHost.Text = String.Empty;
            textbox_pConnectionPort.Text = String.Empty;
            textbox_pConnectionMisc.Text = String.Empty;
            passwordbox_pConnectionPass.Password = String.Empty;
            togglebutton_AddConnection.IsChecked = false;
            togglebutton_AddConnection.Focus();
        }


        /// <summary>
        /// Error checks the user's input to see if they entered a valid group name
        /// (it doesn't exist yet).  Displays popup box containing error message
        /// if there is an error, otherwise it attempts to create the group and
        /// closes the popup.
        /// </summary>
        private void addGroup()
        {
            // Clear old errors.
            popup_pGroupName.IsOpen = false;

            // Get info from UI.
            String name = textbox_pGroupName.Text.Trim();

            // Error check info.
            Interface i = (combobox_Interfaces.SelectedItem as Interface);
            if (name == String.Empty || i.Security.Groups.Find(x => x.Name == name) != null)
                popup_pGroupName.IsOpen = true;

            // Add group if no errors.
            if (!popup_pGroupName.IsOpen)
            {
                i.Security.AddGroup(CommandInitiator.Local, name);
                closeAddGroup();
            }
        }
        /// <summary>
        /// Closes the Add Group popup window.
        /// </summary>
        private void closeAddGroup()
        {
            textbox_pGroupName.Text = String.Empty;
            togglebutton_AddGroup.IsChecked = false;
            togglebutton_AddGroup.Focus();
        }


        /// <summary>
        /// Error checks the user's input to see if they entered a valid account name
        /// (it doesn't exist yet) and password.  Displays popup box containing error
        /// message if there is an error, otherwise it attempts to create the account
        /// and closes the popup.
        /// </summary>
        private void addAccount()
        {
            // Clear old errors.
            popup_pAccountName.IsOpen = false;
            popup_pAccountPass.IsOpen = false;

            // Get info from UI.
            String name = textbox_pAccountName.Text.Trim();
            String pass = textbox_pAccountPass.Password.Trim();

            // Error check info.
            Interface i = (combobox_Interfaces.SelectedItem as Interface);
            if (name == String.Empty || i.Security.Accounts.Find(x => x.Username == name) != null)
                popup_pAccountName.IsOpen = true;
            if (pass == String.Empty)
                popup_pAccountPass.IsOpen = true;

            // Add group if no errors.
            if (!popup_pAccountName.IsOpen && !popup_pAccountPass.IsOpen)
            {
                mNewAccounts.Add(name, pass);
                i.Security.AddAccount(CommandInitiator.Local, name);
                closeAddAccount();
            }
        }
        /// <summary>
        /// Closes the Add Account popup window.
        /// </summary>
        private void closeAddAccount()
        {
            textbox_pAccountName.Text     = String.Empty;
            textbox_pAccountPass.Password = String.Empty;
            togglebutton_AddAccount.IsChecked = false;
            togglebutton_AddAccount.Focus();
        }


        /// <summary>
        /// Handles cleaning up procon stuffs when closing procon such as shutting down
        /// the instance that is controlling all the procon strings.
        /// </summary>
        private void Closing_Procon(object sender, CancelEventArgs e)
        {
            foreach (GameWindow gw in mGameWindows.Values)
                gw.Close();
            (DataContext as Instance).Shutdown();
        }


        private void ConnectionInfo_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = true;
        }
        private void GroupInfo_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = true;

            // Check to see if item is invalid.
            Permission item = (e.Item as Permission);
            if (item == null ||
                item.Name == CommandName.None          || item.Name == CommandName.Action ||
                item.Name == CommandName.Authenticated || item.Name == CommandName.Hashed ||
                item.Name == CommandName.Login         || item.Name == CommandName.Salt   ||
                !item.Authority.HasValue               || item.Authority.Value == 0)
                e.Accepted = false;
        }
        private void AccountInfo_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = true;
        }
    }
}
