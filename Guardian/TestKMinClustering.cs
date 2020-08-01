using Chaos.src.Ethena;
using NUnit.Framework;
using System;
using System.Collections.Immutable;
using static System.Console;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text.Json;

namespace Guardian
{
    class TestKMinClustering
    {
        [SetUp]
        public void SetUp()
        { }

        /// <summary>
        ///     Just testing some basic stuff. 
        /// </summary>
        [Test]
        public void Test1() {

            Point[] Points = new Point[]
            {
                new SpacialPoint(new int[]{1, 0}),
                new SpacialPoint(new int[]{2, 0}),
                new SpacialPoint(new int[]{3, 0}),
                new SpacialPoint(new int[]{3, 10}),
                new SpacialPoint(new int[]{4, 0})
            };

            KMinSpanningTree Kmst = new KMinSpanningTree(Points);
            Kmst.KMinKruskal();
            IImmutableSet<Edge> ChosenEdges = Kmst.ChosenEdges;
            WriteLine($"The clustering threshold: {Kmst.ClusterSizeThreshold}");
            Assert.IsTrue(ChosenEdges.Count != 0);

            foreach (Edge e in Kmst.ChosenEdges)
            {
                WriteLine($"{e}");
            }

            foreach (PointCollection pc in Kmst.KSpanningTree)
            {
                WriteLine(pc);
            }
        }

        /// <summary>
        ///     Gonna test some more advance stuff. 
        /// </summary>
        [Test]
        public void test2()
        {
            WriteLine("Generating the normal samples spacial points. ");
            double[] mu1 = new double[] { 200, 0 }, mu2 = new double[] { -200, 0 };
            double sd = 50;
            int EachClusterSize = 100;
            // SpacialPoint ExpectedCenter1 = new SpacialPoint(mu1);
            // SpacialPoint ExpectedCenter2 = new SpacialPoint(mu2);
            WriteLine($"2 hypothetical centers: [[{mu1[0]}, {mu1[1]}], [{mu2[0]},{mu2[1]}],");
            int Counter = 10;
            WriteLine("Average Normalized Deviations from centroid to centers");

            while (--Counter != 0)
            {
                Point[] samples1 = SpacialPoint.NormalRandomPoints(mu1, sd, EachClusterSize);
                Point[] samples2 = SpacialPoint.NormalRandomPoints(mu2, sd, EachClusterSize);

                Point[] merged = new Point[samples1.Length + samples2.Length];
                Array.Copy(samples1, 0, merged, 0, samples1.Length);
                Array.Copy(samples2, 0, merged, samples1.Length, samples2.Length);

                KMinSpanningTree Kmst = new KMinSpanningTree(merged);
                WriteLine("------------------------Centroids:----------------------------------");
                Point[][] SetOfClusters = MakeNestedArrayFomPointCollection(Kmst.KSpanningTree);
                SaveTheClustersToJson($"tests_output/test2_clusters{Counter}.json", SetOfClusters);

            }
        }

        /// <summary>
        ///     Print out all the points and then see how it actually looks like as clusters. 
        /// </summary>
        public Point[][] MakeNestedArrayFomPointCollection(IImmutableSet<PointCollection> pcs)
        {
            Point[][] PListList = new Point[pcs.Count][];
            {
                int I = 0;
                foreach (PointCollection PntC in pcs)
                {
                    PListList[I] = PntC.ToArray();
                    I++;
                }
            }
            return PListList;
        }

        /// <summary>
        ///     conver all points in the array to a easier format for json when reinterpreting. 
        /// </summary>
        /// <param name="points">
        /// 
        /// </param>
        /// <param name="fileName">
        /// 
        /// </param>
        static Point2D[] Convert(Point[] points)
        {
            IEnumerable<Point> P = points.AsEnumerable();
            IEnumerable<SpacialPoint> Sp = P.Select(e => e as SpacialPoint);
            Point2D[] Points = Sp.Select(e => new Point2D(e[0], e[1])).ToArray();
            return Points;
        }

        /// <summary>
        ///     Save the nested array to a file. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pointClusters"></param>
        static void SaveTheClustersToJson(string fileName, Point[][] pointClusters)
        {
            IEnumerable<Point[]> PListList = pointClusters.AsEnumerable();
            IEnumerable<Point2D[]> P2DListList = PListList.Select(e => Convert(e));
            Point2D[][] Res = P2DListList.ToArray();
            string JsonStr = JsonConvert.SerializeObject(Res, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(JsonStr);
            }
        }

        /// <summary>
        ///     For quick Json Serilization. 
        /// </summary>
        class Point2D
        {
            public double x { get; set; } // must be public for json to serialize? 
            public double y { get; set; }
            public Point2D(double x, double y)
            {
                this.x = x; this.y = y;
            }
        }

    }
}
