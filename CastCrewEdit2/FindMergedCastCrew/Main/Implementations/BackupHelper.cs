using System;
using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal static class BackupHelper
    {
        internal static void BackupFile(String fileName
            , IIOServices ioServices)
        {
            if (ioServices.File.Exists(fileName))
            {
                IFileInfo fi = ioServices.GetFileInfo(fileName);

                String backupFile = fi.Name + ".bak";

                backupFile = ioServices.Path.Combine(fi.DirectoryName, backupFile);

                ioServices.File.Move(fileName, backupFile);
            }
        }
    }
}