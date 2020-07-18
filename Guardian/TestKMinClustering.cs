using Chaos.src.Ethena;
using NUnit.Framework;
using System.Collections.Immutable;
using static System.Console;


namespace Guardian
{
    class TestKMinClustering
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        public void Test1() {

            Point[] Points = new Point[]
            {
                new SpacialPoint(new int[]{ 1, 0}),
                new SpacialPoint(new int[]{ 2, 0}),
                new SpacialPoint(new int[]{ 3, 0}),
                new SpacialPoint(new int[]{ 3, 10}),
                new SpacialPoint(new int[]{ 4, 0})
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
    }
}
