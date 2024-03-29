﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.DVDProfiler.CheckForDuplicatesInCastCrewEdit2Cache.Properties;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CheckForDuplicatesInCastCrewEdit2Cache
{
    public partial class MainForm : Form
    {
        private readonly bool SkipVersionCheck;

        private PersonInfos m_Cache;

        private bool m_CacheHasChanged;

        public MainForm(bool skipVersionCheck)
        {
            SkipVersionCheck = skipVersionCheck;
            InitializeComponent();
            Icon = Properties.Resource.djdsoft;
            if ((string.IsNullOrEmpty(Settings.Default.CacheFile) == false) && (File.Exists(Settings.Default.CacheFile)))
            {
                CacheFileTextBox.Text = Settings.Default.CacheFile;
            }
            m_CacheHasChanged = false;
        }

        private void OnSelectCacheFileButtonClick(object sender, EventArgs e)
        {
            if (CheckCacheChange())
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.CheckFileExists = true;
                    ofd.Filter = "Cast|Cast.xml|Crew|Crew.xml";
                    ofd.Multiselect = false;
                    ofd.Title = "Select Cast/Crew Edit 2 Cache File";
                    ofd.RestoreDirectory = true;
                    if (Directory.Exists(Environment.CurrentDirectory + @"\Data"))
                    {
                        ofd.InitialDirectory = Environment.CurrentDirectory + @"\Data";
                    }
                    if (string.IsNullOrEmpty(Settings.Default.CacheFile) == false)
                    {
                        FileInfo fi;

                        fi = new FileInfo(Settings.Default.CacheFile);
                        ofd.InitialDirectory = fi.Directory.FullName;
                    }
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Settings.Default.CacheFile = ofd.FileName;
                        Settings.Default.Save();
                        CacheFileTextBox.Text = ofd.FileName;
                    }
                }
            }
        }

        private void OnStartButtonClick(object sender, EventArgs e)
        {
            if (CheckCacheChange())
            {
                if (string.IsNullOrEmpty(CacheFileTextBox.Text) == false)
                {
                    DifferentInParsingDataGridView.Rows.Clear();
                    DifferentBirthYearsDataGridView.Rows.Clear();
                    DifferentBirthYearsAllDataGridView.Rows.Clear();
                    EverythingIdenticalgDataGridView.Rows.Clear();
                    try
                    {
                        Dictionary<string, List<PersonInfo>> cacheHash;
                        Dictionary<string, List<PersonInfo>> cacheHash2;

                        m_Cache = PersonInfos.Deserialize(CacheFileTextBox.Text);
                        m_CacheHasChanged = false;
                        cacheHash = new Dictionary<string, List<PersonInfo>>(m_Cache.PersonInfoList.Length);
                        cacheHash2 = new Dictionary<string, List<PersonInfo>>(m_Cache.PersonInfoList.Length);
                        if ((m_Cache != null) && (m_Cache.PersonInfoList != null) && (m_Cache.PersonInfoList.Length > 0))
                        {
                            foreach (var person in m_Cache.PersonInfoList)
                            {
                                string key;

                                key = person.FirstName + person.MiddleName + person.LastName + person.BirthYear;
                                key = key.ToLower();
                                if (cacheHash.ContainsKey(key) == false)
                                {
                                    cacheHash.Add(key, new List<PersonInfo>(1));
                                }
                                cacheHash[key].Add(person);
                                key = person.FirstName + person.MiddleName + person.LastName;
                                key = key.ToLower();
                                if (cacheHash2.ContainsKey(key) == false)
                                {
                                    cacheHash2.Add(key, new List<PersonInfo>(1));
                                }
                                cacheHash2[key].Add(person);
                            }
                        }
                        CheckParsingDifferences(cacheHash);
                        CheckBirthYearDifferences(cacheHash2);
                        MessageBox.Show("Done.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CheckBirthYearDifferences(Dictionary<string, List<PersonInfo>> cacheHash)
        {
            bool oddRow1;
            bool oddRow2;
            List<List<PersonInfo>> list;

            oddRow1 = true;
            oddRow2 = true;
            list = new List<List<PersonInfo>>(cacheHash.Values);
            list.Sort(new Comparison<List<PersonInfo>>(SortList));
            foreach (var personList in list)
            {
                if (personList.Count > 1)
                {
                    bool allHaveBirthYear;
                    bool noneHaveBirthYear;

                    allHaveBirthYear = true;
                    noneHaveBirthYear = true;
                    foreach (var person in personList)
                    {
                        if (string.IsNullOrEmpty(person.BirthYear))
                        {
                            allHaveBirthYear = false;
                        }
                        else
                        {
                            noneHaveBirthYear = false;
                        }
                    }
                    if (allHaveBirthYear)
                    {
                        FillRows(personList, oddRow1, DifferentBirthYearsAllDataGridView, false);
                        oddRow1 = oddRow1 == false;
                    }
                    else if (noneHaveBirthYear == false)
                    {
                        FillRows(personList, oddRow2, DifferentBirthYearsDataGridView, false);
                        oddRow2 = oddRow2 == false;
                    }
                }
            }
        }

        private static int SortList(List<PersonInfo> left, List<PersonInfo> right)
        {
            PersonInfo firstLeftPerson;
            PersonInfo firstRightPerson;
            int compare;

            firstLeftPerson = left[0];
            firstRightPerson = right[0];
            compare = CompareString(firstLeftPerson.LastName, firstRightPerson.LastName);
            if (compare == 0)
            {
                compare = CompareString(firstLeftPerson.FirstName, firstRightPerson.FirstName);
                if (compare == 0)
                {
                    compare = CompareString(firstLeftPerson.MiddleName, firstRightPerson.MiddleName);
                    if (compare == 0)
                    {
                        compare = CompareString(firstLeftPerson.BirthYear, firstRightPerson.BirthYear);
                    }
                }
            }
            return (compare);
        }

        private static int CompareString(string left, string right)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return (0);
                }
                else
                {
                    return (-1);
                }
            }
            return (left.CompareTo(right));
        }

        private static void FillRows(List<PersonInfo> personList, bool oddRow, DataGridView dataGridView, bool isEverythingGrid)
        {
            foreach (var person in personList)
            {
                DataGridViewRow row;

                row = dataGridView.Rows[dataGridView.Rows.Add()];
                if (oddRow == false)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCyan;
                }
                row.Cells["FirstName"].Value = person.FirstName;
                row.Cells["MiddleName"].Value = person.MiddleName;
                row.Cells["LastName"].Value = person.LastName;
                row.Cells["BirthYear"].Value = person.BirthYear;
                row.Cells["FakeBirthYear"].Value = person.FakeBirthYear;
                row.Cells["IMDbLink"].Value = person.PersonLink;
                if (isEverythingGrid)
                {
                    row.Cells["RemoveEntry"].Value = "Remove";
                }
            }
        }

        private void CheckParsingDifferences(Dictionary<string, List<PersonInfo>> cacheHash)
        {
            bool oddRow1;
            bool oddRow2;
            List<List<PersonInfo>> list;

            oddRow1 = true;
            oddRow2 = true;
            list = new List<List<PersonInfo>>(cacheHash.Values);
            list.Sort(new Comparison<List<PersonInfo>>(SortList));
            foreach (var personList in list)
            {
                if (personList.Count > 1)
                {
                    bool parsingDifference;
                    string firstName;
                    string middleName;
                    string lastName;

                    firstName = personList[0].FirstName;
                    middleName = personList[0].MiddleName;
                    lastName = personList[0].LastName;
                    parsingDifference = false;
                    for (var i = 1; i < personList.Count; i++)
                    {
                        if ((personList[i].FirstName != firstName)
                            || (personList[i].MiddleName != middleName)
                            || (personList[i].LastName != lastName))
                        {
                            parsingDifference = true;
                            break;
                        }
                    }
                    if (parsingDifference)
                    {
                        FillRows(personList, oddRow1, DifferentInParsingDataGridView, false);
                        oddRow1 = oddRow1 == false;
                    }
                    else
                    {
                        FillRows(personList, oddRow2, EverythingIdenticalgDataGridView, true);
                        oddRow2 = oddRow2 == false;
                    }
                }
            }
        }

        private void OnMainFormLoad(object sender, EventArgs e)
        {
            DataGridViewButtonColumn removeColumn;

            CheckForNewVersion();
            CreateColumns(DifferentInParsingDataGridView);
            CreateColumns(DifferentBirthYearsDataGridView);
            CreateColumns(DifferentBirthYearsAllDataGridView);
            CreateColumns(EverythingIdenticalgDataGridView);
            removeColumn = new DataGridViewButtonColumn();
            removeColumn.HeaderText = "Remove Entry from Cache";
            removeColumn.Name = "RemoveEntry";
            removeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            EverythingIdenticalgDataGridView.Columns.Add(removeColumn);
        }

        private static void CreateColumns(DataGridView dataGridView)
        {
            DataGridViewTextBoxColumn firstName;
            DataGridViewTextBoxColumn middleName;
            DataGridViewTextBoxColumn lastName;
            DataGridViewTextBoxColumn birthYear;
            DataGridViewTextBoxColumn fakeBirthYear;
            DataGridViewLinkColumn imdbLink;

            firstName = new DataGridViewTextBoxColumn();
            middleName = new DataGridViewTextBoxColumn();
            lastName = new DataGridViewTextBoxColumn();
            birthYear = new DataGridViewTextBoxColumn();
            fakeBirthYear = new DataGridViewTextBoxColumn();
            imdbLink = new DataGridViewLinkColumn();
            firstName.HeaderText = "First Name";
            firstName.Name = "FirstName";
            firstName.ReadOnly = true;
            firstName.SortMode = DataGridViewColumnSortMode.NotSortable;
            middleName.HeaderText = "Middle Name";
            middleName.Name = "MiddleName";
            middleName.ReadOnly = true;
            middleName.SortMode = DataGridViewColumnSortMode.NotSortable;
            lastName.HeaderText = "Last Name";
            lastName.Name = "LastName";
            lastName.ReadOnly = true;
            lastName.SortMode = DataGridViewColumnSortMode.NotSortable;
            birthYear.HeaderText = "Birth Year";
            birthYear.Name = "BirthYear";
            birthYear.ReadOnly = true;
            birthYear.SortMode = DataGridViewColumnSortMode.NotSortable;
            fakeBirthYear.HeaderText = "Fake Birth Year";
            fakeBirthYear.Name = "FakeBirthYear";
            fakeBirthYear.ReadOnly = true;
            fakeBirthYear.SortMode = DataGridViewColumnSortMode.NotSortable;
            imdbLink.HeaderText = "IMDb Link";
            imdbLink.Name = "IMDbLink";
            imdbLink.ReadOnly = true;
            imdbLink.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { firstName, middleName, lastName, birthYear, fakeBirthYear, imdbLink });
        }

        private void CheckForNewVersion()
        {
            if (SkipVersionCheck == false)
            {
                OnlineAccess.Init("Doena Soft.", "CheckForDuplicatesInCastCrewEdit2Cache");
                OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CheckForDuplicatesInCastCrewEdit2Cache"
                    , GetType().Assembly, true);
            }
        }

        private void OnDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView;

            dataGridView = (DataGridView)sender;
            if ((e.ColumnIndex == 5) && (e.RowIndex != -1))
            {
                Process.Start("http://www.imdb.com/name/" + dataGridView.Rows[e.RowIndex].Cells["IMDbLink"].Value.ToString());
            }
            else if ((e.ColumnIndex == 6) && (e.RowIndex != -1))
            {
                List<PersonInfo> personInfos;
                string imdbLink;

                imdbLink = dataGridView.Rows[e.RowIndex].Cells["IMDbLink"].Value.ToString();
                personInfos = new List<PersonInfo>(m_Cache.PersonInfoList);
                for (var i = 0; i < personInfos.Count; i++)
                {
                    if (personInfos[i].PersonLink == imdbLink)
                    {
                        if (MessageBox.Show(string.Format("Permanently remove {0} ({1}) from cache?", personInfos[i].ToString(), imdbLink)
                            , "Remove?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (string.IsNullOrEmpty(personInfos[i].FakeBirthYear) == false)
                            {
                                MessageBox.Show(string.Format("Warning: {0} has the fake birth year {1} assigned, which means he is probably in your DVD Profiler database!"
                                    , personInfos[i].ToString(), personInfos[i].FakeBirthYear), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            personInfos.RemoveAt(i);
                            m_Cache.PersonInfoList = personInfos.ToArray();
                            m_CacheHasChanged = true;
                            dataGridView.Rows.RemoveAt(e.RowIndex);
                        }
                    }
                }
            }
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckCacheChange() == false)
            {
                e.Cancel = true;
            }
        }

        private bool CheckCacheChange()
        {
            if (m_CacheHasChanged)
            {
                DialogResult result;

                result = MessageBox.Show("Save changed cache file?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    return (false);
                }
                else if (result == DialogResult.Yes)
                {
                    UseWaitCursor = true;
                    Cursor = Cursors.WaitCursor;
                    m_Cache.Serialize(CacheFileTextBox.Text);
                    Cursor = Cursors.Default;
                    UseWaitCursor = false;
                    m_CacheHasChanged = false;
                }
                else
                {
                    m_CacheHasChanged = false;
                }
            }
            return (true);
        }
    }
}