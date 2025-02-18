using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public class PersonInfos
    {
        public PersonInfo[] PersonInfoList;

        public void Serialize(string fileName)
        {
            if (PersonInfoList?.Length > 0)
            {
                foreach (var pi in PersonInfoList)
                {
                    if (!pi.LastModifiedSpecified)
                    {
                        pi.LastModified = DateTime.UtcNow;
                        pi.LastModifiedSpecified = true;
                    }
                }
            }

            XmlSerializer<PersonInfos>.Serialize(fileName, this);
        }

        public static PersonInfos Deserialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PersonInfo.CreatorActive = true;

                var personInfos = XmlSerializer<PersonInfos>.Deserialize(fs);

                PersonInfo.CreatorActive = false;

                return personInfos;
            }
        }
    }

    [Serializable]
    public class PersonInfo
    {
        private string _firstName;

        private string _middleName;

        private string _lastName;

        private string _birthYear;

        private bool _birthYearWasRetrieved;

        private string _personLink;

        private string _fakeBirthYear;

        private FilmInfo[] _filmInfoList;

        public string FirstName
        {
            [DebuggerStepThrough]
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    SetTimestamp();
                }

                _firstName = value;
            }
        }



        public string MiddleName
        {
            [DebuggerStepThrough]
            get => _middleName;
            set
            {
                if (_middleName != value)
                {
                    SetTimestamp();
                }

                _middleName = value;
            }
        }



        public string LastName
        {
            [DebuggerStepThrough]
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    SetTimestamp();
                }

                _lastName = value;
            }
        }



        public string BirthYear
        {
            [DebuggerStepThrough]
            get => _birthYear;
            set
            {
                if (_birthYear != value)
                {
                    SetTimestamp();
                }

                _birthYear = value;
            }
        }



        public bool BirthYearWasRetrieved
        {
            [DebuggerStepThrough]
            get => _birthYearWasRetrieved;
            set
            {
                if (_birthYearWasRetrieved != value)
                {
                    SetTimestamp();
                }

                _birthYearWasRetrieved = value;
            }
        }

        public string PersonLink
        {
            [DebuggerStepThrough]
            get => _personLink;
            set
            {
                if (_personLink != value)
                {
                    SetTimestamp();
                }

                _personLink = value;
            }
        }

        public string FakeBirthYear
        {
            [DebuggerStepThrough]
            get => _fakeBirthYear;
            set
            {
                if (_fakeBirthYear != value)
                {
                    SetTimestamp();
                }

                _fakeBirthYear = value;
            }
        }

        public FilmInfo[] FilmInfoList
        {
            [DebuggerStepThrough]
            get => _filmInfoList;
            set
            {
                if ((_filmInfoList != null && value == null)
                    || (_filmInfoList == null && value != null))
                {
                    SetTimestamp();
                }
                else if (_filmInfoList != null)
                {
                    if (_filmInfoList.Length != value.Length)
                    {
                        SetTimestamp();
                    }
                    else
                    {
                        for (var i = 0; i < _filmInfoList.Length; i++)
                        {
                            var oldFI = _filmInfoList[i];

                            var newFi = value[i];

                            if (oldFI.FilmLink != newFi.FilmLink || oldFI.Name != newFi.Name)
                            {
                                SetTimestamp();
                            }
                        }
                    }
                }

                _filmInfoList = value;
            }
        }

        public DateTime LastModified;

        [XmlIgnore]
        public bool LastModifiedSpecified = false;

        [XmlIgnore]
        public string Type = string.Empty;

        [XmlIgnore]
        public string OriginalCredit;

        public DateTime LastLinkCheck;

        public static bool CreatorActive { get; set; }

        public PersonInfo()
        {
            _birthYearWasRetrieved = false;

            _personLink = string.Empty;

            _filmInfoList = null;
        }

        public PersonInfo(string type) : this()
        {
            Type = type;
        }

        public PersonInfo(DataGridViewRow row, string type)
        {
            var value = row.Cells[ColumnNames.LastName].Value;

            if (value != null)
            {
                LastName = value.ToString();
            }
            else
            {
                LastName = string.Empty;
            }

            value = row.Cells[ColumnNames.MiddleName].Value;

            if (value != null)
            {
                MiddleName = value.ToString();
            }
            else
            {
                MiddleName = string.Empty;
            }

            value = row.Cells[ColumnNames.FirstName].Value;

            if (value != null)
            {
                FirstName = value.ToString();
            }
            else
            {
                FirstName = string.Empty;
            }

            value = row.Cells[ColumnNames.BirthYear].Value;

            if (value != null)
            {
                BirthYear = value.ToString();
            }
            else
            {
                BirthYear = string.Empty;
            }

            PersonLink = row.Cells[ColumnNames.Link].Value.ToString();

            Type = type;
        }

        public PersonInfo(PersonInfo personInfo)
        {
            CreatorActive = true;

            LastName = personInfo.LastName;
            MiddleName = personInfo.MiddleName;
            FirstName = personInfo.FirstName;
            BirthYear = personInfo.BirthYear;
            BirthYearWasRetrieved = personInfo.BirthYearWasRetrieved;
            PersonLink = personInfo.PersonLink;
            FakeBirthYear = personInfo.FakeBirthYear;
            FilmInfoList = personInfo.FilmInfoList;

            Type = personInfo.Type;
            LastModified = personInfo.LastModified;
            LastModifiedSpecified = personInfo.LastModifiedSpecified;
            LastLinkCheck = personInfo.LastLinkCheck;

            CreatorActive = false;
        }

        private void SetTimestamp()
        {
            if (!CreatorActive)
            {
                LastModified = DateTime.UtcNow;
                LastModifiedSpecified = true;
            }
        }

        public override int GetHashCode() => PersonLink.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is PersonInfo other))
            {
                return false;
            }
            else
            {
                return PersonLink == other.PersonLink;
            }
        }

        public override string ToString() => FormatActorNameWithBirthYearWithMarkers(true);

        private string FormatPersonNameWithMarkers(bool withFilmList)
        {
            var name = new StringBuilder();

            if (!string.IsNullOrEmpty(FirstName))
            {
                name.Append("<" + FirstName + ">");
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("{" + MiddleName + "}");
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("[" + LastName + "]");
            }

            if (withFilmList)
            {
                AppendFilmInfoList(name);
            }

            return name.ToString();
        }

        private string PadNamePart(string namePart, IEnumerable<PersonInfo> others, Func<PersonInfo, string> getNamePart)
        {
            namePart = namePart ?? string.Empty;

            others = others ?? Enumerable.Empty<PersonInfo>();

            var nameParts = others.Select(getNamePart);

            var padding = GetMaxPadding(namePart, nameParts);

            for (var i = namePart.Length; i < padding; i++)
            {
                namePart += "&nbsp;";
            }

            return namePart;
        }

        private int GetMaxPadding(string namePart, IEnumerable<string> others)
        {
            var padding = namePart.Length;

            foreach (var other in others)
            {
                if (other?.Length > padding)
                {
                    padding = other.Length;
                }
            }

            return padding;
        }

        private void AppendFilmInfoList(StringBuilder name)
        {
            if (FilmInfoList?.Length > 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(");
                name.Append(FilmInfoList[FilmInfoList.Length - 1].Name);
                name.Append(")");
            }
        }

        public string FormatPersonNameWithMarkers() => FormatPersonNameWithMarkers(false);

        private string FormatPersonNameWithMarkersAsHtml(bool withFilmList, IEnumerable<PersonInfo> others)
        {
            var name = new StringBuilder();

            var namePart = PadNamePart(FirstName, others, (pi) => pi.FirstName);

            name.Append("<span style=\"color:Blue;\">" + namePart + "</span>");

            namePart = PadNamePart(MiddleName, others, (pi) => pi.MiddleName);

            if (!string.IsNullOrEmpty(namePart) && name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:White; background-color:Black\"><strong>" + namePart + "</strong></span>");

            namePart = PadNamePart(LastName, others, (pi) => pi.LastName);

            if (!string.IsNullOrEmpty(namePart) && name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:Green;\">" + namePart + "</span>");

            if (withFilmList)
            {
                AppendFilmInfoListAsHtml(name);
            }

            return name.ToString();
        }

        private void AppendFilmInfoListAsHtml(StringBuilder name)
        {
            if (FilmInfoList?.Length > 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(");

                for (var i = FilmInfoList.Length - 1; i >= 0; i--)
                {
                    var fi = FilmInfoList[i];

                    name.Append("<a href=\"https://www.imdb.com/title/");
                    name.Append(fi.FilmLink);
                    name.Append("/\" target=\"_blank\">");
                    name.Append(fi.Name);
                    name.Append("</a>");

                    if (i > 0)
                    {
                        name.Append(", ");
                    }
                }

                name.Append(")");
            }
        }

        public string FormatPersonNameWithMarkersAsHtml(IEnumerable<PersonInfo> others) => FormatPersonNameWithMarkersAsHtml(false, others);

        public string FormatPersonNameWithoutMarkers()
        {
            var name = new StringBuilder();

            if (!string.IsNullOrEmpty(FirstName))
            {
                name.Append(FirstName);
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(MiddleName);
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(LastName);
            }

            return name.ToString();
        }

        public string FormatActorNameWithBirthYearWithMarkers(bool useFakeBirthYear) => FormatActorNameWithBirthYearWithMarkers(useFakeBirthYear, false);

        public string FormatActorNameWithBirthYearWithMarkers(bool useFakeBirthYear, bool withFilmList)
        {
            var name = new StringBuilder(FormatPersonNameWithMarkers(false));

            if (!string.IsNullOrEmpty(BirthYear))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + BirthYear + ")");
            }
            else if (useFakeBirthYear && !string.IsNullOrEmpty(FakeBirthYear) && FakeBirthYear != "0")
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + FakeBirthYear + ")");
            }

            if (withFilmList)
            {
                AppendFilmInfoList(name);
            }

            return name.ToString();
        }

        public string FormatActorNameWithBirthYearWithMarkersAsHtml(bool useFakeBirthYear, IEnumerable<PersonInfo> others) => FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYear, false, others);

        public string FormatActorNameWithBirthYearWithMarkersAsHtml(bool useFakeBirthYear, bool withFilmList, IEnumerable<PersonInfo> others)
        {
            var name = new StringBuilder(FormatPersonNameWithMarkersAsHtml(false, others));

            if (!string.IsNullOrEmpty(BirthYear))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + BirthYear + ")");
            }
            else if (useFakeBirthYear && !string.IsNullOrEmpty(FakeBirthYear) && FakeBirthYear != "0")
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + FakeBirthYear + ")");
            }

            if (withFilmList)
            {
                AppendFilmInfoListAsHtml(name);
            }

            return name.ToString();
        }

        public void AddFilmInfo(string link, string name)
        {
            var filmInfoList = FilmInfoList == null
                ? new List<FilmInfo>(1)
                : new List<FilmInfo>(FilmInfoList);

            var contains = false;

            for (var firstIndex = filmInfoList.Count - 1; firstIndex >= 0; firstIndex--)
            {
                if (filmInfoList[firstIndex].FilmLink == link)
                {
                    var temp = filmInfoList[firstIndex];

                    temp.Name = name;

                    contains = true;

                    for (var secondIndex = firstIndex; secondIndex < filmInfoList.Count - 1; secondIndex++)
                    {
                        filmInfoList[secondIndex] = filmInfoList[secondIndex + 1];
                    }

                    filmInfoList[filmInfoList.Count - 1] = temp;

                    break;
                }
            }

            if (!contains)
            {
                filmInfoList.Add(new FilmInfo(link, name));
            }

            if (filmInfoList.Count > 3)
            {
                filmInfoList.RemoveAt(0);
            }

            FilmInfoList = filmInfoList.ToArray();
        }

        public static int CompareForSorting(PersonInfo left, PersonInfo right)
        {
            var leftName = GetSortingName(left);

            var rightName = GetSortingName(right);

            return leftName.CompareTo(rightName);
        }

        private static string GetSortingName(PersonInfo person) => person.LastName + " " + person.FirstName + " " + person.MiddleName + " " + person.PersonLink;
    }

    [Serializable]
    public class FilmInfo
    {
        public string FilmLink;

        public string Name;

        public FilmInfo()
        { }

        public FilmInfo(string link, string name)
        {
            FilmLink = link;
            Name = name;
        }
    }

    public class PersonInfoWithoutBirthYear : PersonInfo
    {
        public PersonInfoWithoutBirthYear(DataGridViewRow row, string type) : base(row, type)
        { }

        public PersonInfoWithoutBirthYear(PersonInfo personInfo) : base(personInfo)
        { }

        public override int GetHashCode() => FirstName.GetHashCode() ^ MiddleName.GetHashCode() ^ LastName.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is PersonInfoWithoutBirthYear other))
            {
                return false;
            }
            else
            {
                var result = LastName == other.LastName
                    && MiddleName == other.MiddleName
                    && FirstName == other.FirstName;

                return result;
            }
        }
    }
}