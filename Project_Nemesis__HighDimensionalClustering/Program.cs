using Chaos.src.Ethena;
using Chaos.src.Util;
using Project_Nemesis__HighDimensionalClustering.src.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Chaos
{
    /// <summary>
    ///     Things to establish: 
    ///     1. File Directory
    ///         * No identifier, should always be the first parameters.
    ///     2. File Post fix, optional, txt by default. 
    ///         * p: simple string
    ///     3. Recursive or not
    ///         * r (single letter by itself to indicate recursive search)
    ///     4. MatrixTypes
    ///         *mt: ()
    ///     5. MatrixMetricTypes
    ///         *mm: ()
    ///     * A help command. 
    ///         *help (by it self, no other things at all)
    ///     The user inputs: 
    ///         [file directory][space][filepostfix:Should be text files; optional][-r: recursive]
    /// </summary>
    class Program
    {

        // public const string Path = @"C:\Users\victo\source\repos\Math-381-Project-2\data";
        public const string Path =
            @"C:\Users\victo\source\repos\ProjectNemesis-HighDimensionalClustering\Project_Nemesis__HighDimensionalClustering\assets";

        static IDictionary<string, string> FilesAndContent;

        public static void SetThingsup()
        {
            // Check if path exists and has file I guess?

            DirectoryInfo dirinfo = new DirectoryInfo(Path);
            if (!dirinfo.Exists)
            {
                throw new ArgumentException();
            }

            IDictionary<string, string> FilesAndContents = Basic.GetContentForAllFiles(Path, true, "txt");
            FilesAndContent = FilesAndContents;
        }

        static void Main(string[] args)
        {
            /*
            SetThingsup();

            WriteLine($"Files and Content Sizes:{FilesAndContent.Count} ");

            WriteLine("Villena flavor type of matrix and metric. ");
            TextFileClusterReporter tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());

            WriteLine("* Let's use a vectorized metric for it. ");
            SettingsManager.SetDisFxnToVecNorm();
            tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());

            WriteLine("* Let's use the second order transition matrix: ");
            SettingsManager.Set2ndTM27ForTransitionMatrix();
            tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());

            WriteLine("Let's change the metric back to 2 norm");
            SettingsManager.SetDisFxnTo2Norm();
            tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());
            */

            /*
                CancellationTokenSource cts = ConsoleLog.DisplayLoadingBard();
                Thread.Sleep(10000);
                cts.Cancel();
            */

            string UserInputs = ConsoleStuff.GetUserInput("Give me a file directory", @"((?:[^/]*\/)*)(.*)");


        }

    }


}
