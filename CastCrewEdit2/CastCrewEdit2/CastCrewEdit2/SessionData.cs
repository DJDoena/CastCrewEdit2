using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    [Serializable()]
    public sealed class SessionData
    {
        public Dictionary<PersonInfo, String> BirthYearCache;
        public Dictionary<PersonInfo, FileInfo> HeadshotCache;
        public Boolean FirstRunGetHeadShots;

        public static void Serialize(String fileName)
        {
            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter bf;
                SessionData sd;

                sd = new SessionData();
                sd.BirthYearCache = IMDbParser.BirthYearCache;
                sd.HeadshotCache = IMDbParser.HeadshotCache;
                sd.FirstRunGetHeadShots = CastCrewEdit2ParseBaseForm.FirstRunGetHeadShots;
                bf = new BinaryFormatter();
                bf.Serialize(fs, sd);
            }
        }

        public static String Deserialize(String fileName)
        {
            try
            {
                using(FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter bf;
                    SessionData sd;

                    bf = new BinaryFormatter();
                    sd = (SessionData)(bf.Deserialize(fs));
                    IMDbParser.BirthYearCache = sd.BirthYearCache;
                    IMDbParser.HeadshotCache = new Dictionary<PersonInfo, FileInfo>(sd.HeadshotCache.Count);
                    CastCrewEdit2ParseBaseForm.FirstRunGetHeadShots = sd.FirstRunGetHeadShots;
                    foreach(KeyValuePair<PersonInfo, FileInfo> kvp in sd.HeadshotCache)
                    {
                        if(kvp.Value != null)
                        {
                            kvp.Value.Refresh();
                            if(kvp.Value.Exists)
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
                IMDbParser.BirthYearCache = new Dictionary<PersonInfo, String>(1000);
                IMDbParser.HeadshotCache = new Dictionary<PersonInfo, FileInfo>(1000);
            }
            return (IMDbParser.BirthYearCache.Count.ToString());
        }
    }
}
