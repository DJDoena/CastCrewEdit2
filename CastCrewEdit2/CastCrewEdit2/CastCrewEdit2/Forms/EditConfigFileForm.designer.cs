namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    partial class EditConfigFileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditConfigFileForm));
            this.EditTextBox = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CloseWithoutSavingButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EditTextBox
            // 
            resources.ApplyResources(this.EditTextBox, "EditTextBox");
            this.EditTextBox.Name = "EditTextBox";
            // 
            // SaveButton
            // 
            resources.ApplyResources(this.SaveButton, "SaveButton");
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            // 
            // CloseWithoutSavingButton
            // 
            resources.ApplyResources(this.CloseWithoutSavingButton, "CloseWithoutSavingButton");
            this.CloseWithoutSavingButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseWithoutSavingButton.Name = "CloseWithoutSavingButton";
            this.CloseWithoutSavingButton.UseVisualStyleBackColor = true;
            this.CloseWithoutSavingButton.Click += new System.EventHandler(this.OnCloseWithoutSavingButtonClick);
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // EditConfigFileForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.CloseWithoutSavingButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.EditTextBox);
            this.Name = "EditConfigFileForm";
            this.Activated += new System.EventHandler(this.OnEditConfigFileFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnEditConfigFileFormFormClosing);
            this.Load += new System.EventHandler(this.OnEditConfigFileFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TextBox EditTextBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CloseWithoutSavingButton;
        private System.Windows.Forms.Button CloseButton;
    }
}