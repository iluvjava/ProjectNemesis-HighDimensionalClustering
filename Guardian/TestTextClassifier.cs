using System;
using System.Collections.Generic;
using System.IO;
using TheBase.src.Util;
using NUnit.Framework;
using TheBase.src.Core;

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

        /// <summary>
        ///     The previous one but with cosine distance this time. 
        /// </summary>
        [Test]
        public void TestingTheBasicsCosinDistance()
        {
            Console.WriteLine("Using the Cosine metric for the clustering of the elements, TM27 Matrix. ");
            SettingsManager.setDisFxnToCosine();

            TextFileClassifier classifier = TextFileClassifier.GetInstance(Path);
            Console.WriteLine(classifier.GetReport());

            Console.WriteLine("Using the Cosine for the clustering of the elements, 2ndTM27 Matrix. ");
            SettingsManager.Set2ndTM27ForTransitionMatrix();
            classifier = TextFileClassifier.GetInstance(Path);
            Console.WriteLine(classifier.GetReport());

        }

        public void TestingTheBasicsOneNorm()
        {
            Console.WriteLine("Using the 1-norm to cluster the set of points.");
            
        }
        
        
    }
}
