﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.DVDProfilerXML;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;
using DoenaSoft.ToolBox.Generics;
using Microsoft.Win32;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public static class Program
    {
        private static readonly WindowHandle s_WindowHandle = new WindowHandle();

        [STAThread()]
        public static void Main()
        {
            Console.WriteLine("This program converts the Persons.xml file of Cast/Crew Edit 2 version 1.5.x");
            Console.WriteLine("and older to the new version 1.6.x.");
            Console.WriteLine();
            Console.WriteLine("For this, you need an complete DVD Profiler collection.xml and the original");
            Console.WriteLine("Persons.xml.");
            Console.WriteLine();
            Console.WriteLine("Please export your DVD Profiler collection via DVD Proiler -> File ->");
            Console.WriteLine("Export Profile Database and ensure that it exports the complete Cast");
            Console.WriteLine("and Crew section.");
            Console.WriteLine();
            Console.WriteLine("When the export is done, press <Enter> to continue.");
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Please select a \"collection.xml\" and a target location for the Access database!");
            Console.WriteLine("(You should see a file dialog. If not, please minimize your other programs.)");
            try
            {
                string collectionFile;
                string personsFile;
                PersonInfos personInfoList;
                Dictionary<string, List<PersonInfo>> personCache;
                Dictionary<string, List<PersonInfo>> castCache;
                Dictionary<string, List<PersonInfo>> crewCache;
                Collection collection;
                FileInfo personFI;

                using (var ofd = new OpenFileDialog())
                {
                    ofd.CheckFileExists = true;
                    ofd.Filter = "collection.xml|*.xml";
                    ofd.RestoreDirectory = true;
                    ofd.Title = "Select collection.xml file";
                    if (ofd.ShowDialog(s_WindowHandle) == DialogResult.OK)
                    {
                        collectionFile = ofd.FileName;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Aborted.");
                        return;
                    }
                }
                using (var ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = GetDefaultPath();
                    ofd.CheckFileExists = true;
                    ofd.Filter = "Persons.xml|Persons.xml";
                    ofd.RestoreDirectory = true;
                    ofd.Title = "Select Persons.xml file";
                    if (ofd.ShowDialog(s_WindowHandle) == DialogResult.OK)
                    {
                        personsFile = ofd.FileName;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Aborted.");
                        return;
                    }
                }
                personFI = new FileInfo(personsFile);
                if (MessageBox.Show(s_WindowHandle, "Backup Personx.xml before continuing?", "Backup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    File.Copy(personsFile, personsFile + ".org", true);
                }
                personInfoList = PersonInfos.Deserialize(personsFile);
                collection = XmlSerializer<Collection>.Deserialize(collectionFile);
                if ((personInfoList != null) && (personInfoList.PersonInfoList != null)
                  && (personInfoList.PersonInfoList.Length > 0))
                {
                    personCache = new Dictionary<string, List<PersonInfo>>(personInfoList.PersonInfoList.Length);
                    foreach (var personInfo in personInfoList.PersonInfoList)
                    {
                        StringBuilder keyBuilder;
                        string key;

                        keyBuilder = new StringBuilder();
                        keyBuilder.Append(personInfo.FirstName);
                        keyBuilder.Append("_");
                        keyBuilder.Append(personInfo.MiddleName);
                        keyBuilder.Append("_");
                        keyBuilder.Append(personInfo.LastName);
                        keyBuilder.Append("_");
                        if (string.IsNullOrEmpty(personInfo.BirthYear))
                        {
                            keyBuilder.Append("0");
                        }
                        else
                        {
                            keyBuilder.Append(personInfo.BirthYear);
                        }
                        key = keyBuilder.ToString().ToLower();
                        if (personCache.ContainsKey(key) == false)
                        {
                            List<PersonInfo> list;

                            list = new List<PersonInfo>(1);
                            list.Add(personInfo);
                            personCache.Add(key, list);
                        }
                        else
                        {
                            personCache[key].Add(personInfo);
                        }
                    }

                }
                else
                {
                    personCache = new Dictionary<string, List<PersonInfo>>(0);
                }
                castCache = new Dictionary<string, List<PersonInfo>>(personCache.Count);
                crewCache = new Dictionary<string, List<PersonInfo>>(personCache.Count);
                if ((collection != null) && (collection.DVDList != null) && (collection.DVDList.Length > 0))
                {
                    foreach (var dvd in collection.DVDList)
                    {
                        CheckForMatch(personCache, dvd.CastList, castCache);
                        CheckForMatch(personCache, dvd.CrewList, crewCache);
                    }
                }
                foreach (var kvp in castCache)
                {
                    personCache.Remove(kvp.Key);
                }
                foreach (var kvp in crewCache)
                {
                    if (personCache.ContainsKey(kvp.Key))
                    {
                        personCache.Remove(kvp.Key);
                    }
                }
                FlashPersonCache(personFI.DirectoryName, "Persons.xml.old", personCache);
                FlashPersonCache(personFI.DirectoryName, "Cast.xml", castCache);
                FlashPersonCache(personFI.DirectoryName, "Crew.xml", crewCache);
                File.Delete(personFI.FullName);
                Console.WriteLine();
                Console.WriteLine("Finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }

        private static void CheckForMatch(Dictionary<string, List<PersonInfo>> personCache, object[] list, Dictionary<string, List<PersonInfo>> cache)
        {
            if ((list != null) && (list.Length > 0))
            {
                foreach (var potentialPerson in list)
                {
                    IPerson person;

                    person = potentialPerson as IPerson;
                    if (person != null)
                    {
                        StringBuilder keyBuilder;
                        string key;

                        keyBuilder = new StringBuilder();
                        keyBuilder.Append(person.FirstName);
                        keyBuilder.Append("_");
                        keyBuilder.Append(person.MiddleName);
                        keyBuilder.Append("_");
                        keyBuilder.Append(person.LastName);
                        keyBuilder.Append("_");
                        keyBuilder.Append(person.BirthYear);
                        key = keyBuilder.ToString().ToLower();
                        if ((personCache.ContainsKey(key)) && (cache.ContainsKey(key) == false))
                        {
                            cache.Add(key, personCache[key]);
                        }
                    }
                }
            }
        }

        private static void FlashPersonCache(string path, string fileName, Dictionary<string, List<PersonInfo>> personCache)
        {
            PersonInfos personInfoList;
            List<PersonInfo> list;

            personInfoList = new PersonInfos();
            list = new List<PersonInfo>(personCache.Count + 1000);
            foreach (var kvp in personCache)
            {
                list.AddRange(kvp.Value);
            }
            list.Sort
                (new Comparison<PersonInfo>
                    (
                        delegate (PersonInfo left, PersonInfo right)
                        {
                            return (left.PersonLink.CompareTo(right.PersonLink));
                        }
                    )
                );
            personInfoList.PersonInfoList = list.ToArray();
            personInfoList.Serialize(Path.Combine(path, fileName));
        }

        private static string GetDefaultPath()
        {
            RegistryKey regKey;
            string path;

            regKey = Registry.CurrentUser.OpenSubKey(@"Software\Doena Soft.\CastCrewEdit2", false);
            path = string.Empty;
            if (regKey != null)
            {
                path = (string)(regKey.GetValue("DataRoot", string.Empty));
            }
            if (string.IsNullOrEmpty(path) == false)
            {
                path = Path.Combine(path, "Data");
                if (Directory.Exists(path))
                {
                    return (path);
                }
            }
            path = Path.Combine(Environment.CurrentDirectory, "Data");
            if (Directory.Exists(path))
            {
                return (path);
            }
            return (Environment.CurrentDirectory);
        }
    }
}