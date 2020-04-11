using NUnit.Framework;
using System.Collections.Generic;
using static Chaos.src.Util.Basic;
using System;

namespace Guardian
{
    public class TestUntil
    {

        public static string THEPATH = @"C:\Users\victo\source\repos\Math-381-Project-2\data\Mark Twain";
        public static string THEPATH2 = @"C:\Users\victo\source\repos\Math-381-Project-2\data";
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestFilesReading()
        {
            IDictionary<string, string> content = GetContentForAllFiles(THEPATH);
            Console.WriteLine("Test 1...");
            foreach (KeyValuePair<string, string> kvp in content)
            {
                Console.Out.WriteLine($"Filename: \"{kvp.Key}\", ContentLength: {kvp.Value.Length}");
                Console.Out.WriteLine(kvp.Value.Substring(0, 100));
            }
        }


        /// <summary>
        /// Testing if it reads all files from the subdirectory. 
        /// </summary>
        [Test]
        public void TestFileReadingRecur()
        {
            IDictionary<string, string> files = GetContentForAllFiles(THEPATH2, true);
            foreach(string k in files.Keys)
            Console.WriteLine($"Here is a list of files read from the directory: {k}");
        }

        [TestCase("abcd ")]
        public void TestGetTM27(string stuff)
        {
            double[,] m = GetTM27(stuff);
            Console.WriteLine("This is the transition matrix. ");
            for (int I = 0; I < m.GetLength(0); I++)
            {
                for (int J = 0; J < m.GetLength(1); J++)
                {
                    Console.Out.Write(m[I, J] + " ");
                }
                Console.Out.WriteLine();
            }
        }


        //TODO: TEST THIS
        /// <summary>
        /// Make sure the matrix distance is well defined for this .
        /// </summary>
        [Test]
        public void TestMatrix2Norm()
        { 
        
        }



    }
}