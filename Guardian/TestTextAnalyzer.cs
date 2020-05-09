using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Chaos.src.Ethena;
using Chaos.src.Util;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using static System.Console; 
namespace Guardian
{

   

    class TestTextAnalyzer
    { 
        public const string Path = @"C:\Users\victo\source\repos\Math-381-Project-2\data";
        IDictionary<string, string> FilesAndContent; 

        [SetUp]
        public void SetThingsup()
        {
            // Check if path exists and has file I guess? 
            DirectoryInfo dirinfo = new DirectoryInfo(Path);
            if (!dirinfo.Exists)
            {
                throw new ArgumentException(); 
            }

            IDictionary<string, string> FilesAndContents = Basic.GetContentForAllFiles(Path, true, "txt");
            this.FilesAndContent = FilesAndContents; 
        }


        [Test]
        public void TestTextClustring()
        {
            WriteLine($"Files and Content Sizes:{FilesAndContent.Count} ");

            TextFileCluster tfc = new TextFileCluster(FilesAndContent);
            FullGraphClustering fgc = tfc.ClusterIdentifier;
            IImmutableSet<Point> FirstCluster = fgc.ClusterMajor;
            IImmutableSet<Point> SecondCluster = fgc.ClusterMinor;

            WriteLine("-----------------------This is the first cluster: -----------------------");
            foreach (Point p in FirstCluster)
            {
                WriteLine((p as Text).file_name); 
            }
            WriteLine("----------------------This is the second cluster: -----------------------");
            foreach (Point p in SecondCluster)
            {
                WriteLine((p as Text).file_name); 
            }


        }
    }
}

