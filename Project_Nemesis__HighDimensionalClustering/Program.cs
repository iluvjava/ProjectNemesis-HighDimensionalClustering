using Chaos.src.Ethena;
using Chaos.src.Util;
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
    class Program
    {

        // public const string Path = @"C:\Users\victo\source\repos\Math-381-Project-2\data";
        public const string Path =
            @"C:\Users\victo\source\repos\ProjectNemesis-HighDimensionalClustering\Project_Nemesis__HighDimensionalClustering\assets";

        static IDictionary<string, string> FilesAndContent;

        public void SetThingsup()
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

        }

        public static void TryRunClusters()
        {
            WriteLine("Generating the normal samples spacial points. ");
            double[] mu1 = new double[] { 0, 0, 0 }, mu2 = new double[] { 200, 200, 200 };
            double sd = 100;
            int EachClusterSize = 100;
            SpacialPoint ExpectedCenter1 = new SpacialPoint(mu1);
            SpacialPoint ExpectedCenter2 = new SpacialPoint(mu2);
            WriteLine($"2 hypothetical centers: [[{mu1[0]}, {mu1[1]}, {mu1[2]}], [{mu2[0]},{mu2[1]},{mu2[2]}]],");
            int Counter = 100;
            WriteLine("Average Normalized Deviations from centroid to centers");
            while (--Counter != 0)
            {
                Point[] samples1 = SpacialPoint.NormalRandomPoints(mu1, sd, EachClusterSize);
                Point[] samples2 = SpacialPoint.NormalRandomPoints(mu2, sd, EachClusterSize);

                Point[] merged = new Point[samples1.Length + samples2.Length];
                Array.Copy(samples1, 0, merged, 0, samples1.Length);
                Array.Copy(samples2, 0, merged, samples1.Length, samples2.Length);

                FullGraphClustering fg = new FullGraphClustering(merged);


                Point c1 = fg.Centroid1;
                Point c2 = fg.Centroid2;
                double d1 = Math.Min(Point.Dis(c1, ExpectedCenter1), Point.Dis(c1, ExpectedCenter2));
                double d2 = Math.Min(Point.Dis(c2, ExpectedCenter1), Point.Dis(c2, ExpectedCenter2));
                WriteLine(((d1 / (Math.Sqrt(3)*sd)) + (d2 / (Math.Sqrt(3)*sd))) / 2);
            }
        }

        public static void EfficiencyReport(int clustersize, double centroidsdaway, int Repetitions)
        {
            TestRecord tr = new TestRecord(centroidsdaway, clustersize);
            Task[] tlist = new Task[Repetitions];
            for (int I = 0; I < tlist.Length; I++)
            {
                tlist[I] = tr.GetTestInstance();
                tlist[I].Start();
            }
            Task.WaitAll(tlist);
            WriteLine("All tests finished");
            WriteLine(tr);
        }

        public class ThreadWork
        {
            public static void DoWork()
            {
                // EfficiencyReport(100, 2, 100);

                TryRunClusters();
            }

        }

        class TestRecord
        {
            double[] Mu1;
            double[] Mu2;
            double SDaway; // How much standard deviation away is 2 of the cluters. . 
            int SizeofEachCluster;

            int NumberofTestCompleted;
            public double Avgsdfcc { get; protected set; } // Average Standar deviations from correct centroid. 
            public double AverageInstanceTimeDuration { get; protected set; } // Time it takes for each Clustering. 

            /// <summary>
            ///     
            /// </summary>
            /// <param name="centroiddis"></param>
            /// <param name="N">
            /// Size of the clusters, howmay points are ther in 2 of the clusters? 
            /// </param>
            public TestRecord(double centroiddis, int N)
            {
                Mu1 = new double[] { 0, 0, 0 };
                Mu2 = new double[] { centroiddis, centroiddis, centroiddis };
                SDaway = centroiddis;
                SizeofEachCluster = N;
            }

            protected void Register(double sdfc, double duration)
            {
                lock (this)
                {
                    NumberofTestCompleted++;
                    Avgsdfcc = ((NumberofTestCompleted - 1) * Avgsdfcc + sdfc) / NumberofTestCompleted;
                    AverageInstanceTimeDuration =
                        (AverageInstanceTimeDuration * (NumberofTestCompleted - 1) + duration) / NumberofTestCompleted;


                }
            }

            public Task GetTestInstance()
            {
                TestRecord that = this;
                Task r = new Task
                (
                    () =>
                    {
                        Stopwatch sw = new Stopwatch();
                        SpacialPoint ExpectedCenter1 = new SpacialPoint(Mu1), ExpectedCenter2 = new SpacialPoint(Mu2);

                        Point[] samples1 = SpacialPoint.NormalRandomPoints(Mu1, 1, SizeofEachCluster);
                        Point[] samples2 = SpacialPoint.NormalRandomPoints(Mu2, 1, SizeofEachCluster);

                        Point[] merged = new Point[samples1.Length + samples2.Length];
                        Array.Copy(samples1, 0, merged, 0, samples1.Length);
                        Array.Copy(samples2, 0, merged, samples1.Length, samples2.Length);

                        sw.Start();
                        FullGraphClustering fg = new FullGraphClustering(merged);

                        Point c1 = fg.Centroid1;
                        Point c2 = fg.Centroid2;
                        double d1 = Math.Min(Point.Dis(c1, ExpectedCenter1), Point.Dis(c1, ExpectedCenter2));
                        double d2 = Math.Min(Point.Dis(c2, ExpectedCenter1), Point.Dis(c2, ExpectedCenter2));
                        double d = (
                                        d1 / (Math.Sqrt(Mu1.Length) * SDaway)
                                        +
                                        d2 / (Math.Sqrt(Mu2.Length) * SDaway)
                                    ) / 2;
                        sw.Stop();
                        that.Register(d, sw.Elapsed.TotalMilliseconds);
                    }
                );
                return r;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Total tets completed: {NumberofTestCompleted}\n");
                sb.Append($"Average Runtime for each test: {AverageInstanceTimeDuration}\n");
                sb.Append($"Average Error in terms of deviation: {Avgsdfcc}\n");
                return sb.ToString();
            }

        }




    }


}
