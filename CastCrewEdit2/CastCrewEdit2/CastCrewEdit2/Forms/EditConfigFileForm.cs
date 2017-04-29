using System;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    internal partial class EditConfigFileForm : Form
    {
        private String FileName;
        private String FileContent;

        public EditConfigFileForm(String fileName, String name)
        {
            FileName = fileName;
            InitializeComponent();
            Text = String.Format(EditWindowNames.EditConfigFile, name);
            DialogResult = DialogResult.None;
            using (StreamReader sr = new StreamReader(FileName))
            {
                FileContent = sr.ReadToEnd().Trim();
            }
        }

        private void OnEditConfigFileFormActivated(Object sender, EventArgs e)
        {
            EditTextBox.DeselectAll();
        }

        private void OnSaveButtonClick(Object sender, EventArgs e)
        {
            SaveData();
            Close();
        }

        private void SaveData()
        {
            if (File.Exists(FileName + ".bak"))
            {
                File.Delete(FileName + ".bak");
            }
            if (File.Exists(FileName))
            {
                File.Move(FileName, FileName + ".bak");
            }
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                sw.Write(EditTextBox.Text.Trim());
            }
            DialogResult = DialogResult.Yes;
        }

        private void OnEditConfigFileFormFormClosing(Object sender, FormClosingEventArgs e)
        {
            if ((DialogResult != DialogResult.Yes) && (DialogResult != DialogResult.No))
            {
                if (FileContent != EditTextBox.Text)
                {
                    DialogResult result;

                    result = MessageBox.Show(this, MessageBoxTexts.SaveData, MessageBoxTexts.SaveData, MessageBoxButtons.YesNoCancel
                        , MessageBoxIcon.Question);
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

        private void OnEditConfigFileFormLoad(Object sender, EventArgs e)
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
            EditTextBox.Text = FileContent;
            ResumeLayout();
        }

        private void OnCloseWithoutSavingButtonClick(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void OnCloseButtonClick(Object sender, EventArgs e)
        {
            Close();
        }
    }
}