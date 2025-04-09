using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public static class AssemblyTests
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        var assembly = typeof(AssemblyTests).Assembly;

        AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;

        CleanUp();

        Program.RootPath = (new FileInfo(assembly.Location)).DirectoryName;

        Program.InitDataPaths();

        Program.Main(null);
    }

    private static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = args.Name.Split(',')[0];

        var assemblyFileName = Path.Combine(Program.RootPath, $"{assemblyName}.dll");

        if (File.Exists(assemblyFileName))
        {
            var assembly = Assembly.LoadFrom(assemblyFileName);

            return assembly;
        }
        else
        {
            return null;
        }
    }

    private static void CleanUp()
    {
        Process p;

        p = new Process();
        p.StartInfo = new ProcessStartInfo("CleanUp.cmd");
        p.Start();
        p.WaitForExit();
    }
}
