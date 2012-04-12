namespace Procon.Net.Console.Controls {
    partial class BanPanel {
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
            this.grpQuick = new System.Windows.Forms.GroupBox();
            this.pnlActions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblAction = new System.Windows.Forms.Label();
            this.cboActions = new System.Windows.Forms.ComboBox();
            this.btnAction = new System.Windows.Forms.Button();
            this.quickActionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.lsvBanList = new Procon.Net.Console.Controls.ListViewNF();
            this.colId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colReason = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpQuick.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpQuick
            // 
            this.grpQuick.Controls.Add(this.pnlActions);
            this.grpQuick.Controls.Add(this.quickActionsPropertyGrid);
            this.grpQuick.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpQuick.Enabled = false;
            this.grpQuick.Location = new System.Drawing.Point(615, 0);
            this.grpQuick.Name = "grpQuick";
            this.grpQuick.Padding = new System.Windows.Forms.Padding(10);
            this.grpQuick.Size = new System.Drawing.Size(225, 605);
            this.grpQuick.TabIndex = 3;
            this.grpQuick.TabStop = false;
            this.grpQuick.Text = "Quick Actions";
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.lblAction);
            this.pnlActions.Controls.Add(this.cboActions);
            this.pnlActions.Controls.Add(this.btnAction);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActions.Location = new System.Drawing.Point(10, 23);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(205, 75);
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
            // btnAction
            // 
            this.btnAction.Location = new System.Drawing.Point(3, 43);
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
            this.quickActionsPropertyGrid.Location = new System.Drawing.Point(10, 104);
            this.quickActionsPropertyGrid.Name = "quickActionsPropertyGrid";
            this.quickActionsPropertyGrid.Size = new System.Drawing.Size(205, 488);
            this.quickActionsPropertyGrid.TabIndex = 9;
            // 
            // lsvBanList
            // 
            this.lsvBanList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvBanList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colId,
            this.colType,
            this.colReason});
            this.lsvBanList.FullRowSelect = true;
            this.lsvBanList.Location = new System.Drawing.Point(3, 3);
            this.lsvBanList.Name = "lsvBanList";
            this.lsvBanList.Size = new System.Drawing.Size(606, 599);
            this.lsvBanList.TabIndex = 0;
            this.lsvBanList.UseCompatibleStateImageBehavior = false;
            this.lsvBanList.View = System.Windows.Forms.View.Details;
            this.lsvBanList.SelectedIndexChanged += new System.EventHandler(this.lsvPlayerlist_SelectedIndexChanged);
            // 
            // colId
            // 
            this.colId.Text = "Id";
            // 
            // colType
            // 
            this.colType.Text = "Type";
            // 
            // colReason
            // 
            this.colReason.Text = "Reason";
            // 
            // BanPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpQuick);
            this.Controls.Add(this.lsvBanList);
            this.Name = "BanPanel";
            this.Size = new System.Drawing.Size(840, 605);
            this.grpQuick.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNF lsvBanList;
        private System.Windows.Forms.ColumnHeader colId;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colReason;
        private System.Windows.Forms.GroupBox grpQuick;
        private System.Windows.Forms.FlowLayoutPanel pnlActions;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cboActions;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.PropertyGrid quickActionsPropertyGrid;
    }
}
