namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Helper;
    using Resources;

    internal partial class CacheForm : Form
    {
        private readonly List<PersonInfo> _persons;

        public CacheForm(List<PersonInfo> persons, string cacheName)
        {
            _persons = persons;

            this.InitializeComponent();

            this.Text = cacheName;
            this.Icon = Properties.Resource.djdsoft;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            this.Cursor = Cursors.WaitCursor;

            var lastNameColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Last Name",
                Name = "LastName",
                ReadOnly = true,
            };

            var firstNameColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "First Name",
                Name = "FirstName",
                ReadOnly = true,
            };

            var middleNameColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Middle Name",
                Name = "MiddleName",
                ReadOnly = true,
            };

            var birthYearColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Birth Year",
                Name = "BirthYear",
                ReadOnly = true,
            };

            var fakeBirthYearColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Fake Birth Year",
                Name = "FakeBirthYear",
                ReadOnly = true,
            };

            var linkColumn = new DataGridViewLinkColumn()
            {
                HeaderText = "Link",
                Name = "Link",
                ReadOnly = true,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };

            var personTypeColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Type",
                Name = "Type",
                ReadOnly = true,
            };

            DataGridView.Columns.AddRange(firstNameColumn, middleNameColumn, lastNameColumn, birthYearColumn, fakeBirthYearColumn, linkColumn, personTypeColumn);

            foreach (var person in _persons)
            {
                var row = DataGridView.Rows[DataGridView.Rows.Add()];

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

            DataGridView.CellContentClick += this.OnDataGridViewCellContentClick;

            this.Cursor = Cursors.Default;
            this.UseWaitCursor = false;
        }

        private void OnDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                Process.Start(IMDbParser.PersonUrl + DataGridView.Rows[e.RowIndex].Cells["Link"].Value.ToString());
            }
        }

        private void OnCloseButtonClick(object sender, EventArgs e) => this.Close();
    }
}