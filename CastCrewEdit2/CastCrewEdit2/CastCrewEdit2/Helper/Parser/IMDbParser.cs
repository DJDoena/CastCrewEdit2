using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal delegate void SetProgress();

internal static class IMDbParser
{
#if DEBUG || UnitTest
    internal const int MaxTasks = 1;
#else
    internal const int MaxTasks = 4;
#endif

    internal const string DomainPrefix = "https?:\\/\\/((akas.)*|(www.)*|(us.)*|(german.)*)imdb.(com|de)\\/([a-z]{2}\\/)?";

    internal const string BaseUrl = @"https://www.imdb.com";

    internal const string TitleUrl = BaseUrl + @"/title/";

    internal static bool IsInitialized { get; private set; }

    internal static Regex BlockEndRegex { get; }

    internal static Regex TitleUrlRegex { get; }

    internal static Regex TitleRegex { get; }

    internal static Regex UncreditedRegex { get; }

    internal static Regex CreditedAsRegex { get; }

    internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData { get; private set; }

    internal static SessionData SessionData { get; set; }

    internal static string PersonHashCount
        => SessionData.BirthYearCache.Count.ToString();

    internal static Dictionary<PersonInfo, string> PersonHash
        => SessionData.BirthYearCache;

    static IMDbParser()
    {
        BlockEndRegex = new Regex("</table>", RegexOptions.Compiled);

        TitleUrlRegex = new Regex(DomainPrefix + "title\\/(?'TitleLink'tt[0-9]+)\\/.*$", RegexOptions.Compiled);

        TitleRegex = new Regex(@"<title>(?'Title'.+?)</title>", RegexOptions.Compiled);

        UncreditedRegex = new Regex(@"\(?uncredited\)?", RegexOptions.Compiled);

        CreditedAsRegex = new Regex(@"\(as (?'CreditedAs'.+?)\)", RegexOptions.Compiled);

        SessionData = new SessionData();

        IsInitialized = false;
    }

    internal static void Initialize(IWin32Window windowHandle)
    {
        var path = Program.RootPath + @"\Data\";

        NameParser.Initialize(windowHandle, path);

        InitList(path + @"IgnoreCustomInIMDbCategory.txt", out var list, false);
        CrewParser.IgnoreCustomInIMDbCreditType = list;

        InitList(path + @"IgnoreIMDbCategoryInOther.txt", out list, false);
        CrewParser.IgnoreIMDbCreditTypeInOther = list;

        InitList(path + @"ForcedFakeBirthYears.txt", out list, false);
        BirthYearParser.ProcessForcedFakeBirthYears(list);

        InitIMDbToDVDProfilerCrewRoleTransformation(windowHandle);

        IsInitialized = true;
    }

    internal static void InitIMDbToDVDProfilerCrewRoleTransformation(IWin32Window windowHandle)
    {
        var path = Program.RootPath + @"\Data\";

        var fileName = path + @"IMDbToDVDProfilerCrewRoleTransformation.xml";

        if (File.Exists(fileName))
        {
            try
            {
                TransformationData = XmlSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(fileName);
            }
            catch
            {
                TransformationData = new IMDbToDVDProfilerCrewRoleTransformation()
                {
                    CreditTypeList = new CreditType[0],
                };

                MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileCantBeRead, fileName), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            TransformationData = new IMDbToDVDProfilerCrewRoleTransformation()
            {
                CreditTypeList = new CreditType[0],
            };

            MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    internal static void InitList(string fileName, FileNameType fileNameType, IWin32Window windowHandle)
    {
        switch (fileNameType)
        {
            case FileNameType.FirstnamePrefixes:
            case FileNameType.LastnamePrefixes:
            case FileNameType.LastnameSuffixes:
            case FileNameType.KnownNames:
                {
                    NameParser.InitList(fileName, fileNameType, windowHandle);

                    break;
                }
            case FileNameType.IgnoreCustomInIMDbCreditType:
                {
                    InitList(fileName, out var list, windowHandle, false);
                    CrewParser.IgnoreCustomInIMDbCreditType = list;

                    break;
                }
            case FileNameType.IgnoreIMDbCreditTypeInOther:
                {
                    InitList(fileName, out var list, windowHandle, false);
                    CrewParser.IgnoreIMDbCreditTypeInOther = list;

                    break;
                }
            case FileNameType.ForcedFakeBirthYears:
                {
                    InitList(fileName, out var forcedFakeBirthYears, windowHandle, false);

                    BirthYearParser.ProcessForcedFakeBirthYears(forcedFakeBirthYears);

                    break;
                }
        }
    }

    internal static void InitList(string fileName, out List<string> list, IWin32Window windowHandle, bool toLower)
    {
        if (!InitList(fileName, out list, toLower))
        {
            MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static bool InitList(string fileName, out List<string> list, bool toLower)
    {
        list = new List<string>(50);

        if (!File.Exists(fileName))
        {
            return false;
        }

        using (var sr = new StreamReader(fileName))
        {
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    if (toLower)
                    {
                        list.Add(line.ToLower());
                    }
                    else
                    {
                        list.Add(line);
                    }
                }
            }
        }

        return true;
    }
}