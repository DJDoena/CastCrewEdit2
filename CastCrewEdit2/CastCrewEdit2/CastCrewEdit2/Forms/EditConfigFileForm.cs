namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Resources;

    internal partial class EditConfigFileForm : Form
    {
        private readonly string _fileName;

        private readonly string _fileContent;

        public EditConfigFileForm(string fileName, string name)
        {
            _fileName = fileName;

            this.InitializeComponent();

            this.Text = string.Format(EditWindowNames.EditConfigFile, name);

            this.DialogResult = DialogResult.None;

            using (var sr = new StreamReader(_fileName))
            {
                _fileContent = sr.ReadToEnd().Trim();
            }

            this.Icon = Properties.Resource.djdsoft;
        }

        private void OnEditConfigFileFormActivated(object sender, EventArgs e) => EditTextBox.DeselectAll();

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            this.SaveData();

            this.Close();
        }

        private void SaveData()
        {
            if (File.Exists(_fileName + ".bak"))
            {
                File.Delete(_fileName + ".bak");
            }

            if (File.Exists(_fileName))
            {
                File.Move(_fileName, _fileName + ".bak");
            }

            using (var sw = new StreamWriter(_fileName))
            {
                sw.Write(EditTextBox.Text.Trim());
            }

            this.DialogResult = DialogResult.Yes;
        }

        private void OnEditConfigFileFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.Yes && this.DialogResult != DialogResult.No)
            {
                if (_fileContent != EditTextBox.Text)
                {
                    var result = MessageBox.Show(this, MessageBoxTexts.SaveData, MessageBoxTexts.SaveData, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;

                        return;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        this.SaveData();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.No;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.No;
                }
            }

            Program.Settings.EditConfigFilesForm.Left = this.Left;
            Program.Settings.EditConfigFilesForm.Top = this.Top;
            Program.Settings.EditConfigFilesForm.Width = this.Width;
            Program.Settings.EditConfigFilesForm.Height = this.Height;
            Program.Settings.EditConfigFilesForm.WindowState = this.WindowState;
            Program.Settings.EditConfigFilesForm.RestoreBounds = this.RestoreBounds;
        }

        private void OnEditConfigFileFormLoad(object sender, EventArgs e)
        {
            this.SuspendLayout();

            if (Program.Settings.EditConfigFilesForm.WindowState == FormWindowState.Normal)
            {
                this.Left = Program.Settings.EditConfigFilesForm.Left;
                this.Top = Program.Settings.EditConfigFilesForm.Top;
                this.Width = Program.Settings.EditConfigFilesForm.Width;
                this.Height = Program.Settings.EditConfigFilesForm.Height;
            }
            else
            {
                this.Left = Program.Settings.EditConfigFilesForm.RestoreBounds.X;
                this.Top = Program.Settings.EditConfigFilesForm.RestoreBounds.Y;
                this.Width = Program.Settings.EditConfigFilesForm.RestoreBounds.Width;
                this.Height = Program.Settings.EditConfigFilesForm.RestoreBounds.Height;
            }

            if (Program.Settings.EditConfigFilesForm.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Program.Settings.EditConfigFilesForm.WindowState;
            }

            EditTextBox.Text = _fileContent;

            this.ResumeLayout();
        }

        private void OnCloseWithoutSavingButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;

            this.Close();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}