namespace Potato.Tools.NetworkConsole {
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
            this.rtbConsole = new Potato.Tools.NetworkConsole.Controls.CodRichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtConsoleText = new System.Windows.Forms.TextBox();
            this.tabProtocolTest = new System.Windows.Forms.TabPage();
            this.protocolTestControl1 = new Potato.Tools.NetworkConsole.Controls.ProtocolTestControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlConnection.SuspendLayout();
            this.tbcPanels.SuspendLayout();
            this.tabConsole.SuspendLayout();
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
            this.txtAdditional.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdditional.Location = new System.Drawing.Point(626, 20);
            this.txtAdditional.Name = "txtAdditional";
            this.txtAdditional.Size = new System.Drawing.Size(198, 20);
            this.txtAdditional.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(623, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Additional (e.g LogUrl=...):";
            // 
            // cboGames
            // 
            this.cboGames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGames.FormattingEnabled = true;
            this.cboGames.Location = new System.Drawing.Point(10, 20);
            this.cboGames.Name = "cboGames";
            this.cboGames.Size = new System.Drawing.Size(300, 21);
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
            this.lblPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(517, 4);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 10;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(520, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // lblHostname
            // 
            this.lblHostname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHostname.AutoSize = true;
            this.lblHostname.Location = new System.Drawing.Point(316, 4);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(58, 13);
            this.lblHostname.TabIndex = 5;
            this.lblHostname.Text = "Hostname:";
            // 
            // txtHostname
            // 
            this.txtHostname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHostname.Location = new System.Drawing.Point(316, 20);
            this.txtHostname.Name = "txtHostname";
            this.txtHostname.Size = new System.Drawing.Size(145, 20);
            this.txtHostname.TabIndex = 4;
            this.txtHostname.TextChanged += new System.EventHandler(this.txtHostname_TextChanged);
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPort.Location = new System.Drawing.Point(467, 20);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(47, 20);
            this.txtPort.TabIndex = 7;
            // 
            // lblPort
            // 
            this.lblPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(464, 4);
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
            // tabProtocolTest
            // 
            this.tabProtocolTest.Controls.Add(this.protocolTestControl1);
            this.tabProtocolTest.Location = new System.Drawing.Point(4, 22);
            this.tabProtocolTest.Name = "tabProtocolTest";
            this.tabProtocolTest.Size = new System.Drawing.Size(820, 601);
            this.tabProtocolTest.TabIndex = 8;
            this.tabProtocolTest.Text = "Protocol Test";
            this.tabProtocolTest.UseVisualStyleBackColor = true;
            // 
            // protocolTestControl1
            // 
            this.protocolTestControl1.ActiveGame = null;
            this.protocolTestControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protocolTestControl1.IsTestRunning = false;
            this.protocolTestControl1.Location = new System.Drawing.Point(0, 0);
            this.protocolTestControl1.Name = "protocolTestControl1";
            this.protocolTestControl1.Size = new System.Drawing.Size(820, 601);
            this.protocolTestControl1.TabIndex = 0;
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
            this.Text = "Potato - Protocol Test Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlConnection.ResumeLayout(false);
            this.pnlConnection.PerformLayout();
            this.tbcPanels.ResumeLayout(false);
            this.tabConsole.ResumeLayout(false);
            this.tabConsole.PerformLayout();
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
        private System.Windows.Forms.TextBox txtAdditional;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkAnchorScrollbar;
        private System.Windows.Forms.CheckBox chkVerboseLogging;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabProtocolTest;
        private Controls.ProtocolTestControl protocolTest1;
        private Controls.ProtocolTestControl protocolTestControl1;
    }
}

