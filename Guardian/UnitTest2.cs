using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Chaos.src.Util;

using Chaos.src.Ethena;

using static System.Console;
using System;

namespace Guardian
{

    public class TestTextAnalysis
    {

        public static string PATH = @"C:\Users\victo\source\repos\Math-381-Project-2\data"; 

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestPointsClass()
        {
            
            Point p1 = new SpacialPoint(new int[] { 0, 0, 0});
            Point p2 = new SpacialPoint(new int[] { 1, 1 ,1});
            Assert.IsTrue(Point.Dis(p1, p2)== Math.Sqrt(3));
            
        }

        /// <summary>
        ///     * Test the kruskal running on a basic non trivial graph. 
        /// </summary>
        [Test]
        public void TestFullgraphBasics()
        {
            Point[] points = new Point[]
            {
                new SpacialPoint(new int[]{ 1, 0}),
                new SpacialPoint(new int[]{ 2, 0}),
                new SpacialPoint(new int[]{ 3, 0}),
                new SpacialPoint(new int[]{ 3, 10}),
                new SpacialPoint(new int[]{ 4, 0})
            };

            FullGraph fg = new FullGraph(points);
            
            foreach (KeyValuePair<int, Point> kvp in fg.IndexToVertices)
            {
                WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            WriteLine("List of edges chosen by Kruskal: ");
            
            foreach (Edge e in fg.ChosedForMST)
            {
                WriteLine($"{e}");
            }
            WriteLine("Here is the list of partition Sizes: ");

            foreach (int size in fg.MaxPartitionSize)
            {
                WriteLine(size);
            }

        }

        [Test]
        public void TestClustering()
        {

            WriteLine("Generating the normal samples spacial points. ");
            double[] mu1 = new double[] { 0, 0, 0 }, mu2 = new double[] {200, 200, 200};
            double sd = 100;
            int EachClusterSize = 500;
            SpacialPoint ExpectedCenter1 = new SpacialPoint(mu1);
            SpacialPoint ExpectedCenter2 = new SpacialPoint(mu2); 
            WriteLine($"2 hypothetical centers: [[{mu1[0]}, {mu1[1]}, {mu1[2]}], [{mu2[0]},{mu2[1]},{mu2[2]}]],");
            int Counter = 100; 
            WriteLine("Average Normalized Deviations from centroid to centers");

            double DeviationAverage = 0; 
            while(--Counter!=0)
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
                double d = (
                                d1 / (Math.Sqrt(mu1.Length) * sd)
                                +
                                d2 / (Math.Sqrt(mu2.Length) * sd)
                            ) / 2;
               WriteLine(
                           d
                         );
                DeviationAverage += d/100;
            }
            WriteLine($"Average Deviation: {DeviationAverage}"); 
            
        }

        public void ClusteringWithParams(double[] mu1, double[] mu2, double ds, int N)
        { 
            
        }

    }
}
