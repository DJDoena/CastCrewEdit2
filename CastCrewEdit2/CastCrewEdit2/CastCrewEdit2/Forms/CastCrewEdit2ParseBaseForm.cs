using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    [ComVisible(true)]
    public class CastCrewEdit2ParseBaseForm : CastCrewEdit2BaseForm
    {
        protected static readonly Log Log;

        private static readonly List<MessageEntry> MessageQueue;

        internal static Boolean FirstRunGetHeadShots;

        private readonly static Object MessageQueueLock;

        static CastCrewEdit2ParseBaseForm()
        {
            MessageQueueLock = new Object();
            Log = new Log();
            MessageQueue = new List<MessageEntry>();
            FirstRunGetHeadShots = true;
        }

        protected static Boolean ItsMe
            => (Environment.UserName == "djdoe");

        protected static Boolean HasAgreed
        {
            get
            {
                return (ItsMe ? true : s_HasAgreed);
            }
            set
            {
                s_HasAgreed = value;
            }
        }

        protected static void AddMessage(MessageEntry entry)
        {
            lock (MessageQueueLock)
            {
                MessageQueue.Add(entry);
            }
        }

        protected void OnBirthYearsInLocalCacheLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            PersonInfo[] persons;

            Cursor = Cursors.WaitCursor;
            persons = new PersonInfo[IMDbParser.PersonHash.Keys.Count];
            IMDbParser.PersonHash.Keys.CopyTo(persons, 0);
            ShowCache(new List<PersonInfo>(persons), "Local Birth Year Cache");
        }

        protected void OnPersonsInLocalCacheLabelLinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            PersonInfo[] persons;

            Cursor = Cursors.WaitCursor;
            persons = new PersonInfo[Program.PersonCacheCount];
            Program.CastCache.Values.CopyTo(persons, 0);
            Program.CrewCache.Values.CopyTo(persons, Program.CastCache.Values.Count);
            ShowCache(new List<PersonInfo>(persons), "Local Person Cache");
        }

        private void ShowCache(List<PersonInfo> persons
            , String cacheName)
        {
            persons.Sort(ComparePersonInfos);

            Cursor = Cursors.Default;

            using (CacheForm form = new CacheForm(persons, cacheName))
            {
                form.ShowDialog(this);
            }
        }

        private static Int32 ComparePersonInfos(PersonInfo left
            , PersonInfo right)
        {
            Int32 compare = left.LastName.CompareTo(right.LastName);

            if (compare != 0)
            {
                return (compare);
            }

            compare = left.FirstName.CompareTo(right.FirstName);

            if (compare != 0)
            {
                return (compare);
            }

            compare = left.MiddleName.CompareTo(right.MiddleName);

            if (compare != 0)
            {
                return (compare);
            }

            if ((left.BirthYearWasRetrieved) && (String.IsNullOrEmpty(left.BirthYear) == false))
            {
                compare = left.BirthYear.CompareTo(right.BirthYear);
            }
            else
            {
                compare = String.Empty.CompareTo(right.BirthYear);
            }

            if (String.IsNullOrEmpty(left.FakeBirthYear) == false)
            {
                compare = left.FakeBirthYear.CompareTo(right.FakeBirthYear);
            }
            else
            {
                compare = String.Empty.CompareTo(right.FakeBirthYear);
            }

            return (compare);
        }

        protected void OnDataGridViewCellContentClick(Object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                DataGridViewRow row;

                row = ((DataGridView)sender).Rows[e.RowIndex];
                if ((row.Cells[ColumnNames.FirstName].Value.ToString() == FirstNames.Title)
                    || (row.Cells[ColumnNames.FirstName].Value.ToString() == FirstNames.Divider))
                {
                    Process.Start(IMDbParser.TitleUrl
                        + row.Cells[ColumnNames.Link].Value.ToString());
                }
                else
                {
                    Process.Start(IMDbParser.PersonUrl
                        + row.Cells[ColumnNames.Link].Value.ToString());
                }
            }
            else if (((DataGridView)sender).Columns[e.ColumnIndex].Name == ColumnNames.MoveUp)
            {
                DataGridViewHelper.DataGridViewDisableButtonCell cell;
                DataGridViewRow row;

                row = ((DataGridView)sender).Rows[e.RowIndex];
                cell = (DataGridViewHelper.DataGridViewDisableButtonCell)
                    (row.Cells[ColumnNames.MoveUp]);
                if (cell.Enabled)
                {
                    MoveRow((CastInfo)(row.Tag), true);
                }
            }
            else if (((DataGridView)sender).Columns[e.ColumnIndex].Name == ColumnNames.MoveDown)
            {
                DataGridViewHelper.DataGridViewDisableButtonCell cell;
                DataGridViewRow row;

                row = ((DataGridView)sender).Rows[e.RowIndex];
                cell = (DataGridViewHelper.DataGridViewDisableButtonCell)
                    (row.Cells[ColumnNames.MoveDown]);
                if (cell.Enabled)
                {
                    MoveRow((CastInfo)(row.Tag), false);
                }
            }
            else if (((DataGridView)sender).Columns[e.ColumnIndex].Name == ColumnNames.RemoveRow)
            {
                DataGridViewHelper.DataGridViewDisableButtonCell cell;
                DataGridViewRow row;

                row = ((DataGridView)sender).Rows[e.RowIndex];
                cell = (DataGridViewHelper.DataGridViewDisableButtonCell)
                    (row.Cells[ColumnNames.RemoveRow]);
                if (cell.Enabled)
                {
                    RemoveRow((CastInfo)(row.Tag));
                }
            }
        }

        protected virtual void MoveRow(CastInfo castMember, Boolean up)
        {
            //abstract method but class can't be actract ore else the Form Designer will fail
        }

        protected virtual void RemoveRow(CastInfo castMember)
        {
            //abstract method but class can't be actract ore else the Form Designer will fail
        }

        protected static Int32 FindIndexOfCastMember(List<CastInfo> castList, CastInfo castMember)
        {
            Int32 indexOf;

            indexOf = -1;
            for (Int32 i = 0; i < castList.Count; i++)
            {
                if (castList[i].Identifier == castMember.Identifier)
                {
                    indexOf = i;
                    break;
                }
            }
            return (indexOf);
        }

        protected void UpdateUI(List<CastInfo> castList
            , List<CrewInfo> crewList
            , DataGridView movieCastDataGridView
            , DataGridView movieCrewDataGridView
            , Boolean parseCast
            , Boolean parseCrew
            , String filmLink
            , String filmName)
        {
            #region Update Cast UI
            if (parseCast)
            {
                DataGridViewHelper.FillCastRows(movieCastDataGridView, castList, false, false);
                foreach (CastInfo castMember in castList)
                {
                    if (castMember.FirstName == FirstNames.Title)
                    {
                        continue;
                    }
                    if (Program.CastCache.ContainsKey(castMember.PersonLink))
                    {
                        PersonInfo other;

                        other = Program.CastCache[castMember.PersonLink];
                        other.AddFilmInfo(filmLink, filmName);
                        castMember.FilmInfoList = other.FilmInfoList;
                        if ((castMember.FirstName != other.FirstName) || (castMember.MiddleName != other.MiddleName)
                            || (castMember.LastName != other.LastName))
                        {
                            String messageText;
                            String logText;
                            PersonInfoWithoutBirthYear piwby;

                            piwby = new PersonInfoWithoutBirthYear(other);
                            messageText = String.Format(MessageBoxTexts.CommonCastNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkers()
                                , castMember.FormatPersonNameWithMarkers(), castMember.PersonLink);
                            logText = String.Format(MessageBoxTexts.CommonCastNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkersAsHtml(new[] { castMember })
                                , castMember.FormatPersonNameWithMarkersAsHtml(new[] { other }), DataGridViewHelper.CreatePersonLinkHtml(castMember));
                            Log.AppendParagraph(logText);
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
                foreach (CrewInfo crewMember in crewList)
                {
                    if (crewMember.FirstName == FirstNames.Title)
                    {
                        continue;
                    }
                    if (Program.CrewCache.ContainsKey(crewMember.PersonLink))
                    {
                        PersonInfo other;

                        other = Program.CrewCache[crewMember.PersonLink];
                        other.AddFilmInfo(filmLink, filmName);
                        crewMember.FilmInfoList = other.FilmInfoList;
                        if ((crewMember.FirstName != other.FirstName) || (crewMember.MiddleName != other.MiddleName)
                            || (crewMember.LastName != other.LastName))
                        {
                            String text;
                            String logText;
                            PersonInfoWithoutBirthYear piwby;

                            piwby = new PersonInfoWithoutBirthYear(other);
                            text = String.Format(MessageBoxTexts.CommonCrewNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkers()
                                , crewMember.FormatPersonNameWithMarkers(), crewMember.PersonLink);
                            logText = String.Format(MessageBoxTexts.CommonCrewNameHasChanged
                                , other.FormatPersonNameWithoutMarkers(), other.FormatPersonNameWithMarkersAsHtml(new[] { crewMember })
                                , crewMember.FormatPersonNameWithMarkersAsHtml(new[] { other }), DataGridViewHelper.CreatePersonLinkHtml(crewMember));
                            Log.AppendParagraph(logText);
                            AddMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
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
            PersonInfoWithoutBirthYear piwby;

            piwby = new PersonInfoWithoutBirthYear(personInfo);
            if (Program.PossibleCastDuplicateCache.ContainsKey(piwby))
            {
                List<PersonInfo> list;

                list = Program.PossibleCastDuplicateCache[piwby];
                if (list.Contains(personInfo) == false)
                {
                    list.Add(personInfo);
                }
            }
            else
            {
                List<PersonInfo> list;

                list = new List<PersonInfo>(1);
                list.Add(personInfo);
                Program.PossibleCastDuplicateCache.Add(piwby, list);
            }
        }

        private static void AddPossibleDuplicate(CrewInfo personInfo)
        {
            PersonInfoWithoutBirthYear piwby;

            piwby = new PersonInfoWithoutBirthYear(personInfo);
            if (Program.PossibleCrewDuplicateCache.ContainsKey(piwby))
            {
                List<PersonInfo> list;

                list = Program.PossibleCrewDuplicateCache[piwby];
                if (list.Contains(personInfo) == false)
                {
                    list.Add(personInfo);
                }
            }
            else
            {
                List<PersonInfo> list;

                list = new List<PersonInfo>(1);
                list.Add(personInfo);
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
                List<PersonInfo> list;

                list = Program.PossibleCastDuplicateCache[piwby];
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
                List<PersonInfo> list;

                list = Program.PossibleCrewDuplicateCache[piwby];
                list.Remove(personInfo);
            }
        }

        private void GetHeadshots(DataGridView castDataGridView
            , DataGridView crewDataGridView
            , String headshotButtonText)
        {
            Int32 progressMax = 0;

            if (Program.Settings.DefaultValues.GetCastHeadShots)
            {
                progressMax += castDataGridView.RowCount;
            }

            if (Program.Settings.DefaultValues.GetCrewHeadShots)
            {
                progressMax += crewDataGridView.RowCount;
            }

            StartProgress(progressMax, Color.LightGreen);

            try
            {
                GetHeadshots(1, castDataGridView, crewDataGridView, headshotButtonText);
            }
            finally
            {
                EndProgress();
            }
        }

        private void GetHeadshots(Int32 counter
            , DataGridView castDataGridView
            , DataGridView crewDataGridView
            , String headshotButtonText)
        {
            try
            {
                RestartProgress();

                if (Program.Settings.DefaultValues.GetCastHeadShots)
                {
                    DataGridViewHelper.GetHeadshots(castDataGridView, Program.Settings.DefaultValues.UseFakeBirthYears, true
                        , AddMessage, SetProgress);
                }
                if (Program.Settings.DefaultValues.GetCrewHeadShots)
                {
                    DataGridViewHelper.GetHeadshots(crewDataGridView, Program.Settings.DefaultValues.UseFakeBirthYears, false
                        , AddMessage, SetProgress);
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
                        MessageBox.Show(this, MessageBoxTexts.HeadShotsTargetDirectoryInvalid, MessageBoxTexts.HeadShotsTargetDirectoryInvalidHeader
                            , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (Program.Settings.DefaultValues.DisableParsingCompleteMessageBoxForGetHeadshots == false)
                {
                    ProcessMessageQueue();
                    MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK
                        , MessageBoxIcon.Information);
                }
            }
            catch (WebException webEx)
            {
                if ((webEx.Message.Contains("502")) || (webEx.Message.Contains("503")))
                {
                    counter++;
                    if (counter >= 5)
                    {
                        MessageBox.Show(this, String.Format(MessageBoxTexts.Error503, headshotButtonText)
                            , MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Program.WriteError(webEx);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        GetHeadshots(counter, castDataGridView, crewDataGridView, headshotButtonText);
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
            lock (MessageQueueLock)
            {
                Dictionary<MessageEntry, Boolean> dict = new Dictionary<MessageEntry, Boolean>(MessageQueue.Count);

                foreach (MessageEntry entry in MessageQueue)
                {
                    dict[entry] = true;
                }

                foreach (MessageEntry entry in dict.Keys)
                {
                    MessageBox.Show(this, entry.Message, entry.Header, entry.Buttons, entry.Icon);
                }

                MessageQueue.Clear();
            }
        }

        protected void GetHeadshots(DataGridView castDataGridView
            , DataGridView crewDataGridView
            , Button getHeadshotButton)
        {
            StartLongAction();

            try
            {
                if (Directory.Exists(Program.RootPath + @"\Images\CastCrewEdit2") == false)
                {
                    Directory.CreateDirectory(Program.RootPath + @"\Images\CastCrewEdit2");
                }
                if (Directory.Exists(Program.RootPath + @"\Images\DVD Profiler"))
                {
                    if (FirstRunGetHeadShots)
                    {
                        RemoveDVDProfilerHeadShots();
                    }
                    else if (Program.Settings.DefaultValues.StoreHeadshotsPerSession == false)
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
                    else if (Program.Settings.DefaultValues.StoreHeadshotsPerSession == false)
                    {
                        RemoveCCViewerHeadshots();
                    }
                }
                else
                {
                    Directory.CreateDirectory(Program.RootPath + @"\Images\CCViewer");
                }

                GetHeadshots(castDataGridView, crewDataGridView, getHeadshotButton.Text);
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

                EndLongAction();
            }
        }

        private static void RemoveCCViewerHeadshots()
        {
            String[] files;

            files = Directory.GetFiles(Program.RootPath + @"\Images\CCViewer");
            foreach (String file in files)
            {
                File.Delete(file);
            }
        }

        private static void RemoveDVDProfilerHeadShots()
        {
            String[] files;

            files = Directory.GetFiles(Program.RootPath + @"\Images\DVD Profiler");
            foreach (String file in files)
            {
                File.Delete(file);
            }
        }

        protected void GetBirthYears(Boolean parseHeadshotsFollows
            , DataGridView castDataGridView
            , DataGridView crewDataGridView
            , LinkLabel birthYearsInLocalCacheLabel
            , Button getBirthYearsButton
            , WebBrowser logWebBrowser)
        {
            StartLongAction();

            try
            {
                GetBirthYears(parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButton.Text, logWebBrowser);
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

                EndLongAction();
            }
        }

        private void GetBirthYears(Boolean parseHeadshotsFollows
            , DataGridView castDataGridView
            , DataGridView crewDataGridView
            , String getBirthYearsButtonText
            , WebBrowser logWebBrowser)
        {
            Int32 progressMax = castDataGridView.RowCount;

            progressMax += crewDataGridView.RowCount;

            StartProgress(progressMax, Color.LightCoral);

            try
            {
                GetBirthYears(1, parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButtonText, logWebBrowser);
            }
            finally
            {
                EndProgress();
            }
        }

        private void GetBirthYears(Int32 counter
            , Boolean parseHeadshotsFollows
            , DataGridView castDataGridView
            , DataGridView crewDataGridView
            , String getBirthYearsButtonText
            , WebBrowser logWebBrowser)
        {
            try
            {
                RestartProgress();

                DataGridViewHelper.GetBirthYears(castDataGridView, Program.CastCache, Program.Settings.DefaultValues, Log, true
                    , AddMessage, SetProgress);

                DataGridViewHelper.GetBirthYears(crewDataGridView, Program.CrewCache, Program.Settings.DefaultValues, Log, false
                    , AddMessage, SetProgress);

                if (Log.Length > 0)
                {
                    Log.Show(logWebBrowser);
                }

                if ((Program.Settings.DefaultValues.DisableParsingCompleteMessageBoxForGetBirthYears == false)
                    && (parseHeadshotsFollows == false))
                {
                    ProcessMessageQueue();

                    MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete
                        , MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException webEx)
            {
                if ((webEx.Message.Contains("502")) || (webEx.Message.Contains("503")))
                {
                    counter++;
                    if (counter >= 5)
                    {
                        MessageBox.Show(this, String.Format(MessageBoxTexts.Error503, getBirthYearsButtonText)
                            , MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Program.WriteError(webEx);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        GetBirthYears(counter, parseHeadshotsFollows, castDataGridView, crewDataGridView, getBirthYearsButtonText
                            , logWebBrowser);
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