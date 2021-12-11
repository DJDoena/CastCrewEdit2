namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Resources;

    internal partial class EditKnownNamesConfigFileForm : Form
    {
        private readonly string _fileName;

        private readonly string _fileContent;

        public EditKnownNamesConfigFileForm(string fileName, string name)
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

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (this.GetDataFromGrid(out var newFileContent))
            {
                this.SaveData(newFileContent);

                this.Close();
            }
        }

        private void SaveData(string newFileContent)
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
                sw.Write(newFileContent);
            }

            this.DialogResult = DialogResult.Yes;
        }

        private void OnEditConfigFileFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.Yes && this.DialogResult != DialogResult.No)
            {
                if (!this.GetDataFromGrid(out var newFileContent))
                {
                    e.Cancel = true;

                    return;
                }

                if (_fileContent != newFileContent)
                {
                    var result = MessageBox.Show(this, MessageBoxTexts.SaveData, MessageBoxTexts.SaveData, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;

                        return;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        this.SaveData(newFileContent);
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

            Program.Settings.EditKnownNamesConfigFileForm.Left = this.Left;
            Program.Settings.EditKnownNamesConfigFileForm.Top = this.Top;
            Program.Settings.EditKnownNamesConfigFileForm.Width = this.Width;
            Program.Settings.EditKnownNamesConfigFileForm.Height = this.Height;
            Program.Settings.EditKnownNamesConfigFileForm.WindowState = this.WindowState;
            Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds = this.RestoreBounds;
        }

        private bool GetDataFromGrid(out string newFileContent)
        {
            var result = new StringBuilder();

            var fullNames = new Dictionary<string, bool>(KnownNamesDataGridView.Rows.Count);

            for (var rowIndex = 0; rowIndex < KnownNamesDataGridView.Rows.Count - 1; rowIndex++)
            {
                var row = KnownNamesDataGridView.Rows[rowIndex];

                var cell = row.Cells["FullName"];

                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    MessageBox.Show(this, MessageBoxTexts.EmptyFullName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    newFileContent = null;

                    return false;
                }

                var value = cell.Value.ToString();

                if (fullNames.ContainsKey(value))
                {
                    MessageBox.Show(this, MessageBoxTexts.DuplicateFullName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    newFileContent = null;

                    return false;
                }

                fullNames.Add(value, true);

                result.Append(value);
                result.Append(";");

                cell = row.Cells["FirstName"];

                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    MessageBox.Show(this, MessageBoxTexts.EmptyFirstName, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    newFileContent = null;

                    return false;
                }

                value = cell.Value.ToString();

                result.Append(value);
                result.Append(";");

                cell = row.Cells["MiddleName"];

                if (cell.Value != null)
                {
                    value = cell.Value.ToString();
                }
                else
                {
                    value = string.Empty;
                }

                result.Append(value);
                result.Append(";");

                cell = row.Cells["LastName"];

                if (cell.Value != null)
                {
                    value = cell.Value.ToString();
                }
                else
                {
                    value = string.Empty;
                }

                result.AppendLine(value);
            }

            newFileContent = result.ToString().TrimEnd();

            return true;
        }

        private void OnEditConfigFileFormLoad(object sender, EventArgs e)
        {
            this.SuspendLayout();

            if (Program.Settings.EditConfigFilesForm.WindowState == FormWindowState.Normal)
            {
                this.Left = Program.Settings.EditKnownNamesConfigFileForm.Left;
                this.Top = Program.Settings.EditKnownNamesConfigFileForm.Top;
                this.Width = Program.Settings.EditKnownNamesConfigFileForm.Width;
                this.Height = Program.Settings.EditKnownNamesConfigFileForm.Height;
            }
            else
            {
                this.Left = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.X;
                this.Top = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Y;
                this.Width = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Width;
                this.Height = Program.Settings.EditKnownNamesConfigFileForm.RestoreBounds.Height;
            }

            if (Program.Settings.EditConfigFilesForm.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Program.Settings.EditConfigFilesForm.WindowState;
            }

            this.CreateDataGrid();

            this.LoadData();

            this.ResumeLayout();
        }

        private void LoadData()
        {
            using (var sr = new StringReader(_fileContent))
            {
                while (true)
                {
                    var line = sr.ReadLine();

                    if (line != null)
                    {
                        var split = line.Split(';');

                        if (split.Length == 4)
                        {
                            var row = KnownNamesDataGridView.Rows[KnownNamesDataGridView.Rows.Add()];

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
            var fullNameColumn = new DataGridViewTextBoxColumn()
            {
                Name = "FullName",
                HeaderText = DataGridViewTexts.FullName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };

            var firstNameColumn = new DataGridViewTextBoxColumn()
            {
                Name = "FirstName",
                HeaderText = DataGridViewTexts.FirstName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };

            var middleNameColumn = new DataGridViewTextBoxColumn()
            {
                Name = "MiddleName",
                HeaderText = DataGridViewTexts.MiddleName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };

            var lastNameColumn = new DataGridViewTextBoxColumn()
            {
                Name = "LastName",
                HeaderText = DataGridViewTexts.LastName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };

            KnownNamesDataGridView.Columns.AddRange(fullNameColumn, firstNameColumn, middleNameColumn, lastNameColumn);
        }

        private void OnCloseWithoutSavingButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;

            this.Close();
        }

        private void OnCloseButtonClick(object sender, EventArgs e) => this.Close();
    }
}