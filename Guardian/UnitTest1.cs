using NUnit.Framework;
using System.Collections.Generic;
using static Chaos.src.Util.Basic;
using System;

namespace Guardian
{
    public class TestUntil
    {

        public static string THEPATH = @"C:\Users\victo\source\repos\Math-381-Project-2\data\Mark Twain";

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
    }
}