namespace CopyDrive
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tvDevices = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmCollapsAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.cbDriveToCopy = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectDeselectAll = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSIPwd = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cbPcInterface = new System.Windows.Forms.ComboBox();
            this.StatusMessages = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnOpenProjectBckGrnd = new System.Windows.Forms.Button();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvDevices
            // 
            this.tvDevices.CheckBoxes = true;
            this.tvDevices.ContextMenuStrip = this.contextMenuStrip1;
            this.tvDevices.HideSelection = false;
            this.tvDevices.Location = new System.Drawing.Point(6, 19);
            this.tvDevices.Name = "tvDevices";
            this.tvDevices.Size = new System.Drawing.Size(294, 313);
            this.tvDevices.TabIndex = 0;
            this.tvDevices.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.node_AfterCheck);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmCollapsAll,
            this.expandAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(131, 48);
            // 
            // tsmCollapsAll
            // 
            this.tsmCollapsAll.Name = "tsmCollapsAll";
            this.tsmCollapsAll.Size = new System.Drawing.Size(130, 22);
            this.tsmCollapsAll.Text = "Collaps All";
            this.tsmCollapsAll.Click += new System.EventHandler(this.tsmCollapsAll_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(280, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect To Open TIA Portal Project";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(6, 78);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(280, 23);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Start Copying Drives";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // cbDriveToCopy
            // 
            this.cbDriveToCopy.FormattingEnabled = true;
            this.cbDriveToCopy.Location = new System.Drawing.Point(6, 19);
            this.cbDriveToCopy.Name = "cbDriveToCopy";
            this.cbDriveToCopy.Size = new System.Drawing.Size(294, 21);
            this.cbDriveToCopy.TabIndex = 3;
            this.cbDriveToCopy.SelectedIndexChanged += new System.EventHandler(this.cbDriveToCopy_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDriveToCopy);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 53);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drive To Copy";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectDeselectAll);
            this.groupBox2.Controls.Add(this.tvDevices);
            this.groupBox2.Location = new System.Drawing.Point(12, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 367);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Drives To Copy To/Download";
            // 
            // btnSelectDeselectAll
            // 
            this.btnSelectDeselectAll.Location = new System.Drawing.Point(7, 338);
            this.btnSelectDeselectAll.Name = "btnSelectDeselectAll";
            this.btnSelectDeselectAll.Size = new System.Drawing.Size(293, 23);
            this.btnSelectDeselectAll.TabIndex = 1;
            this.btnSelectDeselectAll.Text = "Select/Deselect All";
            this.btnSelectDeselectAll.UseVisualStyleBackColor = true;
            this.btnSelectDeselectAll.Click += new System.EventHandler(this.btnSelectDeselectAll_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtSIPwd);
            this.groupBox3.Controls.Add(this.progressBar);
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnDownload);
            this.groupBox3.Controls.Add(this.cbPcInterface);
            this.groupBox3.Location = new System.Drawing.Point(324, 233);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(292, 205);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Download Drives";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Safety Password:";
            // 
            // txtSIPwd
            // 
            this.txtSIPwd.Location = new System.Drawing.Point(101, 105);
            this.txtSIPwd.Name = "txtSIPwd";
            this.txtSIPwd.Size = new System.Drawing.Size(185, 20);
            this.txtSIPwd.TabIndex = 4;
            this.txtSIPwd.Text = "0";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(7, 176);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(279, 23);
            this.progressBar.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(6, 75);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(280, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(6, 46);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(280, 23);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // cbPcInterface
            // 
            this.cbPcInterface.FormattingEnabled = true;
            this.cbPcInterface.Location = new System.Drawing.Point(6, 19);
            this.cbPcInterface.Name = "cbPcInterface";
            this.cbPcInterface.Size = new System.Drawing.Size(280, 21);
            this.cbPcInterface.TabIndex = 0;
            // 
            // StatusMessages
            // 
            this.StatusMessages.Location = new System.Drawing.Point(18, 444);
            this.StatusMessages.Name = "StatusMessages";
            this.StatusMessages.Size = new System.Drawing.Size(592, 133);
            this.StatusMessages.TabIndex = 7;
            this.StatusMessages.Text = "";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnOpenProjectBckGrnd);
            this.groupBox4.Controls.Add(this.btnConnect);
            this.groupBox4.Controls.Add(this.btnCopy);
            this.groupBox4.Location = new System.Drawing.Point(324, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(292, 215);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            // 
            // btnOpenProjectBckGrnd
            // 
            this.btnOpenProjectBckGrnd.Location = new System.Drawing.Point(6, 48);
            this.btnOpenProjectBckGrnd.Name = "btnOpenProjectBckGrnd";
            this.btnOpenProjectBckGrnd.Size = new System.Drawing.Size(280, 23);
            this.btnOpenProjectBckGrnd.TabIndex = 3;
            this.btnOpenProjectBckGrnd.Text = "Open Project in BackGround";
            this.btnOpenProjectBckGrnd.UseVisualStyleBackColor = true;
            this.btnOpenProjectBckGrnd.Click += new System.EventHandler(this.btnOpenProjectBckGrnd_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 584);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.StatusMessages);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "CopyDrive_V16";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvDevices;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ComboBox cbDriveToCopy;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbPcInterface;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.RichTextBox StatusMessages;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnOpenProjectBckGrnd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnSelectDeselectAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSIPwd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmCollapsAll;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
    }
}

