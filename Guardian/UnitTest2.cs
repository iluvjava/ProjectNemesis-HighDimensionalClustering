using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Chaos.src.Util;

using Chaos.src.Ethena;

using static System.Console;

namespace Guardian
{

    public class TestTextAnalysis
    {

        public static string PATH = @"C:\Users\victo\source\repos\Math-381-Project-2\data"; 

        [SetUp]
        public void Setup()
        {

        }

        /* [Test]
         public void TestTextAnalysisBasic()
         {
             IDictionary<string, string> stuff = Basic.GetContentForAllFiles(PATH, true);
             foreach (KeyValuePair<string, string> kvp in stuff)
             {
                 WriteLine(new TextAnalysis(kvp.Value, kvp.Key));
             }
         }

         /// <summary>
         /// This test will print out all the edges from the full graph, 
         /// and it's up to the programmer to visualize the data and 
         /// determine its correctness. 
         /// </summary>
         [Test]
         public void TestTheFullGraph()
         {
             IDictionary<string, string> stuff = Basic.GetContentForAllFiles(PATH, true);

             ISet<TextAnalysis> thethings = new HashSet<TextAnalysis>();
             foreach (KeyValuePair<string, string> kvp in stuff)
             {
                 thethings.Add(new TextAnalysis(kvp.Value, kvp.Key));
             }

             FullGraph thegraph = new FullGraph(thethings);
             WriteLine(thegraph.DistanceList(true));
             WriteLine($"Total number of verteices in the full graph is: {thegraph.VertexCount()}");
         }*/


        [Test]
        void TestClustering()
        { 
        
        
        }

    }
}
