using System;
using System.IO;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
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

                FileInfo source = CanGetPhoto(person.PersonLink, out source) ? IMDbParser.GetHeadshot(person) : source;

                if (source == null)
                {
                    return;
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

        private static Boolean CanGetPhoto(String personLink
            , out FileInfo existingFile)
        {
            if (Program.Settings.DefaultValues.OverwriteExistingImages)
            {
                existingFile = null;

                return (true);
            }

            FileInfo jpg = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + personLink + ".jpg");

            if (jpg.Exists)
            {
                existingFile = jpg;

                return (false);
            }

            FileInfo gif = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + personLink + ".gif");

            if (gif.Exists)
            {
                existingFile = gif;

                return (false);
            }

            existingFile = null;

            return (true);
        }
    }
}