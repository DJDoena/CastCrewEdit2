namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using DVDProfilerXML;
    using DVDProfilerXML.Version400;
    using Extended;
    using Resources;

    internal static class DataGridViewHelper
    {
        private static bool _dataFillMode;

        private static readonly Dictionary<PersonInfoWithoutBirthYear, bool> _confirmedPossibleCastDuplicates;

        private static readonly Dictionary<PersonInfoWithoutBirthYear, bool> _confirmedPossibleCrewDuplicates;

        static DataGridViewHelper()
        {
            _dataFillMode = false;

            _confirmedPossibleCastDuplicates = new Dictionary<PersonInfoWithoutBirthYear, bool>();

            _confirmedPossibleCrewDuplicates = new Dictionary<PersonInfoWithoutBirthYear, bool>();
        }

        #region GetBirthYears

        public static void GetBirthYears(DataGridView dataGridView, Dictionary<string, PersonInfo> persons, DefaultValues defaultValues, Log log, bool isCast, Action<MessageEntry> addMessage, SetProgress setProgress)
        {
            _dataFillMode = true;

            var groupedBirthYears = BirthYearGetter.GetGroupedBirthYears(dataGridView);

            var maxCount = groupedBirthYears.Count;

            var keys = groupedBirthYears.Keys.ToList();

            for (var keyIndex = 0; keyIndex < maxCount;)
            {
                var progress = 0;

                try
                {
                    progress = TryGetBirthYearsInTasks(dataGridView, groupedBirthYears, keys, persons, defaultValues, log, isCast, addMessage, maxCount, ref keyIndex);
                }
                catch (AggregateException aggrEx)
                {
                    var ex = aggrEx.InnerExceptions.First();

                    throw ex;
                }

                for (var current = 0; current < progress; current++)
                {
                    setProgress();
                }
            }

            AdaptDuplicateBirthYearRows(groupedBirthYears, setProgress);

            _dataFillMode = false;
        }

        private static void AdaptDuplicateBirthYearRows(Dictionary<string, List<DataGridViewRow>> groupedBirthYears, SetProgress setProgress)
        {
            foreach (var rowList in groupedBirthYears.Values)
            {
                if (rowList.Count > 1)
                {
                    AdaptDuplicateBirthYearRows(rowList, setProgress);
                }
            }
        }

        private static void AdaptDuplicateBirthYearRows(List<DataGridViewRow> rowList, SetProgress setProgress)
        {
            var firstRow = rowList[0];

            for (var rowIndex = 1; rowIndex < rowList.Count; rowIndex++)
            {
                var row = rowList[rowIndex];

                AdaptDuplicateBirthYearRow(firstRow, row);

                setProgress();
            }
        }

        private static void AdaptDuplicateBirthYearRow(DataGridViewRow firstRow, DataGridViewRow row)
        {
            var firstPerson = (PersonInfo)firstRow.Tag;

            if (!string.IsNullOrEmpty(firstPerson.PersonLink))
            {
                var person = (PersonInfo)row.Tag;

                person.BirthYear = firstPerson.BirthYear;
                person.FakeBirthYear = firstPerson.FakeBirthYear;
                person.BirthYearWasRetrieved = firstPerson.BirthYearWasRetrieved;

                row.Cells[ColumnNames.BirthYear].Value = firstRow.Cells[ColumnNames.BirthYear].Value;

                var ci = person as CastInfo;

                if (ci == null || !ci.IsAdditionalRow)
                {
                    row.Cells[ColumnNames.BirthYear].Style.BackColor = firstRow.Cells[ColumnNames.BirthYear].Style.BackColor;
                }
            }
        }

        private static int TryGetBirthYearsInTasks(DataGridView dataGridView, Dictionary<string, List<DataGridViewRow>> groupedBirthYears, List<string> keys, Dictionary<string, PersonInfo> persons, DefaultValues defaultValues, Log log, bool isCast, Action<MessageEntry> addMessage, int maxCount, ref int keyIndex)
        {
            var maxTasks = (keyIndex + IMDbParser.MaxTasks - 1) < maxCount
                ? IMDbParser.MaxTasks
                : maxCount - keyIndex;

            var tasks = new List<Task<List<IAsyncResult>>>(maxTasks);

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, keyIndex++)
            {
                var key = keys[keyIndex];

                var row = groupedBirthYears[key].First();

                var task = Task.Run(() => BirthYearGetter.GetBirthYear(persons, defaultValues, log, isCast, addMessage, row));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                foreach (var invokeResult in task.Result)
                {
                    dataGridView.EndInvoke(invokeResult);
                }
            }

            return maxTasks;
        }

        #endregion

        #region GetHeadshots

        public static void GetHeadshots(DataGridView dataGridView, bool useFakeBirthYears, bool isCast, Action<MessageEntry> addMessage, SetProgress setProgress)
        {
            _dataFillMode = true;

            var maxCount = dataGridView.Rows.Count;

            var processedItems = new HashSet<string>();

            for (var rowIndex = 0; rowIndex < maxCount;)
            {
                var progress = 0;

                try
                {
                    progress = TryGetHeadshotsInTask(dataGridView, useFakeBirthYears, isCast, addMessage, maxCount, processedItems, ref rowIndex);
                }
                catch (AggregateException aggrEx)
                {
                    var ex = aggrEx.InnerExceptions.First();

                    throw ex;
                }

                for (var current = 0; current < progress; current++)
                {
                    setProgress();
                }
            }

            _dataFillMode = false;
        }

        private static int TryGetHeadshotsInTask(DataGridView dataGridView, bool useFakeBirthYears, bool isCast, Action<MessageEntry> addMessage, int maxCount, HashSet<string> processedItems, ref int rowIndex)
        {
            var maxTasks = (rowIndex + IMDbParser.MaxTasks - 1) < maxCount
                ? IMDbParser.MaxTasks
                : maxCount - rowIndex;

            var tasks = new List<Task>(maxTasks);

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, rowIndex++)
            {
                var row = dataGridView.Rows[rowIndex];

                var person = (PersonInfo)row.Tag;

                if (processedItems.Add(person.PersonLink))
                {
                    var task = Task.Run(() => HeadshotGetter.Get(useFakeBirthYears, isCast, addMessage, person));

                    tasks.Add(task);
                }
            }

            Task.WaitAll(tasks.ToArray());

            return maxTasks;
        }

        #endregion

        private static void ShowBirthYearMessageBox(Log log, PersonInfo person, bool isCast, Action<MessageEntry> addMessage)
        {
            string text;
            string logText;
            if (isCast)
            {
                text = string.Format(MessageBoxTexts.BirthYearCastHasChanged, person.FormatPersonNameWithoutMarkers(), person.FormatActorNameWithBirthYearWithMarkers(true), person.FormatActorNameWithBirthYearWithMarkers(false), person.PersonLink);

                logText = string.Format(MessageBoxTexts.BirthYearCastHasChanged, person.FormatPersonNameWithoutMarkers(), person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, null), person.FormatActorNameWithBirthYearWithMarkersAsHtml(false, null), CreatePersonLinkHtml(person));
            }
            else
            {
                text = string.Format(MessageBoxTexts.BirthYearCrewHasChanged, person.FormatPersonNameWithoutMarkers(), person.FormatActorNameWithBirthYearWithMarkers(true), person.FormatActorNameWithBirthYearWithMarkers(false), person.PersonLink);

                logText = string.Format(MessageBoxTexts.BirthYearCrewHasChanged, person.FormatPersonNameWithoutMarkers(), person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, null), person.FormatActorNameWithBirthYearWithMarkersAsHtml(false, null), CreatePersonLinkHtml(person));
            }

            log.AppendParagraph(logText);

            addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
        }

        public static void CreateCastColumns(DataGridView dataGridView)
        {
            var firstNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.FirstName,
                HeaderText = DataGridViewTexts.FirstName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var middleNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.MiddleName,
                HeaderText = DataGridViewTexts.MiddleName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var lastNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.LastName,
                HeaderText = DataGridViewTexts.LastName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var birthYearDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.BirthYear,
                HeaderText = DataGridViewTexts.BirthYear,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var roleDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.Role,
                HeaderText = DataGridViewTexts.Role,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var voiceDataGridViewTextBoxColumn = new DataGridViewCheckBoxColumn()
            {
                Name = ColumnNames.Voice,
                HeaderText = DataGridViewTexts.Voice,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var uncreditedDataGridViewTextBoxColumn = new DataGridViewCheckBoxColumn()
            {
                Name = ColumnNames.Uncredited,
                HeaderText = DataGridViewTexts.Uncredited,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var creditedAsDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.CreditedAs,
                HeaderText = DataGridViewTexts.CreditedAs,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var linkDataGridViewLinkColumn = new DataGridViewLinkColumn()
            {
                Name = ColumnNames.Link,
                HeaderText = DataGridViewTexts.Link,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var moveUpColumn = new DataGridViewDisableButtonColumn()
            {
                Name = ColumnNames.MoveUp,
                HeaderText = DataGridViewTexts.MoveUp,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                Text = ColumnNames.MoveUp,
                UseColumnTextForButtonValue = true,
            };

            var moveDownColumn = new DataGridViewDisableButtonColumn()
            {
                Name = ColumnNames.MoveDown,
                HeaderText = DataGridViewTexts.MoveDown,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                Text = ColumnNames.MoveDown,
                UseColumnTextForButtonValue = true,
            };

            var removeRowColumn = new DataGridViewDisableButtonColumn()
            {
                Name = ColumnNames.RemoveRow,
                HeaderText = DataGridViewTexts.RemoveRow,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                Text = ColumnNames.RemoveRow,
                UseColumnTextForButtonValue = true,
            };

            var originalCreditDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.OriginalCredit,
                HeaderText = DataGridViewTexts.OriginalCredit,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            dataGridView.Columns.AddRange(firstNameDataGridViewTextBoxColumn, middleNameDataGridViewTextBoxColumn, lastNameDataGridViewTextBoxColumn, birthYearDataGridViewTextBoxColumn, roleDataGridViewTextBoxColumn, voiceDataGridViewTextBoxColumn, uncreditedDataGridViewTextBoxColumn, creditedAsDataGridViewTextBoxColumn, linkDataGridViewLinkColumn, moveUpColumn, moveDownColumn, removeRowColumn, originalCreditDataGridViewTextBoxColumn);
        }

        public static void CreateCrewColumns(DataGridView dataGridView)
        {
            var firstNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.FirstName,
                HeaderText = DataGridViewTexts.FirstName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var middleNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.MiddleName,
                HeaderText = DataGridViewTexts.MiddleName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var lastNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.LastName,
                HeaderText = DataGridViewTexts.LastName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var birthyearDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.BirthYear,
                HeaderText = DataGridViewTexts.BirthYear,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var creditTypeDataGridViewComboBoxBoxColumn = new DataGridViewComboBoxColumn()
            {
                Name = ColumnNames.CreditType,
                HeaderText = DataGridViewTexts.CreditType,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Direction);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Writing);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Production);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Cinematography);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.FilmEditing);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Music);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Sound);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Art);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Other);

            var creditSubtypeDataGridViewComboBoxColumn = new DataGridViewComboBoxColumn()
            {
                Name = ColumnNames.CreditSubtype,
                HeaderText = DataGridViewTexts.CreditSubtype,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var customRoleDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.CustomRole,
                HeaderText = DataGridViewTexts.CustomRole,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var creditedasDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.CreditedAs,
                HeaderText = DataGridViewTexts.CreditedAs,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var linkDataGridViewLinkColumn = new DataGridViewLinkColumn()
            {
                Name = ColumnNames.Link,
                HeaderText = DataGridViewTexts.Link,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var originalCreditDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = ColumnNames.OriginalCredit,
                HeaderText = DataGridViewTexts.OriginalCredit,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            dataGridView.Columns.AddRange(firstNameDataGridViewTextBoxColumn, middleNameDataGridViewTextBoxColumn, lastNameDataGridViewTextBoxColumn, birthyearDataGridViewTextBoxColumn, creditTypeDataGridViewComboBoxBoxColumn, creditSubtypeDataGridViewComboBoxColumn, customRoleDataGridViewTextBoxColumn, creditedasDataGridViewTextBoxColumn, linkDataGridViewLinkColumn, originalCreditDataGridViewTextBoxColumn);
        }

        public static string CopyCastToClipboard(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage, bool embedded)
        {
            var ci = new CastInformation();

            CreateCastMember(dataGridView, title, log, useFakeBirthYears, addMessage, embedded, ci, (row) => new CastMember(), (row) => new Divider());

            try
            {
                var xml = Utilities.CopyCastInformationToClipboard(ci, embedded);

                Program.AdapterEventHandler.RaiseCastCompleted(xml);

                return xml;
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }
        }

        public static void CopyExtendedCastToClipboard(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage)
        {
            Func<DataGridViewRow, CastMember> createCastMember = (row) => new ExtendedCastMember()
            {
                ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString(),
            };

            Func<DataGridViewRow, Divider> createCastDivider = (row) => new ExtendedCastDivider()
            {
                ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString(),
            };

            var ci = new ExtendedCastInformation();

            if (dataGridView.Rows.Count > 0)
            {
                ci.ImdbLink = dataGridView.Rows[0].Cells[ColumnNames.Link].Value?.ToString();
            }

            CreateCastMember(dataGridView, title, log, useFakeBirthYears, addMessage, false, ci, createCastMember, createCastDivider);

            try
            {
                CopyExtendedCastInformationToClipboard(ci);
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CopyExtendedCastInformationToClipboard(ExtendedCastInformation castInformation)
        {
            var addTypes = new[] { typeof(ExtendedCastMember), typeof(ExtendedCastDivider) };

            var serializer = new ExtendedSerializer<ExtendedCastInformation>(addTypes, CastInformation.DefaultEncoding);

            var xml = serializer.ToString(castInformation);

            Clipboard.SetDataObject(xml, true, 4, 250);
        }

        private static void CreateCastMember(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage, bool embedded, CastInformation ci, Func<DataGridViewRow, CastMember> createCastMember, Func<DataGridViewRow, Divider> createCastDivider)
        {
            var offset = 0;

            if (dataGridView.Rows.Count > 0)
            {
                var value = dataGridView.Rows[0].Cells[ColumnNames.FirstName].Value;

                if (value != null && value.ToString() == FirstNames.Title)
                {
                    value = dataGridView.Rows[0].Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        ci.Title = value.ToString();
                    }
                    else
                    {
                        ci.Title = title;
                    }

                    offset = -1;
                }
                else
                {
                    ci.Title = title;

                    offset = 0;
                }
            }

            ci.CastList = new object[dataGridView.Rows.Count + offset];

            offset = 0;

            for (var rowIndex = 0; rowIndex < dataGridView.Rows.Count; rowIndex++)
            {
                var row = dataGridView.Rows[rowIndex];

                var value = row.Cells[ColumnNames.FirstName].Value;

                if (value != null && value.ToString() == FirstNames.Title)
                {
                    offset = -1;
                }
                else if (value != null && value.ToString() == FirstNames.Divider)
                {
                    row = dataGridView.Rows[rowIndex];

                    var divider = createCastDivider(row);

                    ci.CastList[rowIndex + offset] = divider;

                    var name = string.Empty;

                    value = row.Cells[ColumnNames.MiddleName].Value;

                    if (value != null)
                    {
                        name = value.ToString().Trim() + " ";
                    }

                    value = row.Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        name += value.ToString().Trim();
                    }

                    divider.Caption = name.Trim();
                    divider.Type = DividerType.Episode;
                }
                else
                {
                    var castMember = createCastMember(row);

                    ci.CastList[rowIndex + offset] = castMember;

                    if (value != null)
                    {
                        castMember.FirstName = value.ToString();
                    }

                    row = dataGridView.Rows[rowIndex];

                    value = row.Cells[ColumnNames.MiddleName].Value;

                    if (value != null)
                    {
                        castMember.MiddleName = value.ToString();
                    }

                    value = row.Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        castMember.LastName = value.ToString();
                    }

                    value = row.Cells[ColumnNames.BirthYear].Value;

                    if (value != null)
                    {
                        if (int.TryParse(value.ToString(), out var intValue))
                        {
                            castMember.BirthYear = intValue;
                        }
                    }

                    value = row.Cells[ColumnNames.Role].Value;

                    if (value != null)
                    {
                        castMember.Role = value.ToString();
                    }

                    value = row.Cells[ColumnNames.Voice].Value;

                    if (value != null)
                    {
                        castMember.Voice = bool.Parse(value.ToString());
                    }

                    value = row.Cells[ColumnNames.Uncredited].Value;

                    if (value != null)
                    {
                        castMember.Uncredited = bool.Parse(value.ToString());
                    }

                    value = row.Cells[ColumnNames.CreditedAs].Value;

                    if (value != null)
                    {
                        castMember.CreditedAs = value.ToString();
                    }

                    if (!embedded)
                    {
                        CheckForPossibleDuplicates(log, row, _confirmedPossibleCastDuplicates, Program.PossibleCastDuplicateCache, DataGridViewTexts.Cast, useFakeBirthYears, castMember, Program.CastCache, true, addMessage);
                    }
                }
            }
        }

        private static void CheckForPossibleDuplicates(Log log, DataGridViewRow row, Dictionary<PersonInfoWithoutBirthYear, bool> confirmedPossibleDuplicates, Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> possibleDuplicatesCache, string type, bool useFakeBirthYears, IPerson clipboardPerson, Dictionary<string, PersonInfo> personCache, bool isCast, Action<MessageEntry> addMessage)
        {
            var piwby = new PersonInfoWithoutBirthYear(row, type);

            var rowPerson = new PersonInfo(row, type);

            if (!confirmedPossibleDuplicates.ContainsKey(piwby))
            {
                var list = possibleDuplicatesCache[piwby];

                var newFakeBirthYears = new List<PersonInfo>(list.Count);

                if (list.Count > 1)
                {
                    var birthYears = new Dictionary<string, List<PersonInfo>>(list.Count);

                    var hasBlankEntries = false;

                    foreach (var item in list)
                    {
                        var cachePerson = personCache[item.PersonLink];

                        if (!string.IsNullOrEmpty(cachePerson.BirthYear))
                        {
                            item.BirthYear = cachePerson.BirthYear;

                            ConsolidateBirthYears(birthYears, item, item.BirthYear);

                            if (rowPerson.PersonLink == item.PersonLink)
                            {
                                clipboardPerson.BirthYear = int.Parse(item.BirthYear);
                            }
                        }
                        else
                        {
                            if (useFakeBirthYears)
                            {
                                var fakeBirthYear = CreateFakeBirthYear(clipboardPerson, newFakeBirthYears, item, cachePerson, rowPerson, isCast, addMessage);

                                ConsolidateBirthYears(birthYears, item, fakeBirthYear);
                            }
                            else
                            {
                                hasBlankEntries = true;

                                ConsolidateBirthYears(birthYears, item, string.Empty);

                                if (!string.IsNullOrEmpty(item.FakeBirthYear) && rowPerson.PersonLink == item.PersonLink)
                                {
                                    ShowBirthYearMessageBox(log, item, isCast, addMessage);
                                }
                            }
                        }
                    }

                    if (newFakeBirthYears.Count > 1)
                    {
                        ShowMultipleNamesMessageBox(log, newFakeBirthYears, MessageBoxTexts.NewFakeBirthYears, addMessage, false);
                    }

                    foreach (var kvp in birthYears)
                    {
                        if (kvp.Value.Count > 1)
                        {
                            ShowDuplicatesMessageBox(log, confirmedPossibleDuplicates, piwby, kvp.Value, MessageBoxTexts.PossibleSameYearDuplicates, addMessage, Program.DefaultValues.DisableDuplicatesMessageBox);
                        }
                    }

                    if (hasBlankEntries && birthYears.Count > 1)
                    {
                        ShowDuplicatesMessageBox(log, confirmedPossibleDuplicates, piwby, list, MessageBoxTexts.PossibleDuplicates, addMessage, false);
                    }
                }
                else if (useFakeBirthYears)
                {
                    var cachePerson = personCache[list[0].PersonLink];

                    CreateFakeBirthYear(clipboardPerson, newFakeBirthYears, list[0], cachePerson, rowPerson, isCast, addMessage);
                }
                else if (!string.IsNullOrEmpty(list[0].FakeBirthYear))
                {
                    ShowBirthYearMessageBox(log, list[0], isCast, addMessage);
                }
            }
        }

        private static string CreateFakeBirthYear(IPerson clipboardPerson, List<PersonInfo> newFakeBirthYears, PersonInfo item, PersonInfo cachePerson, PersonInfo rowPerson, bool isCast, Action<MessageEntry> addMessage)
        {
            var fakeBirthYear = string.Empty;

            if (!string.IsNullOrEmpty(cachePerson.BirthYear)) //If we already have a BY, why not use it
            {
                fakeBirthYear = cachePerson.BirthYear;
            }
            else if (string.IsNullOrEmpty(cachePerson.FakeBirthYear))
            {
                fakeBirthYear = CreateFakeBirthYearAsString(cachePerson, isCast, addMessage);

                item.FakeBirthYear = fakeBirthYear;

                cachePerson.FakeBirthYear = item.FakeBirthYear;

                newFakeBirthYears.Add(item);
            }
            else
            {
                fakeBirthYear = cachePerson.FakeBirthYear;
            }

            if (rowPerson.PersonLink == cachePerson.PersonLink)
            {
                if (!string.IsNullOrEmpty(fakeBirthYear))
                {
                    clipboardPerson.BirthYear = int.Parse(fakeBirthYear);
                }
            }

            return fakeBirthYear;
        }

        private static int CreateFakeBirthYearAsInt(PersonInfo item)
        {
            var fakeBirthYearString = item.PersonLink.Substring(item.PersonLink.Length - 5);

            var fakeBirthYear = int.Parse(fakeBirthYearString);

            if (fakeBirthYear > ushort.MaxValue)
            {
                fakeBirthYearString = item.PersonLink.Substring(item.PersonLink.Length - 4);

                fakeBirthYear = int.Parse(fakeBirthYearString);
            }

            return fakeBirthYear;
        }

        internal static string CreateFakeBirthYearAsString(PersonInfo item, bool isCast, Action<MessageEntry> addMessage)
        {
            var fakeBirthYearAsInt = CreateFakeBirthYearAsInt(item);

            string fakeBirthYearAsString;
            if (fakeBirthYearAsInt == 0)
            {
                fakeBirthYearAsString = string.Empty;

                string text;
                if (isCast)
                {
                    text = string.Format(MessageBoxTexts.FakeBirthYearZeroCast, item.PersonLink, item.FormatActorNameWithBirthYearWithMarkers(true));
                }
                else
                {
                    text = string.Format(MessageBoxTexts.FakeBirthYearZeroCrew, item.PersonLink, item.FormatActorNameWithBirthYearWithMarkers(true));
                }

                addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
            }
            else
            {
                fakeBirthYearAsString = fakeBirthYearAsInt.ToString();
            }

            return fakeBirthYearAsString;
        }

        private static void ShowDuplicatesMessageBox(Log log, Dictionary<PersonInfoWithoutBirthYear, bool> confirmedPossibleDuplicates, PersonInfoWithoutBirthYear piwby, List<PersonInfo> list, string messageId, Action<MessageEntry> addMessage, bool ignoreMesssageBox)
        {
            ShowMultipleNamesMessageBox(log, list, messageId, addMessage, ignoreMesssageBox);

            if (!confirmedPossibleDuplicates.ContainsKey(piwby))
            {
                confirmedPossibleDuplicates.Add(piwby, true);
            }
        }

        private static void ShowMultipleNamesMessageBox(Log log, List<PersonInfo> list, string messageId, Action<MessageEntry> addMessage, bool ignoreMesssageBox)
        {
            var nameList = new StringBuilder();

            var nameListLog = new StringBuilder();

            var useFakeBirthNames = Program.DefaultValues.UseFakeBirthYears;

            foreach (var item in list)
            {
                nameList.Append(item.PersonLink);
                nameList.Append(": ");
                nameList.AppendLine(item.FormatActorNameWithBirthYearWithMarkers(useFakeBirthNames, true));

                nameListLog.Append(CreatePersonLinkHtml(item));
                nameListLog.Append(": ");
                nameListLog.AppendLine(item.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthNames, true, list));
            }

            var messageText = string.Format(messageId, nameList.ToString());

            var logText = string.Format(messageId, nameListLog.ToString());

            log.AppendParagraph(logText);

            if (!ignoreMesssageBox)
            {
                addMessage(new MessageEntry(messageText, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
            }
        }

        internal static string CreatePersonLinkHtml(PersonInfo person)
        {
            var sb = new StringBuilder("<a href=\"https://www.imdb.com/name/");

            sb.Append(person.PersonLink);
            sb.Append("/\">");
            sb.Append(person.PersonLink);
            sb.Append("</a>");

            return sb.ToString();
        }

        private static void ConsolidateBirthYears(Dictionary<string, List<PersonInfo>> birthYears, PersonInfo item, string birthYear)
        {
            if (birthYears.TryGetValue(birthYear, out var sameYearList))
            {
                sameYearList.Add(item);
            }
            else
            {
                sameYearList = new List<PersonInfo>(4)
                {
                    item,
                };

                birthYears.Add(birthYear, sameYearList);
            }
        }

        public static string CopyCrewToClipboard(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage, bool embedded)
        {
            var ci = new CrewInformation();

            CreateCrewMember(dataGridView, title, log, useFakeBirthYears, addMessage, embedded, ci, (row) => new CrewMember(), (row) => new CrewDivider());

            try
            {
                var xml = Utilities.CopyCrewInformationToClipboard(ci, embedded);

                Program.AdapterEventHandler.RaiseCrewCompleted(xml);

                return xml;
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }
        }

        public static void CopyExtendedCrewToClipboard(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage)
        {
            Func<DataGridViewRow, CrewMember> createCrewMember = (row) => new ExtendedCrewMember()
            {
                ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString(),
            };

            Func<DataGridViewRow, CrewDivider> createCrewDivider = (row) => new ExtendedCrewDivider()
            {
                ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString(),
            };

            var ci = new ExtendedCrewInformation();

            if (dataGridView.Rows.Count > 0)
            {
                ci.ImdbLink = dataGridView.Rows[0].Cells[ColumnNames.Link].Value?.ToString();
            }

            CreateCrewMember(dataGridView, title, log, useFakeBirthYears, addMessage, false, ci, createCrewMember, createCrewDivider);

            try
            {
                CopyExtendedCrewToClipboard(ci);
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CopyExtendedCrewToClipboard(ExtendedCrewInformation crewInformation)
        {
            crewInformation = ExtendedCrewSorter.GetSortedCrew(crewInformation);

            var addTypes = new[] { typeof(ExtendedCrewMember), typeof(ExtendedCrewDivider) };

            var serializer = new ExtendedSerializer<ExtendedCrewInformation>(addTypes, CrewInformation.DefaultEncoding);

            var xml = serializer.ToString(crewInformation);

            Clipboard.SetDataObject(xml, true, 4, 250);
        }

        private static void CreateCrewMember(DataGridView dataGridView, string title, Log log, bool useFakeBirthYears, Action<MessageEntry> addMessage, bool embedded, CrewInformation ci, Func<DataGridViewRow, CrewMember> createCrewMember, Func<DataGridViewRow, CrewDivider> createCrewDivider)
        {
            var offset = 0;

            if (dataGridView.Rows.Count > 0)
            {
                var value = dataGridView.Rows[0].Cells[ColumnNames.FirstName].Value;

                if (value != null && value.ToString() == FirstNames.Title)
                {
                    value = dataGridView.Rows[0].Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        ci.Title = value.ToString();
                    }
                    else
                    {
                        ci.Title = title;
                    }

                    offset = -1;
                }
                else
                {
                    ci.Title = title;

                    offset = 0;
                }
            }

            ci.CrewList = new object[dataGridView.Rows.Count + offset];

            offset = 0;

            for (var rowIndex = 0; rowIndex < dataGridView.Rows.Count; rowIndex++)
            {
                var row = dataGridView.Rows[rowIndex];

                var value = row.Cells[ColumnNames.FirstName].Value;

                if (value != null && value.ToString() == FirstNames.Title)
                {
                    offset = -1;
                }
                else if (value != null && value.ToString() == FirstNames.Divider)
                {
                    var divider = createCrewDivider(row);

                    ci.CrewList[rowIndex + offset] = divider;

                    var name = string.Empty;

                    value = row.Cells[ColumnNames.MiddleName].Value;

                    if (value != null)
                    {
                        name = value.ToString().Trim() + " ";
                    }

                    value = row.Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        name += value.ToString().Trim();
                    }

                    divider.Caption = name.Trim();
                    divider.Type = DividerType.Episode;
                }
                else if (value != null && value.ToString() == FirstNames.GroupDividerStart)
                {
                    var divider = createCrewDivider(row);

                    ci.CrewList[rowIndex + offset] = divider;

                    var name = string.Empty;

                    value = row.Cells[ColumnNames.MiddleName].Value;

                    if (value != null)
                    {
                        name = value.ToString().Trim() + " ";
                    }

                    divider.Caption = name.Trim();

                    value = row.Cells[ColumnNames.CreditType].Value;

                    if (value != null)
                    {
                        divider.CreditType = value.ToString();
                    }

                    divider.Type = DividerType.Group;
                }
                else if (value != null && value.ToString() == FirstNames.GroupDividerEnd)
                {
                    var divider = createCrewDivider(row);

                    ci.CrewList[rowIndex + offset] = divider;

                    value = row.Cells[ColumnNames.CreditType].Value;

                    if (value != null)
                    {
                        divider.CreditType = value.ToString();
                    }

                    divider.Type = DividerType.EndDiv;
                }
                else
                {
                    var crewMember = createCrewMember(row);

                    ci.CrewList[rowIndex + offset] = crewMember;

                    if (value != null)
                    {
                        crewMember.FirstName = value.ToString();
                    }

                    value = row.Cells[ColumnNames.MiddleName].Value;

                    if (value != null)
                    {
                        crewMember.MiddleName = value.ToString();
                    }

                    value = row.Cells[ColumnNames.LastName].Value;

                    if (value != null)
                    {
                        crewMember.LastName = value.ToString();
                    }

                    value = row.Cells[ColumnNames.BirthYear].Value;

                    if (value != null)
                    {
                        if (int.TryParse(value.ToString(), out var intValue))
                        {
                            crewMember.BirthYear = intValue;
                        }
                    }

                    value = row.Cells[ColumnNames.CreditType].Value;

                    if (value != null)
                    {
                        crewMember.CreditType = value.ToString();
                    }

                    value = row.Cells[ColumnNames.CreditSubtype].Value;

                    if (value != null)
                    {
                        crewMember.CreditSubtype = value.ToString();
                    }

                    value = row.Cells[ColumnNames.CustomRole].Value;

                    if (value != null)
                    {
                        crewMember.CustomRole = value.ToString();

                        if (!string.IsNullOrEmpty(crewMember.CustomRole))
                        {
                            crewMember.CustomRoleSpecified = true;
                        }
                    }

                    value = row.Cells[ColumnNames.CreditedAs].Value;

                    if (value != null)
                    {
                        crewMember.CreditedAs = value.ToString();
                    }

                    if (!embedded)
                    {
                        CheckForPossibleDuplicates(log, row, _confirmedPossibleCrewDuplicates, Program.PossibleCrewDuplicateCache, DataGridViewTexts.Crew, useFakeBirthYears, crewMember, Program.CrewCache, false, addMessage);
                    }
                }
            }
        }

        public static void FillCastRows(DataGridView dataGridView, List<CastInfo> castList, bool isFirstDivider, bool isLastDivider)
        {
            _dataFillMode = true;

            var offset = 0;

            var rows = new List<DataGridViewRow>(castList.Count + castList.Count / 10);

            for (var castIndex = 0; castIndex < castList.Count; castIndex++)
            {
                var castMember = castList[castIndex];

                var row = new DataGridViewRow();

                for (var columnIndex = 0; columnIndex < dataGridView.Columns.Count; columnIndex++)
                {
                    var column = dataGridView.Columns[columnIndex];

                    var cell = (DataGridViewCell)column.CellTemplate.Clone();

                    row.Cells.Add(cell);
                }

                rows.Add(row);

                if (castMember.FirstName == FirstNames.Title)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCyan;
                    row.ReadOnly = true;
                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = false;

                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveUp].Index])).Enabled = false;
                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveDown].Index])).Enabled = false;
                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.RemoveRow].Index])).Enabled = false;

                    offset++;
                }
                else if (castMember.FirstName == FirstNames.Divider)
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.ReadOnly = true;

                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = false;
                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = false;

                    if (isFirstDivider)
                    {
                        ((DataGridViewDisableButtonCell)row.Cells[dataGridView.Columns[ColumnNames.MoveUp].Index]).Enabled = false;
                    }
                    if (isLastDivider)
                    {
                        ((DataGridViewDisableButtonCell)row.Cells[dataGridView.Columns[ColumnNames.MoveDown].Index]).Enabled = false;
                    }

                    ((DataGridViewDisableButtonCell)row.Cells[dataGridView.Columns[ColumnNames.RemoveRow].Index]).Enabled = false;
                }
                else
                {
                    if (castMember.IsAdditionalRow)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White;

                        if (!castMember.BirthYearWasRetrieved)
                        {
                            row.Cells[dataGridView.Columns[ColumnNames.BirthYear].Index].Style.BackColor = Color.LightGray;
                        }
                    }

                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = true;
                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = true;

                    ((DataGridViewDisableButtonCell)row.Cells[dataGridView.Columns[ColumnNames.MoveUp].Index]).Enabled = (castIndex != offset);
                    ((DataGridViewDisableButtonCell)row.Cells[dataGridView.Columns[ColumnNames.MoveDown].Index]).Enabled = (castIndex != castList.Count - 1);
                }

                row.Cells[dataGridView.Columns[ColumnNames.FirstName].Index].Value = castMember.FirstName;
                row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].Value = castMember.MiddleName;
                row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].Value = castMember.LastName;
                row.Cells[dataGridView.Columns[ColumnNames.BirthYear].Index].Value = castMember.BirthYear;
                row.Cells[dataGridView.Columns[ColumnNames.Role].Index].Value = castMember.Role;
                row.Cells[dataGridView.Columns[ColumnNames.Voice].Index].Value = castMember.Voice;
                row.Cells[dataGridView.Columns[ColumnNames.Uncredited].Index].Value = castMember.Uncredited;
                row.Cells[dataGridView.Columns[ColumnNames.CreditedAs].Index].Value = castMember.CreditedAs;
                row.Cells[dataGridView.Columns[ColumnNames.Link].Index].Value = castMember.PersonLink;
                row.Cells[dataGridView.Columns[ColumnNames.OriginalCredit].Index].Value = castMember.OriginalCredit;

                row.Tag = castMember;
            }
            dataGridView.Rows.AddRange(rows.ToArray());

            _dataFillMode = false;

            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public static void FillCrewRows(DataGridView dataGridView, List<CrewInfo> crewList)
        {
            var rows = new List<DataGridViewRow>(crewList.Count + crewList.Count / 10);

            foreach (var crewMember in crewList)
            {
                var row = new DataGridViewRow();

                for (var columnIndex = 0; columnIndex < dataGridView.Columns.Count; columnIndex++)
                {
                    var column = dataGridView.Columns[columnIndex];

                    var cell = (DataGridViewCell)column.CellTemplate.Clone();

                    row.Cells.Add(cell);
                }

                rows.Add(row);

                if (crewMember.FirstName == FirstNames.Title)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCyan;
                    row.ReadOnly = true;

                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = false;
                }
                else if (crewMember.FirstName == FirstNames.Divider)
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.ReadOnly = true;

                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = false;
                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = false;
                }
                else if (crewMember.FirstName == FirstNames.GroupDividerStart)
                {
                    row.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    row.ReadOnly = true;

                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = false;
                }
                else if (crewMember.FirstName == FirstNames.GroupDividerEnd)
                {
                    row.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    row.ReadOnly = true;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;

                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = true;
                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = true;
                    row.Cells[dataGridView.Columns[ColumnNames.BirthYear].Index].Style.BackColor = Color.LightGray;
                }

                row.Cells[dataGridView.Columns[ColumnNames.FirstName].Index].Value = crewMember.FirstName;
                row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].Value = crewMember.MiddleName;
                row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].Value = crewMember.LastName;
                row.Cells[dataGridView.Columns[ColumnNames.CreditType].Index].Value = crewMember.CreditType;

                FillCreditSubtypeCell(dataGridView, row);

                row.Cells[dataGridView.Columns[ColumnNames.CreditSubtype].Index].Value = crewMember.CreditSubtype;
                row.Cells[dataGridView.Columns[ColumnNames.CustomRole].Index].Value = crewMember.CustomRole;
                row.Cells[dataGridView.Columns[ColumnNames.CreditedAs].Index].Value = crewMember.CreditedAs;
                row.Cells[dataGridView.Columns[ColumnNames.Link].Index].Value = crewMember.PersonLink;
                row.Cells[dataGridView.Columns[ColumnNames.OriginalCredit].Index].Value = crewMember.OriginalCredit;

                row.Tag = crewMember;
            }

            dataGridView.Rows.AddRange(rows.ToArray());
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public static void OnCastDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_dataFillMode)
            {
                var row = ((DataGridView)sender).Rows[e.RowIndex];

                var castMember = (CastInfo)row.Tag;

                if (e.ColumnIndex == 1)//middle name for episode number
                {
                    castMember.MiddleName = (string)(row.Cells[ColumnNames.MiddleName].Value);
                }
                else if (e.ColumnIndex == 2)//last name for title
                {
                    castMember.LastName = (string)(row.Cells[ColumnNames.LastName].Value);
                }
                else if (e.ColumnIndex == 4)//role
                {
                    castMember.Role = (string)(row.Cells[ColumnNames.Role].Value);
                }
                else if (e.ColumnIndex == 5)//voice
                {
                    castMember.Voice = row.Cells[ColumnNames.Voice].Value.ToString();
                }
                else if (e.ColumnIndex == 6)//uncredited
                {
                    castMember.Uncredited = row.Cells[ColumnNames.Uncredited].Value.ToString();
                }
                else if (e.ColumnIndex == 7)//credited as
                {
                    castMember.CreditedAs = (string)(row.Cells[ColumnNames.CreditedAs].Value);
                }
            }
        }

        public static void OnCrewDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_dataFillMode)
            {
                if (e.ColumnIndex == 4)
                {
                    var dataGridView = (DataGridView)sender;

                    var row = dataGridView.Rows[e.RowIndex];

                    FillCreditSubtypeCell(dataGridView, row);
                }
            }
        }

        private static void FillCreditSubtypeCell(DataGridView dataGridView, DataGridViewRow row)
        {
            var creditTypeCell = (DataGridViewComboBoxCell)(row.Cells[dataGridView.Columns[ColumnNames.CreditType].Index]);

            if (creditTypeCell.Value != null)
            {
                var creditSubtypeCell = (DataGridViewComboBoxCell)(row.Cells[dataGridView.Columns[ColumnNames.CreditSubtype].Index]);

                creditSubtypeCell.Value = null;

                CreditTypesDataGridViewHelper.FillCreditSubtypes(creditTypeCell.Value.ToString(), creditSubtypeCell.Items);

                creditSubtypeCell.Value = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
            }
        }

        #region Nested Classes

        private class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
        {
            public DataGridViewDisableButtonColumn()
            {
                CellTemplate = new DataGridViewDisableButtonCell();
            }
        }

        internal class DataGridViewDisableButtonCell : DataGridViewButtonCell
        {
            public bool Enabled { get; set; }

            // By default, enable the button cell.
            public DataGridViewDisableButtonCell()
            {
                Enabled = true;
            }

            // Override the Clone method so that the Enabled property is copied.
            public override object Clone()
            {
                var cell = (DataGridViewDisableButtonCell)(base.Clone());

                cell.Enabled = Enabled;

                return cell;
            }

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                // The button cell is disabled, so paint the border,  
                // background, and disabled button for the cell.
                if (!Enabled)
                {
                    // Draw the cell background, if specified.
                    if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                    {
                        var cellBackground = new SolidBrush(cellStyle.BackColor);

                        graphics.FillRectangle(cellBackground, cellBounds);

                        cellBackground.Dispose();
                    }

                    // Draw the cell borders, if specified.
                    if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                    {
                        PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                    }

                    // Calculate the area in which to draw the button.
                    var buttonArea = cellBounds;

                    var buttonAdjustment = BorderWidths(advancedBorderStyle);

                    buttonArea.X += buttonAdjustment.X;
                    buttonArea.Y += buttonAdjustment.Y;
                    buttonArea.Height -= buttonAdjustment.Height;
                    buttonArea.Width -= buttonAdjustment.Width;

                    // Draw the disabled button.
                    ButtonRenderer.DrawButton(graphics, buttonArea, PushButtonState.Disabled);

                    // Draw the disabled button text. 
                    if (FormattedValue is string)
                    {
                        TextRenderer.DrawText(graphics, (string)(FormattedValue), DataGridView.Font, buttonArea, SystemColors.GrayText);
                    }
                }
                else
                {
                    // The button cell is enabled, so let the base class 
                    // handle the painting.
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                }
            }
        }

        #endregion
    }
}