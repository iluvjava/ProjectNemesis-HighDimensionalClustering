using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Project_Nemesis__HighDimensionalClustering.src.Util
{

    /// <summary>
    ///     A class that is going to log stuff into the console. and stores some 
    ///     settings on rules of logging too. 
    ///     
    ///     We assume that there are only one console, so we made singleton with 
    ///     class set to static. 
    /// </summary>
    static class ConsoleLog
    {

        /// <summary>
        ///     Make a extending elipces animation on the current line of the console. 
        /// </summary>
        public static CancellationTokenSource DisplayLoadingBard()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            Thread t = new Thread(
                    () => {
                        while (!ct.IsCancellationRequested)
                        {
                            Write("loading");
                            for (int j = 0; j < 6; j++) 
                            {
                                Thread.Sleep(200);
                                Write(".");
                            };
                            Write("\r");
                            for (int j = 0; j < 6 + 7; j++)
                            {
                                Write(" ");
                            }
                            Write("\r");
                        }
                    }
                );
            t.Start();
            return cts;
        }


        /// <summary>
        ///     Fill the whole line of the console with a repeating character; 
        ///     there will be a new line char at the returned string. 
        /// </summary>
        /// <param name="c">
        ///     The character. 
        /// </param>
        /// <returns>
        ///     A string that can be printed out directly. 
        /// </returns>
        public static string GetSeperator(char c = '=')
        {
            int Width = -1;
            // This is stupid but it's fine. 
            try
            {
                Width = Console.WindowWidth;
            }
            catch {
                Width = 40;           
            }
            StringBuilder sb = new StringBuilder();
            for (int I = 0; I < Width - 1; I++)
            {
                sb.Append(c);
            }
            sb.Append('\n');
            return sb.ToString();
        }

    }

    /// <summary>
    ///     
    /// </summary>
    static class ConsoleInteract
    {

        /// <summary>
        ///     This method get one line of inputs from the user. 
        ///     
        ///     It will ask for the inputs repeatedly until the input matches the regex expression. 
        /// </summary>
        /// <returns>
        ///     A string, formatted, and can be printed out and viewed as a report in the console. 
        /// </returns>
        public static string GetUserInput(string inputInstructions, string patterns)
        {
            WriteLine(inputInstructions);
            string s = ReadLine();
            while (!Regex.IsMatch(s, patterns))
            {
                WriteLine(inputInstructions);
                s = ReadLine();
            };
            return s;
        }


       
    }
}
