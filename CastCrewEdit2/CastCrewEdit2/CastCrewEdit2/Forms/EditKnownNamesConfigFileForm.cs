using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    internal partial class EditKnownNamesConfigFileForm : Form
    {
        private String FileName;
        private String FileContent;

        public EditKnownNamesConfigFileForm(String fileName, String name)
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

        private void OnSaveButtonClick(Object sender, EventArgs e)
        {
            String newFileContent;

            if (GetDataFromGrid(out newFileContent))
            {
                SaveData(newFileContent);
                Close();
            }
        }

        private void SaveData(String newFileContent)
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
                sw.Write(newFileContent);
            }
            DialogResult = DialogResult.Yes;
        }

        private void OnEditConfigFileFormFormClosing(Object sender, FormClosingEventArgs e)
        {
            if ((DialogResult != DialogResult.Yes) && (DialogResult != DialogResult.No))
            {
                String newFileContent;

                if (GetDataFromGrid(out newFileContent) == false)
                {
                    e.Cancel = true;
                    return;
                }
                if (FileContent != newFileContent)
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
                        SaveData(newFileContent);
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
            Program.Settings.EditKnownNamesConfigFileForm.Left = Left;
            Program.Settings.EditKnownNamesConfigFileForm.Top = Top;
            Program.Settings.EditKnownNamesConfigFileForm.Width = Width;
            Program.Settings.EditKnownNamesConfigFileForm.Height = Height;
            Program.Settings.EditKnownNamesConfigFileForm.WindowState = WindowState;
            Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds = RestoreBounds;
        }

        private Boolean GetDataFromGrid(out String newFileContent)
        {
            StringBuilder sb;
            Dictionary<String, Boolean> fullNames;

            sb = new StringBuilder();
            fullNames = new Dictionary<String, Boolean>(KnownNamesDataGridView.Rows.Count);
            for (Int32 i = 0; i < KnownNamesDataGridView.Rows.Count - 1; i++)
            {
                DataGridViewRow row;
                DataGridViewCell cell;
                String value;

                row = KnownNamesDataGridView.Rows[i];
                cell = row.Cells["FullName"];
                if ((cell.Value == null) || (String.IsNullOrEmpty(cell.Value.ToString())))
                {
                    MessageBox.Show(this, MessageBoxTexts.EmptyFullName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileContent = null;
                    return (false);
                }
                value = cell.Value.ToString();
                if (fullNames.ContainsKey(value))
                {
                    MessageBox.Show(this, MessageBoxTexts.DuplicateFullName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileContent = null;
                    return (false);
                }
                fullNames.Add(value, true);
                sb.Append(value);
                sb.Append(";");
                cell = row.Cells["FirstName"];
                if ((cell.Value == null) || (String.IsNullOrEmpty(cell.Value.ToString())))
                {
                    MessageBox.Show(this, MessageBoxTexts.EmptyFirstName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileContent = null;
                    return (false);
                }
                value = cell.Value.ToString();
                sb.Append(value);
                sb.Append(";");
                cell = row.Cells["MiddleName"];
                if (cell.Value != null)
                {
                    value = cell.Value.ToString();
                }
                else
                {
                    value = String.Empty;
                }
                sb.Append(value);
                sb.Append(";");
                cell = row.Cells["LastName"];
                if (cell.Value != null)
                {
                    value = cell.Value.ToString();
                }
                else
                {
                    value = String.Empty;
                }
                sb.AppendLine(value);
            }
            newFileContent = sb.ToString().TrimEnd();
            return (true);
        }

        private void OnEditConfigFileFormLoad(Object sender, EventArgs e)
        {
            SuspendLayout();
            if (Program.Settings.EditConfigFilesForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.EditKnownNamesConfigFileForm.Left;
                Top = Program.Settings.EditKnownNamesConfigFileForm.Top;
                Width = Program.Settings.EditKnownNamesConfigFileForm.Width;
                Height = Program.Settings.EditKnownNamesConfigFileForm.Height;
            }
            else
            {
                Left = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.X;
                Top = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Y;
                Width = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Width;
                Height = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Height;
            }
            if (Program.Settings.EditConfigFilesForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.EditConfigFilesForm.WindowState;
            }
            CreateDataGrid();
            LoadData();
            ResumeLayout();
        }

        private void LoadData()
        {
            using (StringReader sr = new StringReader(FileContent))
            {
                while (true)
                {
                    String line;

                    line = sr.ReadLine();
                    if (line != null)
                    {
                        String[] split;

                        split = line.Split(';');
                        if (split.Length == 4)
                        {
                            DataGridViewRow row;

                            row = KnownNamesDataGridView.Rows[KnownNamesDataGridView.Rows.Add()];
                            row.Cells["FullName"].Value = split[0];
                            row.Cells["FirstName"].Value = split[1];
                            row.Cells["MiddleName"].Value = split[2];
                            row.Cells["LastName"].Value = split[3];
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void CreateDataGrid()
        {
            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn();
            column.Name = "FullName";
            column.HeaderText = DataGridViewTexts.FullName;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.Resizable = DataGridViewTriState.True;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            KnownNamesDataGridView.Columns.Add(column);
            column = new DataGridViewTextBoxColumn();
            column.Name = "FirstName";
            column.HeaderText = DataGridViewTexts.FirstName;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.Resizable = DataGridViewTriState.True;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            KnownNamesDataGridView.Columns.Add(column);
            column = new DataGridViewTextBoxColumn();
            column.Name = "MiddleName";
            column.HeaderText = DataGridViewTexts.MiddleName;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.Resizable = DataGridViewTriState.True;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            KnownNamesDataGridView.Columns.Add(column);
            column = new DataGridViewTextBoxColumn();
            column.Name = "LastName";
            column.HeaderText = DataGridViewTexts.LastName;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.Resizable = DataGridViewTriState.True;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            KnownNamesDataGridView.Columns.Add(column);
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