namespace Potato.Tools.NetworkConsole.Controls {
    partial class ProtocolTestControl {
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtProtocolTestFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.cboTests = new System.Windows.Forms.ComboBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.chkConnectionIsolation = new System.Windows.Forms.CheckBox();
            this.rtbProtocolTestOutput = new Potato.Tools.NetworkConsole.Controls.CodRichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Protocol Test File";
            // 
            // txtProtocolTestFile
            // 
            this.txtProtocolTestFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProtocolTestFile.Location = new System.Drawing.Point(3, 20);
            this.txtProtocolTestFile.Name = "txtProtocolTestFile";
            this.txtProtocolTestFile.ReadOnly = true;
            this.txtProtocolTestFile.Size = new System.Drawing.Size(457, 20);
            this.txtProtocolTestFile.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(466, 18);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(550, 440);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // cboTests
            // 
            this.cboTests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTests.FormattingEnabled = true;
            this.cboTests.Location = new System.Drawing.Point(3, 442);
            this.cboTests.Name = "cboTests";
            this.cboTests.Size = new System.Drawing.Size(413, 21);
            this.cboTests.TabIndex = 5;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Enabled = false;
            this.btnReload.Location = new System.Drawing.Point(547, 18);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 6;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // chkConnectionIsolation
            // 
            this.chkConnectionIsolation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkConnectionIsolation.AutoSize = true;
            this.chkConnectionIsolation.Location = new System.Drawing.Point(422, 444);
            this.chkConnectionIsolation.Name = "chkConnectionIsolation";
            this.chkConnectionIsolation.Size = new System.Drawing.Size(122, 17);
            this.chkConnectionIsolation.TabIndex = 7;
            this.chkConnectionIsolation.Text = "Connection Isolation";
            this.chkConnectionIsolation.UseVisualStyleBackColor = true;
            this.chkConnectionIsolation.CheckedChanged += new System.EventHandler(this.chkConnectionIsolation_CheckedChanged);
            // 
            // rtbProtocolTestOutput
            // 
            this.rtbProtocolTestOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbProtocolTestOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbProtocolTestOutput.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.rtbProtocolTestOutput.Location = new System.Drawing.Point(3, 46);
            this.rtbProtocolTestOutput.Name = "rtbProtocolTestOutput";
            this.rtbProtocolTestOutput.ReadOnly = true;
            this.rtbProtocolTestOutput.Size = new System.Drawing.Size(622, 388);
            this.rtbProtocolTestOutput.TabIndex = 3;
            this.rtbProtocolTestOutput.Text = "";
            // 
            // ProtocolTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkConnectionIsolation);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.cboTests);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.rtbProtocolTestOutput);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtProtocolTestFile);
            this.Controls.Add(this.label1);
            this.Name = "ProtocolTestControl";
            this.Size = new System.Drawing.Size(630, 472);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProtocolTestFile;
        private System.Windows.Forms.Button btnBrowse;
        private CodRichTextBox rtbProtocolTestOutput;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ComboBox cboTests;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.CheckBox chkConnectionIsolation;
    }
}
