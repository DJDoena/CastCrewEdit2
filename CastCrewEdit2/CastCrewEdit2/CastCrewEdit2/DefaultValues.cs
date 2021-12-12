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

            var clone = new DefaultValues()
            {
                ParseRoleSlash = original.ParseRoleSlash,
                ParseVoiceOf = original.ParseVoiceOf,
                IgnoreUncredited = original.IgnoreUncredited,
                IgnoreCreditOnly = original.IgnoreCreditOnly,
                IgnoreScenesDeleted = original.IgnoreScenesDeleted,
                IgnoreArchiveFootage = original.IgnoreArchiveFootage,
                IgnoreLanguageVersion = original.IgnoreLanguageVersion,
                IncludeCustomCredits = original.IncludeCustomCredits,
                RetainCastCreditedAs = original.RetainCastCreditedAs,
                RetainCrewCreditedAs = original.RetainCrewCreditedAs,
                RetainOriginalCredit = original.RetainOriginalCredit,
                IncludePrefixOnOtherCredits = original.IncludePrefixOnOtherCredits,
                CapitalizeCustomRole = original.CapitalizeCustomRole,
                CreditTypeDirection = original.CreditTypeDirection,
                CreditTypeWriting = original.CreditTypeWriting,
                CreditTypeProduction = original.CreditTypeProduction,
                CreditTypeCinematography = original.CreditTypeCinematography,
                CreditTypeFilmEditing = original.CreditTypeFilmEditing,
                CreditTypeMusic = original.CreditTypeMusic,
                CreditTypeSound = original.CreditTypeSound,
                CreditTypeArt = original.CreditTypeArt,
                CreditTypeOther = original.CreditTypeOther,
                CreditTypeSoundtrack = original.CreditTypeSoundtrack,
                ParseFirstNameInitialsIntoFirstAndMiddleName = original.ParseFirstNameInitialsIntoFirstAndMiddleName,
                CheckPersonLinkForRedirect = original.CheckPersonLinkForRedirect,
                AutoCopyHeadShots = original.AutoCopyHeadShots,
                DisableCopyingSuccessfulMessageBox = original.DisableCopyingSuccessfulMessageBox,
                DisableDuplicatesMessageBox = original.DisableDuplicatesMessageBox,
                DisableParsingCompleteMessageBox = original.DisableParsingCompleteMessageBox,
                DisableParsingCompleteMessageBoxForGetBirthYears = original.DisableParsingCompleteMessageBoxForGetBirthYears,
                DisableParsingCompleteMessageBoxForGetHeadshots = original.DisableParsingCompleteMessageBoxForGetHeadshots,
                DownloadGoofs = original.DownloadGoofs,
                DownloadTrivia = original.DownloadTrivia,
                EpisodeDividerFormat = original.EpisodeDividerFormat,
                GetBirthYearsDirectlyAfterNameParsing = original.GetBirthYearsDirectlyAfterNameParsing,
                GetCastHeadShots = original.GetCastHeadShots,
                GetCrewHeadShots = original.GetCrewHeadShots,
                GetHeadShotsDirectlyAfterNameParsing = original.GetHeadShotsDirectlyAfterNameParsing,
                OverwriteExistingImages = original.OverwriteExistingImages,
                ParseCast = original.ParseCast,
                ParseCrew = original.ParseCrew,
                RetrieveBirthYearWhenLocalCacheEmpty = original.RetrieveBirthYearWhenLocalCacheEmpty,
                SaveLogFile = original.SaveLogFile,
                SendToCastCrewCopyPaste = original.SendToCastCrewCopyPaste,
                StoreHeadshotsPerSession = original.StoreHeadshotsPerSession,
                TakeBirthYearFromLocalCache = original.TakeBirthYearFromLocalCache,
                UseDoubleDigitsEpisodeNumber = original.UseDoubleDigitsEpisodeNumber,
                UseFakeBirthYears = original.UseFakeBirthYears,
                IgnoreUnconfirmed = original.IgnoreUnconfirmed,
                GroupSoundtrackCredits = original.GroupSoundtrackCredits,
            };

            return clone;
        }
    }
}