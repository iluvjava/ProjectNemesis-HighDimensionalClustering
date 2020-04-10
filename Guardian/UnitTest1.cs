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
        public void Test1()
        {
            IDictionary<string, string> content = GetContentForAllFiles(THEPATH);
            Console.WriteLine("Test 1...");
            foreach (KeyValuePair<string, string> kvp in content)
            {
                Console.Out.WriteLine($"Filename: \"{kvp.Key}\", ContentLength: {kvp.Value.Length}");
            }
        }
    }
}