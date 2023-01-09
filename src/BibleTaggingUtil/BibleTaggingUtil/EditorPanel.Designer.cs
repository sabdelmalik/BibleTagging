
namespace BibleTaggingUtil
{
    partial class EditorPanel
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
            System.Windows.Forms.Panel panel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDecreseFont = new System.Windows.Forms.Button();
            this.btnIncreasFont = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNextUnknownTag = new System.Windows.Forms.Button();
            this.btnResetVerse = new System.Windows.Forms.Button();
            this.lblNext = new System.Windows.Forms.Label();
            this.lblPrevious = new System.Windows.Forms.Label();
            this.tbCurrentReference = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tbTargetBible = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainerMainEditor = new System.Windows.Forms.SplitContainer();
            this.dgvReferenceVerse = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvTOTHTView = new System.Windows.Forms.DataGridView();
            this.dgvTargetVerse = new System.Windows.Forms.DataGridView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tbReferenceBible = new System.Windows.Forms.ToolStripTextBox();
            this.btnEbaleEdit = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMainEditor)).BeginInit();
            this.splitContainerMainEditor.Panel1.SuspendLayout();
            this.splitContainerMainEditor.Panel2.SuspendLayout();
            this.splitContainerMainEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReferenceVerse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTOTHTView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetVerse)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.btnEbaleEdit);
            panel1.Controls.Add(this.label1);
            panel1.Controls.Add(this.btnDecreseFont);
            panel1.Controls.Add(this.btnIncreasFont);
            panel1.Controls.Add(this.btnSave);
            panel1.Controls.Add(this.btnNextUnknownTag);
            panel1.Controls.Add(this.btnResetVerse);
            panel1.Controls.Add(this.lblNext);
            panel1.Controls.Add(this.lblPrevious);
            panel1.Controls.Add(this.tbCurrentReference);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 788);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1299, 72);
            panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1210, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Font";
            // 
            // btnDecreseFont
            // 
            this.btnDecreseFont.Location = new System.Drawing.Point(1169, 22);
            this.btnDecreseFont.Name = "btnDecreseFont";
            this.btnDecreseFont.Size = new System.Drawing.Size(41, 29);
            this.btnDecreseFont.TabIndex = 9;
            this.btnDecreseFont.Text = "<<";
            this.btnDecreseFont.UseVisualStyleBackColor = true;
            this.btnDecreseFont.Click += new System.EventHandler(this.btnDecreseFont_Click);
            // 
            // btnIncreasFont
            // 
            this.btnIncreasFont.Location = new System.Drawing.Point(1248, 22);
            this.btnIncreasFont.Name = "btnIncreasFont";
            this.btnIncreasFont.Size = new System.Drawing.Size(41, 29);
            this.btnIncreasFont.TabIndex = 9;
            this.btnIncreasFont.Text = ">>";
            this.btnIncreasFont.UseVisualStyleBackColor = true;
            this.btnIncreasFont.Click += new System.EventHandler(this.btnIncreasFont_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(611, 18);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 42);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNextUnknownTag
            // 
            this.btnNextUnknownTag.Location = new System.Drawing.Point(739, 18);
            this.btnNextUnknownTag.Name = "btnNextUnknownTag";
            this.btnNextUnknownTag.Size = new System.Drawing.Size(229, 42);
            this.btnNextUnknownTag.TabIndex = 7;
            this.btnNextUnknownTag.Text = "Next Unknown Tag";
            this.btnNextUnknownTag.UseVisualStyleBackColor = true;
            this.btnNextUnknownTag.Click += new System.EventHandler(this.btnNextUnknownTag_Click);
            // 
            // btnResetVerse
            // 
            this.btnResetVerse.Location = new System.Drawing.Point(441, 18);
            this.btnResetVerse.Name = "btnResetVerse";
            this.btnResetVerse.Size = new System.Drawing.Size(127, 42);
            this.btnResetVerse.TabIndex = 6;
            this.btnResetVerse.Text = "Reset Verse";
            this.btnResetVerse.UseVisualStyleBackColor = true;
            this.btnResetVerse.Click += new System.EventHandler(this.btnResetVerse_Click);
            // 
            // lblNext
            // 
            this.lblNext.AutoSize = true;
            this.lblNext.Image = ((System.Drawing.Image)(resources.GetObject("lblNext.Image")));
            this.lblNext.Location = new System.Drawing.Point(298, 9);
            this.lblNext.MaximumSize = new System.Drawing.Size(78, 51);
            this.lblNext.MinimumSize = new System.Drawing.Size(78, 51);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(78, 51);
            this.lblNext.TabIndex = 5;
            this.lblNext.Click += new System.EventHandler(this.lblNext_Click);
            // 
            // lblPrevious
            // 
            this.lblPrevious.AutoSize = true;
            this.lblPrevious.Image = ((System.Drawing.Image)(resources.GetObject("lblPrevious.Image")));
            this.lblPrevious.Location = new System.Drawing.Point(14, 9);
            this.lblPrevious.MaximumSize = new System.Drawing.Size(70, 51);
            this.lblPrevious.MinimumSize = new System.Drawing.Size(70, 51);
            this.lblPrevious.Name = "lblPrevious";
            this.lblPrevious.Size = new System.Drawing.Size(70, 51);
            this.lblPrevious.TabIndex = 5;
            this.lblPrevious.Click += new System.EventHandler(this.lblPrevious_Click);
            // 
            // tbCurrentReference
            // 
            this.tbCurrentReference.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tbCurrentReference.ForeColor = System.Drawing.Color.DarkRed;
            this.tbCurrentReference.Location = new System.Drawing.Point(98, 16);
            this.tbCurrentReference.Name = "tbCurrentReference";
            this.tbCurrentReference.ReadOnly = true;
            this.tbCurrentReference.Size = new System.Drawing.Size(185, 34);
            this.tbCurrentReference.TabIndex = 1;
            this.tbCurrentReference.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.tbTargetBible});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1299, 27);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(88, 24);
            this.toolStripLabel2.Text = "Target Bible";
            // 
            // tbTargetBible
            // 
            this.tbTargetBible.AutoSize = false;
            this.tbTargetBible.Name = "tbTargetBible";
            this.tbTargetBible.ReadOnly = true;
            this.tbTargetBible.Size = new System.Drawing.Size(350, 27);
            // 
            // splitContainerMainEditor
            // 
            this.splitContainerMainEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMainEditor.Location = new System.Drawing.Point(0, 54);
            this.splitContainerMainEditor.Name = "splitContainerMainEditor";
            this.splitContainerMainEditor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMainEditor.Panel1
            // 
            this.splitContainerMainEditor.Panel1.Controls.Add(this.dgvReferenceVerse);
            // 
            // splitContainerMainEditor.Panel2
            // 
            this.splitContainerMainEditor.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainerMainEditor.Size = new System.Drawing.Size(1299, 734);
            this.splitContainerMainEditor.SplitterDistance = 248;
            this.splitContainerMainEditor.TabIndex = 4;
            // 
            // dgvReferenceVerse
            // 
            this.dgvReferenceVerse.AllowDrop = true;
            this.dgvReferenceVerse.AllowUserToAddRows = false;
            this.dgvReferenceVerse.AllowUserToDeleteRows = false;
            this.dgvReferenceVerse.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvReferenceVerse.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReferenceVerse.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReferenceVerse.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReferenceVerse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReferenceVerse.Location = new System.Drawing.Point(0, 0);
            this.dgvReferenceVerse.Name = "dgvReferenceVerse";
            this.dgvReferenceVerse.ReadOnly = true;
            this.dgvReferenceVerse.RowHeadersWidth = 51;
            this.dgvReferenceVerse.RowTemplate.Height = 29;
            this.dgvReferenceVerse.Size = new System.Drawing.Size(1299, 248);
            this.dgvReferenceVerse.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvTOTHTView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvTargetVerse);
            this.splitContainer1.Size = new System.Drawing.Size(1299, 482);
            this.splitContainer1.SplitterDistance = 241;
            this.splitContainer1.TabIndex = 3;
            // 
            // dgvTOTHTView
            // 
            this.dgvTOTHTView.AllowDrop = true;
            this.dgvTOTHTView.AllowUserToAddRows = false;
            this.dgvTOTHTView.AllowUserToDeleteRows = false;
            this.dgvTOTHTView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTOTHTView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTOTHTView.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTOTHTView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTOTHTView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTOTHTView.Location = new System.Drawing.Point(0, 0);
            this.dgvTOTHTView.Name = "dgvTOTHTView";
            this.dgvTOTHTView.ReadOnly = true;
            this.dgvTOTHTView.RowHeadersWidth = 51;
            this.dgvTOTHTView.RowTemplate.Height = 29;
            this.dgvTOTHTView.Size = new System.Drawing.Size(1299, 241);
            this.dgvTOTHTView.TabIndex = 4;
            // 
            // dgvTargetVerse
            // 
            this.dgvTargetVerse.AllowDrop = true;
            this.dgvTargetVerse.AllowUserToAddRows = false;
            this.dgvTargetVerse.AllowUserToDeleteRows = false;
            this.dgvTargetVerse.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTargetVerse.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTargetVerse.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTargetVerse.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTargetVerse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTargetVerse.Location = new System.Drawing.Point(0, 0);
            this.dgvTargetVerse.Name = "dgvTargetVerse";
            this.dgvTargetVerse.RowHeadersWidth = 51;
            this.dgvTargetVerse.RowTemplate.Height = 29;
            this.dgvTargetVerse.Size = new System.Drawing.Size(1299, 237);
            this.dgvTargetVerse.TabIndex = 2;
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tbReferenceBible});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1299, 27);
            this.toolStrip2.TabIndex = 10;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(113, 24);
            this.toolStripLabel1.Text = "Reference Bible";
            // 
            // tbReferenceBible
            // 
            this.tbReferenceBible.AutoSize = false;
            this.tbReferenceBible.Name = "tbReferenceBible";
            this.tbReferenceBible.Size = new System.Drawing.Size(350, 27);
            // 
            // btnEbaleEdit
            // 
            this.btnEbaleEdit.Location = new System.Drawing.Point(1018, 25);
            this.btnEbaleEdit.Name = "btnEbaleEdit";
            this.btnEbaleEdit.Size = new System.Drawing.Size(94, 29);
            this.btnEbaleEdit.TabIndex = 11;
            this.btnEbaleEdit.Text = "Enable Edit";
            this.btnEbaleEdit.UseVisualStyleBackColor = true;
            this.btnEbaleEdit.Click += new System.EventHandler(this.btnEbaleEdit_Click);
            // 
            // EditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 860);
            this.Controls.Add(this.splitContainerMainEditor);
            this.Controls.Add(panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Name = "EditorPanel";
            this.Text = "EdirorPanel";
            this.Load += new System.EventHandler(this.EditorPanel_Load);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainerMainEditor.Panel1.ResumeLayout(false);
            this.splitContainerMainEditor.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMainEditor)).EndInit();
            this.splitContainerMainEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReferenceVerse)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTOTHTView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetVerse)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.TextBox tbCurrentReference;
        private System.Windows.Forms.Label lblPrevious;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox tbTargetBible;
        private System.Windows.Forms.SplitContainer splitContainerMainEditor;
        private System.Windows.Forms.DataGridView dgvTargetVerse;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox tbReferenceBible;
        private System.Windows.Forms.DataGridView dgvReferenceVerse;
        private System.Windows.Forms.Button btnResetVerse;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvTOTHTView;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNextUnknownTag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDecreseFont;
        private System.Windows.Forms.Button btnIncreasFont;
        private System.Windows.Forms.Button btnEbaleEdit;
    }
}