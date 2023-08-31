using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.DVDProfiler.CompareProfilerXMLAndCastCrewEdit2Cache.Properties;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.DVDProfilerXML;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.CompareProfilerXMLAndCastCrewEdit2Cache
{
    internal partial class MainForm : Form
    {
        private readonly bool SkipVersionCheck;

        public MainForm(bool skipVersionCheck)
        {
            SkipVersionCheck = skipVersionCheck;
            InitializeComponent();
            Icon = Properties.Resource.djdsoft;
            if ((string.IsNullOrEmpty(Settings.Default.CacheFile) == false) && (File.Exists(Settings.Default.CacheFile)))
            {
                CacheFileTextBox.Text = Settings.Default.CacheFile;
            }
            if ((string.IsNullOrEmpty(Settings.Default.DVDProfilerXMLFile) == false)
                && (File.Exists(Settings.Default.DVDProfilerXMLFile)))
            {
                DVDProfilerXMLTextBox.Text = Settings.Default.DVDProfilerXMLFile;
            }
        }

        private void CheckForNewVersion()
        {
            if (SkipVersionCheck == false)
            {
                OnlineAccess.Init("Doena Soft.", "CompareProfilerXMLAndCastCrewEdit2Cache");
                OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CompareProfilerXMLAndCastCrewEdit2Cache"
                    , GetType().Assembly, true);
            }
        }

        private void OnStartButtonClick(Object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(CacheFileTextBox.Text) == false)
                && (string.IsNullOrEmpty(DVDProfilerXMLTextBox.Text) == false))
            {
                CacheDataGridView.Rows.Clear();
                CollectionDataGridView.Rows.Clear();
                try
                {
                    PersonInfos cache;
                    Dictionary<string, List<PersonInfo>> cacheHash;
                    Collection collection;
                    Dictionary<string, IPerson> collectionHash;
                    DataGridViewRow row;
                    List<DataGridViewRow> rowList;

                    collection = Serializer<Collection>.Deserialize(DVDProfilerXMLTextBox.Text);
                    cache = PersonInfos.Deserialize(CacheFileTextBox.Text);
                    collectionHash = new Dictionary<string, IPerson>(cache.PersonInfoList.Length);
                    if ((collection != null) && (collection.DVDList != null) && (collection.DVDList.Length > 0))
                    {
                        foreach (var dvd in collection.DVDList)
                        {
                            if (Settings.Default.CacheFileIsCast)
                            {
                                if ((dvd.CastList != null) && (dvd.CastList.Length > 0))
                                {
                                    foreach (var castEntry in dvd.CastList)
                                    {
                                        CastMember cast;

                                        cast = castEntry as CastMember;
                                        if (cast != null)
                                        {
                                            string key;

                                            if (cast.BirthYear == 0)
                                            {
                                                key = cast.LastName + "_" + cast.FirstName + "_" + cast.MiddleName + "_";
                                            }
                                            else
                                            {
                                                key = cast.LastName + "_" + cast.FirstName + "_" + cast.MiddleName + "_" + cast.BirthYear;
                                            }
                                            if (collectionHash.ContainsKey(key.ToLower()) == false)
                                            {
                                                collectionHash.Add(key.ToLower(), cast);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((dvd.CrewList != null) && (dvd.CrewList.Length > 0))
                                {
                                    foreach (var crewEntry in dvd.CrewList)
                                    {
                                        CrewMember crew;

                                        crew = crewEntry as CrewMember;
                                        if (crew != null)
                                        {
                                            string key;

                                            if (crew.BirthYear == 0)
                                            {
                                                key = crew.LastName + "_" + crew.FirstName + "_" + crew.MiddleName + "_";
                                            }
                                            else
                                            {
                                                key = crew.LastName + "_" + crew.FirstName + "_" + crew.MiddleName + "_" + crew.BirthYear;
                                            }
                                            if (collectionHash.ContainsKey(key.ToLower()) == false)
                                            {
                                                collectionHash.Add(key.ToLower(), crew);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    cacheHash = new Dictionary<string, List<PersonInfo>>(cache.PersonInfoList.Length);
                    if ((cache != null) && (cache.PersonInfoList != null) && (cache.PersonInfoList.Length > 0))
                    {
                        foreach (var person in cache.PersonInfoList)
                        {
                            string key;

                            if (string.IsNullOrEmpty(person.BirthYear) == false)
                            {
                                key = person.LastName + "_" + person.FirstName + "_" + person.MiddleName + "_" + int.Parse(person.BirthYear);
                            }
                            else if ((string.IsNullOrEmpty(person.FakeBirthYear) == false) && (person.FakeBirthYear != "0"))
                            {
                                key = person.LastName + "_" + person.FirstName + "_" + person.MiddleName + "_" + int.Parse(person.FakeBirthYear);
                            }
                            else
                            {
                                key = person.LastName + "_" + person.FirstName + "_" + person.MiddleName + "_";
                            }
                            key = key.ToLower();
                            if (cacheHash.ContainsKey(key) == false)
                            {
                                List<PersonInfo> list;

                                list = new List<PersonInfo>(1);
                                list.Add(person);
                                cacheHash.Add(key, list);
                            }
                            else
                            {
                                cacheHash[key].Add(person);
                            }
                        }
                    }
                    rowList = new List<DataGridViewRow>();
                    foreach (var keyValue in collectionHash)
                    {
                        if (cacheHash.ContainsKey(keyValue.Key) == false)
                        {
                            DataGridViewCell cell;

                            row = new DataGridViewRow();
                            cell = new DataGridViewTextBoxCell();
                            row.Cells.Add(cell);
                            cell = new DataGridViewTextBoxCell();
                            row.Cells.Add(cell);
                            cell = new DataGridViewTextBoxCell();
                            row.Cells.Add(cell);
                            cell = new DataGridViewTextBoxCell();
                            row.Cells.Add(cell);
                            row.Cells[0].Value = keyValue.Value.FirstName;
                            row.Cells[1].Value = keyValue.Value.MiddleName;
                            row.Cells[2].Value = keyValue.Value.LastName;
                            if (keyValue.Value.BirthYear != 0)
                            {
                                row.Cells[3].Value = keyValue.Value.BirthYear;
                            }
                            row.Tag = keyValue.Key;
                            rowList.Add(row);
                        }
                    }
                    rowList.Sort(new Comparison<DataGridViewRow>(delegate (DataGridViewRow left, DataGridViewRow right)
                            {
                                return (left.Tag.ToString().CompareTo(right.Tag.ToString()));
                            }
                        ));
                    CacheDataGridView.Rows.AddRange(rowList.ToArray());
                    rowList = new List<DataGridViewRow>();
                    foreach (var keyValue in cacheHash)
                    {
                        if (collectionHash.ContainsKey(keyValue.Key) == false)
                        {
                            foreach (var person in keyValue.Value)
                            {
                                DataGridViewCell cell;

                                row = new DataGridViewRow();
                                cell = new DataGridViewTextBoxCell();
                                row.Cells.Add(cell);
                                cell = new DataGridViewTextBoxCell();
                                row.Cells.Add(cell);
                                cell = new DataGridViewTextBoxCell();
                                row.Cells.Add(cell);
                                cell = new DataGridViewTextBoxCell();
                                row.Cells.Add(cell);
                                cell = new DataGridViewLinkCell();
                                row.Cells.Add(cell);
                                //row = CollectionDataGridView.Rows[CollectionDataGridView.Rows.Add()];
                                row.Cells[0].Value = person.FirstName;
                                row.Cells[1].Value = person.MiddleName;
                                row.Cells[2].Value = person.LastName;
                                if (string.IsNullOrEmpty(person.BirthYear) == false)
                                {
                                    row.Cells[3].Value = person.BirthYear;
                                }
                                else
                                {
                                    row.Cells[3].Value = person.FakeBirthYear;
                                }
                                row.Cells[4].Value = person.PersonLink;
                                row.Tag = keyValue.Key;
                                rowList.Add(row);
                            }
                        }
                    }
                    rowList.Sort(new Comparison<DataGridViewRow>(delegate (DataGridViewRow left, DataGridViewRow right)
                                {
                                    return (left.Tag.ToString().CompareTo(right.Tag.ToString()));
                                }
                        ));
                    CollectionDataGridView.Rows.AddRange(rowList.ToArray());
                    MessageBox.Show("Done.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnSelectCacheFileButtonClick(Object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Filter = "Cast|Cast.xml|Crew|Crew.xml";
                ofd.Multiselect = false;
                ofd.Title = "Select Cast/Crew Edit 2 Cache File";
                ofd.RestoreDirectory = true;
                if (string.IsNullOrEmpty(Settings.Default.CacheFile) == false)
                {
                    FileInfo fi;

                    fi = new FileInfo(Settings.Default.CacheFile);
                    ofd.InitialDirectory = fi.Directory.FullName;
                }
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Settings.Default.CacheFile = ofd.FileName;
                    CacheFileTextBox.Text = ofd.FileName;
                    if (MessageBox.Show("Is this cache file cast or crew (cast = yes, crew = no)?", "Cast or Crew?", MessageBoxButtons.YesNo
                        , MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Settings.Default.CacheFileIsCast = true;
                    }
                    else
                    {
                        Settings.Default.CacheFileIsCast = false;
                    }
                    Settings.Default.Save();
                }
            }
        }

        private void OnSelectDVDProfilerXMLButtonClick(Object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Filter = "Collection.xml|*.xml";
                ofd.Multiselect = false;
                ofd.Title = "Select DVD Profiler XML File";
                ofd.RestoreDirectory = true;
                if (string.IsNullOrEmpty(Settings.Default.DVDProfilerXMLFile) == false)
                {
                    FileInfo fi;

                    fi = new FileInfo(Settings.Default.DVDProfilerXMLFile);
                    ofd.InitialDirectory = fi.Directory.FullName;
                }
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Settings.Default.DVDProfilerXMLFile = ofd.FileName;
                    Settings.Default.Save();
                    DVDProfilerXMLTextBox.Text = ofd.FileName;
                }
            }
        }

        private void OnMainFormLoad(Object sender, EventArgs e)
        {
            CheckForNewVersion();
        }

        private void OnCollectionDataGridViewCellContentClick(Object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 4) && (e.RowIndex != -1))
            {
                Process.Start("http://www.imdb.com/name/" + CollectionDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString());
            }
        }
    }
}