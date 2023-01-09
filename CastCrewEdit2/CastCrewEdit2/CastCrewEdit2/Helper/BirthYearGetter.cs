namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Resources;

    internal static class BirthYearGetter
    {
        private static readonly Regex _birthYearRegex;

        private static readonly Regex _dateOfBirthRegex;

        static BirthYearGetter()
        {
            _birthYearRegex = new Regex("<a.+?href=\"/search/name\\?birth_year=(?'BirthYear'[0-9]+)", RegexOptions.Compiled);

            _dateOfBirthRegex = new Regex("<h4 class=\"inline\">Born:</h4>", RegexOptions.Compiled);
        }

        #region From IMDbParser

        internal static string Get(string personId)
        {
            var webSite = IMDbParser.GetWebSite($"{IMDbParser.PersonUrl}{personId}/");

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    if (_dateOfBirthRegex.Match(line).Success)
                    {
                        var birthYear = GetBirthYear(sr);

                        return birthYear;
                    }
                }
            }

            return string.Empty;
        }

        private static string GetBirthYear(StringReader sr)
        {
            while (sr.Peek() != -1)
            {
                var line = sr.ReadLine();

                var match = _birthYearRegex.Match(line);

                if (match.Success)
                {
                    return match.Groups["BirthYear"].Value;
                }
            }

            return string.Empty;
        }

        #endregion

        #region From DataGridViewHelper

        #region GetBirthYear

        internal static List<IAsyncResult> GetBirthYear(Dictionary<string, PersonInfo> persons, DefaultValues defaultValues, Log log, bool isCast, Action<MessageEntry> addMessage, DataGridViewRow row)
        {
            var invokeResults = new List<IAsyncResult>();

            var person = (PersonInfo)(row.Tag);

            var castMember = isCast ? (CastInfo)person : null;

            if (person.FirstName == FirstNames.Title 
                || person.FirstName == FirstNames.Divider
                || person.FirstName == FirstNames.GroupDividerStart
                || person.FirstName == FirstNames.GroupDividerEnd
                || !string.IsNullOrEmpty(person.BirthYear))
            {
                return invokeResults;
            }

            var other = persons[person.PersonLink];

            if (IMDbParser.ForcedFakeBirthYears.ContainsKey(person.PersonLink))
            {
                UseForcedFakeBirthYear(row, person, other, log, castMember, addMessage, invokeResults);
            }
            else if (defaultValues.TakeBirthYearFromLocalCache)
            {
                UseBirthYearFromLocalCache(row, person, other, log, castMember, addMessage, invokeResults);
            }
            else
            {
                DownloadBirthYear(row, person, other, log, castMember, addMessage, invokeResults);
            }

            return invokeResults;
        }

        private static void UseForcedFakeBirthYear(DataGridViewRow row, PersonInfo person, PersonInfo other, Log log, CastInfo castMember, Action<MessageEntry> addMessage, List<IAsyncResult> invokeResults)
        {
            string previousBirthYear = null;

            if (!string.IsNullOrEmpty(other.BirthYear))
            {
                previousBirthYear = other.BirthYear;
            }
            else if (!string.IsNullOrEmpty(other.FakeBirthYear))
            {
                previousBirthYear = other.FakeBirthYear;
            }

            other.BirthYear = string.Empty;
            other.FakeBirthYear = IMDbParser.ForcedFakeBirthYears[person.PersonLink].ToString();
            other.BirthYearWasRetrieved = false;

            person.BirthYear = other.BirthYear;
            person.FakeBirthYear = other.FakeBirthYear;
            person.BirthYearWasRetrieved = other.BirthYearWasRetrieved;

            if (!string.IsNullOrEmpty(previousBirthYear) && previousBirthYear != other.FakeBirthYear)
            {
                ShowBirthYearMessageBox(log, person, previousBirthYear, castMember, addMessage);
            }

            ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = other.FakeBirthYear);

            if (castMember == null || !castMember.IsAdditionalRow)
            {
                ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
            }
        }

        private static void UseBirthYearFromLocalCache(DataGridViewRow row, PersonInfo person, PersonInfo other, Log log, CastInfo castMember, Action<MessageEntry> addMessage, List<IAsyncResult> invokeResults)
        {
            if (other.BirthYearWasRetrieved)
            {
                if (string.IsNullOrEmpty(other.BirthYear))
                {
                    if ((Program.DefaultValues.RetrieveBirthYearWhenLocalCacheEmpty))
                    {
                        invokeResults.AddRange(GetBirthYear(row, person, other, log, castMember, addMessage));
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

                    if (!string.IsNullOrEmpty(other.FakeBirthYear))
                    {
                        ShowBirthYearMessageBox(log, other, person.FakeBirthYear, castMember, addMessage);

                        person.FakeBirthYear = null;
                        other.FakeBirthYear = null;
                    }
                }
            }
            else
            {
                invokeResults.AddRange(GetBirthYear(row, person, other, log, castMember, addMessage));
            }

            if (castMember == null || !castMember.IsAdditionalRow)
            {
                ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
            }
        }

        private static void DownloadBirthYear(DataGridViewRow row, PersonInfo person, PersonInfo other, Log log, CastInfo castMember, Action<MessageEntry> addMessage, List<IAsyncResult> invokeResults)
        {
            var previousBirthYear = other.BirthYear;

            invokeResults.AddRange(GetBirthYear(row, person, other, log, castMember, addMessage));

            if (castMember == null || !castMember.IsAdditionalRow)
            {
                ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Style.BackColor = Color.White);
            }

            if (!string.IsNullOrEmpty(person.BirthYear) && !string.IsNullOrEmpty(previousBirthYear))
            {
                if (person.BirthYear != previousBirthYear)
                {
                    ShowBirthYearMessageBox(log, person, previousBirthYear, castMember, addMessage);
                }
            }
            else if (string.IsNullOrEmpty(person.BirthYear) && !string.IsNullOrEmpty(previousBirthYear))
            {
                ShowBirthYearMessageBox(log, person, previousBirthYear, castMember, addMessage);
            }
        }

        private static List<IAsyncResult> GetBirthYear(DataGridViewRow row, PersonInfo person, PersonInfo other, Log log, CastInfo castMember, Action<MessageEntry> addMessage)
        {
            var invokeResults = new List<IAsyncResult>();

            IMDbParser.GetBirthYear(person);

            ExecuteAction(row, invokeResults, () => row.Cells[ColumnNames.BirthYear].Value = person.BirthYear);

            other.BirthYear = person.BirthYear;

            other.BirthYearWasRetrieved = true;

            if (!string.IsNullOrEmpty(other.BirthYear) && !string.IsNullOrEmpty(other.FakeBirthYear))
            {
                ShowBirthYearMessageBox(log, other, person.FakeBirthYear, castMember, addMessage);

                person.FakeBirthYear = null;
                other.FakeBirthYear = null;
            }

            return (invokeResults);
        }

        private static void ExecuteAction(DataGridViewRow row, List<IAsyncResult> invokeResults, Action action)
        {
            var dataGridView = row.DataGridView;

            if (dataGridView.InvokeRequired)
            {
                invokeResults.Add(dataGridView.BeginInvoke(action));
            }
            else
            {
                action();
            }
        }

        private static void ShowBirthYearMessageBox(Log log, PersonInfo person, string previousBirthYear, CastInfo castMember, Action<MessageEntry> addMessage)
        {
            var old = new PersonInfo(person)
            {
                BirthYear = previousBirthYear,
            };

            var useFakeBirthYears = Program.DefaultValues.UseFakeBirthYears;

            var text = castMember != null
                ? string.Format(MessageBoxTexts.BirthYearCastHasChanged, old.FormatPersonNameWithoutMarkers(), old.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), person.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), old.PersonLink)
                : string.Format(MessageBoxTexts.BirthYearCrewHasChanged, old.FormatPersonNameWithoutMarkers(), old.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), person.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYears), old.PersonLink);

            var logText = castMember != null
                ? string.Format(MessageBoxTexts.BirthYearCastHasChanged, old.FormatPersonNameWithoutMarkers(), old.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { person }), person.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { old }), DataGridViewHelper.CreatePersonLinkHtml(old))
                : string.Format(MessageBoxTexts.BirthYearCrewHasChanged, old.FormatPersonNameWithoutMarkers(), old.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { person }), person.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYears, new[] { old }), DataGridViewHelper.CreatePersonLinkHtml(old));

            log.AppendParagraph(logText);

            addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
        }

        #endregion

        internal static Dictionary<string, List<DataGridViewRow>> GetGroupedBirthYears(DataGridView dataGridView)
        {
            var dict = new Dictionary<string, List<DataGridViewRow>>(dataGridView.Rows.Count);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var person = (PersonInfo)(row.Tag);

                if (!dict.TryGetValue(person.PersonLink, out List<DataGridViewRow> list))
                {
                    list = new List<DataGridViewRow>(1);

                    dict.Add(person.PersonLink, list);
                }

                list.Add(row);
            }

            return dict;
        }

        #endregion
    }
}