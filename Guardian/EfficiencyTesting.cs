using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Chaos.src.Util;

using Chaos.src.Ethena;

using static System.Console;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NUnit.Framework.Internal;
using System.Diagnostics;

namespace Guardian
{
    class EfficiencyTesting
    {
        [SetUp]
        public void SetUp()
        { 
        
        }


        [TestCase(30, 2, 100)]
        [TestCase(30, 3, 100)]
        [TestCase(30, 4, 100)]
        [TestCase(30, 5, 100)]
        [TestCase(100, 2, 100)]
        [TestCase(100, 3, 100)]
        [TestCase(100, 4, 100)]
        [TestCase(100, 5, 100)]
        public void EfficiencyReport(int clustersize, double centroidsdaway, int Repetitions)
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
                Mu1 = new double[] {0, 0, 0};
                Mu2 = new double[] {centroiddis, 0, 0};
                SDaway = centroiddis;
                SizeofEachCluster = N;
            }

            protected void Register(double sdfc, double duration)
            {
                lock (this)
                {
                    NumberofTestCompleted++;
                    Avgsdfcc = ((NumberofTestCompleted- 1) * Avgsdfcc + sdfc) / NumberofTestCompleted;
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
                        double d = (d1 + d2)/2;
                        d /= SDaway;
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
                sb.Append($"Average Runtime for each test: {AverageInstanceTimeDuration} ms\n");
                sb.Append($"Average Error in terms of deviation: {Avgsdfcc}\n");
                return sb.ToString(); 
            }

        }


    }
}
