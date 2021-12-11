namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Helper;
    using Resources;

    [ComVisible(true)]
    public class CastCrewEdit2ParseBaseForm : CastCrewEdit2BaseForm
    {
        protected static readonly Log _log;

        private static readonly List<MessageEntry> _messageQueue;

        private readonly static object _messageQueueLock;

        internal static bool FirstRunGetHeadShots { get; set; }

        static CastCrewEdit2ParseBaseForm()
        {
            _messageQueueLock = new object();

            _log = new Log();

            _messageQueue = new List<MessageEntry>();

            FirstRunGetHeadShots = true;
        }

        protected static bool ItsMe => Environment.UserName == "djdoe";

        protected static bool HasAgreed
        {
            get => ItsMe || _hasAgreed;
            set => _hasAgreed = value;
        }

        protected static void AddMessage(MessageEntry entry)
        {
            lock (_messageQueueLock)
            {
                _messageQueue.Add(entry);
            }
        }

        protected void OnBirthYearsInLocalCacheLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var persons = new PersonInfo[IMDbParser.PersonHash.Keys.Count];

            IMDbParser.PersonHash.Keys.CopyTo(persons, 0);

            this.ShowCache(new List<PersonInfo>(persons), "Local Birth Year Cache");
        }

        protected void OnPersonsInLocalCacheLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var persons = new PersonInfo[Program.PersonCacheCount];

            Program.CastCache.Values.CopyTo(persons, 0);
            Program.CrewCache.Values.CopyTo(persons, Program.CastCache.Values.Count);

            this.ShowCache(new List<PersonInfo>(persons), "Local Person Cache");
        }

        private void ShowCache(List<PersonInfo> persons, string cacheName)
        {
            persons.Sort(ComparePersonInfos);

            this.Cursor = Cursors.Default;

            using (var form = new CacheForm(persons, cacheName))
            {
                form.ShowDialog(this);
            }
        }

        private static int ComparePersonInfos(PersonInfo left, PersonInfo right)
        {
            var compare = left.LastName.CompareTo(right.LastName);

            if (compare != 0)
            {
                return compare;
            }

            compare = left.FirstName.CompareTo(right.FirstName);

            if (compare != 0)
            {
                return compare;
            }

            compare = left.MiddleName.CompareTo(right.MiddleName);

            if (compare != 0)
            {
                return compare;
            }

            if (left.BirthYearWasRetrieved && !string.IsNullOrEmpty(left.BirthYear))
            {
                compare = left.BirthYear.CompareTo(right.BirthYear);
            }
            else
            {
                compare = string.Empty.CompareTo(right.BirthYear);
            }

            if (compare != 0)
            {
                return compare;
            }

            if (!string.IsNullOrEmpty(left.FakeBirthYear))
            {
                compare = left.FakeBirthYear.CompareTo(right.FakeBirthYear);
            }
            else
            {
                compare = string.Empty.CompareTo(right.FakeBirthYear);
            }

            return compare;
        }

        protected void OnDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;

            if (e.ColumnIndex == 8)
            {
                var row = grid.Rows[e.RowIndex];

                if (row.Cells[ColumnNames.FirstName].Value.ToString() == FirstNames.Title
                    || row.Cells[ColumnNames.FirstName].Value.ToString() == FirstNames.Divider)
                {
                    Process.Start(IMDbParser.TitleUrl + row.Cells[ColumnNames.Link].Value.ToString());
                }
                else
                {
                    Process.Start(IMDbParser.PersonUrl + row.Cells[ColumnNames.Link].Value.ToString());
                }
            }
            else if (grid.Columns[e.ColumnIndex].Name == ColumnNames.MoveUp)
            {
                var row = grid.Rows[e.RowIndex];

                var cell = (DataGridViewHelper.DataGridViewDisableButtonCell)row.Cells[ColumnNames.MoveUp];

                if (cell.Enabled)
                {
                    this.MoveRow((CastInfo)row.Tag, true);
                }
            }
            else if (grid.Columns[e.ColumnIndex].Name == ColumnNames.MoveDown)
            {
                var row = grid.Rows[e.RowIndex];

                var cell = (DataGridViewHelper.DataGridViewDisableButtonCell)row.Cells[ColumnNames.MoveDown];

                if (cell.Enabled)
                {
                    this.MoveRow((CastInfo)row.Tag, false);
                }
            }
            else if (grid.Columns[e.ColumnIndex].Name == ColumnNames.RemoveRow)
            {
                var row = grid.Rows[e.RowIndex];

                var cell = (DataGridViewHelper.DataGridViewDisableButtonCell)row.Cells[ColumnNames.RemoveRow];

                if (cell.Enabled)
                {
                    this.RemoveRow((CastInfo)(row.Tag));
                }
            }
        }

        protected virtual void MoveRow(CastInfo castMember, bool up)
        {
            //abstract method but class can't be actract ore else the Form Designer will fail
        }

        protected virtual void RemoveRow(CastInfo castMember)
        {
            //abstract method but class can't be actract ore else the Form Designer will fail
        }

        protected static int FindIndexOfCastMember(List<CastInfo> castList, CastInfo castMember)
        {
            var indexOf = -1;

            for (var i = 0; i < castList.Count; i++)
            {
                if (castList[i].Identifier == castMember.Identifier)
                {
                    indexOf = i;

                    break;
                }
            }

            return indexOf;
        }

        protected void UpdateUI(List<CastInfo> castList, List<CrewInfo> crewList, DataGridView movieCastDataGridView, DataGridView movieCrewDataGridView, bool parseCast, bool parseCrew, string filmLink, string filmName)
        {
            #region Update Cast UI

            if (parseCast)
            {
                DataGridViewHelper.FillCastRows(movieCastDataGridView, castList, false, false);

                foreach (var castMember in castList)
                {
                    if (castMember.FirstName == FirstNames.Title)
                    {
                        continue;
                    }

                    if (Program.CastCache.ContainsKey(castMember.PersonLink))
                    {
                        var other = Program.CastCache[castMember.PersonLink];

                        other.AddFilmInfo(filmLink, filmName);

                        castMember.FilmInfoList = other.FilmInfoList;

                        if (castMember.FirstName != other.FirstName
                            || castMember.MiddleName != other.MiddleName
                            || castMember.LastName != other.LastName)
                        {
                            var piwby = new PersonInfoWithoutBirthYear(other);

                            var messageText = string.Format(MessageBoxTexts.CommonCastNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkers()
                                , castMember.FormatPersonNameWithMarkers(), castMember.PersonLink);

                            var logText = string.Format(MessageBoxTexts.CommonCastNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkersAsHtml(new[] { castMember })
                                , castMember.FormatPersonNameWithMarkersAsHtml(new[] { other }), DataGridViewHelper.CreatePersonLinkHtml(castMember));

                            _log.AppendParagraph(logText);

                            AddMessage(new MessageEntry(messageText, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));

                            ReorganizePossibleDuplicatesCache(castMember, piwby);

                            other.FirstName = castMember.FirstName;
                            other.MiddleName = castMember.MiddleName;
                            other.LastName = castMember.LastName;
                        }
                    }
                    else
                    {
                        castMember.AddFilmInfo(filmLink, filmName);

                        Program.CastCache.Add(castMember.PersonLink, new PersonInfo(castMember));
                    }

                    AddPossibleDuplicate(castMember);
                }
            }

            #endregion

            #region Update Crew UI

            if (parseCrew)
            {
                DataGridViewHelper.FillCrewRows(movieCrewDataGridView, crewList);

                foreach (var crewMember in crewList)
                {
                    if (crewMember.FirstName == FirstNames.Title)
                    {
                        continue;
                    }

                    if (Program.CrewCache.ContainsKey(crewMember.PersonLink))
                    {
                        var other = Program.CrewCache[crewMember.PersonLink];

                        other.AddFilmInfo(filmLink, filmName);

                        crewMember.FilmInfoList = other.FilmInfoList;

                        if (crewMember.FirstName != other.FirstName
                            || crewMember.MiddleName != other.MiddleName
                            || crewMember.LastName != other.LastName)
                        {
                            var piwby = new PersonInfoWithoutBirthYear(other);

                            var messageText = string.Format(MessageBoxTexts.CommonCrewNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkers()
                                , crewMember.FormatPersonNameWithMarkers(), crewMember.PersonLink);

                            var logText = string.Format(MessageBoxTexts.CommonCrewNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkersAsHtml(new[] { crewMember })
                                , crewMember.FormatPersonNameWithMarkersAsHtml(new[] { other }), DataGridViewHelper.CreatePersonLinkHtml(crewMember));

                            _log.AppendParagraph(logText);

                            AddMessage(new MessageEntry(messageText, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));

                            ReorganizePossibleDuplicatesCache(crewMember, piwby);

                            other.FirstName = crewMember.FirstName;
                            other.MiddleName = crewMember.MiddleName;
                            other.LastName = crewMember.LastName;
                        }
                    }
                    else
                    {
                        crewMember.AddFilmInfo(filmLink, filmName);

                        Program.CrewCache.Add(crewMember.PersonLink, new PersonInfo(crewMember));
                    }

                    AddPossibleDuplicate(crewMember);
                }
            }

            #endregion
        }

        private static void AddPossibleDuplicate(CastInfo personInfo)
        {
            var piwby = new PersonInfoWithoutBirthYear(personInfo);

            if (Program.PossibleCastDuplicateCache.ContainsKey(piwby))
            {
                var list = Program.PossibleCastDuplicateCache[piwby];

                if (!list.Contains(personInfo))
                {
                    list.Add(personInfo);
                }
            }
            else
            {
                var list = new List<PersonInfo>(1)
                {
                    personInfo,
                };

                Program.PossibleCastDuplicateCache.Add(piwby, list);
            }
        }

        private static void AddPossibleDuplicate(CrewInfo personInfo)
        {
            var piwby = new PersonInfoWithoutBirthYear(personInfo);

            if (Program.PossibleCrewDuplicateCache.ContainsKey(piwby))
            {
                var list = Program.PossibleCrewDuplicateCache[piwby];

                if (!list.Contains(personInfo))
                {
                    list.Add(personInfo);
                }
            }
            else
            {
                var list = new List<PersonInfo>(1)
                {
                    personInfo,
                };

                Program.PossibleCrewDuplicateCache.Add(piwby, list);
            }
        }

        private static void ReorganizePossibleDuplicatesCache(CastInfo personInfo, PersonInfoWithoutBirthYear piwby)
        {
            if (Program.PossibleCastDuplicateCache[piwby].Count == 1)
            {
                Program.PossibleCastDuplicateCache.Remove(piwby);
            }
            else
            {
                var list = Program.PossibleCastDuplicateCache[piwby];

                list.Remove(personInfo);
            }
        }

        private static void ReorganizePossibleDuplicatesCache(CrewInfo personInfo, PersonInfoWithoutBirthYear piwby)
        {
            if (Program.PossibleCrewDuplicateCache[piwby].Count == 1)
            {
                Program.PossibleCrewDuplicateCache.Remove(piwby);
            }
            else
            {
                var list = Program.PossibleCrewDuplicateCache[piwby];

                list.Remove(personInfo);
            }
        }

        private void GetHeadshots(DataGridView castDataGridView, DataGridView crewDataGridView, string headshotButtonText)
        {
            var progressMax = 0;

            if (Program.Settings.DefaultValues.GetCastHeadShots)
            {
                progressMax += castDataGridView.RowCount;
            }

            if (Program.Settings.DefaultValues.GetCrewHeadShots)
            {
                progressMax += crewDataGridView.RowCount;
            }

            this.StartProgress(progressMax, Color.LightGreen);

            try
            {
                this.GetHeadshots(1, castDataGridView, crewDataGridView, headshotButtonText);
            }
            finally
            {
                this.EndProgress();
            }
        }

        private void GetHeadshots(int counter, DataGridView castDataGridView, DataGridView crewDataGridView, string headshotButtonText)
        {
            try
            {
                this.RestartProgress();

                if (Program.Settings.DefaultValues.GetCastHeadShots)
                {
                    DataGridViewHelper.GetHeadshots(castDataGridView, Program.Settings.DefaultValues.UseFakeBirthYears, true, AddMessage, this.SetProgress);
                }

                if (Program.Settings.DefaultValues.GetCrewHeadShots)
                {
                    DataGridViewHelper.GetHeadshots(crewDataGridView, Program.Settings.DefaultValues.UseFakeBirthYears, false, AddMessage, this.SetProgress);
                }

                if (Program.Settings.DefaultValues.AutoCopyHeadShots)
                {
                    if (Directory.Exists(Program.Settings.DefaultValues.CreditPhotosFolder))
                    {
                        var files = Directory.GetFiles(Program.RootPath + @"\Images\DVD Profiler", "*.*");

                        foreach (var file in files)
                        {
                            var sourceFileInfo = new FileInfo(file);

                            var targetFileInfo = new FileInfo(Path.Combine(Program.Settings.DefaultValues.CreditPhotosFolder, sourceFileInfo.Name));

                            if (!targetFileInfo.Exists || Program.Settings.DefaultValues.OverwriteExistingImages)
                            {
                                sourceFileInfo.CopyTo(targetFileInfo.FullName, true);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, MessageBoxTexts.HeadShotsTargetDirectoryInvalid, MessageBoxTexts.HeadShotsTargetDirectoryInvalidHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (!Program.Settings.DefaultValues.DisableParsingCompleteMessageBoxForGetHeadshots)
                {
                    this.ProcessMessageQueue();

                    MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Message.Contains("502") || webEx.Message.Contains("503"))
                {
                    counter++;

                    if (counter >= 5)
                    {
                        MessageBox.Show(this, string.Format(MessageBoxTexts.Error503, headshotButtonText), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        Program.WriteError(webEx);
                    }
                    else
                    {
                        Thread.Sleep(5000);

                        this.GetHeadshots(counter, castDataGridView, crewDataGridView, headshotButtonText);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        protected void ProcessMessageQueue()
        {
            lock (_messageQueueLock)
            {
                var dict = new Dictionary<MessageEntry, bool>(_messageQueue.Count);

                foreach (var entry in _messageQueue)
                {
                    dict[entry] = true;
                }

                foreach (var entry in dict.Keys)
                {
                    MessageBox.Show(this, entry.Message, entry.Header, entry.Buttons, entry.Icon);
                }

                _messageQueue.Clear();
            }
        }

        protected void GetHeadshots(DataGridView castDataGridView, DataGridView crewDataGridView, Button getHeadshotButton)
        {
            this.StartLongAction();

            try
            {
                if (!Directory.Exists(Program.RootPath + @"\Images\CastCrewEdit2"))
                {
                    Directory.CreateDirectory(Program.RootPath + @"\Images\CastCrewEdit2");
                }

                if (Directory.Exists(Program.RootPath + @"\Images\DVD Profiler"))
                {
                    if (FirstRunGetHeadShots)
                    {
                        RemoveDVDProfilerHeadShots();
                    }
                    else if (!Program.Settings.DefaultValues.StoreHeadshotsPerSession)
                    {
                        RemoveDVDProfilerHeadShots();
                    }
                }
                else
                {
                    Directory.CreateDirectory(Program.RootPath + @"\Images\DVD Profiler");
                }

                if (Directory.Exists(Program.RootPath + @"\Images\CCViewer"))
                {
                    if (FirstRunGetHeadShots)
                    {
                        RemoveCCViewerHeadshots();
                    }
                    else if (!Program.Settings.DefaultValues.StoreHeadshotsPerSession)
                    {
                        RemoveCCViewerHeadshots();
                    }
                }
                else
                {
                    Directory.CreateDirectory(Program.RootPath + @"\Images\CCViewer");
                }

                this.GetHeadshots(castDataGridView, crewDataGridView, getHeadshotButton.Text);
            }
            catch (AggregateException ex)
            {
                MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            finally
            {
                FirstRunGetHeadShots = false;

                this.EndLongAction();
            }
        }

        private static void RemoveCCViewerHeadshots()
        {
            var files = Directory.GetFiles(Program.RootPath + @"\Images\CCViewer");

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private static void RemoveDVDProfilerHeadShots()
        {
            var files = Directory.GetFiles(Program.RootPath + @"\Images\DVD Profiler");

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        protected void GetBirthYears(bool parseHeadshotsFollows, DataGridView castDataGridView, DataGridView crewDataGridView, LinkLabel birthYearsInLocalCacheLabel, Button getBirthYearsButton, WebBrowser logWebBrowser)
        {
            this.StartLongAction();

            try
            {
                this.GetBirthYears(parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButton.Text, logWebBrowser);
            }
            catch (AggregateException ex)
            {
                MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            finally
            {
                Program.FlushPersonCache();

                birthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

                this.EndLongAction();
            }
        }

        protected static bool IsShortCutAction(KeyEventArgs e) => (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) || CtrlSWasPressed(e);

        protected static bool CtrlSWasPressed(KeyEventArgs e) => e.Modifiers == Keys.Control && e.KeyCode == Keys.S && ItsMe;

        protected void TrySendToDvdProfiler(KeyEventArgs e)
        {
            if (!CtrlSWasPressed(e))
            {
                return;
            }

            try
            {
                var plain = Clipboard.GetText();

                if (string.IsNullOrWhiteSpace(plain))
                {
                    return;
                }

                var json = EncodeJson(plain);

                using (var content = new StringContent(json, Encoding.UTF8))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                    using (var httpRequest = new HttpRequestMessage()
                    {
                        Method = new HttpMethod("POST"),
                        RequestUri = new Uri("http://localhost:10001/api/Receiver/Receive"),
                        Content = content,
                    })
                    {
                        using (var client = new HttpClient())
                        {
                            var response = client.SendAsync(httpRequest).GetAwaiter().GetResult();

                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                var message = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                                throw new Exception($"Response Code: {response.StatusCode}, Message: {message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to send", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static string EncodeJson(string input)
        {
            var output = input.Replace("\r", "\\r");

            output = output.Replace("\n", "\\n");

            output = output.Replace("\t", "\\t");

            output = output.Replace("\"", "\\\"");

            return $"\"{output}\"";
        }

        private void GetBirthYears(bool parseHeadshotsFollows, DataGridView castDataGridView, DataGridView crewDataGridView, string getBirthYearsButtonText, WebBrowser logWebBrowser)
        {
            var progressMax = castDataGridView.RowCount;

            progressMax += crewDataGridView.RowCount;

            this.StartProgress(progressMax, Color.LightCoral);

            try
            {
                this.GetBirthYears(1, parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButtonText, logWebBrowser);
            }
            finally
            {
                this.EndProgress();
            }
        }

        private void GetBirthYears(int counter, bool parseHeadshotsFollows, DataGridView castDataGridView, DataGridView crewDataGridView, string getBirthYearsButtonText, WebBrowser logWebBrowser)
        {
            try
            {
                this.RestartProgress();

                DataGridViewHelper.GetBirthYears(castDataGridView, Program.CastCache, Program.Settings.DefaultValues, _log, true, AddMessage, this.SetProgress);

                DataGridViewHelper.GetBirthYears(crewDataGridView, Program.CrewCache, Program.Settings.DefaultValues, _log, false, AddMessage, this.SetProgress);

                if (_log.Length > 0)
                {
                    _log.Show(logWebBrowser);
                }

                if (!Program.Settings.DefaultValues.DisableParsingCompleteMessageBoxForGetBirthYears && !parseHeadshotsFollows)
                {
                    this.ProcessMessageQueue();

                    MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Message.Contains("502") || webEx.Message.Contains("503"))
                {
                    counter++;

                    if (counter >= 5)
                    {
                        MessageBox.Show(this, string.Format(MessageBoxTexts.Error503, getBirthYearsButtonText), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        Program.WriteError(webEx);
                    }
                    else
                    {
                        Thread.Sleep(5000);

                        this.GetBirthYears(counter, parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButtonText, logWebBrowser);
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}