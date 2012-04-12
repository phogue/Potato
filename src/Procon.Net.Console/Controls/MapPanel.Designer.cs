namespace Procon.Net.Console.Controls {
    partial class MapPanel {
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
            this.colIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRounds = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvMapList = new Procon.Net.Console.Controls.ListViewNF();
            this.grpQuick.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpQuick
            // 
            this.grpQuick.Controls.Add(this.pnlActions);
            this.grpQuick.Controls.Add(this.quickActionsPropertyGrid);
            this.grpQuick.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpQuick.Location = new System.Drawing.Point(616, 0);
            this.grpQuick.Name = "grpQuick";
            this.grpQuick.Padding = new System.Windows.Forms.Padding(10);
            this.grpQuick.Size = new System.Drawing.Size(225, 610);
            this.grpQuick.TabIndex = 5;
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
            this.quickActionsPropertyGrid.Size = new System.Drawing.Size(205, 493);
            this.quickActionsPropertyGrid.TabIndex = 9;
            // 
            // colIndex
            // 
            this.colIndex.Text = "Index";
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colMode
            // 
            this.colMode.Text = "Mode";
            // 
            // colRounds
            // 
            this.colRounds.Text = "Rounds";
            // 
            // lsvMapList
            // 
            this.lsvMapList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvMapList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colName,
            this.colMode,
            this.colRounds});
            this.lsvMapList.FullRowSelect = true;
            this.lsvMapList.Location = new System.Drawing.Point(3, 3);
            this.lsvMapList.Name = "lsvMapList";
            this.lsvMapList.Size = new System.Drawing.Size(607, 604);
            this.lsvMapList.TabIndex = 4;
            this.lsvMapList.UseCompatibleStateImageBehavior = false;
            this.lsvMapList.View = System.Windows.Forms.View.Details;
            // 
            // MapPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lsvMapList);
            this.Controls.Add(this.grpQuick);
            this.Name = "MapPanel";
            this.Size = new System.Drawing.Size(841, 610);
            this.grpQuick.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpQuick;
        private System.Windows.Forms.FlowLayoutPanel pnlActions;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cboActions;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.PropertyGrid quickActionsPropertyGrid;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colMode;
        private System.Windows.Forms.ColumnHeader colRounds;
        private ListViewNF lsvMapList;
    }
}
