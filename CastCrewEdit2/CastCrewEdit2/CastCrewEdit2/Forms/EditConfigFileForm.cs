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

            InitializeComponent();

            Text = string.Format(EditWindowNames.EditConfigFile, name);

            DialogResult = DialogResult.None;

            using (var sr = new StreamReader(_fileName))
            {
                _fileContent = sr.ReadToEnd().Trim();
            }

            Icon = Properties.Resource.djdsoft;
        }

        private void OnEditConfigFileFormActivated(object sender, EventArgs e) => EditTextBox.DeselectAll();

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            SaveData();

            Close();
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

            DialogResult = DialogResult.Yes;
        }

        private void OnEditConfigFileFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Yes && DialogResult != DialogResult.No)
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
                        SaveData();
                    }
                    else
                    {
                        DialogResult = DialogResult.No;
                    }
                }
                else
                {
                    DialogResult = DialogResult.No;
                }
            }

            Program.Settings.EditConfigFilesForm.Left = Left;
            Program.Settings.EditConfigFilesForm.Top = Top;
            Program.Settings.EditConfigFilesForm.Width = Width;
            Program.Settings.EditConfigFilesForm.Height = Height;
            Program.Settings.EditConfigFilesForm.WindowState = WindowState;
            Program.Settings.EditConfigFilesForm.RestoreBounds = RestoreBounds;
        }

        private void OnEditConfigFileFormLoad(object sender, EventArgs e)
        {
            SuspendLayout();

            if (Program.Settings.EditConfigFilesForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.EditConfigFilesForm.Left;
                Top = Program.Settings.EditConfigFilesForm.Top;
                Width = Program.Settings.EditConfigFilesForm.Width;
                Height = Program.Settings.EditConfigFilesForm.Height;
            }
            else
            {
                Left = Program.Settings.EditConfigFilesForm.RestoreBounds.X;
                Top = Program.Settings.EditConfigFilesForm.RestoreBounds.Y;
                Width = Program.Settings.EditConfigFilesForm.RestoreBounds.Width;
                Height = Program.Settings.EditConfigFilesForm.RestoreBounds.Height;
            }

            if (Program.Settings.EditConfigFilesForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.EditConfigFilesForm.WindowState;
            }

            EditTextBox.Text = _fileContent;

            ResumeLayout();
        }

        private void OnCloseWithoutSavingButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;

            Close();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}