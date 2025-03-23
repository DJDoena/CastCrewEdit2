namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

    [Serializable]
    public sealed class SessionData
    {
        public Dictionary<PersonInfo, string> BirthYearCache;

        public Dictionary<PersonInfo, FileInfo> HeadshotCache;

        public Dictionary<string, string> WebSites;

        public bool FirstRunGetHeadShots;

        public SessionData()
        {
            BirthYearCache = new Dictionary<PersonInfo, string>(1000);

            HeadshotCache = new Dictionary<PersonInfo, FileInfo>(1000);

            WebSites = new Dictionary<string, string>();
        }

        public static void Serialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var sd = IMDbParser.SessionData;

                var bf = new BinaryFormatter();

                bf.Serialize(fs, sd);
            }
        }

        public static string Deserialize(string fileName)
        {
            try
            {
                using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                var bf = new BinaryFormatter();

                var sd = (SessionData)(bf.Deserialize(fs));

                sd.BirthYearCache ??= new Dictionary<PersonInfo, string>(1000);

                sd.HeadshotCache ??= new Dictionary<PersonInfo, FileInfo>(1000);

                sd.WebSites ??= new Dictionary<string, string>();

                var headshotCache = HeadshotParser.HeadshotCache;

                foreach (var kvp in sd.HeadshotCache)
                {
                    if (kvp.Value != null)
                    {
                        kvp.Value.Refresh();

                        if (kvp.Value.Exists)
                        {
                            headshotCache.Add(kvp.Key, kvp.Value);
                        }
                    }
                    else
                    {
                        headshotCache.Add(kvp.Key, kvp.Value);
                    }
                }

                sd.HeadshotCache = headshotCache;

                IMDbParser.SessionData = sd;
            }
            catch
            {
                IMDbParser.SessionData = new SessionData();
            }

            return BirthYearParser.BirthYearCache.Count.ToString();
        }
    }
}