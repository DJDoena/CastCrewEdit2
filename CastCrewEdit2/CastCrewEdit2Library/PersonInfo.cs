using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public class PersonInfos
    {
        public PersonInfo[] PersonInfoList;

        public void Serialize(String fileName)
        {
            if (PersonInfoList?.Length > 0)
            {
                foreach (PersonInfo pi in PersonInfoList)
                {
                    if (pi.LastModifiedSpecified == false)
                    {
                        pi.LastModified = DateTime.UtcNow;
                        pi.LastModifiedSpecified = true;
                    }
                }
            }

            DVDProfilerSerializer<PersonInfos>.Serialize(fileName, this);
        }

        public static PersonInfos Deserialize(String fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PersonInfo.CreatorActive = true;

                PersonInfos personInfos = DVDProfilerSerializer<PersonInfos>.Deserialize(fs);

                PersonInfo.CreatorActive = false;

                return (personInfos);
            }
        }
    }

    [Serializable]
    public class PersonInfo
    {
        private String m_FirstName;

        public String FirstName
        {
            [DebuggerStepThrough]
            get
            {
                return (m_FirstName);
            }
            set
            {
                if (m_FirstName != value)
                {
                    SetTimestamp();
                }

                m_FirstName = value;
            }
        }

        private String m_MiddleName;

        public String MiddleName
        {
            [DebuggerStepThrough]
            get
            {
                return (m_MiddleName);
            }
            set
            {
                if (m_MiddleName != value)
                {
                    SetTimestamp();
                }

                m_MiddleName = value;
            }
        }

        private String m_LastName;

        public String LastName
        {
            [DebuggerStepThrough]
            get
            {
                return (m_LastName);
            }
            set
            {
                if (m_LastName != value)
                {
                    SetTimestamp();
                }

                m_LastName = value;
            }
        }

        private String m_BirthYear;

        public String BirthYear
        {
            [DebuggerStepThrough]
            get
            {
                return (m_BirthYear);
            }
            set
            {
                if (m_BirthYear != value)
                {
                    SetTimestamp();
                }

                m_BirthYear = value;
            }
        }

        private Boolean m_BirthYearWasRetrieved = false;

        public Boolean BirthYearWasRetrieved
        {
            [DebuggerStepThrough]
            get
            {
                return (m_BirthYearWasRetrieved);
            }
            set
            {
                if (m_BirthYearWasRetrieved != value)
                {
                    SetTimestamp();
                }

                m_BirthYearWasRetrieved = value;
            }
        }

        private String m_PersonLink = String.Empty;

        public String PersonLink
        {
            [DebuggerStepThrough]
            get
            {
                return (m_PersonLink);
            }
            set
            {
                if (m_PersonLink != value)
                {
                    SetTimestamp();
                }

                m_PersonLink = value;
            }
        }

        private String m_FakeBirthYear;

        public String FakeBirthYear
        {
            [DebuggerStepThrough]
            get
            {
                return (m_FakeBirthYear);
            }
            set
            {
                if (m_FakeBirthYear != value)
                {
                    SetTimestamp();
                }

                m_FakeBirthYear = value;
            }
        }

        private FilmInfo[] m_FilmInfoList = null;

        public FilmInfo[] FilmInfoList
        {
            [DebuggerStepThrough]
            get
            {
                return (m_FilmInfoList);
            }
            set
            {
                if (((m_FilmInfoList != null) && (value == null))
                    || ((m_FilmInfoList == null) && (value != null)))
                {
                    SetTimestamp();
                }
                else if (m_FilmInfoList != null)
                {
                    if (m_FilmInfoList.Length != value.Length)
                    {
                        SetTimestamp();
                    }
                    else
                    {
                        for (Int32 i = 0; i < m_FilmInfoList.Length; i++)
                        {
                            FilmInfo oldFI = m_FilmInfoList[i];

                            FilmInfo newFi = value[i];

                            if ((oldFI.FilmLink != newFi.FilmLink) || (oldFI.Name != newFi.Name))
                            {
                                SetTimestamp();
                            }
                        }
                    }
                }

                m_FilmInfoList = value;
            }
        }

        public DateTime LastModified;

        [XmlIgnore]
        public Boolean LastModifiedSpecified = false;

        [XmlIgnore]
        public String Type = String.Empty;

        [XmlIgnore]
        public String OriginalCredit;

        public static Boolean CreatorActive { get; set; }

        public PersonInfo()
        { }

        public PersonInfo(String type)
        {
            Type = type;
        }

        public PersonInfo(DataGridViewRow row
            , String type)
        {
            Object value = row.Cells[ColumnNames.LastName].Value;

            if (value != null)
            {
                LastName = value.ToString();
            }
            else
            {
                LastName = String.Empty;
            }

            value = row.Cells[ColumnNames.MiddleName].Value;

            if (value != null)
            {
                MiddleName = value.ToString();
            }
            else
            {
                MiddleName = String.Empty;
            }

            value = row.Cells[ColumnNames.FirstName].Value;

            if (value != null)
            {
                FirstName = value.ToString();
            }
            else
            {
                FirstName = String.Empty;
            }

            value = row.Cells[ColumnNames.BirthYear].Value;

            if (value != null)
            {
                BirthYear = value.ToString();
            }
            else
            {
                BirthYear = String.Empty;
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
            Type = personInfo.Type;
            FakeBirthYear = personInfo.FakeBirthYear;
            FilmInfoList = personInfo.FilmInfoList;
            LastModified = personInfo.LastModified;
            LastModifiedSpecified = personInfo.LastModifiedSpecified;
            CreatorActive = false;
        }

        private void SetTimestamp()
        {
            if (CreatorActive == false)
            {
                LastModified = DateTime.UtcNow;
                LastModifiedSpecified = true;
            }
        }

        public override Int32 GetHashCode()
            => (PersonLink.GetHashCode());

        public override Boolean Equals(Object obj)
        {
            PersonInfo other = obj as PersonInfo;

            if (other == null)
            {
                return (false);
            }
            else
            {
                return (PersonLink == other.PersonLink);
            }
        }

        public override String ToString()
            => (FormatActorNameWithBirthYearWithMarkers(true));

        private String FormatPersonNameWithMarkers(Boolean withFilmList)
        {
            StringBuilder name = new StringBuilder();

            if (String.IsNullOrEmpty(FirstName) == false)
            {
                name.Append("<" + FirstName + ">");
            }

            if (String.IsNullOrEmpty(MiddleName) == false)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("{" + MiddleName + "}");
            }

            if (String.IsNullOrEmpty(LastName) == false)
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

            return (name.ToString());
        }

        private String PadNamePart(String namePart
            , IEnumerable<PersonInfo> others
            , Func<PersonInfo, String> getNamePart)
        {
            namePart = namePart ?? String.Empty;

            others = others ?? Enumerable.Empty<PersonInfo>();

            IEnumerable<String> nameParts = others.Select(getNamePart);

            Int32 padding = GetMaxPadding(namePart, nameParts);

            for (Int32 i = namePart.Length; i < padding; i++)
            {
                namePart += "&nbsp;";
            }

            return (namePart);
        }

        private Int32 GetMaxPadding(String namePart
            , IEnumerable<String> others)
        {
            Int32 padding = namePart.Length;

            foreach (String other in others)
            {
                if (other?.Length > padding)
                {
                    padding = other.Length;
                }
            }

            return (padding);
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

        public String FormatPersonNameWithMarkers()
            => (FormatPersonNameWithMarkers(false));

        private String FormatPersonNameWithMarkersAsHtml(Boolean withFilmList
            , IEnumerable<PersonInfo> others)
        {
            StringBuilder name = new StringBuilder();

            String namePart = PadNamePart(FirstName, others, (pi) => pi.FirstName);

            name.Append("<span style=\"color:Blue;\">" + namePart + "</span>");

            namePart = PadNamePart(MiddleName, others, (pi) => pi.MiddleName);

            if ((String.IsNullOrEmpty(namePart) == false) && (name.Length != 0))
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:White; background-color:Black\"><strong>" + namePart + "</strong></span>");
            
            namePart = PadNamePart(LastName, others, (pi) => pi.LastName);

            if ((String.IsNullOrEmpty(namePart) == false) && (name.Length != 0))
            {
                name.Append(" ");
            }

            name.Append("<span style=\"color:Green;\">" + namePart + "</span>");

            if (withFilmList)
            {
                AppendFilmInfoListAsHtml(name);
            }

            return (name.ToString());
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

                for (Int32 i = FilmInfoList.Length - 1; i >= 0; i--)
                {
                    FilmInfo fi = FilmInfoList[i];

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

        public String FormatPersonNameWithMarkersAsHtml(IEnumerable<PersonInfo> others)
            => (FormatPersonNameWithMarkersAsHtml(false, others));

        public String FormatPersonNameWithoutMarkers()
        {
            StringBuilder name = new StringBuilder();

            if (String.IsNullOrEmpty(FirstName) == false)
            {
                name.Append(FirstName);
            }

            if (String.IsNullOrEmpty(MiddleName) == false)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(MiddleName);
            }

            if (String.IsNullOrEmpty(LastName) == false)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append(LastName);
            }

            return (name.ToString());
        }

        public String FormatActorNameWithBirthYearWithMarkers(Boolean useFakeBirthYear)
            => (FormatActorNameWithBirthYearWithMarkers(useFakeBirthYear, false));

        public String FormatActorNameWithBirthYearWithMarkers(Boolean useFakeBirthYear
            , Boolean withFilmList)
        {
            StringBuilder name = new StringBuilder(FormatPersonNameWithMarkers(false));

            if (String.IsNullOrEmpty(BirthYear) == false)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + BirthYear + ")");
            }
            else if ((useFakeBirthYear) && (String.IsNullOrEmpty(FakeBirthYear) == false) && (FakeBirthYear != "0"))
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

            return (name.ToString());
        }

        public String FormatActorNameWithBirthYearWithMarkersAsHtml(Boolean useFakeBirthYear
            , IEnumerable<PersonInfo> others)
            => (FormatActorNameWithBirthYearWithMarkersAsHtml(useFakeBirthYear, false, others));

        public String FormatActorNameWithBirthYearWithMarkersAsHtml(Boolean useFakeBirthYear
            , Boolean withFilmList
            , IEnumerable<PersonInfo> others)
        {
            StringBuilder name = new StringBuilder(FormatPersonNameWithMarkersAsHtml(false, others));

            if (String.IsNullOrEmpty(BirthYear) == false)
            {
                if (name.Length != 0)
                {
                    name.Append(" ");
                }

                name.Append("(" + BirthYear + ")");
            }
            else if ((useFakeBirthYear) && (String.IsNullOrEmpty(FakeBirthYear) == false) && (FakeBirthYear != "0"))
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

            return (name.ToString());
        }

        public void AddFilmInfo(String link, String name)
        {
            List<FilmInfo> filmInfoList = (FilmInfoList == null) ? new List<FilmInfo>(1) : new List<FilmInfo>(FilmInfoList);

            Boolean contains = false;

            for (Int32 i = filmInfoList.Count - 1; i >= 0; i--)
            {
                if (filmInfoList[i].FilmLink == link)
                {
                    FilmInfo temp = filmInfoList[i];

                    temp.Name = name;

                    contains = true;

                    for (Int32 j = i; j < filmInfoList.Count - 1; j++)
                    {
                        filmInfoList[j] = filmInfoList[j + 1];
                    }

                    filmInfoList[filmInfoList.Count - 1] = temp;

                    break;
                }
            }

            if (contains == false)
            {
                filmInfoList.Add(new FilmInfo(link, name));
            }

            if (filmInfoList.Count > 3)
            {
                filmInfoList.RemoveAt(0);
            }

            FilmInfoList = filmInfoList.ToArray();
        }

        public static Int32 CompareForSorting(PersonInfo left
            , PersonInfo right)
        {
            String leftName = GetSortingName(left);

            String rightName = GetSortingName(right);

            return (leftName.CompareTo(rightName));
        }

        private static String GetSortingName(PersonInfo person)
            => (person.LastName + " " + person.FirstName + " " + person.MiddleName + " " + person.PersonLink);
    }

    [Serializable]
    public class FilmInfo
    {
        public String FilmLink;

        public String Name;

        public FilmInfo()
        { }

        public FilmInfo(String link
            , String name)
        {
            FilmLink = link;
            Name = name;
        }
    }

    public class PersonInfoWithoutBirthYear : PersonInfo
    {
        public PersonInfoWithoutBirthYear(DataGridViewRow row
            , String type)
            : base(row, type)
        { }

        public PersonInfoWithoutBirthYear(PersonInfo personInfo)
            : base(personInfo)
        { }

        public override Int32 GetHashCode()
            => (FirstName.GetHashCode() / 3 + MiddleName.GetHashCode() / 3 + LastName.GetHashCode() / 3);

        public override Boolean Equals(Object obj)
        {
            PersonInfoWithoutBirthYear other = obj as PersonInfoWithoutBirthYear;

            if (other == null)
            {
                return (false);
            }
            else
            {
                return ((LastName == other.LastName) && (MiddleName == other.MiddleName) && (FirstName == other.FirstName));
            }
        }
    }
}