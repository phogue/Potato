namespace Procon.Net.Console.Controls {
    partial class EventPanel {
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
            this.quickActionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.lblGameEvent = new System.Windows.Forms.Label();
            this.chkPostEvents = new System.Windows.Forms.CheckBox();
            this.lsvEventsList = new Procon.Net.Console.Controls.ListViewNF();
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEvent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInnerEvent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConnectionState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colException = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpQuick.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpQuick
            // 
            this.grpQuick.Controls.Add(this.quickActionsPropertyGrid);
            this.grpQuick.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpQuick.Location = new System.Drawing.Point(603, 0);
            this.grpQuick.Name = "grpQuick";
            this.grpQuick.Padding = new System.Windows.Forms.Padding(10);
            this.grpQuick.Size = new System.Drawing.Size(225, 612);
            this.grpQuick.TabIndex = 6;
            this.grpQuick.TabStop = false;
            this.grpQuick.Text = "Quick Actions";
            // 
            // quickActionsPropertyGrid
            // 
            this.quickActionsPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.quickActionsPropertyGrid.Location = new System.Drawing.Point(10, 23);
            this.quickActionsPropertyGrid.Name = "quickActionsPropertyGrid";
            this.quickActionsPropertyGrid.Size = new System.Drawing.Size(205, 576);
            this.quickActionsPropertyGrid.TabIndex = 9;
            // 
            // lblGameEvent
            // 
            this.lblGameEvent.AutoSize = true;
            this.lblGameEvent.Location = new System.Drawing.Point(3, 7);
            this.lblGameEvent.Name = "lblGameEvent";
            this.lblGameEvent.Size = new System.Drawing.Size(71, 13);
            this.lblGameEvent.TabIndex = 8;
            this.lblGameEvent.Text = "Game Events";
            // 
            // chkPostEvents
            // 
            this.chkPostEvents.AutoSize = true;
            this.chkPostEvents.Checked = true;
            this.chkPostEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPostEvents.Location = new System.Drawing.Point(500, 3);
            this.chkPostEvents.Name = "chkPostEvents";
            this.chkPostEvents.Size = new System.Drawing.Size(83, 17);
            this.chkPostEvents.TabIndex = 9;
            this.chkPostEvents.Text = "Post Events";
            this.chkPostEvents.UseVisualStyleBackColor = true;
            // 
            // lsvEventsList
            // 
            this.lsvEventsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvEventsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTime,
            this.colEvent,
            this.colInnerEvent,
            this.colConnectionState,
            this.colException});
            this.lsvEventsList.FullRowSelect = true;
            this.lsvEventsList.Location = new System.Drawing.Point(3, 23);
            this.lsvEventsList.Name = "lsvEventsList";
            this.lsvEventsList.Size = new System.Drawing.Size(594, 576);
            this.lsvEventsList.TabIndex = 7;
            this.lsvEventsList.UseCompatibleStateImageBehavior = false;
            this.lsvEventsList.View = System.Windows.Forms.View.Details;
            this.lsvEventsList.SelectedIndexChanged += new System.EventHandler(this.lsvEventsList_SelectedIndexChanged);
            // 
            // colTime
            // 
            this.colTime.Text = "Time";
            this.colTime.Width = 61;
            // 
            // colEvent
            // 
            this.colEvent.Text = "Event";
            this.colEvent.Width = 105;
            // 
            // colInnerEvent
            // 
            this.colInnerEvent.Text = "Inner Event";
            this.colInnerEvent.Width = 118;
            // 
            // colConnectionState
            // 
            this.colConnectionState.Text = "Connection State";
            this.colConnectionState.Width = 113;
            // 
            // colException
            // 
            this.colException.Text = "Exception";
            this.colException.Width = 156;
            // 
            // EventPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkPostEvents);
            this.Controls.Add(this.lblGameEvent);
            this.Controls.Add(this.lsvEventsList);
            this.Controls.Add(this.grpQuick);
            this.Name = "EventPanel";
            this.Size = new System.Drawing.Size(828, 612);
            this.grpQuick.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpQuick;
        private System.Windows.Forms.PropertyGrid quickActionsPropertyGrid;
        private ListViewNF lsvEventsList;
        private System.Windows.Forms.ColumnHeader colEvent;
        private System.Windows.Forms.ColumnHeader colConnectionState;
        private System.Windows.Forms.ColumnHeader colException;
        private System.Windows.Forms.Label lblGameEvent;
        private System.Windows.Forms.ColumnHeader colInnerEvent;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.CheckBox chkPostEvents;
    }
}
