using TheBase.src.Core;
using TheBase.src.Util;
using Project_Nemesis__HighDimensionalClustering.src.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace TheBase
{
    /// <summary>
    ///     Things to establish: 
    ///     1. File Directory
    ///         * No identifier, should always be the first parameters, should be in quotes if there  
    ///         are spaces in it. 
    ///     2. File Post fix, optional, txt by default. 
    ///         * p: simple string
    ///     3. Recursive or not
    ///         * r (single letter by itself to indicate recursive search)
    ///     4. MatrixTypes
    ///         *mt: (tm27, 2ndtm27)
    ///     5. MatrixMetricTypes
    ///         *mm: (vec, norm2)
    ///     * A help command. 
    ///         *help (by it self, no other things at all)
    ///     Regex Description: 
    ///         "((?:[^/]*\/)*)(.*)" (p:\w+\s*|r\s*|mt:\w+\s*|mm:\w+\s*)*
    ///     The user inputs:
    ///         Parse, get all specified parameters and it's value from the user. 
    ///         
    ///     While(true)
    ///         get
    ///         interpret
    ///         do
    ///     
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
            
            string UserInputs = ConsoleInteract.
                GetUserInput("Give me a file directory",
                @"^""((?:[^/]*\/)*)(.*)"" (p:\w+\s*|r\s*|tm:\w+\s*|mm:\w+\s*)*$");

            IDictionary<string, string> inputs = InterpParameters(UserInputs);
            foreach(KeyValuePair<string, string> kvp in inputs)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            Console.ReadKey();
        }

        /// <summary>
        ///     print the welcome screen and help information for the user. 
        /// </summary>
        static void PrintWelcomeScreen()
        {
            return;
        }


        /// <summary>
        ///     This function will interpret the parameters from the function. 
        /// </summary>
        /// <param name="userInputs">
        ///     Inputs matches the regex 
        /// </param>
        /// <returns>
        ///     Params |--> Values. 
        /// </returns>
        static IDictionary<string, string> InterpParameters(string userInputs)
        {
            IDictionary<string, string> Res = new Dictionary<string, string>();
            string[] Splited = userInputs.Split();
            Res["dir"] = Splited[0].Substring(1, Splited[0].Length - 2);
            for (int I = 1; I < Splited.Length; I++)
            {
                string Token = Splited[I];
                if (Token.Equals("r"))
                {
                    Res[Token] = Token;
                }
                else
                {
                    string[] TokenSplitted = Token.Split(':');
                    Res[TokenSplitted[0]] = TokenSplitted[1];
                }
            }
            return Res;
        }
    }
}
