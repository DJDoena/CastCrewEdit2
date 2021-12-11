namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Forms;
    using Helper;

    [Serializable]
    public sealed class SessionData
    {
        public Dictionary<PersonInfo, string> BirthYearCache;

        public Dictionary<PersonInfo, FileInfo> HeadshotCache;

        public bool FirstRunGetHeadShots;

        public static void Serialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var sd = new SessionData()
                {
                    BirthYearCache = IMDbParser.BirthYearCache,
                    HeadshotCache = IMDbParser.HeadshotCache,
                    FirstRunGetHeadShots = CastCrewEdit2ParseBaseForm.FirstRunGetHeadShots,
                };

                var bf = new BinaryFormatter();

                bf.Serialize(fs, sd);
            }
        }

        public static string Deserialize(string fileName)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bf = new BinaryFormatter();

                    var sd = (SessionData)(bf.Deserialize(fs));

                    IMDbParser.BirthYearCache = sd.BirthYearCache;

                    IMDbParser.HeadshotCache = new Dictionary<PersonInfo, FileInfo>(sd.HeadshotCache.Count);

                    CastCrewEdit2ParseBaseForm.FirstRunGetHeadShots = sd.FirstRunGetHeadShots;

                    foreach (KeyValuePair<PersonInfo, FileInfo> kvp in sd.HeadshotCache)
                    {
                        if (kvp.Value != null)
                        {
                            kvp.Value.Refresh();

                            if (kvp.Value.Exists)
                            {
                                IMDbParser.HeadshotCache.Add(kvp.Key, kvp.Value);
                            }
                        }
                        else
                        {
                            IMDbParser.HeadshotCache.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            catch
            {
                IMDbParser.BirthYearCache = new Dictionary<PersonInfo, string>(1000);

                IMDbParser.HeadshotCache = new Dictionary<PersonInfo, FileInfo>(1000);
            }

            return IMDbParser.BirthYearCache.Count.ToString();
        }
    }
}