namespace DoenaSoft.DVDProfiler.FindMergedCastCrew
{
    internal interface IWindowFactory
    {
        void OpenMainWindow();

        void OpenOutputWindow(ILog log);
    }
}
