namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.IO;
    using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
    using DVDProfilerHelper;

    internal static class HeadshotGetter
    {
        private static readonly object _profilerLock;

        private static readonly object _ccViewerLock;

        static HeadshotGetter()
        {
            _profilerLock = new object();

            _ccViewerLock = new object();
        }

        internal static void Get(bool useFakeBirthYears, bool isCast, Action<MessageEntry> addMessage, PersonInfo person)
        {
            if (person.FirstName != FirstNames.Title
                && person.FirstName != FirstNames.Divider
                && person.FirstName != FirstNames.GroupDividerStart
                && person.FirstName != FirstNames.GroupDividerEnd)
            {
                var target = ProfilePhotoHelper.FileNameFromCreditName(person.FirstName, person.MiddleName, person.LastName, 0);

                if (!string.IsNullOrEmpty(person.BirthYear))
                {
                    var birthYear = int.Parse(person.BirthYear);

                    target = ProfilePhotoHelper.FileNameFromCreditName(person.FirstName, person.MiddleName, person.LastName, birthYear);
                }
                else if (useFakeBirthYears)
                {
                    var fakeBirthYear = DataGridViewHelper.CreateFakeBirthYearAsString(person, isCast, addMessage);

                    if (!string.IsNullOrEmpty(fakeBirthYear))
                    {
                        var birthYear = int.Parse(fakeBirthYear);

                        target = ProfilePhotoHelper.FileNameFromCreditName(person.FirstName, person.MiddleName, person.LastName, birthYear);
                    }
                }

                FileInfo source = CanGetPhoto(person.PersonLink, out source)
                    ? HeadshotParser.GetHeadshot(person)
                    : source;

                if (source == null)
                {
                    return;
                }

                try
                {
                    lock (_profilerLock)
                    {
                        source.CopyTo(Program.RootPath + @"\Images\DVD Profiler\" + target + source.Extension, true);
                    }
                }
                catch (IOException)
                { }

                target = person.FirstName;

                if (!string.IsNullOrEmpty(person.MiddleName))
                {
                    target += " " + person.MiddleName;
                }

                target += " " + person.LastName;

                target = ProfilePhotoHelper.CleanupFilename(target);

                try
                {
                    lock (_ccViewerLock)
                    {
                        source.CopyTo(Program.RootPath + @"\Images\CCViewer\" + target + source.Extension, true);
                    }
                }
                catch (IOException)
                { }
            }
        }

        private static bool CanGetPhoto(string personLink, out FileInfo existingFile)
        {
            if (Program.DefaultValues.OverwriteExistingImages)
            {
                existingFile = null;

                return true;
            }

            var jpg = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + personLink + ".jpg");

            if (jpg.Exists)
            {
                existingFile = jpg;

                return false;
            }

            var gif = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + personLink + ".gif");

            if (gif.Exists)
            {
                existingFile = gif;

                return false;
            }

            var png = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + personLink + ".png");

            if (png.Exists)
            {
                existingFile = png;

                return false;
            }

            existingFile = null;

            return true;
        }
    }
}