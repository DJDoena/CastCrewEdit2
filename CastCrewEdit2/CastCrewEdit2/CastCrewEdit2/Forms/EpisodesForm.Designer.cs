namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    partial class EpisodesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EpisodesForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.ScanEpisodesButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.SettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditConfigFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FirstnamePrefixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LastnamePrefixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LastnameSuffixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.KnownNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IMDbToDVDProfilerTransformationDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IgnoreCustomInIMDbCreditTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IgnoreIMDbCreditTypeInOtherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ForcedFakeBirthYearsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReadmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ScanAllEpisodesButton = new System.Windows.Forms.Button();
            this.ProgressBar = new DoenaSoft.DVDProfiler.CastCrewEdit2.ColorProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            resources.ApplyResources(this.DataGridView, "DataGridView");
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // ScanEpisodesButton
            // 
            resources.ApplyResources(this.ScanEpisodesButton, "ScanEpisodesButton");
            this.ScanEpisodesButton.Name = "ScanEpisodesButton";
            this.ScanEpisodesButton.UseVisualStyleBackColor = true;
            this.ScanEpisodesButton.Click += new System.EventHandler(this.OnScanEpisodesButtonClick);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // MenuStrip
            // 
            resources.ApplyResources(this.MenuStrip, "MenuStrip");
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingsToolStripMenuItem,
            this.EditConfigFilesToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.MenuStrip.Name = "MenuStrip";
            // 
            // SettingsToolStripMenuItem
            // 
            resources.ApplyResources(this.SettingsToolStripMenuItem, "SettingsToolStripMenuItem");
            this.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem";
            // 
            // EditConfigFilesToolStripMenuItem
            // 
            resources.ApplyResources(this.EditConfigFilesToolStripMenuItem, "EditConfigFilesToolStripMenuItem");
            this.EditConfigFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FirstnamePrefixesToolStripMenuItem,
            this.LastnamePrefixesToolStripMenuItem,
            this.LastnameSuffixesToolStripMenuItem,
            this.KnownNamesToolStripMenuItem,
            this.IMDbToDVDProfilerTransformationDataToolStripMenuItem,
            this.IgnoreCustomInIMDbCreditTypeToolStripMenuItem,
            this.IgnoreIMDbCreditTypeInOtherToolStripMenuItem,
            this.ForcedFakeBirthYearsToolStripMenuItem});
            this.EditConfigFilesToolStripMenuItem.Name = "EditConfigFilesToolStripMenuItem";
            // 
            // FirstnamePrefixesToolStripMenuItem
            // 
            resources.ApplyResources(this.FirstnamePrefixesToolStripMenuItem, "FirstnamePrefixesToolStripMenuItem");
            this.FirstnamePrefixesToolStripMenuItem.Name = "FirstnamePrefixesToolStripMenuItem";
            // 
            // LastnamePrefixesToolStripMenuItem
            // 
            resources.ApplyResources(this.LastnamePrefixesToolStripMenuItem, "LastnamePrefixesToolStripMenuItem");
            this.LastnamePrefixesToolStripMenuItem.Name = "LastnamePrefixesToolStripMenuItem";
            // 
            // LastnameSuffixesToolStripMenuItem
            // 
            resources.ApplyResources(this.LastnameSuffixesToolStripMenuItem, "LastnameSuffixesToolStripMenuItem");
            this.LastnameSuffixesToolStripMenuItem.Name = "LastnameSuffixesToolStripMenuItem";
            // 
            // KnownNamesToolStripMenuItem
            // 
            resources.ApplyResources(this.KnownNamesToolStripMenuItem, "KnownNamesToolStripMenuItem");
            this.KnownNamesToolStripMenuItem.Name = "KnownNamesToolStripMenuItem";
            // 
            // IMDbToDVDProfilerTransformationDataToolStripMenuItem
            // 
            resources.ApplyResources(this.IMDbToDVDProfilerTransformationDataToolStripMenuItem, "IMDbToDVDProfilerTransformationDataToolStripMenuItem");
            this.IMDbToDVDProfilerTransformationDataToolStripMenuItem.Name = "IMDbToDVDProfilerTransformationDataToolStripMenuItem";
            // 
            // IgnoreCustomInIMDbCreditTypeToolStripMenuItem
            // 
            resources.ApplyResources(this.IgnoreCustomInIMDbCreditTypeToolStripMenuItem, "IgnoreCustomInIMDbCreditTypeToolStripMenuItem");
            this.IgnoreCustomInIMDbCreditTypeToolStripMenuItem.Name = "IgnoreCustomInIMDbCreditTypeToolStripMenuItem";
            // 
            // IgnoreIMDbCreditTypeInOtherToolStripMenuItem
            // 
            resources.ApplyResources(this.IgnoreIMDbCreditTypeInOtherToolStripMenuItem, "IgnoreIMDbCreditTypeInOtherToolStripMenuItem");
            this.IgnoreIMDbCreditTypeInOtherToolStripMenuItem.Name = "IgnoreIMDbCreditTypeInOtherToolStripMenuItem";
            // 
            // ForcedFakeBirthYearsToolStripMenuItem
            // 
            resources.ApplyResources(this.ForcedFakeBirthYearsToolStripMenuItem, "ForcedFakeBirthYearsToolStripMenuItem");
            this.ForcedFakeBirthYearsToolStripMenuItem.Name = "ForcedFakeBirthYearsToolStripMenuItem";
            // 
            // HelpToolStripMenuItem
            // 
            resources.ApplyResources(this.HelpToolStripMenuItem, "HelpToolStripMenuItem");
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReadmeToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            // 
            // ReadmeToolStripMenuItem
            // 
            resources.ApplyResources(this.ReadmeToolStripMenuItem, "ReadmeToolStripMenuItem");
            this.ReadmeToolStripMenuItem.Name = "ReadmeToolStripMenuItem";
            // 
            // AboutToolStripMenuItem
            // 
            resources.ApplyResources(this.AboutToolStripMenuItem, "AboutToolStripMenuItem");
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            // 
            // ScanAllEpisodesButton
            // 
            resources.ApplyResources(this.ScanAllEpisodesButton, "ScanAllEpisodesButton");
            this.ScanAllEpisodesButton.Name = "ScanAllEpisodesButton";
            this.ScanAllEpisodesButton.UseVisualStyleBackColor = true;
            this.ScanAllEpisodesButton.Click += new System.EventHandler(this.OnScanAllEpisodesButtonClick);
            // 
            // ProgressBar
            // 
            resources.ApplyResources(this.ProgressBar, "ProgressBar");
            this.ProgressBar.BackColor = System.Drawing.SystemColors.Control;
            this.ProgressBar.BarColor = System.Drawing.SystemColors.Control;
            this.ProgressBar.BorderColor = System.Drawing.Color.Black;
            this.ProgressBar.FillStyle = DoenaSoft.DVDProfiler.CastCrewEdit2.ColorProgressBar.FillStyles.Solid;
            this.ProgressBar.ForeColor = System.Drawing.Color.White;
            this.ProgressBar.Maximum = 0;
            this.ProgressBar.Minimum = 0;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Step = 0;
            this.ProgressBar.Value = 0;
            // 
            // EpisodesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.ScanAllEpisodesButton);
            this.Controls.Add(this.MenuStrip);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ScanEpisodesButton);
            this.Controls.Add(this.DataGridView);
            this.MinimizeBox = false;
            this.Name = "EpisodesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnEpisodeFormClosing);
            this.Load += new System.EventHandler(this.OnEpisodeFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.Button ScanEpisodesButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem SettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditConfigFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirstnamePrefixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LastnamePrefixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LastnameSuffixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem KnownNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IMDbToDVDProfilerTransformationDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReadmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.Button ScanAllEpisodesButton;
        private System.Windows.Forms.ToolStripMenuItem IgnoreCustomInIMDbCreditTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IgnoreIMDbCreditTypeInOtherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ForcedFakeBirthYearsToolStripMenuItem;
        private ColorProgressBar ProgressBar;
    }
}

