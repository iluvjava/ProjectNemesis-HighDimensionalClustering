using System;
using System.Collections.Generic;
using System.IO;
using Chaos.src.Util;
using NUnit.Framework;
using Chaos.src.Ethena;

namespace Guardian
{

    
    class TestTextClassifier
    {

        public const string Path =
            @"C:\Users\victo\source\repos\ProjectNemesis-HighDimensionalClustering\Project_Nemesis__HighDimensionalClustering\assets";

        IDictionary<string, string> FilesAndContent;

        [SetUp]
        public void SettingThingsUp()
        {
            // Check if path exists and has file I guess?

            DirectoryInfo dirinfo = new DirectoryInfo(Path);
            if (!dirinfo.Exists)
            {
                throw new IOException("Directory doesn't exist. ");
            }

            IDictionary<string, string> FilesAndContents = Basic.GetContentForAllFiles(Path, true, "txt");
            this.FilesAndContent = FilesAndContents;

        }

        /// <summary>
        ///     Basic shit. 
        /// </summary>
        [Test]
        public void TestingTheBasics()
        {
            TextFileClassifier Classifier = TextFileClassifier.GetInstance(Path);
            Console.WriteLine(Classifier.GetReport());

        }

        
    }
}
