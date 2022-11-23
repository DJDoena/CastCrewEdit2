namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using DVDProfilerHelper;

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

            DVDProfilerSerializer<PersonInfos>.Serialize(fileName, this);
        }

        public static PersonInfos Deserialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PersonInfo.CreatorActive = true;

                PersonInfos personInfos = DVDProfilerSerializer<PersonInfos>.Deserialize(fs);

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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
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
                    this.SetTimestamp();
                }
                else if (_filmInfoList != null)
                {
                    if (_filmInfoList.Length != value.Length)
                    {
                        this.SetTimestamp();
                    }
                    else
                    {
                        for (var i = 0; i < _filmInfoList.Length; i++)
                        {
                            var oldFI = _filmInfoList[i];

                            var newFi = value[i];

                            if (oldFI.FilmLink != newFi.FilmLink || oldFI.Name != newFi.Name)
                            {
                                this.SetTimestamp();
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
                this.LastName = value.ToString();
            }
            else
            {
                this.LastName = string.Empty;
            }

            value = row.Cells[ColumnNames.MiddleName].Value;

            if (value != null)
            {
                this.MiddleName = value.ToString();
            }
            else
            {
                this.MiddleName = string.Empty;
            }

            value = row.Cells[ColumnNames.FirstName].Value;

            if (value != null)
            {
                this.FirstName = value.ToString();
            }
            else
            {
                this.FirstName = string.Empty;
            }

            value = row.Cells[ColumnNames.BirthYear].Value;

            if (value != null)
            {
                this.BirthYear = value.ToString();
            }
            else
            {
                this.BirthYear = string.Empty;
            }

            this.PersonLink = row.Cells[ColumnNames.Link].Value.ToString();

            Type = type;
        }

        public PersonInfo(PersonInfo personInfo)
        {
            CreatorActive = true;

            this.LastName = personInfo.LastName;
            this.MiddleName = personInfo.MiddleName;
            this.FirstName = personInfo.FirstName;
            this.BirthYear = personInfo.BirthYear;
            this.BirthYearWasRetrieved = personInfo.BirthYearWasRetrieved;
            this.PersonLink = personInfo.PersonLink;
            this.FakeBirthYear = personInfo.FakeBirthYear;
            this.FilmInfoList = personInfo.FilmInfoList;

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

        public override int GetHashCode() => this.PersonLink.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is PersonInfo other))
            {
                return false;
            }
            else
            {
                return this.PersonLink == other.PersonLink;
            }
        }

        public override string ToString() => this.FormatActorNameWithBirthYearWithMarkers(true);

        private string FormatPersonNameWithMarkers(bool withFilmList)
        {
            var name = new StringBuilder();

            if (!string.IsNullOrEmpty(this.FirstName))
            {
                name.Append("<" + this.FirstName + ">");
            }

            if (!string.IsNullOrEmpty(this.MiddleName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("{" + this.MiddleName + "}");
            }

            if (!string.IsNullOrEmpty(this.LastName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("[" + this.LastName + "]");
            }

            if (withFilmList)
            {
                this.AppendFilmInfoList(name);
            }

            return name.ToString();
        }

        private string PadNamePart(string namePart, IEnumerable<PersonInfo> others, Func<PersonInfo, string> getNamePart)
        {
            namePart = namePart ?? string.Empty;

            others = others ?? Enumerable.Empty<PersonInfo>();

            var nameParts = others.Select(getNamePart);

            var padding = this.GetMaxPadding(namePart, nameParts);

            for (var i = namePart.Length; i < padding; i++)
            {
                namePart += "&nbsp;";
            }

            return namePart;
        }

        private int GetMaxPadding(string namePart, IEnumerable<string> others)
        {
            int padding = namePart.Length;

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
            if (this.FilmInfoList?.Length > 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(");
                name.Append(this.FilmInfoList[this.FilmInfoList.Length - 1].Name);
                name.Append(")");
            }
        }

        public string FormatPersonNameWithMarkers() => this.FormatPersonNameWithMarkers(false);

        private string FormatPersonNameWithMarkersAsHtml(bool withFilmList, IEnumerable<PersonInfo> others)
        {
            var name = new StringBuilder();

            var namePart = this.PadNamePart(this.FirstName, others, (pi) => pi.FirstName);

            name.Append("<span style=\"color:Blue;\">" + namePart + "</span>");

            namePart = this.PadNamePart(this.MiddleName, others, (pi) => pi.MiddleName);

            if (!string.IsNullOrEmpty(namePart) && name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:White; background-color:Black\"><strong>" + namePart + "</strong></span>");

            namePart = this.PadNamePart(this.LastName, others, (pi) => pi.LastName);

            if (!string.IsNullOrEmpty(namePart) && name.Length != 0)
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:Green;\">" + namePart + "</span>");

            if (withFilmList)
            {
                this.AppendFilmInfoListAsHtml(name);
            }

            return name.ToString();
        }

        private void AppendFilmInfoListAsHtml(StringBuilder name)
        {
            if (this.FilmInfoList?.Length > 0)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(");

                for (var i = this.FilmInfoList.Length - 1; i >= 0; i--)
                {
                    var fi = this.FilmInfoList[i];

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

        public string FormatPersonNameWithMarkersAsHtml(IEnumerable<PersonInfo> others) => this.FormatPersonNameWithMarkersAsHtml(false, others);

        public string FormatPersonNameWithoutMarkers()
        {
            var name = new StringBuilder();

            if (!string.IsNullOrEmpty(this.FirstName))
            {
                name.Append(this.FirstName);
            }

            if (!string.IsNullOrEmpty(this.MiddleName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(this.MiddleName);
            }

            if (!string.IsNullOrEmpty(this.LastName))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(this.LastName);
            }

            return name.ToString();
        }

        public string FormatActorNameWithBirthYearWithMarkers(bool useFakeBirthYear) => this.FormatActorNameWithBirthYearWithMarkers(useFakeBirthYear, false);

        public string FormatActorNameWithBirthYearWithMarkers(bool useFakeBirthYear, bool withFilmList)
        {
            var name = new StringBuilder(this.FormatPersonNameWithMarkers(false));

            if (!string.IsNullOrEmpty(this.BirthYear))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + this.BirthYear + ")");
            }
            else if (useFakeBirthYear && !string.IsNullOrEmpty(this.FakeBirthYear) && this.FakeBirthYear != "0")
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + this.FakeBirthYear + ")");
            }

            if (withFilmList)
            {
                this.AppendFilmInfoList(name);
            }

            return name.ToString();
        }

        public string FormatActorNameWithBirthYearWithMarkersAsHtml(bool useFakeBirthYear, IEnumerable<PersonInfo> others) => this.FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYear, false, others);

        public string FormatActorNameWithBirthYearWithMarkersAsHtml(bool useFakeBirthYear, bool withFilmList, IEnumerable<PersonInfo> others)
        {
            var name = new StringBuilder(this.FormatPersonNameWithMarkersAsHtml(false, others));

            if (!string.IsNullOrEmpty(this.BirthYear))
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + this.BirthYear + ")");
            }
            else if (useFakeBirthYear && !string.IsNullOrEmpty(this.FakeBirthYear) && this.FakeBirthYear != "0")
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + this.FakeBirthYear + ")");
            }

            if (withFilmList)
            {
                this.AppendFilmInfoListAsHtml(name);
            }

            return name.ToString();
        }

        public void AddFilmInfo(string link, string name)
        {
            var filmInfoList = this.FilmInfoList == null
                ? new List<FilmInfo>(1)
                : new List<FilmInfo>(this.FilmInfoList);

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

            this.FilmInfoList = filmInfoList.ToArray();
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

        public override int GetHashCode() => this.FirstName.GetHashCode() ^ this.MiddleName.GetHashCode() ^ this.LastName.GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is PersonInfoWithoutBirthYear other))
            {
                return false;
            }
            else
            {
                var result = this.LastName == other.LastName
                    && this.MiddleName == other.MiddleName
                    && this.FirstName == other.FirstName;

                return result;
            }
        }
    }
}