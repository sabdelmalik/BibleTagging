
namespace BibleTagging
{
    partial class VerseSelectionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerseSelectionPanel));
            this.splitContainerBook = new System.Windows.Forms.SplitContainer();
            this.lbBookNames = new System.Windows.Forms.ListBox();
            this.splitContainerChVs = new System.Windows.Forms.SplitContainer();
            this.lbChapters = new System.Windows.Forms.ListBox();
            this.lbVerses = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripPrevious = new System.Windows.Forms.ToolStripLabel();
            this.toolStripNext = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBook)).BeginInit();
            this.splitContainerBook.Panel1.SuspendLayout();
            this.splitContainerBook.Panel2.SuspendLayout();
            this.splitContainerBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChVs)).BeginInit();
            this.splitContainerChVs.Panel1.SuspendLayout();
            this.splitContainerChVs.Panel2.SuspendLayout();
            this.splitContainerChVs.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerBook
            // 
            this.splitContainerBook.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainerBook.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBook.Location = new System.Drawing.Point(0, 25);
            this.splitContainerBook.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerBook.Name = "splitContainerBook";
            // 
            // splitContainerBook.Panel1
            // 
            this.splitContainerBook.Panel1.Controls.Add(this.lbBookNames);
            // 
            // splitContainerBook.Panel2
            // 
            this.splitContainerBook.Panel2.Controls.Add(this.splitContainerChVs);
            this.splitContainerBook.Size = new System.Drawing.Size(700, 313);
            this.splitContainerBook.SplitterDistance = 185;
            this.splitContainerBook.TabIndex = 2;
            // 
            // lbBookNames
            // 
            this.lbBookNames.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbBookNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbBookNames.FormattingEnabled = true;
            this.lbBookNames.ItemHeight = 15;
            this.lbBookNames.Location = new System.Drawing.Point(0, 0);
            this.lbBookNames.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbBookNames.Name = "lbBookNames";
            this.lbBookNames.Size = new System.Drawing.Size(185, 313);
            this.lbBookNames.TabIndex = 0;
            this.lbBookNames.SelectedIndexChanged += new System.EventHandler(this.lbBookNames_SelectedIndexChanged);
            // 
            // splitContainerChVs
            // 
            this.splitContainerChVs.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainerChVs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerChVs.Location = new System.Drawing.Point(0, 0);
            this.splitContainerChVs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerChVs.Name = "splitContainerChVs";
            // 
            // splitContainerChVs.Panel1
            // 
            this.splitContainerChVs.Panel1.Controls.Add(this.lbChapters);
            // 
            // splitContainerChVs.Panel2
            // 
            this.splitContainerChVs.Panel2.Controls.Add(this.lbVerses);
            this.splitContainerChVs.Size = new System.Drawing.Size(511, 313);
            this.splitContainerChVs.SplitterDistance = 227;
            this.splitContainerChVs.TabIndex = 0;
            // 
            // lbChapters
            // 
            this.lbChapters.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbChapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbChapters.FormattingEnabled = true;
            this.lbChapters.ItemHeight = 15;
            this.lbChapters.Location = new System.Drawing.Point(0, 0);
            this.lbChapters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbChapters.Name = "lbChapters";
            this.lbChapters.Size = new System.Drawing.Size(227, 313);
            this.lbChapters.TabIndex = 0;
            this.lbChapters.SelectedIndexChanged += new System.EventHandler(this.lbChapters_SelectedIndexChanged);
            // 
            // lbVerses
            // 
            this.lbVerses.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbVerses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbVerses.FormattingEnabled = true;
            this.lbVerses.ItemHeight = 15;
            this.lbVerses.Location = new System.Drawing.Point(0, 0);
            this.lbVerses.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbVerses.Name = "lbVerses";
            this.lbVerses.Size = new System.Drawing.Size(280, 313);
            this.lbVerses.TabIndex = 0;
            this.lbVerses.SelectedIndexChanged += new System.EventHandler(this.lbVerses_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPrevious,
            this.toolStripNext});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(700, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripPrevious
            // 
            this.toolStripPrevious.Image = ((System.Drawing.Image)(resources.GetObject("toolStripPrevious.Image")));
            this.toolStripPrevious.Name = "toolStripPrevious";
            this.toolStripPrevious.Size = new System.Drawing.Size(20, 22);
            this.toolStripPrevious.Click += new System.EventHandler(this.toolStripPrevious_Click);
            // 
            // toolStripNext
            // 
            this.toolStripNext.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripNext.Image = ((System.Drawing.Image)(resources.GetObject("toolStripNext.Image")));
            this.toolStripNext.Name = "toolStripNext";
            this.toolStripNext.Size = new System.Drawing.Size(20, 22);
            this.toolStripNext.Click += new System.EventHandler(this.toolStripNext_Click);
            // 
            // VerseSelectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.splitContainerBook);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "VerseSelectionPanel";
            this.Text = "VerseSelectionPanel";
            this.Load += new System.EventHandler(this.VerseSelectionPanel_Load);
            this.splitContainerBook.Panel1.ResumeLayout(false);
            this.splitContainerBook.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBook)).EndInit();
            this.splitContainerBook.ResumeLayout(false);
            this.splitContainerChVs.Panel1.ResumeLayout(false);
            this.splitContainerChVs.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChVs)).EndInit();
            this.splitContainerChVs.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerBook;
        private System.Windows.Forms.ListBox lbBookNames;
        private System.Windows.Forms.SplitContainer splitContainerChVs;
        private System.Windows.Forms.ListBox lbChapters;
        private System.Windows.Forms.ListBox lbVerses;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripPrevious;
        private System.Windows.Forms.ToolStripLabel toolStripNext;
    }
}