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

            TextFileClusterReporter tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());

            WriteLine("*Let's use a vectorized metric for it. ");
            SettingsManager.SetDisFxnToVecNorm();
            tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport());

            WriteLine("* Let's use the second order transition matrix: ");
            SettingsManager.Set2ndTM27ForTransitionMatrix();
            tfr = new TextFileClusterReporter(FilesAndContent);
            WriteLine(tfr.GetReport()); 

        }
    }
}

