using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    internal partial class CacheForm : Form
    {
        private List<PersonInfo> Persons;

        public CacheForm(List<PersonInfo> persons, String cacheName)
        {
            Persons = persons;
            InitializeComponent();
            Text = cacheName;
        }

        private void OnFormLoad(Object sender, EventArgs e)
        {
            DataGridViewLinkColumn LinkColumn;
            DataGridViewTextBoxColumn LastNameColumn;
            DataGridViewTextBoxColumn BirthYearColumn;
            DataGridViewTextBoxColumn FakeBirthYearColumn;
            DataGridViewTextBoxColumn FirstNameColumn;
            DataGridViewTextBoxColumn MiddleNameColumn;
            DataGridViewTextBoxColumn PersonTypeColumn;

            UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            LastNameColumn = new DataGridViewTextBoxColumn();
            FirstNameColumn = new DataGridViewTextBoxColumn();
            MiddleNameColumn = new DataGridViewTextBoxColumn();
            BirthYearColumn = new DataGridViewTextBoxColumn();
            FakeBirthYearColumn = new DataGridViewTextBoxColumn();
            LinkColumn = new DataGridViewLinkColumn();
            PersonTypeColumn = new DataGridViewTextBoxColumn();
            LastNameColumn.HeaderText = "Last Name";
            LastNameColumn.Name = "LastName";
            LastNameColumn.ReadOnly = true;
            FirstNameColumn.HeaderText = "First Name";
            FirstNameColumn.Name = "FirstName";
            FirstNameColumn.ReadOnly = true;
            MiddleNameColumn.HeaderText = "Middle Name";
            MiddleNameColumn.Name = "MiddleName";
            MiddleNameColumn.ReadOnly = true;
            BirthYearColumn.HeaderText = "Birth Year";
            BirthYearColumn.Name = "BirthYear";
            BirthYearColumn.ReadOnly = true;
            FakeBirthYearColumn.HeaderText = "Fake Birth Year";
            FakeBirthYearColumn.Name = "FakeBirthYear";
            FakeBirthYearColumn.ReadOnly = true;
            LinkColumn.HeaderText = "Link";
            LinkColumn.Name = "Link";
            LinkColumn.ReadOnly = true;
            LinkColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            LinkColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            PersonTypeColumn.HeaderText = "Type";
            PersonTypeColumn.Name = "Type";
            PersonTypeColumn.ReadOnly = true;
            DataGridView.Columns.AddRange(new DataGridViewColumn[] {FirstNameColumn, MiddleNameColumn
                , LastNameColumn, BirthYearColumn, FakeBirthYearColumn, LinkColumn, PersonTypeColumn });
            foreach (PersonInfo person in Persons)
            {
                DataGridViewRow row;

                row = DataGridView.Rows[DataGridView.Rows.Add()];
                row.Cells["Link"].Value = person.PersonLink;
                row.Cells["LastName"].Value = person.LastName;
                row.Cells["MiddleName"].Value = person.MiddleName;
                row.Cells["FirstName"].Value = person.FirstName;
                if (person.BirthYearWasRetrieved)
                {
                    row.Cells["BirthYear"].Value = person.BirthYear;
                }
                else
                {
                    row.Cells["BirthYear"].Value = DataGridViewTexts.NotRetrievedYet;
                }
                row.Cells["FakeBirthYear"].Value = person.FakeBirthYear;
                row.Cells["Type"].Value = person.Type;
            }
            DataGridView.CellContentClick += OnDataGridViewCellContentClick;
            Cursor = Cursors.Default;
            UseWaitCursor = false;
        }

        void OnDataGridViewCellContentClick(Object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                Process.Start(IMDbParser.PersonUrl + DataGridView.Rows[e.RowIndex].Cells["Link"].Value.ToString());
            }
        }

        private void OnCloseButtonClick(Object sender, EventArgs e)
        {
            Close();
        }
    }
}