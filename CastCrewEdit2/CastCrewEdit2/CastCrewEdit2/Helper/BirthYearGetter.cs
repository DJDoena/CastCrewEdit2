using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class BirthYearGetter
    {
        private static readonly Regex BirthYearRegex;
        private static readonly Regex DateOfBirthRegex;

        static BirthYearGetter()
        {
            BirthYearRegex = new Regex("<a.+?href=\"/search/name\\?birth_year=(?'BirthYear'[0-9]+)", RegexOptions.Compiled);
            DateOfBirthRegex = new Regex("<h4 class=\"inline\">Born:</h4>", RegexOptions.Compiled);
        }

        #region From IMDbParser

        internal static String Get(String personId)
        {
            String webSite = IMDbParser.GetWebSite(IMDbParser.PersonUrl + personId);

            using (StringReader sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    String line = sr.ReadLine();

                    Match match = DateOfBirthRegex.Match(line);

                    if (match.Success)
                    {
                        return (GetBirthYear(sr));
                    }
                }
            }

            return (String.Empty);
        }

        private static String GetBirthYear(StringReader sr)
        {
            while (sr.Peek() != -1)
            {
                String line = sr.ReadLine();

                Match match = BirthYearRegex.Match(line);

                if (match.Success)
                {
                    return (match.Groups["BirthYear"].Value);
                }
            }

            return (String.Empty);
        }

        #endregion

        #region From DataGridViewHelper

        #region GetBirthYear

        internal static List<IAsyncResult> GetBirthYear(Dictionary<String, PersonInfo> persons
            , DefaultValues defaultValues
            , Log log
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , DataGridViewRow row)
        {
            List<IAsyncResult> invokeResults = new List<IAsyncResult>();

            PersonInfo person = (PersonInfo)(row.Tag);

            CastInfo castMember = person as CastInfo;

            if ((person.FirstName != FirstNames.Title) && (person.FirstName != FirstNames.Divider) && (String.IsNullOrEmpty(person.BirthYear)))
            {
                PersonInfo other = persons[person.PersonLink];

                if (IMDbParser.ForcedFakeBirthYears.ContainsKey(person.PersonLink))
                {
                    String previousBirthYear = null;

                    if (String.IsNullOrEmpty(other.BirthYear) == false)
                    {
                        previousBirthYear = other.BirthYear;
                    }
                    else if (String.IsNullOrEmpty(other.FakeBirthYear) == false)
                    {
                        previousBirthYear = other.FakeBirthYear;
                    }

                    other.BirthYear = String.Empty;
                    other.FakeBirthYear = IMDbParser.ForcedFakeBirthYears[person.PersonLink].ToString();
                    other.BirthYearWasRetrieved = false;

                    person.BirthYear = other.BirthYear;
                    person.FakeBirthYear = other.FakeBirthYear;
                    person.BirthYearWasRetrieved = other.BirthYearWasRetrieved;

                    if ((String.IsNullOrEmpty(previousBirthYear) == false) && (previousBirthYear != other.FakeBirthYear))
                    {
                        ShowBirthYearMessageBox(log, person, previousBirthYear, isCast, addMessage);
                    }

                    ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = other.FakeBirthYear);

                    if ((castMember == null) || (castMember.IsAdditionalRow == false))
                    {
                        ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
                    }
                }
                else if (defaultValues.TakeBirthYearFromLocalCache)
                {
                    if (other.BirthYearWasRetrieved)
                    {
                        if (String.IsNullOrEmpty(other.BirthYear))
                        {
                            if ((Program.Settings.DefaultValues.RetrieveBirthYearWhenLocalCacheEmpty))
                            {
                                invokeResults.AddRange(GetBirthYear(row, person, other, log, isCast, addMessage));
                            }
                            else
                            {
                                person.BirthYear = other.BirthYear;
                                person.BirthYearWasRetrieved = other.BirthYearWasRetrieved;

                                ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = other.BirthYear);
                            }
                        }
                        else
                        {
                            person.BirthYear = other.BirthYear;
                            person.BirthYearWasRetrieved = other.BirthYearWasRetrieved;

                            ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = other.BirthYear);

                            if (String.IsNullOrEmpty(other.FakeBirthYear) == false)
                            {
                                ShowBirthYearMessageBox(log, other, person.FakeBirthYear, isCast, addMessage);

                                person.FakeBirthYear = null;
                                other.FakeBirthYear = null;
                            }
                        }
                    }
                    else
                    {
                        invokeResults.AddRange(GetBirthYear(row, person, other, log, isCast, addMessage));
                    }

                    if ((castMember == null) || (castMember.IsAdditionalRow == false))
                    {
                        ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
                    }
                }
                else
                {
                    String previousBirthYear = other.BirthYear;

                    invokeResults.AddRange(GetBirthYear(row, person, other, log, isCast, addMessage));

                    if ((castMember == null) || (castMember.IsAdditionalRow == false))
                    {
                        ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
                    }

                    if ((String.IsNullOrEmpty(person.BirthYear) == false) && (String.IsNullOrEmpty(previousBirthYear) == false))
                    {
                        if (person.BirthYear != previousBirthYear)
                        {
                            ShowBirthYearMessageBox(log, person, previousBirthYear, isCast, addMessage);
                        }
                    }
                    else if ((String.IsNullOrEmpty(person.BirthYear)) && (String.IsNullOrEmpty(previousBirthYear) == false))
                    {
                        ShowBirthYearMessageBox(log, person, previousBirthYear, isCast, addMessage);
                    }
                }
            }

            return (invokeResults);
        }

        private static List<IAsyncResult> GetBirthYear(DataGridViewRow row
            , PersonInfo person
            , PersonInfo other
            , Log log
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            List<IAsyncResult> invokeResults = new List<IAsyncResult>();

            IMDbParser.GetBirthYear(person);

            ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = person.BirthYear);

            other.BirthYear = person.BirthYear;

            other.BirthYearWasRetrieved = true;

            if ((String.IsNullOrEmpty(other.BirthYear) == false) && (String.IsNullOrEmpty(other.FakeBirthYear) == false))
            {
                ShowBirthYearMessageBox(log, other, person.FakeBirthYear, isCast, addMessage);

                person.FakeBirthYear = null;
                other.FakeBirthYear = null;
            }

            return (invokeResults);
        }

        private static void ExecuteAction(DataGridViewRow row
            , List<IAsyncResult> invokeResults
            , Action action)
        {
            DataGridView dataGridView = row.DataGridView;

            if (dataGridView.InvokeRequired)
            {
                invokeResults.Add(dataGridView.BeginInvoke(action));
            }
            else
            {
                action();
            }
        }

        private static void ShowBirthYearMessageBox(Log log
            , PersonInfo person
            , String previousBirthYear
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            String text;
            String logText;

            PersonInfo old = new PersonInfo(person);

            old.BirthYear = previousBirthYear;

            Boolean useFakeBirthYears = Program.Settings.DefaultValues.UseFakeBirthYears;

            if (isCast)
            {
                text = String.Format(MessageBoxTexts.BirthYearCastHasChanged, old.FormatPersonNameWithoutMarkers()
                    , old.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), person.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), old.PersonLink);

                logText = String.Format(MessageBoxTexts.BirthYearCastHasChanged, old.FormatPersonNameWithoutMarkers()
                    , old.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { person }), person.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { old }), DataGridViewHelper.CreatePersonLinkHtml(old));
            }
            else
            {
                text = String.Format(MessageBoxTexts.BirthYearCrewHasChanged, old.FormatPersonNameWithoutMarkers()
                    , old.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), person.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), old.PersonLink);

                logText = String.Format(MessageBoxTexts.BirthYearCrewHasChanged, old.FormatPersonNameWithoutMarkers()
                    , old.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { person }), person.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { old }), DataGridViewHelper.CreatePersonLinkHtml(old));
            }

            log.AppendParagraph(logText);

            addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
        }

        #endregion

        internal static Dictionary<String, List<DataGridViewRow>> GetGroupedBirthYears(DataGridView dataGridView)
        {
            Dictionary<String, List<DataGridViewRow>> dict = new Dictionary<String, List<DataGridViewRow>>(dataGridView.Rows.Count);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                PersonInfo person = (PersonInfo)(row.Tag);

                List<DataGridViewRow> list;
                if (dict.TryGetValue(person.PersonLink, out list) == false)
                {
                    list = new List<DataGridViewRow>(1);

                    dict.Add(person.PersonLink, list);
                }

                list.Add(row);
            }

            return (dict);
        }

        #endregion
    }
}