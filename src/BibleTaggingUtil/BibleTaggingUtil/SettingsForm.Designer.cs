
namespace BibleTaggingUtil
{
    partial class SettingsForm
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
            this.cbPeriodicSave = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.nudSavePeriod = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudSavePeriod)).BeginInit();
            this.SuspendLayout();
            // 
            // cbPeriodicSave
            // 
            this.cbPeriodicSave.AutoSize = true;
            this.cbPeriodicSave.Location = new System.Drawing.Point(52, 33);
            this.cbPeriodicSave.Name = "cbPeriodicSave";
            this.cbPeriodicSave.Size = new System.Drawing.Size(119, 24);
            this.cbPeriodicSave.TabIndex = 0;
            this.cbPeriodicSave.Text = "Periodic Save";
            this.cbPeriodicSave.UseVisualStyleBackColor = true;
            this.cbPeriodicSave.CheckedChanged += new System.EventHandler(this.cbPeriodicSave_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Evrey";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minutes";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(171, 309);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(331, 309);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 35);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // nudSavePeriod
            // 
            this.nudSavePeriod.Enabled = false;
            this.nudSavePeriod.Location = new System.Drawing.Point(139, 70);
            this.nudSavePeriod.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudSavePeriod.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSavePeriod.Name = "nudSavePeriod";
            this.nudSavePeriod.Size = new System.Drawing.Size(58, 27);
            this.nudSavePeriod.TabIndex = 5;
            this.nudSavePeriod.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(612, 374);
            this.Controls.Add(this.nudSavePeriod);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbPeriodicSave);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.nudSavePeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbPeriodicSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.NumericUpDown nudSavePeriod;
    }
}