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

        [Test]
        public void TestFullgraphBasics()
        {
            Point[] points = new Point[]
            {
                new SpacialPoint(new int[]{ 1, 0}),
                new SpacialPoint(new int[]{ 2, 0}),
                new SpacialPoint(new int[]{ 3, 0}),
                new SpacialPoint(new int[]{ 3, 10})
            };

            FullGraph fg = new FullGraph(points);
            WriteLine(fg.IndexToVertices);
            WriteLine(fg.ChosedForMST);

        }

        [Test]
        public void TestClustering()
        {
               
        }

    }
}
