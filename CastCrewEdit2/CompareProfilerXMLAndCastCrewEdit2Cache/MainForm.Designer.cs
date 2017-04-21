namespace DoenaSoft.DVDProfiler.CompareProfilerXMLAndCastCrewEdit2Cache
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
            this.SelectCacheFileButton = new System.Windows.Forms.Button();
            this.SelectDVDProfilerXMLButton = new System.Windows.Forms.Button();
            this.CacheFileTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DVDProfilerXMLTextBox = new System.Windows.Forms.TextBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.CacheDataGridView = new System.Windows.Forms.DataGridView();
            this.CacheFirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CacheMiddleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CacheLastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CacheBirthYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.CollectionDataGridView = new System.Windows.Forms.DataGridView();
            this.CollectionFirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CollectionMiddleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CollectionLastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CollectionBirthYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CollectionIMDbLink = new System.Windows.Forms.DataGridViewLinkColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CacheDataGridView)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CollectionDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectCacheFileButton
            // 
            this.SelectCacheFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectCacheFileButton.Location = new System.Drawing.Point(858, 12);
            this.SelectCacheFileButton.Name = "SelectCacheFileButton";
            this.SelectCacheFileButton.Size = new System.Drawing.Size(75, 23);
            this.SelectCacheFileButton.TabIndex = 0;
            this.SelectCacheFileButton.Text = "...";
            this.SelectCacheFileButton.UseVisualStyleBackColor = true;
            this.SelectCacheFileButton.Click += new System.EventHandler(this.OnSelectCacheFileButtonClick);
            // 
            // SelectDVDProfilerXMLButton
            // 
            this.SelectDVDProfilerXMLButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectDVDProfilerXMLButton.Location = new System.Drawing.Point(858, 41);
            this.SelectDVDProfilerXMLButton.Name = "SelectDVDProfilerXMLButton";
            this.SelectDVDProfilerXMLButton.Size = new System.Drawing.Size(75, 23);
            this.SelectDVDProfilerXMLButton.TabIndex = 1;
            this.SelectDVDProfilerXMLButton.Text = "...";
            this.SelectDVDProfilerXMLButton.UseVisualStyleBackColor = true;
            this.SelectDVDProfilerXMLButton.Click += new System.EventHandler(this.OnSelectDVDProfilerXMLButtonClick);
            // 
            // CacheFileTextBox
            // 
            this.CacheFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CacheFileTextBox.Location = new System.Drawing.Point(161, 14);
            this.CacheFileTextBox.Name = "CacheFileTextBox";
            this.CacheFileTextBox.ReadOnly = true;
            this.CacheFileTextBox.Size = new System.Drawing.Size(691, 20);
            this.CacheFileTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Cast/Crew Edit 2 Cache File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "DVD Porfiler XML File:";
            // 
            // DVDProfilerXMLTextBox
            // 
            this.DVDProfilerXMLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DVDProfilerXMLTextBox.Location = new System.Drawing.Point(161, 43);
            this.DVDProfilerXMLTextBox.Name = "DVDProfilerXMLTextBox";
            this.DVDProfilerXMLTextBox.ReadOnly = true;
            this.DVDProfilerXMLTextBox.Size = new System.Drawing.Size(691, 20);
            this.DVDProfilerXMLTextBox.TabIndex = 5;
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(858, 70);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 6;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.OnStartButtonClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(15, 99);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(918, 552);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.CacheDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(910, 526);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Not in Cache File";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // CacheDataGridView
            // 
            this.CacheDataGridView.AllowUserToAddRows = false;
            this.CacheDataGridView.AllowUserToDeleteRows = false;
            this.CacheDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CacheDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CacheDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CacheFirstName,
            this.CacheMiddleName,
            this.CacheLastName,
            this.CacheBirthYear});
            this.CacheDataGridView.Location = new System.Drawing.Point(6, 6);
            this.CacheDataGridView.Name = "CacheDataGridView";
            this.CacheDataGridView.ReadOnly = true;
            this.CacheDataGridView.Size = new System.Drawing.Size(898, 514);
            this.CacheDataGridView.TabIndex = 9;
            // 
            // CacheFirstName
            // 
            this.CacheFirstName.HeaderText = "First Name";
            this.CacheFirstName.Name = "CacheFirstName";
            this.CacheFirstName.ReadOnly = true;
            // 
            // CacheMiddleName
            // 
            this.CacheMiddleName.HeaderText = "Middle Name";
            this.CacheMiddleName.Name = "CacheMiddleName";
            this.CacheMiddleName.ReadOnly = true;
            // 
            // CacheLastName
            // 
            this.CacheLastName.HeaderText = "Last Name";
            this.CacheLastName.Name = "CacheLastName";
            this.CacheLastName.ReadOnly = true;
            // 
            // CacheBirthYear
            // 
            this.CacheBirthYear.HeaderText = "Birth Year";
            this.CacheBirthYear.Name = "CacheBirthYear";
            this.CacheBirthYear.ReadOnly = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.CollectionDataGridView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(910, 526);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Not in Collection File";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // CollectionDataGridView
            // 
            this.CollectionDataGridView.AllowUserToAddRows = false;
            this.CollectionDataGridView.AllowUserToDeleteRows = false;
            this.CollectionDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CollectionDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CollectionDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CollectionFirstName,
            this.CollectionMiddleName,
            this.CollectionLastName,
            this.CollectionBirthYear,
            this.CollectionIMDbLink});
            this.CollectionDataGridView.Location = new System.Drawing.Point(6, 6);
            this.CollectionDataGridView.Name = "CollectionDataGridView";
            this.CollectionDataGridView.ReadOnly = true;
            this.CollectionDataGridView.Size = new System.Drawing.Size(898, 514);
            this.CollectionDataGridView.TabIndex = 9;
            this.CollectionDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCollectionDataGridViewCellContentClick);
            // 
            // CollectionFirstName
            // 
            this.CollectionFirstName.HeaderText = "First Name";
            this.CollectionFirstName.Name = "CollectionFirstName";
            this.CollectionFirstName.ReadOnly = true;
            // 
            // CollectionMiddleName
            // 
            this.CollectionMiddleName.HeaderText = "Middle Name";
            this.CollectionMiddleName.Name = "CollectionMiddleName";
            this.CollectionMiddleName.ReadOnly = true;
            // 
            // CollectionLastName
            // 
            this.CollectionLastName.HeaderText = "Last Name";
            this.CollectionLastName.Name = "CollectionLastName";
            this.CollectionLastName.ReadOnly = true;
            // 
            // CollectionBirthYear
            // 
            this.CollectionBirthYear.HeaderText = "Birth Year";
            this.CollectionBirthYear.Name = "CollectionBirthYear";
            this.CollectionBirthYear.ReadOnly = true;
            // 
            // CollectionIMDbLink
            // 
            this.CollectionIMDbLink.HeaderText = "IMDb Link";
            this.CollectionIMDbLink.Name = "CollectionIMDbLink";
            this.CollectionIMDbLink.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 663);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.DVDProfilerXMLTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CacheFileTextBox);
            this.Controls.Add(this.SelectDVDProfilerXMLButton);
            this.Controls.Add(this.SelectCacheFileButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(750, 550);
            this.Name = "MainForm";
            this.Text = "Compare DVD Profiler XML and Cast/Crew Edit 2 Cache";
            this.Load += new System.EventHandler(this.OnMainFormLoad);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CacheDataGridView)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CollectionDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectCacheFileButton;
        private System.Windows.Forms.Button SelectDVDProfilerXMLButton;
        private System.Windows.Forms.TextBox CacheFileTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DVDProfilerXMLTextBox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView CacheDataGridView;
        private System.Windows.Forms.DataGridView CollectionDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn CollectionFirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CollectionMiddleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CollectionLastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CollectionBirthYear;
        private System.Windows.Forms.DataGridViewLinkColumn CollectionIMDbLink;
        private System.Windows.Forms.DataGridViewTextBoxColumn CacheFirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CacheMiddleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CacheLastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CacheBirthYear;
    }
}

