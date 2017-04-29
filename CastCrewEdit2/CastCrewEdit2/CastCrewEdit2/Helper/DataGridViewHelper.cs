using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerXML;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version390;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class DataGridViewHelper
    {
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
            private Boolean EnabledValue;

            public Boolean Enabled
            {
                get
                {
                    return (EnabledValue);
                }
                set
                {
                    EnabledValue = value;
                }
            }

            // By default, enable the button cell.
            public DataGridViewDisableButtonCell()
            {
                EnabledValue = true;
            }

            // Override the Clone method so that the Enabled property is copied.
            public override Object Clone()
            {
                DataGridViewDisableButtonCell cell;

                cell = (DataGridViewDisableButtonCell)(base.Clone());
                cell.Enabled = Enabled;
                return (cell);
            }

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, Int32 rowIndex
                , DataGridViewElementStates elementState, Object value, Object formattedValue, String errorText
                , DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle
                , DataGridViewPaintParts paintParts)
            {
                // The button cell is disabled, so paint the border,  
                // background, and disabled button for the cell.
                if (EnabledValue == false)
                {
                    Rectangle buttonArea;
                    Rectangle buttonAdjustment;

                    // Draw the cell background, if specified.
                    if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                    {
                        SolidBrush cellBackground;

                        cellBackground = new SolidBrush(cellStyle.BackColor);
                        graphics.FillRectangle(cellBackground, cellBounds);
                        cellBackground.Dispose();
                    }

                    // Draw the cell borders, if specified.
                    if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                    {
                        PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                    }

                    // Calculate the area in which to draw the button.
                    buttonArea = cellBounds;
                    buttonAdjustment = BorderWidths(advancedBorderStyle);
                    buttonArea.X += buttonAdjustment.X;
                    buttonArea.Y += buttonAdjustment.Y;
                    buttonArea.Height -= buttonAdjustment.Height;
                    buttonArea.Width -= buttonAdjustment.Width;

                    // Draw the disabled button.                
                    ButtonRenderer.DrawButton(graphics, buttonArea, PushButtonState.Disabled);

                    // Draw the disabled button text. 
                    if (FormattedValue is String)
                    {
                        TextRenderer.DrawText(graphics, (String)(FormattedValue), DataGridView.Font
                            , buttonArea, SystemColors.GrayText);
                    }
                }
                else
                {
                    // The button cell is enabled, so let the base class 
                    // handle the painting.
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue
                        , errorText, cellStyle, advancedBorderStyle, paintParts);
                }
            }
        }
        #endregion

        private static Boolean DataFillMode;

        private static Dictionary<PersonInfoWithoutBirthYear, Boolean> ConfirmedPossibleCastDuplicates;

        private static Dictionary<PersonInfoWithoutBirthYear, Boolean> ConfirmedPossibleCrewDuplicates;

        static DataGridViewHelper()
        {
            DataFillMode = false;
            ConfirmedPossibleCastDuplicates = new Dictionary<PersonInfoWithoutBirthYear, Boolean>();
            ConfirmedPossibleCrewDuplicates = new Dictionary<PersonInfoWithoutBirthYear, Boolean>();
        }

        #region GetBirthYears

        public static void GetBirthYears(DataGridView dataGridView
            , Dictionary<String, PersonInfo> persons
            , DefaultValues defaultValues
            , Log log
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , SetProgress setProgress)
        {
            DataFillMode = true;

            Dictionary<String, List<DataGridViewRow>> groupedBirthYears = BirthYearGetter.GetGroupedBirthYears(dataGridView);

            Int32 maxCount = groupedBirthYears.Count;

            List<String> keys = groupedBirthYears.Keys.ToList();

            for (Int32 keyIndex = 0; keyIndex < maxCount;)
            {
                Int32 progress = 0;

                try
                {
                    progress = TryGetBirthYearsInTasks(dataGridView, groupedBirthYears, keys, persons, defaultValues, log
                        , isCast, addMessage, maxCount, ref keyIndex);
                }
                catch (AggregateException aggrEx)
                {
                    Exception ex = aggrEx.InnerExceptions.First();

                    throw (ex);
                }

                for (Int32 i = 0; i < progress; i++)
                {
                    setProgress();
                }
            }

            AdaptDuplicateBirthYearRows(groupedBirthYears, setProgress);

            DataFillMode = false;
        }

        private static void AdaptDuplicateBirthYearRows(Dictionary<String, List<DataGridViewRow>> groupedBirthYears
            , SetProgress setProgress)
        {
            foreach (List<DataGridViewRow> rowList in groupedBirthYears.Values)
            {
                if (rowList.Count > 1)
                {
                    AdaptDuplicateBirthYearRows(rowList, setProgress);
                }
            }
        }

        private static void AdaptDuplicateBirthYearRows(List<DataGridViewRow> rowList
            , SetProgress setProgress)
        {
            DataGridViewRow firstRow = rowList[0];

            for (Int32 rowIndex = 1; rowIndex < rowList.Count; rowIndex++)
            {
                DataGridViewRow row = rowList[rowIndex];

                AdaptDuplicateBirthYearRow(firstRow, row);

                setProgress();
            }
        }

        private static void AdaptDuplicateBirthYearRow(DataGridViewRow firstRow
            , DataGridViewRow row)
        {
            PersonInfo firstPerson = (PersonInfo)(firstRow.Tag);

            if (String.IsNullOrEmpty(firstPerson.PersonLink) == false)
            {
                PersonInfo person = (PersonInfo)(row.Tag);

                person.BirthYear = firstPerson.BirthYear;
                person.FakeBirthYear = firstPerson.FakeBirthYear;
                person.BirthYearWasRetrieved = firstPerson.BirthYearWasRetrieved;

                row.Cells[ColumnNames.BirthYear].Value = firstRow.Cells[ColumnNames.BirthYear].Value;

                CastInfo ci = person as CastInfo;

                if ((ci == null) || (ci.IsAdditionalRow == false))
                {
                    row.Cells[ColumnNames.BirthYear].Style.BackColor = firstRow.Cells[ColumnNames.BirthYear].Style.BackColor;
                }
            }
        }

        private static Int32 TryGetBirthYearsInTasks(DataGridView dataGridView
            , Dictionary<String, List<DataGridViewRow>> groupedBirthYears
            , List<String> keys
            , Dictionary<String, PersonInfo> persons
            , DefaultValues defaultValues
            , Log log
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , Int32 maxCount
            , ref Int32 keyIndex)
        {
            Int32 maxTasks = ((keyIndex + IMDbParser.MaxTasks - 1) < maxCount) ? IMDbParser.MaxTasks : (maxCount - keyIndex);

            List<Task<List<IAsyncResult>>> tasks = new List<Task<List<IAsyncResult>>>(maxTasks);

            for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, keyIndex++)
            {
                String key = keys[keyIndex];

                DataGridViewRow row = groupedBirthYears[key].First();

                Task<List<IAsyncResult>> task = Task.Run(() => BirthYearGetter.GetBirthYear(persons, defaultValues, log, isCast, addMessage, row));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<List<IAsyncResult>> task in tasks)
            {
                foreach (IAsyncResult invokeResult in task.Result)
                {
                    dataGridView.EndInvoke(invokeResult);
                }
            }

            return (maxTasks);
        }

        #endregion

        #region GetHeadshots

        public static void GetHeadshots(DataGridView dataGridView
            , Boolean useFakeBirthYears
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , SetProgress setProgress)
        {
            DataFillMode = true;

            Int32 maxCount = dataGridView.Rows.Count;

            HashSet<String> processedItems = new HashSet<String>();

            for (Int32 rowIndex = 0; rowIndex < maxCount;)
            {
                Int32 progress = 0;

                try
                {
                    progress = TryGetHeadshotsInTask(dataGridView, useFakeBirthYears, isCast, addMessage, maxCount, processedItems, ref rowIndex);
                }
                catch (AggregateException aggrEx)
                {
                    Exception ex = aggrEx.InnerExceptions.First();

                    throw (ex);
                }

                for (Int32 i = 0; i < progress; i++)
                {
                    setProgress();
                }
            }

            DataFillMode = false;
        }

        private static Int32 TryGetHeadshotsInTask(DataGridView dataGridView
            , Boolean useFakeBirthYears
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , Int32 maxCount
            , HashSet<String> processedItems
            , ref Int32 rowIndex)
        {
            Int32 maxTasks = ((rowIndex + IMDbParser.MaxTasks - 1) < maxCount) ? IMDbParser.MaxTasks : (maxCount - rowIndex);

            List<Task> tasks = new List<Task>(maxTasks);

            for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, rowIndex++)
            {
                DataGridViewRow row = dataGridView.Rows[rowIndex];

                PersonInfo person = (PersonInfo)(row.Tag);

                if (processedItems.Add(person.PersonLink))
                {
                    Task task = Task.Run(() => HeadshotGetter.Get(useFakeBirthYears, isCast, addMessage, person));

                    tasks.Add(task);
                }
            }

            Task.WaitAll(tasks.ToArray());

            return (maxTasks);
        }

        #endregion

        private static void ShowBirthYearMessageBox(Log log
            , PersonInfo person
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            String text;
            String logText;

            if (isCast)
            {
                text = String.Format(MessageBoxTexts.BirthYearCastHasChanged, person.FormatPersonNameWithoutMarkers()
                    , person.FormatActorNameWithBirthYearWithMarkers(true), person.FormatActorNameWithBirthYearWithMarkers(false), person.PersonLink);

                logText = String.Format(MessageBoxTexts.BirthYearCastHasChanged, person.FormatPersonNameWithoutMarkers()
                    , person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, null), person.FormatActorNameWithBirthYearWithMarkersAsHtml(false, null), CreatePersonLinkHtml(person));
            }
            else
            {
                text = String.Format(MessageBoxTexts.BirthYearCrewHasChanged, person.FormatPersonNameWithoutMarkers()
                    , person.FormatActorNameWithBirthYearWithMarkers(true), person.FormatActorNameWithBirthYearWithMarkers(false), person.PersonLink);

                logText = String.Format(MessageBoxTexts.BirthYearCrewHasChanged, person.FormatPersonNameWithoutMarkers()
                    , person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, null), person.FormatActorNameWithBirthYearWithMarkersAsHtml(false, null), CreatePersonLinkHtml(person));
            }

            log.AppendParagraph(logText);

            addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
        }

        public static void CreateCastColumns(DataGridView dataGridView)
        {
            DataGridViewTextBoxColumn firstnameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn middlenameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn lastnameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn birthyearDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn roleDataGridViewTextBoxColumn;
            DataGridViewCheckBoxColumn voiceDataGridViewTextBoxColumn;
            DataGridViewCheckBoxColumn uncreditedDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn creditedasDataGridViewTextBoxColumn;
            DataGridViewLinkColumn linkDataGridViewLinkColumn;
            DataGridViewDisableButtonColumn moveUpColumn;
            DataGridViewDisableButtonColumn moveDownColumn;
            DataGridViewDisableButtonColumn removeRowColumn;

            firstnameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            firstnameDataGridViewTextBoxColumn.Name = ColumnNames.FirstName;
            firstnameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.FirstName;
            firstnameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            firstnameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            firstnameDataGridViewTextBoxColumn.ReadOnly = true;
            firstnameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(firstnameDataGridViewTextBoxColumn);

            middlenameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            middlenameDataGridViewTextBoxColumn.Name = ColumnNames.MiddleName;
            middlenameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.MiddleName;
            middlenameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            middlenameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            middlenameDataGridViewTextBoxColumn.ReadOnly = true;
            middlenameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(middlenameDataGridViewTextBoxColumn);

            lastnameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            lastnameDataGridViewTextBoxColumn.Name = ColumnNames.LastName;
            lastnameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.LastName;
            lastnameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            lastnameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            lastnameDataGridViewTextBoxColumn.ReadOnly = true;
            lastnameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(lastnameDataGridViewTextBoxColumn);

            birthyearDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            birthyearDataGridViewTextBoxColumn.Name = ColumnNames.BirthYear;
            birthyearDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.BirthYear;
            birthyearDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            birthyearDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            birthyearDataGridViewTextBoxColumn.ReadOnly = true;
            birthyearDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(birthyearDataGridViewTextBoxColumn);

            roleDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            roleDataGridViewTextBoxColumn.Name = ColumnNames.Role;
            roleDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.Role;
            roleDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            roleDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            roleDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(roleDataGridViewTextBoxColumn);

            voiceDataGridViewTextBoxColumn = new DataGridViewCheckBoxColumn();
            voiceDataGridViewTextBoxColumn.Name = ColumnNames.Voice;
            voiceDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.Voice;
            voiceDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            voiceDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            voiceDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(voiceDataGridViewTextBoxColumn);

            uncreditedDataGridViewTextBoxColumn = new DataGridViewCheckBoxColumn();
            uncreditedDataGridViewTextBoxColumn.Name = ColumnNames.Uncredited;
            uncreditedDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.Uncredited;
            uncreditedDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            uncreditedDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            uncreditedDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(uncreditedDataGridViewTextBoxColumn);

            creditedasDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            creditedasDataGridViewTextBoxColumn.Name = ColumnNames.CreditedAs;
            creditedasDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.CreditedAs;
            creditedasDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            creditedasDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            creditedasDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(creditedasDataGridViewTextBoxColumn);

            linkDataGridViewLinkColumn = new DataGridViewLinkColumn();
            linkDataGridViewLinkColumn.Name = ColumnNames.Link;
            linkDataGridViewLinkColumn.HeaderText = DataGridViewTexts.Link;
            linkDataGridViewLinkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            linkDataGridViewLinkColumn.Resizable = DataGridViewTriState.True;
            linkDataGridViewLinkColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(linkDataGridViewLinkColumn);

            moveUpColumn = new DataGridViewDisableButtonColumn();
            moveUpColumn.Name = ColumnNames.MoveUp;
            moveUpColumn.HeaderText = DataGridViewTexts.MoveUp;
            moveUpColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            moveUpColumn.Resizable = DataGridViewTriState.True;
            moveUpColumn.Text = ColumnNames.MoveUp;
            moveUpColumn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(moveUpColumn);

            moveDownColumn = new DataGridViewDisableButtonColumn();
            moveDownColumn.Name = ColumnNames.MoveDown;
            moveDownColumn.HeaderText = DataGridViewTexts.MoveDown;
            moveDownColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            moveDownColumn.Resizable = DataGridViewTriState.True;
            moveDownColumn.Text = ColumnNames.MoveDown;
            moveDownColumn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(moveDownColumn);

            removeRowColumn = new DataGridViewDisableButtonColumn();
            removeRowColumn.Name = ColumnNames.RemoveRow;
            removeRowColumn.HeaderText = DataGridViewTexts.RemoveRow;
            removeRowColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            removeRowColumn.Resizable = DataGridViewTriState.True;
            removeRowColumn.Text = ColumnNames.RemoveRow;
            removeRowColumn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(removeRowColumn);

            DataGridViewTextBoxColumn originalCreditDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            originalCreditDataGridViewTextBoxColumn.Name = ColumnNames.OriginalCredit;
            originalCreditDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.OriginalCredit;
            originalCreditDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            originalCreditDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            originalCreditDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(originalCreditDataGridViewTextBoxColumn);
        }

        public static void CreateCrewColumns(DataGridView dataGridView)
        {
            DataGridViewTextBoxColumn firstnameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn middlenameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn lastnameDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn birthyearDataGridViewTextBoxColumn;
            DataGridViewComboBoxColumn creditTypeDataGridViewComboBoxBoxColumn;
            DataGridViewComboBoxColumn creditSubtypeDataGridViewComboBoxColumn;
            DataGridViewTextBoxColumn customRoleDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn creditedasDataGridViewTextBoxColumn;
            DataGridViewLinkColumn linkDataGridViewLinkColumn;

            firstnameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            firstnameDataGridViewTextBoxColumn.Name = ColumnNames.FirstName;
            firstnameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.FirstName;
            firstnameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            firstnameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            firstnameDataGridViewTextBoxColumn.ReadOnly = true;
            firstnameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(firstnameDataGridViewTextBoxColumn);

            middlenameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            middlenameDataGridViewTextBoxColumn.Name = ColumnNames.MiddleName;
            middlenameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.MiddleName;
            middlenameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            middlenameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            middlenameDataGridViewTextBoxColumn.ReadOnly = true;
            middlenameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(middlenameDataGridViewTextBoxColumn);

            lastnameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            lastnameDataGridViewTextBoxColumn.Name = ColumnNames.LastName;
            lastnameDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.LastName;
            lastnameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            lastnameDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            lastnameDataGridViewTextBoxColumn.ReadOnly = true;
            lastnameDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(lastnameDataGridViewTextBoxColumn);

            birthyearDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            birthyearDataGridViewTextBoxColumn.Name = ColumnNames.BirthYear;
            birthyearDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.BirthYear;
            birthyearDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            birthyearDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            birthyearDataGridViewTextBoxColumn.ReadOnly = true;
            birthyearDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(birthyearDataGridViewTextBoxColumn);

            creditTypeDataGridViewComboBoxBoxColumn = new DataGridViewComboBoxColumn();
            creditTypeDataGridViewComboBoxBoxColumn.Name = ColumnNames.CreditType;
            creditTypeDataGridViewComboBoxBoxColumn.HeaderText = DataGridViewTexts.CreditType;
            creditTypeDataGridViewComboBoxBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            creditTypeDataGridViewComboBoxBoxColumn.Resizable = DataGridViewTriState.True;
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Direction);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Writing);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Production);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Cinematography);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.FilmEditing);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Music);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Sound);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Art);
            creditTypeDataGridViewComboBoxBoxColumn.Items.Add(CreditTypesDataGridViewHelper.CreditTypes.Other);
            creditTypeDataGridViewComboBoxBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(creditTypeDataGridViewComboBoxBoxColumn);

            creditSubtypeDataGridViewComboBoxColumn = new DataGridViewComboBoxColumn();
            creditSubtypeDataGridViewComboBoxColumn.Name = ColumnNames.CreditSubtype;
            creditSubtypeDataGridViewComboBoxColumn.HeaderText = DataGridViewTexts.CreditSubtype;
            creditSubtypeDataGridViewComboBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            creditSubtypeDataGridViewComboBoxColumn.Resizable = DataGridViewTriState.True;
            creditSubtypeDataGridViewComboBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(creditSubtypeDataGridViewComboBoxColumn);

            customRoleDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            customRoleDataGridViewTextBoxColumn.Name = ColumnNames.CustomRole;
            customRoleDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.CustomRole;
            customRoleDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            customRoleDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            customRoleDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(customRoleDataGridViewTextBoxColumn);

            creditedasDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            creditedasDataGridViewTextBoxColumn.Name = ColumnNames.CreditedAs;
            creditedasDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.CreditedAs;
            creditedasDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            creditedasDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            creditedasDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(creditedasDataGridViewTextBoxColumn);

            linkDataGridViewLinkColumn = new DataGridViewLinkColumn();
            linkDataGridViewLinkColumn.Name = ColumnNames.Link;
            linkDataGridViewLinkColumn.HeaderText = DataGridViewTexts.Link;
            linkDataGridViewLinkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            linkDataGridViewLinkColumn.Resizable = DataGridViewTriState.True;
            linkDataGridViewLinkColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(linkDataGridViewLinkColumn);

            DataGridViewTextBoxColumn originalCreditDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            originalCreditDataGridViewTextBoxColumn.Name = ColumnNames.OriginalCredit;
            originalCreditDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.OriginalCredit;
            originalCreditDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            originalCreditDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            originalCreditDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.Columns.Add(originalCreditDataGridViewTextBoxColumn);
        }

        public static void CopyCastToClipboard(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage
            , Boolean embedded)
        {
            CastInformation ci = new CastInformation();

            CreateCastMember(dataGridView, title, log, useFakeBirthYears, addMessage, embedded, ci, (row) => new CastMember(), (row) => new Divider());

            try
            {
                String xml = Utilities.CopyCastInformationToClipboard(ci, embedded);

                Program.AdapterEventHandler.RaiseCastCompleted(xml);
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CopyExtendedCastToClipboard(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage)
        {
            Func<DataGridViewRow, CastMember> createCastMember = (row) =>
                    {
                        ExtendedCastMember castMember = new ExtendedCastMember();

                        castMember.ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString();

                        return (castMember);
                    }
                ;

            Func<DataGridViewRow, Divider> createCastDivider = (row) =>
                    {
                        ExtendedCastDivider castDivider = new ExtendedCastDivider();

                        castDivider.ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString();

                        return (castDivider);
                    }
                ;

            ExtendedCastInformation ci = new ExtendedCastInformation();

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
            Type[] addTypes = new[] { typeof(ExtendedCastMember), typeof(ExtendedCastDivider) };

            ExtendedSerializer<ExtendedCastInformation> serializer = new ExtendedSerializer<ExtendedCastInformation>(addTypes, CastInformation.DefaultEncoding);

            String xml = serializer.ToString(castInformation);

            Clipboard.SetDataObject(xml, true, 4, 250);
        }

        private static void CreateCastMember(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage
            , Boolean embedded
            , CastInformation ci
            , Func<DataGridViewRow, CastMember> createCastMember
            , Func<DataGridViewRow, Divider> createCastDivider)
        {
            Int32 offset = 0;

            if (dataGridView.Rows.Count > 0)
            {
                Object value = dataGridView.Rows[0].Cells[ColumnNames.FirstName].Value;

                if ((value != null) && (value.ToString() == FirstNames.Title))
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

            ci.CastList = new Object[dataGridView.Rows.Count + offset];

            offset = 0;

            for (Int32 i = 0; i < dataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView.Rows[i];

                Object value = row.Cells[ColumnNames.FirstName].Value;

                if ((value != null) && (value.ToString() == FirstNames.Title))
                {
                    offset = -1;
                }
                else if ((value != null) && (value.ToString() == FirstNames.Divider))
                {
                    row = dataGridView.Rows[i];

                    Divider divider = createCastDivider(row);

                    ci.CastList[i + offset] = divider;

                    String name = String.Empty;

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
                    CastMember castMember = createCastMember(row);

                    ci.CastList[i + offset] = castMember;

                    if (value != null)
                    {
                        castMember.FirstName = value.ToString();
                    }

                    row = dataGridView.Rows[i];

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
                        Int32 intValue;
                        if (Int32.TryParse(value.ToString(), out intValue))
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
                        castMember.Voice = Boolean.Parse(value.ToString());
                    }

                    value = row.Cells[ColumnNames.Uncredited].Value;

                    if (value != null)
                    {
                        castMember.Uncredited = Boolean.Parse(value.ToString());
                    }

                    value = row.Cells[ColumnNames.CreditedAs].Value;

                    if (value != null)
                    {
                        castMember.CreditedAs = value.ToString();
                    }

                    if (embedded == false)
                    {
                        CheckForPossibleDuplicates(log, row, ConfirmedPossibleCastDuplicates
                            , Program.PossibleCastDuplicateCache, DataGridViewTexts.Cast, useFakeBirthYears, castMember
                            , Program.CastCache, true, addMessage);
                    }
                }
            }
        }

        private static void CheckForPossibleDuplicates(Log log
            , DataGridViewRow row
            , Dictionary<PersonInfoWithoutBirthYear, Boolean> confirmedPossibleDuplicates
            , Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> possibleDuplicatesCache
            , String type
            , Boolean useFakeBirthYears
            , IPerson clipboardPerson
            , Dictionary<String, PersonInfo> personCache
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            PersonInfoWithoutBirthYear piwby;
            List<PersonInfo> list;
            PersonInfo rowPerson;

            piwby = new PersonInfoWithoutBirthYear(row, type);
            rowPerson = new PersonInfo(row, type);
            if (confirmedPossibleDuplicates.ContainsKey(piwby) == false)
            {
                List<PersonInfo> newFakeBirthYears;

                list = possibleDuplicatesCache[piwby];
                newFakeBirthYears = new List<PersonInfo>(list.Count);
                if (list.Count > 1)
                {
                    Dictionary<String, List<PersonInfo>> birthYears;
                    Boolean hasBlankEntries;

                    birthYears = new Dictionary<String, List<PersonInfo>>(list.Count);
                    hasBlankEntries = false;
                    foreach (PersonInfo item in list)
                    {
                        PersonInfo cachePerson;

                        cachePerson = personCache[item.PersonLink];
                        if (String.IsNullOrEmpty(cachePerson.BirthYear) == false)
                        {
                            item.BirthYear = cachePerson.BirthYear;
                            ConsolidateBirthYears(birthYears, item, item.BirthYear);
                            if (rowPerson.PersonLink == item.PersonLink)
                            {
                                clipboardPerson.BirthYear = Int32.Parse(item.BirthYear);
                            }
                        }
                        else
                        {
                            if (useFakeBirthYears)
                            {
                                String fakeBirthYear;

                                fakeBirthYear = CreateFakeBirthYear(clipboardPerson, newFakeBirthYears, item, cachePerson, rowPerson, isCast, addMessage);
                                ConsolidateBirthYears(birthYears, item, fakeBirthYear);
                            }
                            else
                            {
                                hasBlankEntries = true;
                                ConsolidateBirthYears(birthYears, item, String.Empty);
                                if ((String.IsNullOrEmpty(item.FakeBirthYear) == false) && (rowPerson.PersonLink == item.PersonLink))
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
                    foreach (KeyValuePair<String, List<PersonInfo>> kvp in birthYears)
                    {
                        if (kvp.Value.Count > 1)
                        {
                            ShowDuplicatesMessageBox(log, confirmedPossibleDuplicates, piwby, kvp.Value, MessageBoxTexts.PossibleSameYearDuplicates, addMessage
                                , Program.Settings.DefaultValues.DisableDuplicatesMessageBox);
                        }
                    }
                    if ((hasBlankEntries) && (birthYears.Count > 1))
                    {
                        ShowDuplicatesMessageBox(log, confirmedPossibleDuplicates, piwby, list, MessageBoxTexts.PossibleDuplicates, addMessage, false);
                    }
                }
                else if (useFakeBirthYears)
                {
                    PersonInfo cachePerson;

                    cachePerson = personCache[list[0].PersonLink];
                    CreateFakeBirthYear(clipboardPerson, newFakeBirthYears, list[0], cachePerson, rowPerson, isCast, addMessage);
                }
                else if (String.IsNullOrEmpty(list[0].FakeBirthYear) == false)
                {
                    ShowBirthYearMessageBox(log, list[0], isCast, addMessage);
                }
            }
        }

        private static String CreateFakeBirthYear(IPerson clipboardPerson
            , List<PersonInfo> newFakeBirthYears
            , PersonInfo item, PersonInfo cachePerson
            , PersonInfo rowPerson
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            String fakeBirthYear;

            fakeBirthYear = String.Empty;
            if (String.IsNullOrEmpty(cachePerson.BirthYear) == false) //If we already have a BY, why not use it
            {
                fakeBirthYear = cachePerson.BirthYear;
            }
            else if (String.IsNullOrEmpty(cachePerson.FakeBirthYear))
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
                if (String.IsNullOrEmpty(fakeBirthYear) == false)
                {
                    clipboardPerson.BirthYear = Int32.Parse(fakeBirthYear);
                }
            }
            return (fakeBirthYear);
        }

        private static Int32 CreateFakeBirthYearAsInt(PersonInfo item)
        {
            Int32 fakeBirthYear;
            String fakeBirthYearString;

            fakeBirthYearString = item.PersonLink.Substring(item.PersonLink.Length - 5);
            fakeBirthYear = Int32.Parse(fakeBirthYearString);
            if (fakeBirthYear > UInt16.MaxValue)
            {
                fakeBirthYearString = item.PersonLink.Substring(item.PersonLink.Length - 4);
                fakeBirthYear = Int32.Parse(fakeBirthYearString);
            }
            return (fakeBirthYear);
        }

        internal static String CreateFakeBirthYearAsString(PersonInfo item
            , Boolean isCast
            , Action<MessageEntry> addMessage)
        {
            Int32 fakeBirthYearAsInt = CreateFakeBirthYearAsInt(item);

            String fakeBirthYearAsString;
            if (fakeBirthYearAsInt == 0)
            {
                fakeBirthYearAsString = String.Empty;

                String text;
                if (isCast)
                {
                    text = String.Format(MessageBoxTexts.FakeBirthYearZeroCast, item.PersonLink, item.FormatActorNameWithBirthYearWithMarkers(true));
                }
                else
                {
                    text = String.Format(MessageBoxTexts.FakeBirthYearZeroCrew, item.PersonLink, item.FormatActorNameWithBirthYearWithMarkers(true));
                }

                addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
            }
            else
            {
                fakeBirthYearAsString = fakeBirthYearAsInt.ToString();
            }

            return (fakeBirthYearAsString);
        }

        private static void ShowDuplicatesMessageBox(Log log
            , Dictionary<PersonInfoWithoutBirthYear, Boolean> confirmedPossibleDuplicates
            , PersonInfoWithoutBirthYear piwby
            , List<PersonInfo> list
            , String messageId
            , Action<MessageEntry> addMessage
            , Boolean ignoreMesssageBox)
        {
            ShowMultipleNamesMessageBox(log, list, messageId, addMessage, ignoreMesssageBox);

            if (confirmedPossibleDuplicates.ContainsKey(piwby) == false)
            {
                confirmedPossibleDuplicates.Add(piwby, true);
            }
        }

        private static void ShowMultipleNamesMessageBox(Log log
            , List<PersonInfo> list
            , String messageId
            , Action<MessageEntry> addMessage
            , Boolean ignoreMesssageBox)
        {
            String text;
            String logText;
            StringBuilder nameList;
            StringBuilder nameListLog;
            Boolean useFakeBirthNames;

            nameList = new StringBuilder();
            nameListLog = new StringBuilder();
            useFakeBirthNames = Program.Settings.DefaultValues.UseFakeBirthYears;
            foreach (PersonInfo item in list)
            {
                nameList.Append(item.PersonLink);
                nameList.Append(": ");
                nameList.AppendLine(item.FormatActorNameWithBirthYearWithMarkers(useFakeBirthNames, true));
                nameListLog.Append(CreatePersonLinkHtml(item));
                nameListLog.Append(": ");
                nameListLog.AppendLine(item.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthNames, true, list));
            }
            text = String.Format(messageId, nameList.ToString());
            logText = String.Format(messageId, nameListLog.ToString());
            log.AppendParagraph(logText);

            if (ignoreMesssageBox == false)
            {
                addMessage(new MessageEntry(text, MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning));
            }
        }

        internal static String CreatePersonLinkHtml(PersonInfo person)
        {
            StringBuilder sb;

            sb = new StringBuilder("<a href=\"http://akas.imdb.com/name/");
            sb.Append(person.PersonLink);
            sb.Append("/\">");
            sb.Append(person.PersonLink);
            sb.Append("</a>");

            return (sb.ToString());
        }

        private static void ConsolidateBirthYears(Dictionary<String, List<PersonInfo>> birthYears, PersonInfo item, String birthYear)
        {
            List<PersonInfo> sameYearList;

            if (birthYears.TryGetValue(birthYear, out sameYearList))
            {
                sameYearList.Add(item);
            }
            else
            {
                sameYearList = new List<PersonInfo>(4);
                sameYearList.Add(item);
                birthYears.Add(birthYear, sameYearList);
            }
        }

        public static void CopyCrewToClipboard(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage
            , Boolean embedded)
        {
            Func<DataGridViewRow, CrewMember> createCrewMember = (row) => new CrewMember();

            CrewInformation ci = new CrewInformation();

            CreateCrewMember(dataGridView, title, log, useFakeBirthYears, addMessage, embedded, ci, (row) => new CrewMember(), (row) => new CrewDivider());

            try
            {
                String xml = Utilities.CopyCrewInformationToClipboard(ci, embedded);

                Program.AdapterEventHandler.RaiseCrewCompleted(xml);
            }
            catch (ExternalException)
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CopyExtendedCrewToClipboard(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage)
        {
            Func<DataGridViewRow, CrewMember> createCrewMember = (row) =>
                    {
                        ExtendedCrewMember crewMember = new ExtendedCrewMember();

                        crewMember.ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString();

                        return (crewMember);
                    }
                ;

            Func<DataGridViewRow, CrewDivider> createCrewDivider = (row) =>
                    {
                        ExtendedCrewDivider crewDivider = new ExtendedCrewDivider();

                        crewDivider.ImdbLink = row.Cells[ColumnNames.Link].Value?.ToString();

                        return (crewDivider);
                    }
               ;

            ExtendedCrewInformation ci = new ExtendedCrewInformation();

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
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CopyExtendedCrewToClipboard(ExtendedCrewInformation crewInformation)
        {
            crewInformation = ExtendedCrewSorter.GetSortedCrew(crewInformation);

            Type[] addTypes = new[] { typeof(ExtendedCrewMember), typeof(ExtendedCrewDivider) };

            ExtendedSerializer<ExtendedCrewInformation> serializer = new ExtendedSerializer<ExtendedCrewInformation>(addTypes, CrewInformation.DefaultEncoding);

            String xml = serializer.ToString(crewInformation);

            Clipboard.SetDataObject(xml, true, 4, 250);
        }

        private static void CreateCrewMember(DataGridView dataGridView
            , String title
            , Log log
            , Boolean useFakeBirthYears
            , Action<MessageEntry> addMessage
            , Boolean embedded
            , CrewInformation ci
            , Func<DataGridViewRow, CrewMember> createCrewMember
            , Func<DataGridViewRow, CrewDivider> createEpisodeDivider)
        {
            Int32 offset = 0;

            if (dataGridView.Rows.Count > 0)
            {
                Object value = dataGridView.Rows[0].Cells[ColumnNames.FirstName].Value;

                if ((value != null) && (value.ToString() == FirstNames.Title))
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

            ci.CrewList = new Object[dataGridView.Rows.Count + offset];

            offset = 0;

            for (Int32 i = 0; i < dataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView.Rows[i];

                Object value = row.Cells[ColumnNames.FirstName].Value;

                if ((value != null) && (value.ToString() == FirstNames.Title))
                {
                    offset = -1;
                }

                else if ((value != null) && (value.ToString() == FirstNames.Divider))
                {
                    CrewDivider divider = createEpisodeDivider(row);

                    ci.CrewList[i + offset] = divider;

                    String name = String.Empty;

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
                    CrewMember crewMember = createCrewMember(row);

                    ci.CrewList[i + offset] = crewMember;

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
                        Int32 intValue;
                        if (Int32.TryParse(value.ToString(), out intValue))
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

                        if (String.IsNullOrEmpty(crewMember.CustomRole) == false)
                        {
                            crewMember.CustomRoleSpecified = true;
                        }
                    }

                    value = row.Cells[ColumnNames.CreditedAs].Value;

                    if (value != null)
                    {
                        crewMember.CreditedAs = value.ToString();
                    }

                    if (embedded == false)
                    {
                        CheckForPossibleDuplicates(log, row, ConfirmedPossibleCrewDuplicates
                            , Program.PossibleCrewDuplicateCache, DataGridViewTexts.Crew, useFakeBirthYears, crewMember
                            , Program.CrewCache, false, addMessage);
                    }
                }
            }
        }

        public static void FillCastRows(DataGridView dataGridView, List<CastInfo> castList, Boolean isFirstDivider
            , Boolean isLastDivider)
        {
            Int32 offset;
            List<DataGridViewRow> rows;

            DataFillMode = true;
            offset = 0;
            rows = new List<DataGridViewRow>(castList.Count + castList.Count / 10);
            for (Int32 i = 0; i < castList.Count; i++)
            {
                CastInfo castMember;
                DataGridViewRow row;

                castMember = castList[i];
                row = new DataGridViewRow();
                for (Int32 j = 0; j < dataGridView.Columns.Count; j++)
                {
                    DataGridViewCell cell;
                    DataGridViewColumn column;

                    column = dataGridView.Columns[j];
                    cell = (DataGridViewCell)(column.CellTemplate.Clone());
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
                        ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveUp].Index])).Enabled = false;
                    }
                    if (isLastDivider)
                    {
                        ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveDown].Index])).Enabled = false;
                    }
                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.RemoveRow].Index])).Enabled = false;
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
                        if (castMember.BirthYearWasRetrieved == false)
                        {
                            row.Cells[dataGridView.Columns[ColumnNames.BirthYear].Index].Style.BackColor = Color.LightGray;
                        }
                    }
                    row.Cells[dataGridView.Columns[ColumnNames.LastName].Index].ReadOnly = true;
                    row.Cells[dataGridView.Columns[ColumnNames.MiddleName].Index].ReadOnly = true;
                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveUp].Index])).Enabled = (i != offset);
                    ((DataGridViewDisableButtonCell)(row.Cells[dataGridView.Columns[ColumnNames.MoveDown].Index])).Enabled
                        = (i != castList.Count - 1);
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
            DataFillMode = false;
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public static void FillCrewRows(DataGridView dataGridView, List<CrewInfo> crewList)
        {
            List<DataGridViewRow> rows;

            rows = new List<DataGridViewRow>(crewList.Count + crewList.Count / 10);
            foreach (CrewInfo crewMember in crewList)
            {
                DataGridViewRow row;

                row = new DataGridViewRow();
                for (Int32 j = 0; j < dataGridView.Columns.Count; j++)
                {
                    DataGridViewCell cell;
                    DataGridViewColumn column;

                    column = dataGridView.Columns[j];
                    cell = (DataGridViewCell)(column.CellTemplate.Clone());
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

        public static void OnCastDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            if (DataFillMode == false)
            {
                CastInfo castMember;
                DataGridViewRow row;

                row = ((DataGridView)sender).Rows[e.RowIndex];
                castMember = (CastInfo)(row.Tag);
                if (e.ColumnIndex == 1)//middle name for episode number
                {
                    castMember.MiddleName = (String)(row.Cells[ColumnNames.MiddleName].Value);
                }
                else if (e.ColumnIndex == 2)//last name for title
                {
                    castMember.LastName = (String)(row.Cells[ColumnNames.LastName].Value);
                }
                else if (e.ColumnIndex == 4)//role
                {
                    castMember.Role = (String)(row.Cells[ColumnNames.Role].Value);
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
                    castMember.CreditedAs = (String)(row.Cells[ColumnNames.CreditedAs].Value);
                }
            }
        }

        public static void OnCrewDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            if (DataFillMode == false)
            {
                if (e.ColumnIndex == 4)
                {
                    DataGridView dataGridView;
                    DataGridViewRow row;

                    dataGridView = (DataGridView)sender;
                    row = dataGridView.Rows[e.RowIndex];
                    FillCreditSubtypeCell(dataGridView, row);
                }
            }
        }

        private static void FillCreditSubtypeCell(DataGridView dataGridView, DataGridViewRow row)
        {
            DataGridViewComboBoxCell creditTypeCell;
            DataGridViewComboBoxCell creditSubtypeCell;

            creditTypeCell = (DataGridViewComboBoxCell)(row.Cells[dataGridView.Columns[ColumnNames.CreditType].Index]);
            if (creditTypeCell.Value != null)
            {
                creditSubtypeCell = (DataGridViewComboBoxCell)(row.Cells[dataGridView.Columns[ColumnNames.CreditSubtype].Index]);
                creditSubtypeCell.Value = null;
                CreditTypesDataGridViewHelper.FillCreditSubtypes(creditTypeCell.Value.ToString(), creditSubtypeCell.Items);
                creditSubtypeCell.Value = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
            }
        }
    }
}