namespace DoenaSoft.DVDProfiler.CheckForDuplicatesInCastCrewEdit2Cache
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
            if(disposing && (components != null))
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
            this.CacheFileTextBox = new System.Windows.Forms.TextBox();
            this.SelectCacheFileButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.DifferentParsingTab = new System.Windows.Forms.TabPage();
            this.DifferentInParsingDataGridView = new System.Windows.Forms.DataGridView();
            this.DifferentBirthYearsTab = new System.Windows.Forms.TabPage();
            this.DifferentBirthYearsDataGridView = new System.Windows.Forms.DataGridView();
            this.DifferentBirthYearsAllTab = new System.Windows.Forms.TabPage();
            this.DifferentBirthYearsAllDataGridView = new System.Windows.Forms.DataGridView();
            this.IdenticalTab = new System.Windows.Forms.TabPage();
            this.EverythingIdenticalgDataGridView = new System.Windows.Forms.DataGridView();
            this.TabControl.SuspendLayout();
            this.DifferentParsingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentInParsingDataGridView)).BeginInit();
            this.DifferentBirthYearsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentBirthYearsDataGridView)).BeginInit();
            this.DifferentBirthYearsAllTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentBirthYearsAllDataGridView)).BeginInit();
            this.IdenticalTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EverythingIdenticalgDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Cast/Crew Edit 2 Cache File:";
            // 
            // CacheFileTextBox
            // 
            this.CacheFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CacheFileTextBox.Location = new System.Drawing.Point(161, 14);
            this.CacheFileTextBox.Name = "CacheFileTextBox";
            this.CacheFileTextBox.ReadOnly = true;
            this.CacheFileTextBox.Size = new System.Drawing.Size(555, 20);
            this.CacheFileTextBox.TabIndex = 5;
            // 
            // SelectCacheFileButton
            // 
            this.SelectCacheFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectCacheFileButton.Location = new System.Drawing.Point(722, 12);
            this.SelectCacheFileButton.Name = "SelectCacheFileButton";
            this.SelectCacheFileButton.Size = new System.Drawing.Size(75, 23);
            this.SelectCacheFileButton.TabIndex = 4;
            this.SelectCacheFileButton.Text = "...";
            this.SelectCacheFileButton.UseVisualStyleBackColor = true;
            this.SelectCacheFileButton.Click += new System.EventHandler(this.OnSelectCacheFileButtonClick);
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(722, 41);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 7;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.OnStartButtonClick);
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.DifferentParsingTab);
            this.TabControl.Controls.Add(this.DifferentBirthYearsTab);
            this.TabControl.Controls.Add(this.DifferentBirthYearsAllTab);
            this.TabControl.Controls.Add(this.IdenticalTab);
            this.TabControl.Location = new System.Drawing.Point(15, 70);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(782, 477);
            this.TabControl.TabIndex = 9;
            // 
            // DifferentParsingTab
            // 
            this.DifferentParsingTab.Controls.Add(this.DifferentInParsingDataGridView);
            this.DifferentParsingTab.Location = new System.Drawing.Point(4, 22);
            this.DifferentParsingTab.Name = "DifferentParsingTab";
            this.DifferentParsingTab.Padding = new System.Windows.Forms.Padding(3);
            this.DifferentParsingTab.Size = new System.Drawing.Size(774, 451);
            this.DifferentParsingTab.TabIndex = 0;
            this.DifferentParsingTab.Text = "Different in Parsing";
            this.DifferentParsingTab.UseVisualStyleBackColor = true;
            // 
            // DifferentInParsingDataGridView
            // 
            this.DifferentInParsingDataGridView.AllowUserToAddRows = false;
            this.DifferentInParsingDataGridView.AllowUserToDeleteRows = false;
            this.DifferentInParsingDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DifferentInParsingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DifferentInParsingDataGridView.Location = new System.Drawing.Point(6, 6);
            this.DifferentInParsingDataGridView.Name = "DifferentInParsingDataGridView";
            this.DifferentInParsingDataGridView.ReadOnly = true;
            this.DifferentInParsingDataGridView.Size = new System.Drawing.Size(762, 439);
            this.DifferentInParsingDataGridView.TabIndex = 9;
            this.DifferentInParsingDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataGridViewCellContentClick);
            // 
            // DifferentBirthYearsTab
            // 
            this.DifferentBirthYearsTab.Controls.Add(this.DifferentBirthYearsDataGridView);
            this.DifferentBirthYearsTab.Location = new System.Drawing.Point(4, 22);
            this.DifferentBirthYearsTab.Name = "DifferentBirthYearsTab";
            this.DifferentBirthYearsTab.Padding = new System.Windows.Forms.Padding(3);
            this.DifferentBirthYearsTab.Size = new System.Drawing.Size(774, 451);
            this.DifferentBirthYearsTab.TabIndex = 3;
            this.DifferentBirthYearsTab.Text = "Different Birth Years";
            this.DifferentBirthYearsTab.UseVisualStyleBackColor = true;
            // 
            // DifferentBirthYearsDataGridView
            // 
            this.DifferentBirthYearsDataGridView.AllowUserToAddRows = false;
            this.DifferentBirthYearsDataGridView.AllowUserToDeleteRows = false;
            this.DifferentBirthYearsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DifferentBirthYearsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DifferentBirthYearsDataGridView.Location = new System.Drawing.Point(6, 6);
            this.DifferentBirthYearsDataGridView.Name = "DifferentBirthYearsDataGridView";
            this.DifferentBirthYearsDataGridView.ReadOnly = true;
            this.DifferentBirthYearsDataGridView.Size = new System.Drawing.Size(762, 439);
            this.DifferentBirthYearsDataGridView.TabIndex = 10;
            this.DifferentBirthYearsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataGridViewCellContentClick);
            // 
            // DifferentBirthYearsAllTab
            // 
            this.DifferentBirthYearsAllTab.Controls.Add(this.DifferentBirthYearsAllDataGridView);
            this.DifferentBirthYearsAllTab.Location = new System.Drawing.Point(4, 22);
            this.DifferentBirthYearsAllTab.Name = "DifferentBirthYearsAllTab";
            this.DifferentBirthYearsAllTab.Padding = new System.Windows.Forms.Padding(3);
            this.DifferentBirthYearsAllTab.Size = new System.Drawing.Size(774, 451);
            this.DifferentBirthYearsAllTab.TabIndex = 1;
            this.DifferentBirthYearsAllTab.Text = "Different Birth Years (all have one)";
            this.DifferentBirthYearsAllTab.UseVisualStyleBackColor = true;
            // 
            // DifferentBirthYearsAllDataGridView
            // 
            this.DifferentBirthYearsAllDataGridView.AllowUserToAddRows = false;
            this.DifferentBirthYearsAllDataGridView.AllowUserToDeleteRows = false;
            this.DifferentBirthYearsAllDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DifferentBirthYearsAllDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DifferentBirthYearsAllDataGridView.Location = new System.Drawing.Point(6, 6);
            this.DifferentBirthYearsAllDataGridView.Name = "DifferentBirthYearsAllDataGridView";
            this.DifferentBirthYearsAllDataGridView.ReadOnly = true;
            this.DifferentBirthYearsAllDataGridView.Size = new System.Drawing.Size(762, 439);
            this.DifferentBirthYearsAllDataGridView.TabIndex = 10;
            this.DifferentBirthYearsAllDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataGridViewCellContentClick);
            // 
            // IdenticalTab
            // 
            this.IdenticalTab.Controls.Add(this.EverythingIdenticalgDataGridView);
            this.IdenticalTab.Location = new System.Drawing.Point(4, 22);
            this.IdenticalTab.Name = "IdenticalTab";
            this.IdenticalTab.Padding = new System.Windows.Forms.Padding(3);
            this.IdenticalTab.Size = new System.Drawing.Size(774, 451);
            this.IdenticalTab.TabIndex = 2;
            this.IdenticalTab.Text = "Everything Identical";
            this.IdenticalTab.UseVisualStyleBackColor = true;
            // 
            // EverythingIdenticalgDataGridView
            // 
            this.EverythingIdenticalgDataGridView.AllowUserToAddRows = false;
            this.EverythingIdenticalgDataGridView.AllowUserToDeleteRows = false;
            this.EverythingIdenticalgDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EverythingIdenticalgDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EverythingIdenticalgDataGridView.Location = new System.Drawing.Point(6, 6);
            this.EverythingIdenticalgDataGridView.Name = "EverythingIdenticalgDataGridView";
            this.EverythingIdenticalgDataGridView.ReadOnly = true;
            this.EverythingIdenticalgDataGridView.Size = new System.Drawing.Size(762, 439);
            this.EverythingIdenticalgDataGridView.TabIndex = 10;
            this.EverythingIdenticalgDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnDataGridViewCellContentClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 559);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CacheFileTextBox);
            this.Controls.Add(this.SelectCacheFileButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Check for Duplicates in Cast/Crew Edit 2 Cache";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMainFormClosing);
            this.Load += new System.EventHandler(this.OnMainFormLoad);
            this.TabControl.ResumeLayout(false);
            this.DifferentParsingTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DifferentInParsingDataGridView)).EndInit();
            this.DifferentBirthYearsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DifferentBirthYearsDataGridView)).EndInit();
            this.DifferentBirthYearsAllTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DifferentBirthYearsAllDataGridView)).EndInit();
            this.IdenticalTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.EverythingIdenticalgDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CacheFileTextBox;
        private System.Windows.Forms.Button SelectCacheFileButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage DifferentParsingTab;
        private System.Windows.Forms.DataGridView DifferentInParsingDataGridView;

        private System.Windows.Forms.TabPage DifferentBirthYearsAllTab;
        private System.Windows.Forms.DataGridView DifferentBirthYearsAllDataGridView;
        private System.Windows.Forms.TabPage IdenticalTab;
        private System.Windows.Forms.DataGridView EverythingIdenticalgDataGridView;
        private System.Windows.Forms.TabPage DifferentBirthYearsTab;
        private System.Windows.Forms.DataGridView DifferentBirthYearsDataGridView;
    }
}

