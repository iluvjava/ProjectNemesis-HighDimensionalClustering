using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;
using System.Text;
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

    }
}
