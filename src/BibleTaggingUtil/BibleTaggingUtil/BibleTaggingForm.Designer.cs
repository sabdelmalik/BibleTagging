
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
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveKJVPlainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHebrewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSWORDFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usfmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateUSFMFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertUSFMToOSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
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
            this.dockPanel.Location = new System.Drawing.Point(0, 28);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.RightToLeftLayout = true;
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(1060, 522);
            this.dockPanel.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveKJVPlainToolStripMenuItem,
            this.saveHebrewToolStripMenuItem,
            this.nextVerseToolStripMenuItem,
            this.generateSWORDFilesToolStripMenuItem,
            this.usfmToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1060, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setBibleFolderToolStripMenuItem,
            this.saveUpdatedTartgetToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // setBibleFolderToolStripMenuItem
            // 
            this.setBibleFolderToolStripMenuItem.Name = "setBibleFolderToolStripMenuItem";
            this.setBibleFolderToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.setBibleFolderToolStripMenuItem.Text = "Set Bible Folder";
            this.setBibleFolderToolStripMenuItem.Click += new System.EventHandler(this.setBibleFolderToolStripMenuItem_Click);
            // 
            // saveUpdatedTartgetToolStripMenuItem
            // 
            this.saveUpdatedTartgetToolStripMenuItem.Name = "saveUpdatedTartgetToolStripMenuItem";
            this.saveUpdatedTartgetToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.saveUpdatedTartgetToolStripMenuItem.Text = "Save Updated Tartget";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.settingsToolStripMenuItem.Text = "settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
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
            // nextVerseToolStripMenuItem
            // 
            this.nextVerseToolStripMenuItem.Name = "nextVerseToolStripMenuItem";
            this.nextVerseToolStripMenuItem.Size = new System.Drawing.Size(117, 24);
            this.nextVerseToolStripMenuItem.Text = "Next ??? verse";
            this.nextVerseToolStripMenuItem.Click += new System.EventHandler(this.nextVerseToolStripMenuItem_Click);
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
            this.convertUSFMToOSISToolStripMenuItem});
            this.usfmToolStripMenuItem.Name = "usfmToolStripMenuItem";
            this.usfmToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            this.usfmToolStripMenuItem.Text = "USFM";
            // 
            // generateUSFMFilesToolStripMenuItem
            // 
            this.generateUSFMFilesToolStripMenuItem.Name = "generateUSFMFilesToolStripMenuItem";
            this.generateUSFMFilesToolStripMenuItem.Size = new System.Drawing.Size(238, 26);
            this.generateUSFMFilesToolStripMenuItem.Text = "Generate USFM Files";
            this.generateUSFMFilesToolStripMenuItem.Click += new System.EventHandler(this.generateUSFMFilesToolStripMenuItem_Click);
            // 
            // convertUSFMToOSISToolStripMenuItem
            // 
            this.convertUSFMToOSISToolStripMenuItem.Name = "convertUSFMToOSISToolStripMenuItem";
            this.convertUSFMToOSISToolStripMenuItem.Size = new System.Drawing.Size(238, 26);
            this.convertUSFMToOSISToolStripMenuItem.Text = "Convert USFM to OSIS";
            this.convertUSFMToOSISToolStripMenuItem.Click += new System.EventHandler(this.convertUSFMToOSISToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // BibleTaggingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 550);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BibleTaggingForm";
            this.Text = "Bible Tagging V3";
            this.Load += new System.EventHandler(this.BibleTaggingForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveKJVPlainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHebrewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextVerseToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usfmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateUSFMFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertUSFMToOSISToolStripMenuItem;
    }
}

