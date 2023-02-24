
namespace BibleTaggingUtil
{
    partial class BibleTaggingForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BibleTaggingForm));
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2013LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013LightTheme();
            this.vS2013BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme();
            this.vS2013DarkTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBibleFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveUpdatedTartgetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveKJVPlainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHebrewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSWORDFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usfmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateUSFMFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertUSFMToOSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSWORDFilesUsfmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateOSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSWORDFilesOsisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
            this.waitCursorAnimation = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitCursorAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(57)))), ((int)(((byte)(85)))));
            this.dockPanel.DockBottomPortion = 150D;
            this.dockPanel.DockLeftPortion = 200D;
            this.dockPanel.DockRightPortion = 200D;
            this.dockPanel.DockTopPortion = 150D;
            this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.dockPanel.Location = new System.Drawing.Point(0, 30);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.RightToLeftLayout = true;
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(1061, 519);
            this.dockPanel.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.saveKJVPlainToolStripMenuItem,
            this.saveHebrewToolStripMenuItem,
            this.generateSWORDFilesToolStripMenuItem,
            this.usfmToolStripMenuItem,
            this.oSISToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1061, 30);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setBibleFolderToolStripMenuItem,
            this.saveUpdatedTartgetToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // setBibleFolderToolStripMenuItem
            // 
            this.setBibleFolderToolStripMenuItem.Name = "setBibleFolderToolStripMenuItem";
            this.setBibleFolderToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.setBibleFolderToolStripMenuItem.Text = "Set Bible Folder";
            this.setBibleFolderToolStripMenuItem.Click += new System.EventHandler(this.setBibleFolderToolStripMenuItem_Click);
            // 
            // saveUpdatedTartgetToolStripMenuItem
            // 
            this.saveUpdatedTartgetToolStripMenuItem.Name = "saveUpdatedTartgetToolStripMenuItem";
            this.saveUpdatedTartgetToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.saveUpdatedTartgetToolStripMenuItem.Text = "Save Updatest";
            this.saveUpdatedTartgetToolStripMenuItem.Click += new System.EventHandler(this.saveUpdatedTartgetToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // saveKJVPlainToolStripMenuItem
            // 
            this.saveKJVPlainToolStripMenuItem.Name = "saveKJVPlainToolStripMenuItem";
            this.saveKJVPlainToolStripMenuItem.Size = new System.Drawing.Size(118, 24);
            this.saveKJVPlainToolStripMenuItem.Text = "Save KJV Plain";
            this.saveKJVPlainToolStripMenuItem.Click += new System.EventHandler(this.saveKJVPlainToolStripMenuItem_Click);
            // 
            // saveHebrewToolStripMenuItem
            // 
            this.saveHebrewToolStripMenuItem.Name = "saveHebrewToolStripMenuItem";
            this.saveHebrewToolStripMenuItem.Size = new System.Drawing.Size(110, 24);
            this.saveHebrewToolStripMenuItem.Text = "Save Hebrew";
            this.saveHebrewToolStripMenuItem.Click += new System.EventHandler(this.saveHebrewToolStripMenuItem_Click);
            // 
            // generateSWORDFilesToolStripMenuItem
            // 
            this.generateSWORDFilesToolStripMenuItem.Name = "generateSWORDFilesToolStripMenuItem";
            this.generateSWORDFilesToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            this.generateSWORDFilesToolStripMenuItem.Text = "Generate SWORD Files";
            this.generateSWORDFilesToolStripMenuItem.Click += new System.EventHandler(this.generateSWORDFilesToolStripMenuItem_Click);
            // 
            // usfmToolStripMenuItem
            // 
            this.usfmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateUSFMFilesToolStripMenuItem,
            this.convertUSFMToOSISToolStripMenuItem,
            this.generateSWORDFilesUsfmToolStripMenuItem});
            this.usfmToolStripMenuItem.Name = "usfmToolStripMenuItem";
            this.usfmToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.usfmToolStripMenuItem.Text = "USFM";
            // 
            // generateUSFMFilesToolStripMenuItem
            // 
            this.generateUSFMFilesToolStripMenuItem.Name = "generateUSFMFilesToolStripMenuItem";
            this.generateUSFMFilesToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.generateUSFMFilesToolStripMenuItem.Text = "Generate USFM Files";
            this.generateUSFMFilesToolStripMenuItem.Click += new System.EventHandler(this.generateUSFMFilesToolStripMenuItem_Click);
            // 
            // convertUSFMToOSISToolStripMenuItem
            // 
            this.convertUSFMToOSISToolStripMenuItem.Name = "convertUSFMToOSISToolStripMenuItem";
            this.convertUSFMToOSISToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.convertUSFMToOSISToolStripMenuItem.Text = "Convert USFM to OSIS";
            this.convertUSFMToOSISToolStripMenuItem.Click += new System.EventHandler(this.convertUSFMToOSISToolStripMenuItem_Click);
            // 
            // generateSWORDFilesUsfmToolStripMenuItem
            // 
            this.generateSWORDFilesUsfmToolStripMenuItem.Name = "generateSWORDFilesUsfmToolStripMenuItem";
            this.generateSWORDFilesUsfmToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.generateSWORDFilesUsfmToolStripMenuItem.Text = "Generate SWORD Files";
            this.generateSWORDFilesUsfmToolStripMenuItem.Click += new System.EventHandler(this.generateSWORDFilesUsfmToolStripMenuItem_Click);
            // 
            // oSISToolStripMenuItem
            // 
            this.oSISToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateOSISToolStripMenuItem,
            this.generateSWORDFilesOsisToolStripMenuItem});
            this.oSISToolStripMenuItem.Name = "oSISToolStripMenuItem";
            this.oSISToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            this.oSISToolStripMenuItem.Text = "OSIS";
            // 
            // generateOSISToolStripMenuItem
            // 
            this.generateOSISToolStripMenuItem.Name = "generateOSISToolStripMenuItem";
            this.generateOSISToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.generateOSISToolStripMenuItem.Text = "Generate OSIS";
            this.generateOSISToolStripMenuItem.Click += new System.EventHandler(this.generateOSISToolStripMenuItem_Click);
            // 
            // generateSWORDFilesOsisToolStripMenuItem
            // 
            this.generateSWORDFilesOsisToolStripMenuItem.Name = "generateSWORDFilesOsisToolStripMenuItem";
            this.generateSWORDFilesOsisToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.generateSWORDFilesOsisToolStripMenuItem.Text = "Generate SWORD Files";
            this.generateSWORDFilesOsisToolStripMenuItem.Click += new System.EventHandler(this.generateSWORDFilesOsisToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // waitCursorAnimation
            // 
            this.waitCursorAnimation.Image = ((System.Drawing.Image)(resources.GetObject("waitCursorAnimation.Image")));
            this.waitCursorAnimation.Location = new System.Drawing.Point(414, 203);
            this.waitCursorAnimation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waitCursorAnimation.Name = "waitCursorAnimation";
            this.waitCursorAnimation.Size = new System.Drawing.Size(111, 111);
            this.waitCursorAnimation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.waitCursorAnimation.TabIndex = 4;
            this.waitCursorAnimation.TabStop = false;
            this.waitCursorAnimation.Visible = false;
            // 
            // BibleTaggingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 549);
            this.Controls.Add(this.waitCursorAnimation);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BibleTaggingForm";
            this.Load += new System.EventHandler(this.BibleTaggingForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitCursorAnimation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2013LightTheme vS2013LightTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme vS2013BlueTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme vS2013DarkTheme1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBibleFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveUpdatedTartgetToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem saveKJVPlainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHebrewToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usfmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateUSFMFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertUSFMToOSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesUsfmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateOSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesOsisToolStripMenuItem;
        private System.Windows.Forms.PictureBox waitCursorAnimation;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    }
}

