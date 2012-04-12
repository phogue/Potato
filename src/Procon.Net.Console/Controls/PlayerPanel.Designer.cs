namespace Procon.Net.Console.Controls {
    partial class PlayerPanel {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lsvPlayerlist = new Procon.Net.Console.Controls.ListViewNF();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScore = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colKills = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDeaths = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colKdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPing = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTeamId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpQuick = new System.Windows.Forms.GroupBox();
            this.pnlActions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblAction = new System.Windows.Forms.Label();
            this.cboActions = new System.Windows.Forms.ComboBox();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.btnAction = new System.Windows.Forms.Button();
            this.quickActionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.grpQuick.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsvPlayerlist
            // 
            this.lsvPlayerlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvPlayerlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colUid,
            this.colScore,
            this.colKills,
            this.colDeaths,
            this.colKdr,
            this.colPing,
            this.colTeamId});
            this.lsvPlayerlist.FullRowSelect = true;
            this.lsvPlayerlist.Location = new System.Drawing.Point(3, 3);
            this.lsvPlayerlist.Name = "lsvPlayerlist";
            this.lsvPlayerlist.Size = new System.Drawing.Size(601, 605);
            this.lsvPlayerlist.TabIndex = 0;
            this.lsvPlayerlist.UseCompatibleStateImageBehavior = false;
            this.lsvPlayerlist.View = System.Windows.Forms.View.Details;
            this.lsvPlayerlist.SelectedIndexChanged += new System.EventHandler(this.lsvPlayerlist_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colUid
            // 
            this.colUid.Text = "UID";
            // 
            // colScore
            // 
            this.colScore.Text = "Score";
            // 
            // colKills
            // 
            this.colKills.Text = "Kills";
            // 
            // colDeaths
            // 
            this.colDeaths.Text = "Deaths";
            // 
            // colKdr
            // 
            this.colKdr.Text = "Kdr";
            // 
            // colPing
            // 
            this.colPing.Text = "Ping";
            // 
            // colTeamId
            // 
            this.colTeamId.Text = "TeamId";
            // 
            // grpQuick
            // 
            this.grpQuick.Controls.Add(this.pnlActions);
            this.grpQuick.Controls.Add(this.quickActionsPropertyGrid);
            this.grpQuick.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpQuick.Enabled = false;
            this.grpQuick.Location = new System.Drawing.Point(610, 0);
            this.grpQuick.Name = "grpQuick";
            this.grpQuick.Padding = new System.Windows.Forms.Padding(10);
            this.grpQuick.Size = new System.Drawing.Size(225, 611);
            this.grpQuick.TabIndex = 2;
            this.grpQuick.TabStop = false;
            this.grpQuick.Text = "Quick Actions";
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.lblAction);
            this.pnlActions.Controls.Add(this.cboActions);
            this.pnlActions.Controls.Add(this.lblReason);
            this.pnlActions.Controls.Add(this.txtReason);
            this.pnlActions.Controls.Add(this.btnAction);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActions.Location = new System.Drawing.Point(10, 23);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(205, 112);
            this.pnlActions.TabIndex = 0;
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(3, 0);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(42, 13);
            this.lblAction.TabIndex = 8;
            this.lblAction.Text = "Actions";
            // 
            // cboActions
            // 
            this.cboActions.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboActions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActions.FormattingEnabled = true;
            this.cboActions.Location = new System.Drawing.Point(3, 16);
            this.cboActions.Name = "cboActions";
            this.cboActions.Size = new System.Drawing.Size(199, 21);
            this.cboActions.TabIndex = 7;
            this.cboActions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cboActions_DrawItem);
            this.cboActions.SelectedIndexChanged += new System.EventHandler(this.cboActions_SelectedIndexChanged);
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.Location = new System.Drawing.Point(3, 40);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(44, 13);
            this.lblReason.TabIndex = 5;
            this.lblReason.Text = "Reason";
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(3, 56);
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(199, 20);
            this.txtReason.TabIndex = 4;
            this.txtReason.TextChanged += new System.EventHandler(this.txtReason_TextChanged);
            // 
            // btnAction
            // 
            this.btnAction.Location = new System.Drawing.Point(3, 82);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(95, 23);
            this.btnAction.TabIndex = 6;
            this.btnAction.Text = "Action";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // quickActionsPropertyGrid
            // 
            this.quickActionsPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.quickActionsPropertyGrid.Location = new System.Drawing.Point(10, 141);
            this.quickActionsPropertyGrid.Name = "quickActionsPropertyGrid";
            this.quickActionsPropertyGrid.Size = new System.Drawing.Size(205, 457);
            this.quickActionsPropertyGrid.TabIndex = 9;
            // 
            // PlayerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpQuick);
            this.Controls.Add(this.lsvPlayerlist);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(835, 611);
            this.grpQuick.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNF lsvPlayerlist;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colUid;
        private System.Windows.Forms.ColumnHeader colScore;
        private System.Windows.Forms.ColumnHeader colKills;
        private System.Windows.Forms.ColumnHeader colDeaths;
        private System.Windows.Forms.ColumnHeader colKdr;
        private System.Windows.Forms.ColumnHeader colTeamId;
        private System.Windows.Forms.ColumnHeader colPing;
        private System.Windows.Forms.GroupBox grpQuick;
        private System.Windows.Forms.FlowLayoutPanel pnlActions;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cboActions;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.PropertyGrid quickActionsPropertyGrid;
    }
}
