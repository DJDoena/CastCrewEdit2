namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;

    [Serializable]
    public class Settings
    {
        public SizableForm MainForm;

        public BaseForm SettingsForm;

        public SizableForm EpisodesForm;

        public SizableForm EpisodeForm;

        public SizableForm EditConfigFilesForm;

        public SizableForm EditKnownNamesConfigFileForm;

        public DefaultValues DefaultValues;

        public string CurrentVersion;
    }
}