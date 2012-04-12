namespace Procon.Net.Console.Controls {
    partial class ChatPanel {
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
            System.Windows.Forms.ColumnHeader colTime;
            System.Windows.Forms.ColumnHeader colSource;
            System.Windows.Forms.ColumnHeader colAuthor;
            System.Windows.Forms.ColumnHeader colText;
            this.lblChatObjectList = new System.Windows.Forms.Label();
            this.lsvChatObjects = new Procon.Net.Console.Controls.ListViewNF();
            this.lblRawChat = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnAction = new System.Windows.Forms.Button();
            this.lblQuickAnnounce = new System.Windows.Forms.Label();
            this.txtQuickAnnounce = new System.Windows.Forms.TextBox();
            this.btnQuickAnnounce = new System.Windows.Forms.Button();
            this.chatPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.rtbChat = new Procon.Net.Console.Controls.CodRichTextBox();
            colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // colTime
            // 
            colTime.Text = "Time";
            // 
            // colSource
            // 
            colSource.Text = "Source";
            // 
            // colAuthor
            // 
            colAuthor.Text = "Author";
            // 
            // colText
            // 
            colText.Text = "Text";
            // 
            // lblChatObjectList
            // 
            this.lblChatObjectList.AutoSize = true;
            this.lblChatObjectList.Location = new System.Drawing.Point(3, 239);
            this.lblChatObjectList.Name = "lblChatObjectList";
            this.lblChatObjectList.Size = new System.Drawing.Size(82, 13);
            this.lblChatObjectList.TabIndex = 10;
            this.lblChatObjectList.Text = "Chat Object List";
            // 
            // lsvChatObjects
            // 
            this.lsvChatObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvChatObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            colTime,
            colSource,
            colAuthor,
            colText});
            this.lsvChatObjects.FullRowSelect = true;
            this.lsvChatObjects.GridLines = true;
            this.lsvChatObjects.Location = new System.Drawing.Point(3, 255);
            this.lsvChatObjects.Name = "lsvChatObjects";
            this.lsvChatObjects.Size = new System.Drawing.Size(557, 346);
            this.lsvChatObjects.TabIndex = 9;
            this.lsvChatObjects.UseCompatibleStateImageBehavior = false;
            this.lsvChatObjects.View = System.Windows.Forms.View.Details;
            this.lsvChatObjects.SelectedIndexChanged += new System.EventHandler(this.lsvChatObjects_SelectedIndexChanged);
            // 
            // lblRawChat
            // 
            this.lblRawChat.AutoSize = true;
            this.lblRawChat.Location = new System.Drawing.Point(5, 7);
            this.lblRawChat.Name = "lblRawChat";
            this.lblRawChat.Size = new System.Drawing.Size(54, 13);
            this.lblRawChat.TabIndex = 12;
            this.lblRawChat.Text = "Raw Chat";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(566, 572);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnAction
            // 
            this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Location = new System.Drawing.Point(744, 572);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(75, 23);
            this.btnAction.TabIndex = 14;
            this.btnAction.Text = "Action";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // lblQuickAnnounce
            // 
            this.lblQuickAnnounce.AutoSize = true;
            this.lblQuickAnnounce.Location = new System.Drawing.Point(3, 198);
            this.lblQuickAnnounce.Name = "lblQuickAnnounce";
            this.lblQuickAnnounce.Size = new System.Drawing.Size(87, 13);
            this.lblQuickAnnounce.TabIndex = 15;
            this.lblQuickAnnounce.Text = "Quick Announce";
            // 
            // txtQuickAnnounce
            // 
            this.txtQuickAnnounce.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuickAnnounce.Location = new System.Drawing.Point(6, 214);
            this.txtQuickAnnounce.Name = "txtQuickAnnounce";
            this.txtQuickAnnounce.Size = new System.Drawing.Size(732, 20);
            this.txtQuickAnnounce.TabIndex = 16;
            this.txtQuickAnnounce.TextChanged += new System.EventHandler(this.txtQuickAnnounce_TextChanged);
            this.txtQuickAnnounce.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuickAnnounce_KeyDown);
            // 
            // btnQuickAnnounce
            // 
            this.btnQuickAnnounce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuickAnnounce.Location = new System.Drawing.Point(744, 211);
            this.btnQuickAnnounce.Name = "btnQuickAnnounce";
            this.btnQuickAnnounce.Size = new System.Drawing.Size(75, 23);
            this.btnQuickAnnounce.TabIndex = 17;
            this.btnQuickAnnounce.Text = "Send";
            this.btnQuickAnnounce.UseVisualStyleBackColor = true;
            this.btnQuickAnnounce.Click += new System.EventHandler(this.btnQuickAnnounce_Click);
            // 
            // chatPropertyGrid
            // 
            this.chatPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatPropertyGrid.Location = new System.Drawing.Point(566, 255);
            this.chatPropertyGrid.Name = "chatPropertyGrid";
            this.chatPropertyGrid.Size = new System.Drawing.Size(253, 311);
            this.chatPropertyGrid.TabIndex = 18;
            // 
            // rtbChat
            // 
            this.rtbChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbChat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.rtbChat.Location = new System.Drawing.Point(6, 23);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ReadOnly = true;
            this.rtbChat.Size = new System.Drawing.Size(813, 172);
            this.rtbChat.TabIndex = 8;
            this.rtbChat.Text = "";
            // 
            // ChatPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.chatPropertyGrid);
            this.Controls.Add(this.lsvChatObjects);
            this.Controls.Add(this.btnQuickAnnounce);
            this.Controls.Add(this.txtQuickAnnounce);
            this.Controls.Add(this.lblQuickAnnounce);
            this.Controls.Add(this.lblRawChat);
            this.Controls.Add(this.lblChatObjectList);
            this.Controls.Add(this.rtbChat);
            this.Name = "ChatPanel";
            this.Size = new System.Drawing.Size(831, 604);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblChatObjectList;
        private ListViewNF lsvChatObjects;
        private CodRichTextBox rtbChat;
        private System.Windows.Forms.Label lblRawChat;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Label lblQuickAnnounce;
        private System.Windows.Forms.TextBox txtQuickAnnounce;
        private System.Windows.Forms.Button btnQuickAnnounce;
        private System.Windows.Forms.PropertyGrid chatPropertyGrid;
    }
}
