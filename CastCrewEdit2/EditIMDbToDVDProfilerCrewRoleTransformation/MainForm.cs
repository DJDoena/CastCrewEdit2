using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml.Serialization;
using System.Reflection;
using DoenaSoft.DVDProfiler.DVDProfilerXML;
using System.IO;
using System.Diagnostics;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.EditIMDbToDVDProfilerCrewRoleTransformation
{
    internal partial class MainForm : Form
    {
        private static class ColumnNames
        {
            public static String IMDbCreditType = "IMDb Category";
            public static String DVDProfilerCreditType = "DVD Profiler Category";
            public static String IMDbCreditSubtype = "IMDb Role";
            public static String DVDProfilerCreditSubtype = "DVD Profiler Role";
            public static String DVDProfilerCustomRole = "DVD Profiler CustomRole";
            public static String StartsWith = "Starts With";
        }

        private Boolean SkipVersionCheck;
        private Int32 PreviousRow = -1;
        private List<DoenaSoft.DVDProfiler.CastCrewEdit2.CreditType> CreditTypes;
        private Boolean IsInit = false;
        private Boolean NeedToSave = false;
        private Boolean NewCreditTypeRow = false;
        private Boolean CellChangeOnNewCreditTypeRow = false;

        public MainForm(Boolean skipVersionCheck)
        {
            SkipVersionCheck = skipVersionCheck;
            InitializeComponent();
            this.Icon = Properties.Resource.djdsoft;
        }

        private void OnMainFormLoad(Object sender, EventArgs e)
        {
            IsInit = true;
            SuspendLayout();
            if (Program.Settings.MainForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.MainForm.Left;
                Top = Program.Settings.MainForm.Top;
                Width = Program.Settings.MainForm.Width;
                Height = Program.Settings.MainForm.Height;
            }
            else
            {
                Left = Program.Settings.MainForm.RestoreBounds.X;
                Top = Program.Settings.MainForm.RestoreBounds.Y;
                Width = Program.Settings.MainForm.RestoreBounds.Width;
                Height = Program.Settings.MainForm.RestoreBounds.Height;
            }
            if (Program.Settings.MainForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.MainForm.WindowState;
            }
            CreateColumns();
            CreditTypeDataGridView.SelectionChanged += OnCreditTypeDataGridViewSelectionChanged;
            CreditTypeDataGridView.DefaultValuesNeeded += OnCreditTypeDataGridViewDefaultValuesNeeded;
            CreditSubtypeDataGridView.DefaultValuesNeeded += OnCreditSubtypeDataGridViewDefaultValuesNeeded;
            CreditTypeDataGridView.CellValueChanged += OnCreditTypeDataGridViewCellValueChanged;
            CreditTypeDataGridView.CellBeginEdit += OnDataGridViewCellBeginEdit;
            CreditSubtypeDataGridView.CellBeginEdit += OnDataGridViewCellBeginEdit;
            CreditSubtypeDataGridView.RowsRemoved += OnCreditSubtypeDataGridViewRowsRemoved;
            if (Program.TransformationData.CreditTypeList != null)
            {
                CreditTypes = new List<CastCrewEdit2.CreditType>(Program.TransformationData.CreditTypeList);
                foreach (CastCrewEdit2.CreditType creditType in CreditTypes)
                {
                    DataGridViewRow row;

                    row = CreditTypeDataGridView.Rows[CreditTypeDataGridView.Rows.Add()];
                    row.Cells[ColumnNames.IMDbCreditType].Value = creditType.IMDbCreditType;
                    row.Cells[ColumnNames.DVDProfilerCreditType].Value = creditType.DVDProfilerCreditType;
                }
            }
            else
            {
                CreditTypes = new List<CastCrewEdit2.CreditType>();
            }
            ResumeLayout();
            if (SkipVersionCheck == false)
            {
                OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, ""
                      , GetType().Assembly, true);
            }
            IsInit = false;
        }

        private void OnCreditSubtypeDataGridViewRowsRemoved(Object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NeedToSave = true;
        }

        private void OnDataGridViewCellBeginEdit(Object sender, DataGridViewCellCancelEventArgs e)
        {
            NeedToSave = true;
        }

        private void OnCreditTypeDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            if ((IsInit == false) && (CellChangeOnNewCreditTypeRow == false))
            {
                CastCrewEdit2.CreditType creditType;
                DataGridViewRow creditTypeRow;

                creditTypeRow = CreditTypeDataGridView.Rows[e.RowIndex];
                if ((CreditTypes.Count) - 1 < e.RowIndex)
                {
                    creditType = new CastCrewEdit2.CreditType();
                    if (creditTypeRow.Cells[ColumnNames.IMDbCreditType].Value != null)
                    {
                        creditType.IMDbCreditType = creditTypeRow.Cells[ColumnNames.IMDbCreditType].Value.ToString();
                    }
                    else
                    {
                        creditType.IMDbCreditType = String.Empty;
                    }
                    if (creditTypeRow.Cells[ColumnNames.DVDProfilerCreditType].Value == null)
                    {
                        if (NewCreditTypeRow)
                        {
                            CellChangeOnNewCreditTypeRow = true;
                        }
                        creditTypeRow.Cells[ColumnNames.DVDProfilerCreditType].Value = CreditTypesDataGridViewHelper.CreditTypes.Direction;
                        CellChangeOnNewCreditTypeRow = false;
                    }
                    creditType.DVDProfilerCreditType = creditTypeRow.Cells[ColumnNames.DVDProfilerCreditType].Value.ToString();
                    creditType.CreditSubtypeList = new CastCrewEdit2.CreditSubtype[0];
                    CreditTypes.Add(creditType);
                }
                if (e.ColumnIndex == 0)
                {
                    if (CreditTypeDataGridView.Rows[e.RowIndex].Cells[ColumnNames.IMDbCreditType].Value != null)
                    {
                        CreditTypes[e.RowIndex].IMDbCreditType = CreditTypeDataGridView.Rows[e.RowIndex]
                            .Cells[ColumnNames.IMDbCreditType].Value.ToString();
                    }
                    else
                    {
                        if (NewCreditTypeRow)
                        {
                            CellChangeOnNewCreditTypeRow = true;
                        }
                        CreditTypes[e.RowIndex].IMDbCreditType = String.Empty;
                        CellChangeOnNewCreditTypeRow = false;
                    }
                }
                if ((e.ColumnIndex == 1) && (NewCreditTypeRow == false))
                {
                    CreditTypes[e.RowIndex].DVDProfilerCreditType = CreditTypeDataGridView.Rows[e.RowIndex]
                        .Cells[ColumnNames.DVDProfilerCreditType].Value.ToString();
                    for (Int32 i = 0; i < CreditSubtypeDataGridView.Rows.Count - 1; i++)
                    {
                        DataGridViewRow creditSubtypeRow;

                        creditSubtypeRow = CreditSubtypeDataGridView.Rows[i];
                        creditSubtypeRow.Cells[ColumnNames.DVDProfilerCreditSubtype].Value = null;
                    }
                    SetDropDownValues(CreditTypes[e.RowIndex]);
                    for (Int32 i = 0; i < CreditSubtypeDataGridView.Rows.Count - 1; i++)
                    {
                        DataGridViewRow creditSubtypeRow;

                        creditSubtypeRow = CreditSubtypeDataGridView.Rows[i];
                        creditSubtypeRow.Cells[ColumnNames.DVDProfilerCreditSubtype].Value = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
                    }
                }
            }
        }

        private void OnCreditTypeDataGridViewDefaultValuesNeeded(Object sender, DataGridViewRowEventArgs e)
        {
            NewCreditTypeRow = true;
            e.Row.Cells[ColumnNames.IMDbCreditType].Value = String.Empty;
            e.Row.Cells[ColumnNames.DVDProfilerCreditType].Value = CreditTypesDataGridViewHelper.CreditTypes.Direction;
            NewCreditTypeRow = false;
        }

        private void OnCreditSubtypeDataGridViewDefaultValuesNeeded(Object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[ColumnNames.IMDbCreditSubtype].Value = String.Empty;
            if (PreviousRow != -1)
            {
                e.Row.Cells[ColumnNames.DVDProfilerCreditSubtype].Value = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
            }
            e.Row.Cells[ColumnNames.DVDProfilerCustomRole].Value = String.Empty;
        }

        private void OnCreditTypeDataGridViewSelectionChanged(Object sender, EventArgs e)
        {
            DataGridViewRow creditTypeRow;

            WriteCreditSubtypeData();
            creditTypeRow = null;
            if (CreditTypeDataGridView.SelectedRows.Count == 1)
            {
                creditTypeRow = CreditTypeDataGridView.SelectedRows[0];
            }
            if (CreditTypeDataGridView.SelectedCells.Count == 1)
            {
                creditTypeRow = CreditTypeDataGridView.Rows[CreditTypeDataGridView.SelectedCells[0].RowIndex];
            }
            if (creditTypeRow != null)
            {
                CastCrewEdit2.CreditType creditType;
                Boolean needToSave;

                creditType = CreditTypes[creditTypeRow.Index];
                needToSave = NeedToSave;
                CreditSubtypeDataGridView.Rows.Clear();
                NeedToSave = needToSave;
                SetDropDownValues(creditType);
                foreach (CastCrewEdit2.CreditSubtype creditSubtype in creditType.CreditSubtypeList)
                {
                    DataGridViewRow creditSubtypeRow;

                    creditSubtypeRow = CreditSubtypeDataGridView.Rows[CreditSubtypeDataGridView.Rows.Add()];
                    if (creditSubtype.IMDbCreditSubtype != null)
                    {
                        creditSubtypeRow.Cells[ColumnNames.IMDbCreditSubtype].Value = creditSubtype.IMDbCreditSubtype.Value;
                    }
                    else
                    {
                        creditSubtypeRow.Cells[ColumnNames.IMDbCreditSubtype].Value = String.Empty;
                    }
                    creditSubtypeRow.Cells[ColumnNames.DVDProfilerCreditSubtype].Value = creditSubtype.DVDProfilerCreditSubtype;
                    if (creditSubtype.DVDProfilerCustomRole != null)
                    {
                        creditSubtypeRow.Cells[ColumnNames.DVDProfilerCustomRole].Value = creditSubtype.DVDProfilerCustomRole;
                    }
                    else
                    {
                        creditSubtypeRow.Cells[ColumnNames.DVDProfilerCustomRole].Value = String.Empty;
                    }
                    if (creditSubtype.IMDbCreditSubtype != null)
                    {
                        creditSubtypeRow.Cells[ColumnNames.StartsWith].Value = creditSubtype.IMDbCreditSubtype.StartsWith;
                    }
                    else
                    {
                        creditSubtypeRow.Cells[ColumnNames.StartsWith].Value = false;
                    }
                }
                PreviousRow = creditTypeRow.Index;
            }
        }

        private void SetDropDownValues(CastCrewEdit2.CreditType creditType)
        {
            DataGridViewComboBoxColumn creditSubtypeColumn;

            creditSubtypeColumn =
                (DataGridViewComboBoxColumn)(CreditSubtypeDataGridView.Columns[ColumnNames.DVDProfilerCreditSubtype]);
            CreditTypesDataGridViewHelper.FillCreditSubtypes(creditType.DVDProfilerCreditType, creditSubtypeColumn.Items);
        }

        private void WriteCreditSubtypeData()
        {
            if ((PreviousRow != -1) && (PreviousRow < CreditTypes.Count))
            {
                CastCrewEdit2.CreditType creditType;

                creditType = CreditTypes[PreviousRow];
                creditType.CreditSubtypeList
                    = new CastCrewEdit2.CreditSubtype[CreditSubtypeDataGridView.RowCount - 1];
                for (Int32 i = 0; i < CreditSubtypeDataGridView.Rows.Count - 1; i++)
                {
                    DataGridViewRow creditSubtypeRow;

                    creditSubtypeRow = CreditSubtypeDataGridView.Rows[i];
                    creditType.CreditSubtypeList[i] = new CastCrewEdit2.CreditSubtype();
                    creditType.CreditSubtypeList[i].IMDbCreditSubtype = new IMDbCreditSubtype();
                    if (creditSubtypeRow.Cells[ColumnNames.IMDbCreditSubtype].Value != null)
                    {
                        creditType.CreditSubtypeList[i].IMDbCreditSubtype.Value
                            = creditSubtypeRow.Cells[ColumnNames.IMDbCreditSubtype].Value.ToString();
                    }
                    else
                    {
                        creditType.CreditSubtypeList[i].IMDbCreditSubtype.Value = String.Empty;
                    }
                    creditType.CreditSubtypeList[i].DVDProfilerCreditSubtype
                        = creditSubtypeRow.Cells[ColumnNames.DVDProfilerCreditSubtype].Value.ToString();
                    if (creditSubtypeRow.Cells[ColumnNames.DVDProfilerCustomRole].Value != null)
                    {
                        creditType.CreditSubtypeList[i].DVDProfilerCustomRole
                            = creditSubtypeRow.Cells[ColumnNames.DVDProfilerCustomRole].Value.ToString();
                    }
                    else
                    {
                        creditType.CreditSubtypeList[i].DVDProfilerCustomRole = String.Empty;
                    }
                    if (creditSubtypeRow.Cells[ColumnNames.StartsWith].Value != null)
                    {
                        Boolean startsWith;

                        startsWith = Boolean.Parse(creditSubtypeRow.Cells[ColumnNames.StartsWith].Value.ToString());
                        if (startsWith)
                        {
                            creditType.CreditSubtypeList[i].IMDbCreditSubtype.StartsWith = true;
                            creditType.CreditSubtypeList[i].IMDbCreditSubtype.StartsWithSpecified = true;
                        }
                        else
                        {
                            creditType.CreditSubtypeList[i].IMDbCreditSubtype.StartsWith = false;
                            creditType.CreditSubtypeList[i].IMDbCreditSubtype.StartsWithSpecified = false;
                        }
                    }
                }
            }
        }

        private void CreateColumns()
        {
            DataGridViewTextBoxColumn imdbCreditTypeDataGridViewTextBoxColumn;
            DataGridViewComboBoxColumn dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn;
            DataGridViewTextBoxColumn imdbCreditSubtypeDataGridViewTextBoxColumn;
            DataGridViewComboBoxColumn dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn;
            DataGridViewTextBoxColumn dvdProfilerCustomRoleDataGridViewTextBoxColumn;
            DataGridViewCheckBoxColumn startsWithDataGridViewCheckBoxColumn;

            imdbCreditTypeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            imdbCreditTypeDataGridViewTextBoxColumn.Name = ColumnNames.IMDbCreditType;
            imdbCreditTypeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            imdbCreditTypeDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            CreditTypeDataGridView.Columns.Add(imdbCreditTypeDataGridViewTextBoxColumn);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn = new DataGridViewComboBoxColumn();
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Name = ColumnNames.DVDProfilerCreditType;
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Resizable = DataGridViewTriState.True;
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Direction);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Writing);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Production);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Cinematography);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.FilmEditing);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Music);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Sound);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Art);
            dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Other);
            CreditTypeDataGridView.Columns.Add(dvdProfilerCreditTypeDataGridViewComboBoxBoxColumn);
            imdbCreditSubtypeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            imdbCreditSubtypeDataGridViewTextBoxColumn.Name = ColumnNames.IMDbCreditSubtype;
            imdbCreditSubtypeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            imdbCreditSubtypeDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            CreditSubtypeDataGridView.Columns.Add(imdbCreditSubtypeDataGridViewTextBoxColumn);
            dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn = new DataGridViewComboBoxColumn();
            dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn.Name = ColumnNames.DVDProfilerCreditSubtype;
            dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn.Resizable = DataGridViewTriState.True;
            CreditSubtypeDataGridView.Columns.Add(dvdProfilerCreditSubtypeDataGridViewComboBoxBoxColumn);
            dvdProfilerCustomRoleDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dvdProfilerCustomRoleDataGridViewTextBoxColumn.Name = ColumnNames.DVDProfilerCustomRole;
            dvdProfilerCustomRoleDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dvdProfilerCustomRoleDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            CreditSubtypeDataGridView.Columns.Add(dvdProfilerCustomRoleDataGridViewTextBoxColumn);
            startsWithDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
            startsWithDataGridViewCheckBoxColumn.Name = ColumnNames.StartsWith;
            startsWithDataGridViewCheckBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            startsWithDataGridViewCheckBoxColumn.Resizable = DataGridViewTriState.True;
            CreditSubtypeDataGridView.Columns.Add(startsWithDataGridViewCheckBoxColumn);
        }

        private void OnMainFormFormClosing(Object sender, FormClosingEventArgs e)
        {
            DialogResult result;

            WriteCreditSubtypeData();
            if (NeedToSave)
            {
                result
                    = MessageBox.Show(this, "Save Data?", "Save Data?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    for (Int32 i = CreditTypes.Count - 1; i >= 0; i--)
                    {
                        if ((CreditTypes[i].CreditSubtypeList == null)
                            || (CreditTypes[i].CreditSubtypeList.Length == 0))
                        {
                            CreditTypes.RemoveAt(i);
                        }
                    }
                    Program.TransformationData.CreditTypeList = CreditTypes.ToArray();
                    DialogResult = DialogResult.Yes;
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
            CreditTypeDataGridView.SelectionChanged -= OnCreditTypeDataGridViewSelectionChanged;
            CreditTypeDataGridView.DefaultValuesNeeded -= OnCreditTypeDataGridViewDefaultValuesNeeded;
            CreditSubtypeDataGridView.DefaultValuesNeeded -= OnCreditSubtypeDataGridViewDefaultValuesNeeded;
            CreditTypeDataGridView.CellValueChanged -= OnCreditTypeDataGridViewCellValueChanged;
            CreditTypeDataGridView.CellBeginEdit -= OnDataGridViewCellBeginEdit;
            CreditSubtypeDataGridView.CellBeginEdit -= OnDataGridViewCellBeginEdit;
            //CreditTypeDataGridView.RowsRemoved += OnCreditTypeDataGridViewRowsRemoved;
            CreditSubtypeDataGridView.RowsRemoved += OnCreditSubtypeDataGridViewRowsRemoved;
        }

        private void OnCreditTypeDataGridViewRowsRemoved(Object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NeedToSave = true;
            CreditTypes.RemoveAt(e.RowIndex);
            PreviousRow = -1;
        }
    }
}