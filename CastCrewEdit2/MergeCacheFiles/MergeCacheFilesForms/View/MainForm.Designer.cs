namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LeftFileTextBox = new System.Windows.Forms.TextBox();
            this.RightFileTextBox = new System.Windows.Forms.TextBox();
            this.SelectLeftFileButton = new System.Windows.Forms.Button();
            this.SelectRightFileButton = new System.Windows.Forms.Button();
            this.MergeThirdFileButton = new System.Windows.Forms.Button();
            this.SelectTargetFileButton = new System.Windows.Forms.Button();
            this.TargetFileTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MergeButton = new System.Windows.Forms.Button();
            this.ClearFileNamesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left Cache File:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Right Cache File:";
            // 
            // LeftFileTextBox
            // 
            this.LeftFileTextBox.Location = new System.Drawing.Point(111, 14);
            this.LeftFileTextBox.Name = "LeftFileTextBox";
            this.LeftFileTextBox.ReadOnly = true;
            this.LeftFileTextBox.Size = new System.Drawing.Size(510, 20);
            this.LeftFileTextBox.TabIndex = 3;
            // 
            // RightFileTextBox
            // 
            this.RightFileTextBox.Location = new System.Drawing.Point(111, 43);
            this.RightFileTextBox.Name = "RightFileTextBox";
            this.RightFileTextBox.ReadOnly = true;
            this.RightFileTextBox.Size = new System.Drawing.Size(510, 20);
            this.RightFileTextBox.TabIndex = 4;
            // 
            // SelectLeftFileButton
            // 
            this.SelectLeftFileButton.Location = new System.Drawing.Point(627, 12);
            this.SelectLeftFileButton.Name = "SelectLeftFileButton";
            this.SelectLeftFileButton.Size = new System.Drawing.Size(32, 23);
            this.SelectLeftFileButton.TabIndex = 6;
            this.SelectLeftFileButton.Text = "...";
            this.SelectLeftFileButton.UseVisualStyleBackColor = true;
            this.SelectLeftFileButton.Click += new System.EventHandler(this.OnSelectLeftFileButtonClick);
            // 
            // SelectRightFileButton
            // 
            this.SelectRightFileButton.Location = new System.Drawing.Point(627, 41);
            this.SelectRightFileButton.Name = "SelectRightFileButton";
            this.SelectRightFileButton.Size = new System.Drawing.Size(32, 23);
            this.SelectRightFileButton.TabIndex = 7;
            this.SelectRightFileButton.Text = "...";
            this.SelectRightFileButton.UseVisualStyleBackColor = true;
            this.SelectRightFileButton.Click += new System.EventHandler(this.OnSelectRightFileButtonClick);
            // 
            // MergeThirdFileButton
            // 
            this.MergeThirdFileButton.Location = new System.Drawing.Point(528, 127);
            this.MergeThirdFileButton.Name = "MergeThirdFileButton";
            this.MergeThirdFileButton.Size = new System.Drawing.Size(131, 23);
            this.MergeThirdFileButton.TabIndex = 9;
            this.MergeThirdFileButton.Text = "Merge Into Third File";
            this.MergeThirdFileButton.UseVisualStyleBackColor = true;
            this.MergeThirdFileButton.Click += new System.EventHandler(this.OnMergeThirdFileButtonClick);
            // 
            // SelectTargetFileButton
            // 
            this.SelectTargetFileButton.Location = new System.Drawing.Point(627, 98);
            this.SelectTargetFileButton.Name = "SelectTargetFileButton";
            this.SelectTargetFileButton.Size = new System.Drawing.Size(32, 23);
            this.SelectTargetFileButton.TabIndex = 12;
            this.SelectTargetFileButton.Text = "...";
            this.SelectTargetFileButton.UseVisualStyleBackColor = true;
            this.SelectTargetFileButton.Click += new System.EventHandler(this.OnSelectTargetFileButtonClick);
            // 
            // TargetFileTextBox
            // 
            this.TargetFileTextBox.Location = new System.Drawing.Point(111, 100);
            this.TargetFileTextBox.Name = "TargetFileTextBox";
            this.TargetFileTextBox.ReadOnly = true;
            this.TargetFileTextBox.Size = new System.Drawing.Size(510, 20);
            this.TargetFileTextBox.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Merge Target File:";
            // 
            // MergeButton
            // 
            this.MergeButton.Location = new System.Drawing.Point(528, 69);
            this.MergeButton.Name = "MergeButton";
            this.MergeButton.Size = new System.Drawing.Size(131, 23);
            this.MergeButton.TabIndex = 13;
            this.MergeButton.Text = "Merge Into Each Other";
            this.MergeButton.UseVisualStyleBackColor = true;
            this.MergeButton.Click += new System.EventHandler(this.OnMergeButtonClick);
            // 
            // ClearFileNamesButton
            // 
            this.ClearFileNamesButton.Location = new System.Drawing.Point(528, 155);
            this.ClearFileNamesButton.Name = "ClearFileNamesButton";
            this.ClearFileNamesButton.Size = new System.Drawing.Size(131, 23);
            this.ClearFileNamesButton.TabIndex = 14;
            this.ClearFileNamesButton.Text = "Clear File Names";
            this.ClearFileNamesButton.UseVisualStyleBackColor = true;
            this.ClearFileNamesButton.Click += new System.EventHandler(this.OnClearFileNamesButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 190);
            this.Controls.Add(this.ClearFileNamesButton);
            this.Controls.Add(this.MergeButton);
            this.Controls.Add(this.SelectTargetFileButton);
            this.Controls.Add(this.TargetFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MergeThirdFileButton);
            this.Controls.Add(this.SelectRightFileButton);
            this.Controls.Add(this.SelectLeftFileButton);
            this.Controls.Add(this.RightFileTextBox);
            this.Controls.Add(this.LeftFileTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Merge Cast/Crew Edit 2 Cache Files";
            this.Load += new System.EventHandler(this.OnMainFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LeftFileTextBox;
        private System.Windows.Forms.TextBox RightFileTextBox;
        private System.Windows.Forms.Button SelectLeftFileButton;
        private System.Windows.Forms.Button SelectRightFileButton;
        private System.Windows.Forms.Button MergeThirdFileButton;
        private System.Windows.Forms.Button SelectTargetFileButton;
        private System.Windows.Forms.TextBox TargetFileTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button MergeButton;
        private System.Windows.Forms.Button ClearFileNamesButton;
    }
}