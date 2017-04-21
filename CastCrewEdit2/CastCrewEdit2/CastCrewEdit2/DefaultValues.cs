using System;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public class DefaultValues
    {
        public Boolean ParseFirstNameInitialsIntoFirstAndMiddleName = true;

        public Boolean TakeBirthYearFromLocalCache = false;

        public Boolean RetrieveBirthYearWhenLocalCacheEmpty = false;

        public Boolean GetBirthYearsDirectlyAfterNameParsing = false;

        public Boolean ParseCast = true;

        public Boolean ParseRoleSlash = false;

        public Boolean ParseVoiceOf = false;

        public Boolean IgnoreUncredited = false;

        public Boolean IgnoreCreditOnly = false;

        public Boolean IgnoreScenesDeleted = false;

        public Boolean IgnoreArchiveFootage = false;

        public Boolean IgnoreLanguageVersion = false;

        public Boolean IgnoreUnconfirmed = false;

        public Boolean RetainCastCreditedAs = true;

        public Boolean ParseCrew = true;

        public Boolean IncludeCustomCredits = true;

        public Boolean RetainOriginalCredit = true;

        public Boolean IncludePrefixOnOtherCredits = false;

        public Boolean CapitalizeCustomRole = true;

        public Boolean RetainCrewCreditedAs = true;

        public Boolean CreditTypeDirection = true;

        public Boolean CreditTypeWriting = true;

        public Boolean CreditTypeProduction = true;

        public Boolean CreditTypeCinematography = true;

        public Boolean CreditTypeFilmEditing = true;

        public Boolean CreditTypeMusic = true;

        public Boolean CreditTypeSound = true;

        public Boolean CreditTypeArt = true;

        public Boolean CreditTypeOther = false;

        public Boolean CreditTypeSoundtrack = false;

        public Boolean DisableParsingCompleteMessageBox = false;

        public Boolean DisableParsingCompleteMessageBoxForGetBirthYears = false;

        public Boolean DisableCopyingSuccessfulMessageBox = false;

        public Boolean DisableParsingCompleteMessageBoxForGetHeadshots = false;

        public Boolean DisableDuplicatesMessageBox = false;

        public String EpisodeDividerFormat = "{season}.{episode}";

        public Boolean UseDoubleDigitsEpisodeNumber = true;

        public Boolean GetCastHeadShots = true;

        public Boolean GetCrewHeadShots = false;

        public Boolean AutoCopyHeadShots = false;

        public Boolean DownloadTrivia = false;

        public Boolean DownloadGoofs = false;

        public Boolean GetHeadShotsDirectlyAfterNameParsing = false;

        public Boolean UseFakeBirthYears = false;

        public Boolean SaveLogFile = false;

        public Boolean StoreHeadshotsPerSession = false;

        public Boolean CheckPersonLinkForRedirect = true;

        [XmlIgnore]
        internal String CreditPhotosFolder
        {
            get
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Invelos Software\DVD Profiler", false);

                String path = (String)(regKey.GetValue("PathCreditPhotos", String.Empty));

                return (path);
            }
        }
    }
}