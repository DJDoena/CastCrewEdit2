using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal static class BackupHelper
    {
        internal static void BackupFile(string fileName
            , IIOServices ioServices)
        {
            if (ioServices.File.Exists(fileName))
            {
                var fi = ioServices.GetFile(fileName);

                var backupFile = fi.Name + ".bak";

                backupFile = ioServices.Path.Combine(fi.FolderName, backupFile);

                ioServices.File.Move(fileName, backupFile);
            }
        }
    }
}