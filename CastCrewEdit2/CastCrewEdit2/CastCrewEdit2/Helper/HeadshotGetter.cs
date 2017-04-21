using System;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    internal static class HeadshotGetter
    {
        private static readonly Object DVDPLock;

        private static readonly Object CCVLock;

        static HeadshotGetter()
        {
            DVDPLock = new Object();
            CCVLock = new Object();
        }

        internal static void Get(Boolean useFakeBirthYears
            , Boolean isCast
            , Action<MessageEntry> addMessage
            , PersonInfo person)
        {
            if ((person.FirstName != FirstNames.Title) && (person.FirstName != FirstNames.Divider))
            {
                FileInfo source = IMDbParser.GetHeadshot(person);

                if (source != null)
                {

                    String target = ProfilePhotoHelper.FileNameFromCreditName(person.FirstName, person.MiddleName, person.LastName, 0);

                    Int32 birthYear;
                    if (String.IsNullOrEmpty(person.BirthYear) == false)
                    {
                        birthYear = Int32.Parse(person.BirthYear);

                        target = ProfilePhotoHelper.FileNameFromCreditName(person.FirstName, person.MiddleName, person.LastName, birthYear);
                    }
                    else if (useFakeBirthYears)
                    {
                        String fakeBirthYear = DataGridViewHelper.CreateFakeBirthYearAsString(person, isCast, addMessage);

                        if (String.IsNullOrEmpty(fakeBirthYear) == false)
                        {
                            birthYear = Int32.Parse(fakeBirthYear);
                        }
                    }

                    try
                    {
                        lock (DVDPLock)
                        {
                            source.CopyTo(Program.RootPath + @"\Images\DVD Profiler\" + target + source.Extension, true);
                        }
                    }
                    catch (IOException)
                    { }

                    target = person.FirstName;

                    if (String.IsNullOrEmpty(person.MiddleName) == false)
                    {
                        target += " " + person.MiddleName;
                    }

                    target += " " + person.LastName;

                    target = ProfilePhotoHelper.CleanupFilename(target);

                    try
                    {
                        lock (CCVLock)
                        {
                            source.CopyTo(Program.RootPath + @"\Images\CCViewer\" + target + source.Extension, true);
                        }
                    }
                    catch (IOException)
                    { }
                }
            }
        }
    }
}
