using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;

namespace UnitTests
{
    public static class Tester
    {
        private static readonly WindowHandle s_WindowHandle = new WindowHandle();

        private const string Winnetou = "tt0057687";
        private const string EmmaWatson = "nm0914612";
        private const string AlexanderRhodes = "nm4659673";
        //private const string Tamara = "nm0848453";
        private const string ThisIsSpinalTap = "tt0088258";
        private const string Ferdinand = "tt1307254";
        private const string Yojimbo = "tt0055630";
        private const string HotShots = "tt0102059";
        private const string LoveIsTheDrug = "tt0266732";
        private const string FridayNightLights = "tt0758745";
        private const string ABiggerSplash = "tt2056771";
        private const string Roots = "tt4338588";

        [STAThread]
        public static void Main()
        {
            TestFixtureSetup();
            Tests();
        }

        private static void Tests()
        {
            Test(MovieCrewLoveIsTheDrug);
            Test(MovieCastWinnetou);
            Test(MovieCrewWinnetou);
            Test(PersonWithHeadshotEmmaWatson);
            Test(PersonWithoutHeadshotAlexanderRhodes);
            //Test(PersonWithCircaBirthYearTamara);
            Test(PersonWithBirthYearEmmaWatson);
            Test(SoundtrackThisIsSpinalTap);
            Test(GoofsYojimbo);
            Test(TriviaYojimbo);
            Test(EpisodeCastFerdinand);
            Test(EpisodeCrewFerdinand);
            Test(MovieCastHotShots);
            Test(MovieCrewHotShots);
            Test(MovieCastABiggerSplash);
            Test(EpisodeCrewRoots);
            Test(StageNameSplit);
        }

        private static void TestFixtureSetup()
        {
            CleanUp();
            Program.Main(null);
            CreateMockWebResponse(IMDbParser.PersonUrl, EmmaWatson);
            CreateMockWebResponse(IMDbParser.PersonUrl, AlexanderRhodes);
            //CreateMockWebResponse(IMDbParser.PersonUrl, Tamara);
            CreateMockWebResponse(IMDbParser.TitleUrl, ThisIsSpinalTap, "soundtrack");
            CreateMockWebResponse(IMDbParser.TitleUrl, Winnetou, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, Ferdinand, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, Ferdinand, "soundtrack");
            CreateMockWebResponse(IMDbParser.TitleUrl, Yojimbo, "trivia");
            CreateMockWebResponse(IMDbParser.TitleUrl, Yojimbo, "goofs");
            CreateMockWebResponse(IMDbParser.TitleUrl, HotShots, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, LoveIsTheDrug, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, FridayNightLights, "episodes?season=1");
            CreateMockWebResponse(IMDbParser.TitleUrl, ABiggerSplash, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, Roots, "fullcredits");
            CreateMockWebResponse(IMDbParser.TitleUrl, Roots, "soundtrack");
        }

        #region Tests
        private static void MovieCastABiggerSplash()
        {
            List<Match> castMatches;
            int progressBarMaxValue;
            List<CastInfo> castList;
            FileInfo existing;
            FileInfo current;
            MovieCast(ABiggerSplash, out castMatches, out castList, out progressBarMaxValue, out existing, out current);

            Assert.AreEqual(33, castMatches.Count, "castMatches.Count");
            Assert.AreEqual(33, castList.Count, "castList.Count");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void MovieCastHotShots()
        {
            List<Match> castMatches;
            int progressBarMaxValue;
            List<CastInfo> castList;
            FileInfo existing;
            FileInfo current;

            MovieCast(HotShots, out castMatches, out castList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(73, castMatches.Count, "castMatches.Count");
            Assert.AreEqual(78, castList.Count, "castList.Count");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void MovieCrewHotShots()
        {
            List<KeyValuePair<Match, List<Match>>> crewMatches;
            int progressBarMaxValue;
            List<CrewInfo> crewList;
            FileInfo existing;
            FileInfo current;

            MovieCrew(HotShots, out crewMatches, out crewList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(27, crewMatches.Count, "castMatches.Count");
            Assert.AreEqual(204, progressBarMaxValue, "progressBarMaxValue");
            Assert.AreEqual(194, crewList.Count, "castList.Count");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void EpisodeCrewFerdinand()
        {
            EpisodeInfo episodeInfo;

            episodeInfo = new EpisodeInfo();
            episodeInfo.Link = Ferdinand;
            episodeInfo.SeasonNumber = "1";
            episodeInfo.EpisodeNumber = "1";
            EpisodeCrew(episodeInfo);
            Assert.AreEqual(19, episodeInfo.CrewMatches.Count, "episodeInfo.CrewMatches.Count");
            Assert.AreEqual(30, episodeInfo.CrewList.Count, "episodeInfo.CrewList.Count");
        }

        private static void EpisodeCastFerdinand()
        {
            EpisodeInfo episodeInfo;

            episodeInfo = new EpisodeInfo();
            episodeInfo.Link = Ferdinand;
            episodeInfo.SeasonNumber = "1";
            episodeInfo.EpisodeNumber = "1";
            EpisodeCast(episodeInfo);
            Assert.AreEqual(30, episodeInfo.CastMatches.Count, "episodeInfo.CastMatches.Count");
            Assert.AreEqual(30, episodeInfo.CastList.Count, "episodeInfo.CastList.Count");
            Assert.AreEqual("True", episodeInfo.CastList[29].Uncredited, "episodeInfo.CastList[29].Uncredited");
        }

        private static void TriviaYojimbo()
        {
            Trivia(Yojimbo);
        }

        private static void GoofsYojimbo()
        {
            Goofs(Yojimbo);
        }

        private static void MovieCrewLoveIsTheDrug()
        {
            List<CrewInfo> crewList;
            List<KeyValuePair<Match, List<Match>>> crewMatches;
            int progressBarMaxValue;
            FileInfo existing;
            FileInfo current;

            MovieCrew(LoveIsTheDrug, out crewMatches, out crewList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(24, crewMatches.Count, "crewMatches.Count");
            Assert.AreEqual(105, progressBarMaxValue, "progressBarMaxValue");
            Assert.AreEqual(109, crewList.Count, "crewList.Count");
            Assert.AreEqual("Steven", crewList[51].FirstName, "crewList[51].FirstName");
            Assert.AreEqual("Avila", crewList[51].LastName, "crewList[51].LastName");
            Assert.AreEqual("Sound Designer", crewList[51].CreditSubtype, "crewList[51].CreditSubtype");
            Assert.AreEqual("Steven", crewList[52].FirstName, "crewList[52].FirstName");
            Assert.AreEqual("Avila", crewList[52].LastName, "crewList[52].LastName");
            Assert.AreEqual("Custom", crewList[52].CreditSubtype, "crewList[52].CreditSubtype");
            Assert.AreEqual("Trip", crewList[54].FirstName, "crewList[54].FirstName");
            Assert.AreEqual("Brock", crewList[54].LastName, "crewList[54].LastName");
            Assert.AreEqual("Sound Re-Recording Mixer", crewList[54].CreditSubtype, "crewList[54].CreditSubtype");
            Assert.AreEqual("Trip", crewList[55].FirstName, "crewList[55].FirstName");
            Assert.AreEqual("Brock", crewList[55].LastName, "crewList[55].LastName");
            Assert.AreEqual("Supervising Sound Editor", crewList[55].CreditSubtype, "crewList[55].CreditSubtype");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void MovieCrewWinnetou()
        {
            List<CrewInfo> crewList;
            List<KeyValuePair<Match, List<Match>>> crewMatches;
            int progressBarMaxValue;
            FileInfo existing;
            FileInfo current;

            MovieCrew(Winnetou, out crewMatches, out crewList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(14, crewMatches.Count, "crewMatches.Count");
            Assert.AreEqual(21, progressBarMaxValue, "progressBarMaxValue");
            Assert.AreEqual(19, crewList.Count, "crewList.Count");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void MovieCastWinnetou()
        {
            List<Match> castMatches;
            int progressBarMaxValue;
            List<CastInfo> castList;
            FileInfo existing;
            FileInfo current;

            MovieCast(Winnetou, out castMatches, out castList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(40, castMatches.Count, "castMatches.Count");
            Assert.AreEqual(40, castList.Count, "castList.Count");
            Assert.AreEqual("Old Shatterhand", castList[0].Role, "castList[0].Role");
            Assert.AreEqual("Frederick Santer", castList[3].Role, "castList[3].Role");
            Assert.AreEqual("Vlado Krstulovic", castList[16].CreditedAs, "castList[16].CreditedAs");
            Assert.AreEqual("", castList[24].Role, "castList[24].Role");
            Assert.AreEqual("True", castList[25].Voice, "castList[25].Voice");
            Assert.AreEqual("True", castList[25].Uncredited, "castList[25].Uncredited");
            Assert.AreEqual("Schröder", castList[38].LastName, "castList[38].LastName");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void SoundtrackThisIsSpinalTap()
        {
            Dictionary<string, List<SoundtrackMatch>> matches;
            List<CrewInfo> crewList;
            int progressBarMaxValue;
            FileInfo existing;
            FileInfo current;

            Soundtrack(ThisIsSpinalTap, out matches, out crewList, out progressBarMaxValue, out existing, out current);
            Assert.AreEqual(17, matches.Count, "matches.Count");
            Assert.AreEqual(95, progressBarMaxValue, "progressBarMaxValue");
            Assert.AreEqual(95, crewList.Count, "crewList.Count");
            Assert.AreEqual(existing.Length, current.Length, "current.Length");
        }

        private static void EpisodeCrewRoots()
        {
            EpisodeInfo episodeInfo;

            episodeInfo = new EpisodeInfo();
            episodeInfo.Link = Roots;
            episodeInfo.SeasonNumber = "1";
            episodeInfo.EpisodeNumber = "1";
            EpisodeCrew(episodeInfo);
            Assert.AreEqual(28, episodeInfo.CrewMatches.Count, "episodeInfo.CrewMatches.Count");
            Assert.AreEqual(293, episodeInfo.CrewList.Count, "episodeInfo.CrewList.Count");
        }

        private static void StageNameSplit()
        {
            var fullName = "Bud Spencer & Terence Hill & Oliver Onions & DJ Doena";

            var name = NameParser.Parse(fullName, true).ToString();

            Assert.AreEqual("<Bud Spencer & Terence Hill & Oliver> [Onions & DJ Doena]", name, "name");
        }

        private static void PersonWithBirthYearEmmaWatson()
        {
            PersonInfo personInfo;

            personInfo = new PersonInfo();
            personInfo.PersonLink = EmmaWatson;
            IMDbParser.GetBirthYear(personInfo);
            Assert.AreEqual("1990", personInfo.BirthYear, "personInfo.BirthYear");
        }

        //private static void PersonWithCircaBirthYearTamara()
        //{
        //    PersonInfo personInfo;

        //    personInfo = new PersonInfo();
        //    personInfo.PersonLink = Tamara;
        //    IMDbParser.GetBirthYear(personInfo);
        //    Assert.AreEqual("1910", personInfo.BirthYear, "personInfo.BirthYear");
        //}

        private static void PersonWithHeadshotEmmaWatson()
        {
            FileInfo fileInfo;

            fileInfo = GetHeadshot(EmmaWatson);
            Assert.IsNotNull(fileInfo, "fileInfo");
            Assert.IsTrue(fileInfo.Exists, "fileInfo.Exists");
        }

        private static void PersonWithoutHeadshotAlexanderRhodes()
        {
            FileInfo fileInfo;

            fileInfo = GetHeadshot(AlexanderRhodes);
            Assert.IsNull(fileInfo, "FileInfo is not null!");
        }
        #endregion

        #region Helper
        private static void Test(Action testMethod)
        {
            try
            {
                testMethod();
            }
            catch (AssertionException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(s_WindowHandle, ex.Message);
            }
        }

        private static void Goofs(string key)
        {
            Program.Settings.DefaultValues.DownloadGoofs = true;
            using (var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault))
            {
                Type mainFormType;
                FieldInfo fieldInfo;
                MethodInfo methodInfo;
                TextBox goofsTextBox;

                mainFormType = mainForm.GetType();
                fieldInfo = mainFormType.GetField("_movieTitleLink", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                fieldInfo.SetValue(mainForm, key);
                methodInfo = mainFormType.GetMethod("ParseGoofs", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                Assert.IsNotNull(methodInfo, "methodInfo");
                methodInfo.Invoke(mainForm, null);
                fieldInfo = mainFormType.GetField("GoofsTextBox", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                goofsTextBox = (TextBox)(fieldInfo.GetValue(mainForm));
                using (var sw = new StreamWriter(@"Current\" + key + ".goofs.txt", false, Encoding.UTF8))
                {
                    sw.Write(goofsTextBox.Text);
                }
                using (var sr = new StreamReader(@"Existing\" + key + ".goofs.txt", Encoding.UTF8))
                {
                    string existing;

                    existing = sr.ReadToEnd();
                    Assert.AreEqual(existing.Length, goofsTextBox.Text.Length, "goofsTextBox.Text.Length");
                }
            }
        }

        private static void Trivia(string key)
        {
            Program.Settings.DefaultValues.DownloadTrivia = true;
            using (var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault))
            {
                Type mainFormType;
                FieldInfo fieldInfo;
                MethodInfo methodInfo;
                TextBox triviaTextBox;

                mainFormType = mainForm.GetType();
                fieldInfo = mainFormType.GetField("_movieTitleLink", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                fieldInfo.SetValue(mainForm, key);
                methodInfo = mainFormType.GetMethod("ParseTrivia", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                Assert.IsNotNull(methodInfo, "methodInfo");
                methodInfo.Invoke(mainForm, null);
                fieldInfo = mainFormType.GetField("TriviaTextBox", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                triviaTextBox = (TextBox)(fieldInfo.GetValue(mainForm));
                using (var sw = new StreamWriter(@"Current\" + key + ".trivia.txt", false, Encoding.UTF8))
                {
                    sw.Write(triviaTextBox.Text);
                }
                using (var sr = new StreamReader(@"Existing\" + key + ".trivia.txt", Encoding.UTF8))
                {
                    string existing;

                    existing = sr.ReadToEnd();
                    Assert.AreEqual(existing.Length, triviaTextBox.Text.Length, "triviaTextBox.Text.Length");
                }
            }
        }

        private static void EpisodeCrew(EpisodeInfo episodeInfo)
        {
            List<EpisodeInfo> episodes;

            episodes = new List<EpisodeInfo>(1);
            episodes.Add(episodeInfo);
            using (var episodesForm = new EpisodesForm(episodes))
            {
                Type episodesFormType;
                MethodInfo methodInfo;

                episodesFormType = episodesForm.GetType();
                methodInfo = episodesFormType.GetMethod("ParseIMDb", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(methodInfo, "methodInfo");
                Program.Settings.DefaultValues.ParseCrew = true;
                Program.Settings.DefaultValues.CapitalizeCustomRole = true;
                Program.Settings.DefaultValues.CreditTypeArt = true;
                Program.Settings.DefaultValues.CreditTypeCinematography = true;
                Program.Settings.DefaultValues.CreditTypeDirection = true;
                Program.Settings.DefaultValues.CreditTypeFilmEditing = true;
                Program.Settings.DefaultValues.CreditTypeMusic = true;
                Program.Settings.DefaultValues.CreditTypeOther = true;
                Program.Settings.DefaultValues.CreditTypeProduction = true;
                Program.Settings.DefaultValues.CreditTypeSound = true;
                Program.Settings.DefaultValues.CreditTypeSoundtrack = true;
                Program.Settings.DefaultValues.CreditTypeWriting = true;
                Program.Settings.DefaultValues.IncludeCustomCredits = true;
                Program.Settings.DefaultValues.RetainCrewCreditedAs = true;
                Program.Settings.DefaultValues.RetainOriginalCredit = true;
                Program.Settings.DefaultValues.CheckPersonLinkForRedirect = false;
                methodInfo.Invoke(episodesForm, new Object[] { episodeInfo });
            }
        }

        private static void EpisodeCast(EpisodeInfo episodeInfo)
        {
            List<EpisodeInfo> episodes;

            episodes = new List<EpisodeInfo>(1);
            episodes.Add(episodeInfo);
            using (var episodesForm = new EpisodesForm(episodes))
            {
                Type episodesFormType;
                MethodInfo methodInfo;

                episodesFormType = episodesForm.GetType();
                methodInfo = episodesFormType.GetMethod("ParseIMDb", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(methodInfo, "methodInfo");
                Program.Settings.DefaultValues.ParseCast = true;
                Program.Settings.DefaultValues.IgnoreArchiveFootage = false;
                Program.Settings.DefaultValues.IgnoreCreditOnly = false;
                Program.Settings.DefaultValues.IgnoreLanguageVersion = false;
                Program.Settings.DefaultValues.IgnoreScenesDeleted = false;
                Program.Settings.DefaultValues.IgnoreUnconfirmed = false;
                Program.Settings.DefaultValues.IgnoreUncredited = false;
                Program.Settings.DefaultValues.ParseRoleSlash = true;
                Program.Settings.DefaultValues.ParseVoiceOf = true;
                Program.Settings.DefaultValues.RetainCastCreditedAs = true;
                //Program.Settings.DefaultValues.ParseCrew = true;
                //Program.Settings.DefaultValues.CapitalizeCustomRole = true;
                //Program.Settings.DefaultValues.CreditTypeArt = true;
                //Program.Settings.DefaultValues.CreditTypeCinematography = true;
                //Program.Settings.DefaultValues.CreditTypeDirection = true;
                //Program.Settings.DefaultValues.CreditTypeFilmEditing = true;
                //Program.Settings.DefaultValues.CreditTypeMusic = true;
                //Program.Settings.DefaultValues.CreditTypeOther = true;
                //Program.Settings.DefaultValues.CreditTypeProduction = true;
                //Program.Settings.DefaultValues.CreditTypeSound = true;
                //Program.Settings.DefaultValues.CreditTypeSoundtrack = true;
                //Program.Settings.DefaultValues.CreditTypeWriting = true;
                //Program.Settings.DefaultValues.IncludeCustomCredits = true;
                //Program.Settings.DefaultValues.RetainCrewCreditedAs = true;
                //Program.Settings.DefaultValues.RetainOriginalCredit = true;
                Program.Settings.DefaultValues.CheckPersonLinkForRedirect = false;
                methodInfo.Invoke(episodesForm, new Object[] { episodeInfo });
            }
        }

        private static void MovieCrew(string key
            , out List<KeyValuePair<Match, List<Match>>> crewMatches
            , out List<CrewInfo> crewList
            , out int progressBarMaxValue
            , out FileInfo existing
            , out FileInfo current)
        {
            var defaultValues = new DefaultValues();

            defaultValues.ParseCrew = true;
            defaultValues.CapitalizeCustomRole = true;
            defaultValues.CreditTypeArt = true;
            defaultValues.CreditTypeCinematography = true;
            defaultValues.CreditTypeDirection = true;
            defaultValues.CreditTypeFilmEditing = true;
            defaultValues.CreditTypeMusic = true;
            defaultValues.CreditTypeOther = true;
            defaultValues.CreditTypeProduction = true;
            defaultValues.CreditTypeSound = true;
            defaultValues.CreditTypeSoundtrack = true;
            defaultValues.CreditTypeWriting = true;
            defaultValues.IncludeCustomCredits = true;
            defaultValues.RetainCrewCreditedAs = true;
            defaultValues.RetainOriginalCredit = true;
            defaultValues.CheckPersonLinkForRedirect = false;

            using (var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault))
            {
                Type mainFormType;
                MethodInfo methodInfo;
                FieldInfo fieldInfo;
                CheckBox parseCrewCheckBox;
                Object[] parameters;
                CrewInformation crewInformation;

                mainFormType = mainForm.GetType();
                fieldInfo = mainFormType.GetField("ParseCrewCheckBox", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                parseCrewCheckBox = (CheckBox)(fieldInfo.GetValue(mainForm));
                parseCrewCheckBox.Checked = true;
                methodInfo = mainFormType.GetMethod("ParseCastAndCrew", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                Assert.IsNotNull(methodInfo, "methodInfo");
                crewMatches = new List<KeyValuePair<Match, List<Match>>>();
                crewList = new List<CrewInfo>();
                parameters = new Object[10];
                //parameters[0] = defaultValues;
                parameters[0] = key;
                parameters[1] = false;
                parameters[2] = true;
                parameters[3] = false;
                parameters[4] = false;
                parameters[5] = null;
                parameters[6] = null;
                parameters[7] = crewMatches;
                parameters[8] = crewList;
                parameters[9] = null;
                methodInfo.Invoke(mainForm, parameters);
                crewMatches = (List<KeyValuePair<Match, List<Match>>>)(parameters[7]);
                crewList = (List<CrewInfo>)(parameters[8]);
                mainForm._progressMax = 0;
                mainForm._progressInterval = int.MaxValue;
                foreach (var kvp in crewMatches)
                {
                    mainForm._progressMax += kvp.Value.Count;
                }
                mainForm._progressBar = new ColorProgressBar();
                mainForm._progressBar.Minimum = 0;
                mainForm._progressBar.Maximum = mainForm._progressMax;
                mainForm._progressValue = 0;
                crewList = new List<CrewInfo>(crewMatches.Count);
                IMDbParser.ProcessCrewLine(crewList, crewMatches, defaultValues, mainForm.SetProgress);
                crewInformation = new CrewInformation();
                crewInformation.Title = key;
                crewInformation.CrewList = crewList.ConvertAll<CrewMember>(crewInfo => ConvertCrew(crewInfo)).ToArray();
                crewInformation.Serialize(@"Current\" + key + ".crew.xml");
                existing = new FileInfo(@"Existing\" + key + ".crew.xml");
                current = new FileInfo(@"Current\" + key + ".crew.xml");

                progressBarMaxValue = mainForm._progressMax;
            }
        }

        private static void MovieCast(string key
            , out List<Match> castMatches
            , out List<CastInfo> castList
            , out int progressBarMaxValue
            , out FileInfo existing
            , out FileInfo current)
        {
            DefaultValues defaultValues;

            defaultValues = new DefaultValues();
            defaultValues.ParseCast = true;
            defaultValues.IgnoreArchiveFootage = false;
            defaultValues.IgnoreCreditOnly = false;
            defaultValues.IgnoreLanguageVersion = false;
            defaultValues.IgnoreScenesDeleted = false;
            defaultValues.IgnoreUnconfirmed = false;
            defaultValues.IgnoreUncredited = false;
            defaultValues.ParseRoleSlash = true;
            defaultValues.ParseVoiceOf = true;
            defaultValues.RetainCastCreditedAs = true;
            defaultValues.CheckPersonLinkForRedirect = false;
            using (var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault))
            {
                Type mainFormType;
                MethodInfo methodInfo;
                FieldInfo fieldInfo;
                CheckBox parseCastCheckBox;
                Object[] parameters;
                CastInformation castInformation;

                mainFormType = mainForm.GetType();
                fieldInfo = mainFormType.GetField("ParseCastCheckBox", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(fieldInfo, "fieldInfo");
                parseCastCheckBox = (CheckBox)(fieldInfo.GetValue(mainForm));
                parseCastCheckBox.Checked = true;
                methodInfo = mainFormType.GetMethod("ParseCastAndCrew", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                Assert.IsNotNull(methodInfo, "methodInfo");
                castMatches = new List<Match>();
                castList = new List<CastInfo>();
                parameters = new Object[10];
                //parameters[0] = defaultValues;
                parameters[0] = key;
                parameters[1] = true;
                parameters[2] = false;
                parameters[3] = false;
                parameters[4] = false;
                parameters[5] = castMatches;
                parameters[6] = castList;
                parameters[7] = null;
                parameters[8] = null;
                parameters[9] = null;
                methodInfo.Invoke(mainForm, parameters);
                castMatches = (List<Match>)(parameters[5]);
                castList = (List<CastInfo>)(parameters[6]);
                mainForm._progressMax = castMatches.Count;
                mainForm._progressInterval = int.MaxValue;
                mainForm._progressBar = new ColorProgressBar();
                mainForm._progressBar.Minimum = 0;
                mainForm._progressBar.Maximum = mainForm._progressMax;
                mainForm._progressValue = 0;
                castList = new List<CastInfo>(castMatches.Count);
                IMDbParser.ProcessCastLine(castList, castMatches, defaultValues, mainForm.SetProgress);
                castInformation = new CastInformation();
                castInformation.Title = key;
                castInformation.CastList = castList.ConvertAll<CastMember>(castInfo => ConvertCast(castInfo)).ToArray();
                castInformation.Serialize(@"Current\" + key + ".cast.xml");
                existing = new FileInfo(@"Existing\" + key + ".cast.xml");
                current = new FileInfo(@"Current\" + key + ".cast.xml");

                progressBarMaxValue = mainForm._progressMax;
            }
        }

        private static void Soundtrack(string key
            , out Dictionary<string, List<SoundtrackMatch>> matches
            , out List<CrewInfo> crewList
            , out int progressBarMaxValue
            , out FileInfo existing, out FileInfo current)
        {
            MethodInfo methodInfo;
            DefaultValues defaultValues;
            CrewInformation crewInformation;

            methodInfo = typeof(CastCrewEdit2BaseForm).GetMethod("ParseSoundtrack"
                , BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(methodInfo, "methodInfo");
            matches = (Dictionary<string, List<SoundtrackMatch>>)(methodInfo.Invoke(null, new Object[] { key }));
            Assert.IsNotNull(matches, "matches");

            using (var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault))
            {
                mainForm._progressMax = 0;
                mainForm._progressInterval = int.MaxValue;
                foreach (var kvp in matches)
                {
                    mainForm._progressMax += kvp.Value.Count;
                }
                defaultValues = new DefaultValues();
                defaultValues.CreditTypeSoundtrack = true;
                defaultValues.CheckPersonLinkForRedirect = false;
                mainForm._progressBar = new ColorProgressBar();
                mainForm._progressBar.Minimum = 0;
                mainForm._progressBar.Maximum = mainForm._progressMax;
                mainForm._progressValue = 0;
                crewList = new List<CrewInfo>(mainForm._progressMax);
                IMDbParser.ProcessSoundtrackLine(crewList, matches, defaultValues, mainForm.SetProgress);
                crewInformation = new CrewInformation();
                crewInformation.Title = key;
                crewInformation.CrewList = crewList.ConvertAll<CrewMember>(crewInfo => ConvertCrew(crewInfo)).ToArray();
                crewInformation.Serialize(@"Current\" + key + ".soundtrack.xml");
                existing = new FileInfo(@"Existing\" + key + ".soundtrack.xml");
                current = new FileInfo(@"Current\" + key + ".soundtrack.xml");

                progressBarMaxValue = mainForm._progressMax;
            }
        }

        private static CrewMember ConvertCrew(CrewInfo crewInfo)
        {
            CrewMember crewMember;
            int birthYear;

            crewMember = new CrewMember();
            crewMember.LastName = crewInfo.LastName;
            crewMember.FirstName = crewInfo.FirstName;
            crewMember.MiddleName = crewInfo.MiddleName;
            if (int.TryParse(crewInfo.BirthYear, out birthYear))
            {
                crewMember.BirthYear = birthYear;
            }
            crewMember.CreditedAs = crewInfo.CreditedAs;
            crewMember.CreditSubtype = crewInfo.CreditSubtype;
            crewMember.CreditType = crewInfo.CreditType;
            if (string.IsNullOrEmpty(crewInfo.CustomRole) == false)
            {
                crewMember.CustomRole = crewInfo.CustomRole;
                crewMember.CustomRoleSpecified = true;
            }
            return (crewMember);
        }

        private static CastMember ConvertCast(CastInfo castInfo)
        {
            CastMember castMember;
            int birthYear;

            castMember = new CastMember();
            castMember.LastName = castInfo.LastName;
            castMember.FirstName = castInfo.FirstName;
            castMember.MiddleName = castInfo.MiddleName;
            if (int.TryParse(castInfo.BirthYear, out birthYear))
            {
                castMember.BirthYear = birthYear;
            }
            castMember.CreditedAs = castInfo.CreditedAs;
            castMember.Role = castInfo.Role;
            castMember.Uncredited = bool.Parse(castInfo.Uncredited);
            castMember.Voice = bool.Parse(castInfo.Voice);
            return (castMember);
        }

        private static FileInfo GetHeadshot(string key)
        {
            PersonInfo personInfo;
            FileInfo fileInfo;

            personInfo = new PersonInfo();
            personInfo.PersonLink = key;
            fileInfo = IMDbParser.GetHeadshot(personInfo);
            return (fileInfo);
        }

        private static void CreateMockWebResponse(string baseUrl, string key, string appendix)
        {
            string targetUrl;
            IWebResponse webResponse;
            string fileName;

            targetUrl = baseUrl + key;
            fileName = @"Current\" + key;
            if (appendix != null)
            {
                string fileAppendix;

                fileAppendix = appendix;
                foreach (var c in Path.GetInvalidFileNameChars())
                {
                    fileAppendix = fileAppendix.Replace(c, '_');
                }
                fileName += "." + fileAppendix;
                targetUrl += "/" + appendix;
            }
            fileName += ".html.txt";
            webResponse = OnlineAccess.GetSystemSettingsWebResponseAsync(targetUrl).GetAwaiter().GetResult();
            using (var webStream = webResponse.GetResponseStream())
            {
                using (var sr = new StreamReader(webStream, IMDbParser.Encoding))
                {
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var sw = new StreamWriter(fileStream, IMDbParser.Encoding))
                        {
                            sw.Write(sr.ReadToEnd());
                        }
                    }
                }
            }
            IMDbParser.WebResponses[targetUrl] = new MockWebResponse(fileName);
            webResponse.Close();
        }

        private static void CreateMockWebResponse(string baseUrl, string key)
        {
            CreateMockWebResponse(baseUrl, key, null);
        }

        private static void CleanUp()
        {
            Process p;

            p = new Process();
            p.StartInfo = new ProcessStartInfo("CleanUp.cmd");
            p.Start();
            p.WaitForExit();
        }
        #endregion
    }
}