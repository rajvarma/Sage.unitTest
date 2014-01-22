using System;
using System.Collections.Generic;

namespace VSProjectCleaner
{
    using System.IO;
    using System.Linq;

    using Microsoft.Build.Evaluation;

    class Program
    {
        static void Main(string []args)
        {
            try
            {
                if (args.Count() != 2)
                {
                    PrintHelp();
                    Environment.ExitCode = 1;
                }
                else
                {
                    // parse options out of the arguments
                    var settings = ParseOptions(args);

                    foreach (var project in FindVisualStudioProjects(settings))
                    {
                        project.ProcessProject(settings);
                        project.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine("!!! " + ex.ToString());
            }
        }

        private static Settings ParseOptions(string[] args)
        {
            var settings =  new Settings( Path.GetFullPath(args[0]), Path.GetFullPath(args[1]));
            return settings;
        }

        private static IEnumerable<CsProjectFileProcessor> FindVisualStudioProjects(Settings settings)
        {
            var csprojFilePaths = Directory.GetFiles(settings.TargetDirectory , "*.csproj", settings.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            return csprojFilePaths.Select(vsProjFilePath => new CsProjectFileProcessor(vsProjFilePath));
        }


        internal static void WriteLine(String line)
        {
            Console.WriteLine(line);
        }

        private static void PrintHelp()
        {
            WriteLine(@"Sage Visual Studio C# Project files configuration tool.");
            WriteLine(String.Empty);
            WriteLine("Syntax: VSProjectCleaner <root folder> <base projects folder>");
            WriteLine("");
        }
    }
}
