namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System.Xml.Serialization;
    using Microsoft.Win32;

    public sealed class DefaultValues
    {
        public bool ParseFirstNameInitialsIntoFirstAndMiddleName = true;

        public bool TakeBirthYearFromLocalCache = false;

        public bool RetrieveBirthYearWhenLocalCacheEmpty = false;

        public bool GetBirthYearsDirectlyAfterNameParsing = false;

        public bool ParseCast = true;

        public bool ParseRoleSlash = false;

        public bool ParseVoiceOf = false;

        public bool IgnoreUncredited = false;

        public bool IgnoreCreditOnly = false;

        public bool IgnoreScenesDeleted = false;

        public bool IgnoreArchiveFootage = false;

        public bool IgnoreLanguageVersion = false;

        public bool IgnoreUnconfirmed = false;

        public bool RetainCastCreditedAs = true;

        public bool ParseCrew = true;

        public bool IncludeCustomCredits = true;

        public bool RetainOriginalCredit = true;

        public bool IncludePrefixOnOtherCredits = false;

        public bool CapitalizeCustomRole = true;

        public bool RetainCrewCreditedAs = true;

        public bool CreditTypeDirection = true;

        public bool CreditTypeWriting = true;

        public bool CreditTypeProduction = true;

        public bool CreditTypeCinematography = true;

        public bool CreditTypeFilmEditing = true;

        public bool CreditTypeMusic = true;

        public bool CreditTypeSound = true;

        public bool CreditTypeArt = true;

        public bool CreditTypeOther = false;

        public bool CreditTypeSoundtrack = false;

        public bool DisableParsingCompleteMessageBox = false;

        public bool DisableParsingCompleteMessageBoxForGetBirthYears = false;

        public bool DisableCopyingSuccessfulMessageBox = false;

        public bool DisableParsingCompleteMessageBoxForGetHeadshots = false;

        public bool DisableDuplicatesMessageBox = false;

        public string EpisodeDividerFormat = "{season}.{episode}";

        public bool UseDoubleDigitsEpisodeNumber = true;

        public bool GetCastHeadShots = true;

        public bool GetCrewHeadShots = false;

        public bool OverwriteExistingImages = true;

        public bool AutoCopyHeadShots = false;

        public bool DownloadTrivia = false;

        public bool DownloadGoofs = false;

        public bool GetHeadShotsDirectlyAfterNameParsing = false;

        public bool UseFakeBirthYears = false;

        public bool SaveLogFile = false;

        public bool StoreHeadshotsPerSession = false;

        public bool CheckPersonLinkForRedirect = true;

        public bool SendToCastCrewCopyPaste = false;

        public bool GroupSoundtrackCredits = false;

        public bool StandardizeJuniorSenior = false;

        [XmlIgnore]
        internal string CreditPhotosFolder
        {
            get
            {
                var regKey = Registry.CurrentUser.OpenSubKey(@"Software\Invelos Software\DVD Profiler", false);

                var path = (string)regKey.GetValue("PathCreditPhotos", string.Empty);

                return path;
            }
        }

        public static DefaultValues GetFromProgramSettings()
        {
            var original = Program.DefaultValues;

            var clone = new DefaultValues();

            var fields = original.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                var value = field.GetValue(original);

                field.SetValue(clone, value);
            }

            return clone;
        }
    }
}