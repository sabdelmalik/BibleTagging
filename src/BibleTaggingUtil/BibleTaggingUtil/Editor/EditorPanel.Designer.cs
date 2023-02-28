
namespace BibleTaggingUtil.Editor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Panel panel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.picRedo = new System.Windows.Forms.PictureBox();
            this.picFindTagForward = new System.Windows.Forms.PictureBox();
            this.picDecreaseFont = new System.Windows.Forms.PictureBox();
            this.picIncreaseFont = new System.Windows.Forms.PictureBox();
            this.picEnableEdit = new System.Windows.Forms.PictureBox();
            this.picUndo = new System.Windows.Forms.PictureBox();
            this.picPrevVerse = new System.Windows.Forms.PictureBox();
            this.picNextVerse = new System.Windows.Forms.PictureBox();
            this.picSave = new System.Windows.Forms.PictureBox();
            this.cbTagToFind = new System.Windows.Forms.ComboBox();
            this.tbCurrentReference = new System.Windows.Forms.TextBox();
            this.splitContainerMainEditor = new System.Windows.Forms.SplitContainer();
            this.dgvReferenceVerse = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvTOTHTView = new System.Windows.Forms.DataGridView();
            this.dgvTargetVerse = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRedo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFindTagForward)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDecreaseFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIncreaseFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnableEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUndo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPrevVerse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNextVerse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSave)).BeginInit();
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
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.picRedo);
            panel1.Controls.Add(this.picFindTagForward);
            panel1.Controls.Add(this.picDecreaseFont);
            panel1.Controls.Add(this.picIncreaseFont);
            panel1.Controls.Add(this.picEnableEdit);
            panel1.Controls.Add(this.picUndo);
            panel1.Controls.Add(this.picPrevVerse);
            panel1.Controls.Add(this.picNextVerse);
            panel1.Controls.Add(this.picSave);
            panel1.Controls.Add(this.cbTagToFind);
            panel1.Controls.Add(this.tbCurrentReference);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 788);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1299, 72);
            panel1.TabIndex = 8;
            // 
            // picRedo
            // 
            this.picRedo.Image = ((System.Drawing.Image)(resources.GetObject("picRedo.Image")));
            this.picRedo.Location = new System.Drawing.Point(405, 15);
            this.picRedo.Name = "picRedo";
            this.picRedo.Size = new System.Drawing.Size(40, 40);
            this.picRedo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRedo.TabIndex = 21;
            this.picRedo.TabStop = false;
            this.toolTip1.SetToolTip(this.picRedo, "Redo");
            this.picRedo.Click += new System.EventHandler(this.picRedo_Click);
            // 
            // picFindTagForward
            // 
            this.picFindTagForward.Image = ((System.Drawing.Image)(resources.GetObject("picFindTagForward.Image")));
            this.picFindTagForward.Location = new System.Drawing.Point(617, 15);
            this.picFindTagForward.Name = "picFindTagForward";
            this.picFindTagForward.Size = new System.Drawing.Size(40, 40);
            this.picFindTagForward.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFindTagForward.TabIndex = 20;
            this.picFindTagForward.TabStop = false;
            this.toolTip1.SetToolTip(this.picFindTagForward, "Find Tag Forward");
            this.picFindTagForward.Click += new System.EventHandler(this.picFindTagForward_Click);
            // 
            // picDecreaseFont
            // 
            this.picDecreaseFont.Image = ((System.Drawing.Image)(resources.GetObject("picDecreaseFont.Image")));
            this.picDecreaseFont.Location = new System.Drawing.Point(564, 15);
            this.picDecreaseFont.Name = "picDecreaseFont";
            this.picDecreaseFont.Size = new System.Drawing.Size(40, 40);
            this.picDecreaseFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDecreaseFont.TabIndex = 19;
            this.picDecreaseFont.TabStop = false;
            this.toolTip1.SetToolTip(this.picDecreaseFont, "Decrease Font Size");
            this.picDecreaseFont.Click += new System.EventHandler(this.picDecreaseFont_Click);
            // 
            // picIncreaseFont
            // 
            this.picIncreaseFont.Image = ((System.Drawing.Image)(resources.GetObject("picIncreaseFont.Image")));
            this.picIncreaseFont.Location = new System.Drawing.Point(511, 15);
            this.picIncreaseFont.Name = "picIncreaseFont";
            this.picIncreaseFont.Size = new System.Drawing.Size(40, 40);
            this.picIncreaseFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIncreaseFont.TabIndex = 18;
            this.picIncreaseFont.TabStop = false;
            this.toolTip1.SetToolTip(this.picIncreaseFont, "Increase Font Size");
            this.picIncreaseFont.Click += new System.EventHandler(this.picIncreaseFont_Click);
            // 
            // picEnableEdit
            // 
            this.picEnableEdit.Image = ((System.Drawing.Image)(resources.GetObject("picEnableEdit.Image")));
            this.picEnableEdit.Location = new System.Drawing.Point(458, 15);
            this.picEnableEdit.Name = "picEnableEdit";
            this.picEnableEdit.Size = new System.Drawing.Size(40, 40);
            this.picEnableEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picEnableEdit.TabIndex = 17;
            this.picEnableEdit.TabStop = false;
            this.toolTip1.SetToolTip(this.picEnableEdit, "Enable Target Editing ");
            this.picEnableEdit.Click += new System.EventHandler(this.picEnableEdit_Click);
            // 
            // picUndo
            // 
            this.picUndo.Image = ((System.Drawing.Image)(resources.GetObject("picUndo.Image")));
            this.picUndo.Location = new System.Drawing.Point(352, 15);
            this.picUndo.Name = "picUndo";
            this.picUndo.Size = new System.Drawing.Size(40, 40);
            this.picUndo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUndo.TabIndex = 16;
            this.picUndo.TabStop = false;
            this.toolTip1.SetToolTip(this.picUndo, "Undo");
            this.picUndo.Click += new System.EventHandler(this.picUndo_Click);
            // 
            // picPrevVerse
            // 
            this.picPrevVerse.Image = ((System.Drawing.Image)(resources.GetObject("picPrevVerse.Image")));
            this.picPrevVerse.Location = new System.Drawing.Point(12, 17);
            this.picPrevVerse.Name = "picPrevVerse";
            this.picPrevVerse.Size = new System.Drawing.Size(40, 40);
            this.picPrevVerse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPrevVerse.TabIndex = 15;
            this.picPrevVerse.TabStop = false;
            this.toolTip1.SetToolTip(this.picPrevVerse, "Previous Verse");
            this.picPrevVerse.Click += new System.EventHandler(this.picPrevVerse_Click);
            // 
            // picNextVerse
            // 
            this.picNextVerse.Image = ((System.Drawing.Image)(resources.GetObject("picNextVerse.Image")));
            this.picNextVerse.Location = new System.Drawing.Point(233, 17);
            this.picNextVerse.Name = "picNextVerse";
            this.picNextVerse.Size = new System.Drawing.Size(40, 40);
            this.picNextVerse.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNextVerse.TabIndex = 14;
            this.picNextVerse.TabStop = false;
            this.toolTip1.SetToolTip(this.picNextVerse, "Next Verse");
            this.picNextVerse.Click += new System.EventHandler(this.picNextVerse_Click);
            // 
            // picSave
            // 
            this.picSave.Image = ((System.Drawing.Image)(resources.GetObject("picSave.Image")));
            this.picSave.Location = new System.Drawing.Point(299, 15);
            this.picSave.Name = "picSave";
            this.picSave.Size = new System.Drawing.Size(40, 40);
            this.picSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSave.TabIndex = 13;
            this.picSave.TabStop = false;
            this.toolTip1.SetToolTip(this.picSave, "Save");
            this.picSave.Click += new System.EventHandler(this.picSave_Click);
            // 
            // cbTagToFind
            // 
            this.cbTagToFind.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbTagToFind.FormattingEnabled = true;
            this.cbTagToFind.Items.AddRange(new object[] {
            "???",
            "<blank>"});
            this.cbTagToFind.Location = new System.Drawing.Point(663, 17);
            this.cbTagToFind.Name = "cbTagToFind";
            this.cbTagToFind.Size = new System.Drawing.Size(127, 28);
            this.cbTagToFind.TabIndex = 12;
            this.cbTagToFind.Text = "???";
            // 
            // tbCurrentReference
            // 
            this.tbCurrentReference.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tbCurrentReference.ForeColor = System.Drawing.Color.DarkRed;
            this.tbCurrentReference.Location = new System.Drawing.Point(58, 15);
            this.tbCurrentReference.Name = "tbCurrentReference";
            this.tbCurrentReference.ReadOnly = true;
            this.tbCurrentReference.Size = new System.Drawing.Size(177, 34);
            this.tbCurrentReference.TabIndex = 1;
            this.tbCurrentReference.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // splitContainerMainEditor
            // 
            this.splitContainerMainEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMainEditor.Location = new System.Drawing.Point(0, 0);
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
            this.splitContainerMainEditor.Size = new System.Drawing.Size(1299, 788);
            this.splitContainerMainEditor.SplitterDistance = 266;
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
            this.dgvReferenceVerse.Size = new System.Drawing.Size(1299, 266);
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
            this.splitContainer1.Size = new System.Drawing.Size(1299, 518);
            this.splitContainer1.SplitterDistance = 259;
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
            this.dgvTOTHTView.Size = new System.Drawing.Size(1299, 259);
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
            this.dgvTargetVerse.Size = new System.Drawing.Size(1299, 255);
            this.dgvTargetVerse.TabIndex = 2;
            // 
            // EditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 860);
            this.Controls.Add(this.splitContainerMainEditor);
            this.Controls.Add(panel1);
            this.Name = "EditorPanel";
            this.Text = "EdirorPanel";
            this.Load += new System.EventHandler(this.EditorPanel_Load);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRedo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFindTagForward)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDecreaseFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIncreaseFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnableEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUndo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPrevVerse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNextVerse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSave)).EndInit();
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
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.TextBox tbCurrentReference;
        private System.Windows.Forms.PictureBox picPrevVerse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainerMainEditor;
        private System.Windows.Forms.DataGridView dgvTargetVerse;
        private System.Windows.Forms.DataGridView dgvReferenceVerse;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvTOTHTView;
        private System.Windows.Forms.ComboBox cbTagToFind;
        private System.Windows.Forms.PictureBox picNextVerse;
        private System.Windows.Forms.PictureBox picSave;
        private System.Windows.Forms.PictureBox picUndo;
        private System.Windows.Forms.PictureBox picEnableEdit;
        private System.Windows.Forms.PictureBox picDecreaseFont;
        private System.Windows.Forms.PictureBox picIncreaseFont;
        private System.Windows.Forms.PictureBox picFindTagForward;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picRedo;
    }
}