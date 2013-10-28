namespace Procon.Tools.NetworkConsole {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlConnection = new System.Windows.Forms.Panel();
            this.txtAdditional = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboGames = new System.Windows.Forms.ComboBox();
            this.lblGameType = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblHostname = new System.Windows.Forms.Label();
            this.txtHostname = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbcPanels = new System.Windows.Forms.TabControl();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.chkVerboseLogging = new System.Windows.Forms.CheckBox();
            this.chkAnchorScrollbar = new System.Windows.Forms.CheckBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtConsoleText = new System.Windows.Forms.TextBox();
            this.tabGameState = new System.Windows.Forms.TabPage();
            this.gameStatePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabEvents = new System.Windows.Forms.TabPage();
            this.tabChat = new System.Windows.Forms.TabPage();
            this.tabPlayers = new System.Windows.Forms.TabPage();
            this.tabMaplist = new System.Windows.Forms.TabPage();
            this.tabBanlist = new System.Windows.Forms.TabPage();
            this.tabProtocolTest = new System.Windows.Forms.TabPage();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rtbConsole = new Procon.Tools.NetworkConsole.Controls.CodRichTextBox();
            this.eventPanel1 = new Procon.Tools.NetworkConsole.Controls.EventPanel();
            this.chat1 = new Procon.Tools.NetworkConsole.Controls.ChatPanel();
            this.playerPanel1 = new Procon.Tools.NetworkConsole.Controls.PlayerPanel();
            this.mapPanel1 = new Procon.Tools.NetworkConsole.Controls.MapPanel();
            this.banPanel1 = new Procon.Tools.NetworkConsole.Controls.BanPanel();
            this.protocolTest1 = new Procon.Tools.NetworkConsole.Controls.ProtocolTestControl();
            this.pnlConnection.SuspendLayout();
            this.tbcPanels.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.tabGameState.SuspendLayout();
            this.tabEvents.SuspendLayout();
            this.tabChat.SuspendLayout();
            this.tabPlayers.SuspendLayout();
            this.tabMaplist.SuspendLayout();
            this.tabBanlist.SuspendLayout();
            this.tabProtocolTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(627, 698);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(213, 9);
            this.lblVersion.TabIndex = 13;
            this.lblVersion.Text = "version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pnlConnection
            // 
            this.pnlConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnection.Controls.Add(this.txtAdditional);
            this.pnlConnection.Controls.Add(this.label1);
            this.pnlConnection.Controls.Add(this.cboGames);
            this.pnlConnection.Controls.Add(this.lblGameType);
            this.pnlConnection.Controls.Add(this.lblPassword);
            this.pnlConnection.Controls.Add(this.txtPassword);
            this.pnlConnection.Controls.Add(this.lblHostname);
            this.pnlConnection.Controls.Add(this.txtHostname);
            this.pnlConnection.Controls.Add(this.txtPort);
            this.pnlConnection.Controls.Add(this.lblPort);
            this.pnlConnection.Location = new System.Drawing.Point(3, 12);
            this.pnlConnection.Name = "pnlConnection";
            this.pnlConnection.Size = new System.Drawing.Size(827, 50);
            this.pnlConnection.TabIndex = 12;
            // 
            // txtAdditional
            // 
            this.txtAdditional.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdditional.Location = new System.Drawing.Point(469, 20);
            this.txtAdditional.Name = "txtAdditional";
            this.txtAdditional.Size = new System.Drawing.Size(355, 20);
            this.txtAdditional.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(467, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Additional (e.g LogUrl=...):";
            // 
            // cboGames
            // 
            this.cboGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGames.FormattingEnabled = true;
            this.cboGames.Location = new System.Drawing.Point(10, 20);
            this.cboGames.Name = "cboGames";
            this.cboGames.Size = new System.Drawing.Size(143, 21);
            this.cboGames.TabIndex = 12;
            // 
            // lblGameType
            // 
            this.lblGameType.AutoSize = true;
            this.lblGameType.Location = new System.Drawing.Point(7, 4);
            this.lblGameType.Name = "lblGameType";
            this.lblGameType.Size = new System.Drawing.Size(38, 13);
            this.lblGameType.TabIndex = 11;
            this.lblGameType.Text = "Game:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(360, 4);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 10;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(363, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.Location = new System.Drawing.Point(159, 4);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(58, 13);
            this.lblHostname.TabIndex = 5;
            this.lblHostname.Text = "Hostname:";
            // 
            // txtHostname
            // 
            this.txtHostname.Location = new System.Drawing.Point(159, 20);
            this.txtHostname.Name = "txtHostname";
            this.txtHostname.Size = new System.Drawing.Size(145, 20);
            this.txtHostname.TabIndex = 4;
            this.txtHostname.TextChanged += new System.EventHandler(this.txtHostname_TextChanged);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(310, 20);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(47, 20);
            this.txtPort.TabIndex = 7;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(307, 4);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 6;
            this.lblPort.Text = "Port:";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(745, 58);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(85, 23);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbcPanels
            // 
            this.tbcPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcPanels.Controls.Add(this.tabConsole);
            this.tbcPanels.Controls.Add(this.tabGameState);
            this.tbcPanels.Controls.Add(this.tabEvents);
            this.tbcPanels.Controls.Add(this.tabChat);
            this.tbcPanels.Controls.Add(this.tabPlayers);
            this.tbcPanels.Controls.Add(this.tabMaplist);
            this.tbcPanels.Controls.Add(this.tabBanlist);
            this.tbcPanels.Controls.Add(this.tabProtocolTest);
            this.tbcPanels.Location = new System.Drawing.Point(12, 68);
            this.tbcPanels.Name = "tbcPanels";
            this.tbcPanels.SelectedIndex = 0;
            this.tbcPanels.Size = new System.Drawing.Size(828, 627);
            this.tbcPanels.TabIndex = 11;
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.chkVerboseLogging);
            this.tabConsole.Controls.Add(this.chkAnchorScrollbar);
            this.tabConsole.Controls.Add(this.rtbConsole);
            this.tabConsole.Controls.Add(this.btnSend);
            this.tabConsole.Controls.Add(this.txtConsoleText);
            this.tabConsole.Location = new System.Drawing.Point(4, 22);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsole.Size = new System.Drawing.Size(820, 601);
            this.tabConsole.TabIndex = 1;
            this.tabConsole.Text = "Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // chkVerboseLogging
            // 
            this.chkVerboseLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkVerboseLogging.AutoSize = true;
            this.chkVerboseLogging.Location = new System.Drawing.Point(598, 549);
            this.chkVerboseLogging.Name = "chkVerboseLogging";
            this.chkVerboseLogging.Size = new System.Drawing.Size(106, 17);
            this.chkVerboseLogging.TabIndex = 5;
            this.chkVerboseLogging.Text = "Verbose Logging";
            this.toolTip1.SetToolTip(this.chkVerboseLogging, "Enables logging in the console while viewing another tab page");
            this.chkVerboseLogging.UseVisualStyleBackColor = true;
            // 
            // chkAnchorScrollbar
            // 
            this.chkAnchorScrollbar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAnchorScrollbar.AutoSize = true;
            this.chkAnchorScrollbar.Location = new System.Drawing.Point(710, 549);
            this.chkAnchorScrollbar.Name = "chkAnchorScrollbar";
            this.chkAnchorScrollbar.Size = new System.Drawing.Size(104, 17);
            this.chkAnchorScrollbar.TabIndex = 4;
            this.chkAnchorScrollbar.Text = "Anchor Scrollbar";
            this.chkAnchorScrollbar.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(739, 572);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtConsoleText
            // 
            this.txtConsoleText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleText.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.txtConsoleText.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtConsoleText.Location = new System.Drawing.Point(6, 572);
            this.txtConsoleText.Name = "txtConsoleText";
            this.txtConsoleText.Size = new System.Drawing.Size(727, 20);
            this.txtConsoleText.TabIndex = 1;
            this.txtConsoleText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleText_KeyDown);
            // 
            // tabGameState
            // 
            this.tabGameState.Controls.Add(this.gameStatePropertyGrid);
            this.tabGameState.Location = new System.Drawing.Point(4, 22);
            this.tabGameState.Name = "tabGameState";
            this.tabGameState.Padding = new System.Windows.Forms.Padding(3);
            this.tabGameState.Size = new System.Drawing.Size(820, 601);
            this.tabGameState.TabIndex = 2;
            this.tabGameState.Text = "Game State";
            this.tabGameState.UseVisualStyleBackColor = true;
            // 
            // gameStatePropertyGrid
            // 
            this.gameStatePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameStatePropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.gameStatePropertyGrid.Name = "gameStatePropertyGrid";
            this.gameStatePropertyGrid.Size = new System.Drawing.Size(814, 595);
            this.gameStatePropertyGrid.TabIndex = 0;
            // 
            // tabEvents
            // 
            this.tabEvents.Controls.Add(this.eventPanel1);
            this.tabEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEvents.Name = "tabEvents";
            this.tabEvents.Padding = new System.Windows.Forms.Padding(3);
            this.tabEvents.Size = new System.Drawing.Size(820, 601);
            this.tabEvents.TabIndex = 7;
            this.tabEvents.Text = "Events";
            this.tabEvents.UseVisualStyleBackColor = true;
            // 
            // tabChat
            // 
            this.tabChat.Controls.Add(this.chat1);
            this.tabChat.Location = new System.Drawing.Point(4, 22);
            this.tabChat.Name = "tabChat";
            this.tabChat.Padding = new System.Windows.Forms.Padding(3);
            this.tabChat.Size = new System.Drawing.Size(820, 601);
            this.tabChat.TabIndex = 3;
            this.tabChat.Text = "Chat";
            this.tabChat.UseVisualStyleBackColor = true;
            // 
            // tabPlayers
            // 
            this.tabPlayers.Controls.Add(this.playerPanel1);
            this.tabPlayers.Location = new System.Drawing.Point(4, 22);
            this.tabPlayers.Name = "tabPlayers";
            this.tabPlayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlayers.Size = new System.Drawing.Size(820, 601);
            this.tabPlayers.TabIndex = 4;
            this.tabPlayers.Text = "Players";
            this.tabPlayers.UseVisualStyleBackColor = true;
            // 
            // tabMaplist
            // 
            this.tabMaplist.Controls.Add(this.mapPanel1);
            this.tabMaplist.Location = new System.Drawing.Point(4, 22);
            this.tabMaplist.Name = "tabMaplist";
            this.tabMaplist.Padding = new System.Windows.Forms.Padding(3);
            this.tabMaplist.Size = new System.Drawing.Size(820, 601);
            this.tabMaplist.TabIndex = 5;
            this.tabMaplist.Text = "Maplist";
            this.tabMaplist.UseVisualStyleBackColor = true;
            // 
            // tabBanlist
            // 
            this.tabBanlist.Controls.Add(this.banPanel1);
            this.tabBanlist.Location = new System.Drawing.Point(4, 22);
            this.tabBanlist.Name = "tabBanlist";
            this.tabBanlist.Padding = new System.Windows.Forms.Padding(3);
            this.tabBanlist.Size = new System.Drawing.Size(820, 601);
            this.tabBanlist.TabIndex = 6;
            this.tabBanlist.Text = "Banlist";
            this.tabBanlist.UseVisualStyleBackColor = true;
            // 
            // tabProtocolTest
            // 
            this.tabProtocolTest.Controls.Add(this.protocolTest1);
            this.tabProtocolTest.Location = new System.Drawing.Point(4, 22);
            this.tabProtocolTest.Name = "tabProtocolTest";
            this.tabProtocolTest.Size = new System.Drawing.Size(820, 601);
            this.tabProtocolTest.TabIndex = 8;
            this.tabProtocolTest.Text = "Protocol Test";
            this.tabProtocolTest.UseVisualStyleBackColor = true;
            // 
            // rtbConsole
            // 
            this.rtbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbConsole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.rtbConsole.Location = new System.Drawing.Point(6, 6);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(808, 538);
            this.rtbConsole.TabIndex = 3;
            this.rtbConsole.Text = "";
            this.rtbConsole.WordWrap = false;
            // 
            // eventPanel1
            // 
            this.eventPanel1.ActiveGame = null;
            this.eventPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventPanel1.Location = new System.Drawing.Point(3, 3);
            this.eventPanel1.Name = "eventPanel1";
            this.eventPanel1.Size = new System.Drawing.Size(814, 595);
            this.eventPanel1.TabIndex = 0;
            // 
            // chat1
            // 
            this.chat1.ActiveGame = null;
            this.chat1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chat1.Location = new System.Drawing.Point(3, 3);
            this.chat1.Name = "chat1";
            this.chat1.Size = new System.Drawing.Size(814, 595);
            this.chat1.TabIndex = 0;
            // 
            // playerPanel1
            // 
            this.playerPanel1.ActiveGame = null;
            this.playerPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playerPanel1.Location = new System.Drawing.Point(3, 3);
            this.playerPanel1.Name = "playerPanel1";
            this.playerPanel1.Size = new System.Drawing.Size(814, 595);
            this.playerPanel1.TabIndex = 0;
            // 
            // mapPanel1
            // 
            this.mapPanel1.ActiveGame = null;
            this.mapPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel1.Location = new System.Drawing.Point(3, 3);
            this.mapPanel1.Name = "mapPanel1";
            this.mapPanel1.Size = new System.Drawing.Size(814, 595);
            this.mapPanel1.TabIndex = 0;
            // 
            // banPanel1
            // 
            this.banPanel1.ActiveGame = null;
            this.banPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.banPanel1.Location = new System.Drawing.Point(3, 3);
            this.banPanel1.Name = "banPanel1";
            this.banPanel1.Size = new System.Drawing.Size(814, 595);
            this.banPanel1.TabIndex = 0;
            // 
            // protocolTest1
            // 
            this.protocolTest1.ActiveGame = null;
            this.protocolTest1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protocolTest1.Location = new System.Drawing.Point(0, 0);
            this.protocolTest1.Name = "protocolTest1";
            this.protocolTest1.Size = new System.Drawing.Size(820, 601);
            this.protocolTest1.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 716);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.pnlConnection);
            this.Controls.Add(this.tbcPanels);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Procon - Protocol Test Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlConnection.ResumeLayout(false);
            this.pnlConnection.PerformLayout();
            this.tbcPanels.ResumeLayout(false);
            this.tabConsole.ResumeLayout(false);
            this.tabConsole.PerformLayout();
            this.tabGameState.ResumeLayout(false);
            this.tabEvents.ResumeLayout(false);
            this.tabChat.ResumeLayout(false);
            this.tabPlayers.ResumeLayout(false);
            this.tabMaplist.ResumeLayout(false);
            this.tabBanlist.ResumeLayout(false);
            this.tabProtocolTest.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Panel pnlConnection;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblHostname;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtHostname;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TabControl tbcPanels;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtConsoleText;
        private System.Windows.Forms.ComboBox cboGames;
        private System.Windows.Forms.Label lblGameType;
        private Controls.CodRichTextBox rtbConsole;
        private System.Windows.Forms.TabPage tabGameState;
        private System.Windows.Forms.PropertyGrid gameStatePropertyGrid;
        private System.Windows.Forms.TextBox txtAdditional;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabChat;
        private System.Windows.Forms.TabPage tabPlayers;
        private System.Windows.Forms.TabPage tabMaplist;
        private System.Windows.Forms.TabPage tabBanlist;
        private Controls.ChatPanel chat1;
        private Controls.PlayerPanel playerPanel1;
        private Controls.BanPanel banPanel1;
        private Controls.MapPanel mapPanel1;
        private System.Windows.Forms.TabPage tabEvents;
        private Controls.EventPanel eventPanel1;
        private System.Windows.Forms.CheckBox chkAnchorScrollbar;
        private System.Windows.Forms.CheckBox chkVerboseLogging;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabProtocolTest;
        private Controls.ProtocolTestControl protocolTest1;
    }
}

