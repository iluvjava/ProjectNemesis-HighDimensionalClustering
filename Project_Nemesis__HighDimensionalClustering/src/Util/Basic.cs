﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.IO;
using System.Collections.Immutable;
using MathNet.Numerics.Optimization;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using MathNet.Numerics.Integration;
using MathNet.Numerics;
using System.Runtime.InteropServices;

namespace TheBase.src.Util
{
    /// <summary>
    ///     Stores all the fragments of functions that are going to be needed 
    ///     for computing stuff. 
    ///     
    ///     * I don't know why this class is not static... 
    /// </summary>
    public class Basic
    {

        public static bool Verbalize = false; // whether to print a lot of stuff to the console. 

        /// <summary>
        ///     Take the average, entries wise, for a list of doubles. 
        ///     * Zagged array is ok, it will do it entrywise, divided by the number of 
        ///     inner arrays having that entry available. 
        /// </summary>
        /// <param name="arg">
        ///     Nested array. 
        /// </param>
        /// <returns>
        ///     The entrywiese average. 
        /// </returns>
        public static double[] EntrywiseAverage(double[][] arg)
        {
            if (arg.Length == 0) return null;
            int MaxArrayLength = arg[0].Length;

            for (int I = 0; I < arg.Length; I++)
            {
                MaxArrayLength = Math.Max(MaxArrayLength, arg[I].Length);
            }

            double[] EntrySum = new double[MaxArrayLength];
            double[] EntryCount = new double[MaxArrayLength];
            double[] Avg = new double[MaxArrayLength];
            for (int I = 0; I < arg.Length; I++)
                for (int J = 0; J < arg[I].Length; J++)
                {
                    EntrySum[J] += arg[I][J];
                    EntryCount[J]++;
                }
            for (int I = 0; I < MaxArrayLength; I++)
            {
                Avg[I] = EntrySum[I] / EntryCount[I];
            }

            return Avg;
        }

        /// <summary>
        ///     Takes the average of entries wise average for 2d array, 
        ///     the size of the 2d uniform arrays can be arbitrarily sized, 
        ///     and it will stake take the average because it only focuses the averages 
        ///     of each entries
        ///     
        ///     The resulting 2D array will have width and height of the 2d array 
        ///     that has the largest width and height in the give list of 2d array. 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static double[,] EntrywiseAverage(double[][,] arg)
        {
            if (arg.Length == 0) throw new ArgumentException("Cannot accept empty argument. ");
            int MaxH = arg[0].GetLength(0), MaxW = arg[0].GetLength(1);
            {
                for (int I = 0; I < arg.Length; I++)
                {
                    MaxH = Math.Max(arg[I].GetLength(0), MaxH);
                    MaxW = Math.Max(arg[I].GetLength(1), MaxW);
                }
            }

            int[,] FrequenciesTable = new int[MaxH, MaxW];
            double[,] SumTable = new double[MaxH, MaxW];
            {
                for (int I = 0; I < arg.Length; I++)
                {
                    double[,] M = arg[I];
                    for (int J = 0; J < M.GetLength(0); J++)
                        for (int K = 0; K < M.GetLength(1); K++)
                        {
                            FrequenciesTable[J, K]++;
                            SumTable[J, K] += M[J, K];
                        }
                }
            }

            double[,] AverageTable = new double[MaxH, MaxW];
            {
                for (int I = 0; I < MaxH; I++)
                    for (int J = 0; J < MaxW; J++)
                    {
                        AverageTable[I, J] = SumTable[I, J] / FrequenciesTable[I, J];
                    }
            }
            return AverageTable;
        }

        /// <summary>
        /// Prepare the string for the transition matrices. 
        ///     * Capital letters are not removed. 
        ///     * Apostrophe is ignored. 
        ///     * Multiple Spaces are merged into on space. 
        /// 
        /// </summary>
        /// <returns>
        /// A list of characters
        /// </returns>
        public static IList<char> FilterByAlphebeticalChars(string arg) // UNDONE: Paralize? 
        {
            IList<char> charStream = new List<char>();
            arg = arg.ToLower();
            foreach (char c in arg)
            {
                char cc = c;
                if (cc >= 'a' && cc <= 'z')
                {
                    charStream.Add(cc);
                }
                else if (cc == ' ')
                {
                    if (charStream.Count == 0 || charStream[charStream.Count - 1] != ' ')
                        charStream.Add(cc);
                }
                else; // empty. 
            }
            return charStream;
        }

        /// <summary>
        ///     A 27 character transition matrix, second order. 
        /// </summary>
        /// <param name ="s">
        ///     The raw text in English. 
        /// </param>
        /// <returns>
        ///     A double, which is gotten from the converted string. 
        /// </returns>
        public static double[,] Get2ndTM27(string s) // UNDONE: Paralize?
        {
            int[,] res = new int[27 * 27, 27 * 27];
            IList<char> charlist = FilterByAlphebeticalChars(s);
            for (int I = 2; I < charlist.Count - 1; I++)
            {
                int c1 = charlist[I - 2] == ' ' ? 26 : charlist[I - 2] - 'a';
                int c2 = charlist[I - 1] == ' ' ? 26 : charlist[I - 1] - 'a';
                int c3 = charlist[I] == ' ' ? 26 : charlist[I] - 'a';
                int c4 = charlist[I + 1] == ' ' ? 26 : charlist[I + 1] - 'a';
                res[c1 * c2, c3 * c4] += 1;
            }
            double[,] normalizedFreqMatrix = NormalizeAllRow(res);
            return normalizedFreqMatrix;
        }

        /// <summary>
        ///     Function gets the content of all files under a certain directory on the system, 
        ///     * Recursive scan
        ///     * Filter by one postfix. 
        /// </summary>
        /// <param name="Directory">
        ///     String representing the directory, without / at the end of the string. 
        /// </param>
        /// <param name="recursive">
        ///     When this is set to true, it will scan all folders under the directory, recursively. 
        /// </param>
        /// <param name="pstfix">
        ///     Given a post fix, e.g: .txt so that only .txt files will get returned. 
        /// </param>
        /// <returns></returns>
        public static IDictionary<String, String> GetContentForAllFiles(
                String Directory,
                bool recursive = false, 
                string pstfix = null
            )
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory);
            if (!dirInfo.Exists) return null;
            FileInfo[] files = recursive? ListFilesRecur(Directory):dirInfo.GetFiles();
            if (!(pstfix is null))
            {
                files = FilterFilesByPostFix(files, pstfix);
            }
            else 
            {
                files = FilterFilesByPostFix(files, pstfix); 
            }
            IDictionary<String, String> content = new SortedDictionary<String, String>();
            try 
            {
                foreach(FileInfo eachFile in files)
                {
                   String name = eachFile.Name;
                   using (StreamReader sr = File.OpenText(eachFile.FullName))
                   {
                       StringBuilder sb = new StringBuilder();
                       sb.Append(sr.ReadToEnd());
                       content[name] = sb.ToString();
                   }
               }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return null;
            }
            return content;
        }

        /// <summary>
        /// The string is unprocseed file, 
        /// how it will be filtered or configured is not depended 
        /// in this class, it will be managed by settingsmanager. 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double[,] GetTM27(string s)
        {
            int[,] res = new int[27, 27];

            IList<char> charList = FilterByAlphebeticalChars(s); // HACK: This part for tweaking later. 
            for (int I = 1; I < charList.Count; I++)
            {
                char c1 = charList[I - 1], c2 = charList[I];
                int row = c1 == ' ' ? 26 : c1 - 'a';
                int col = c2 == ' ' ? 26 : c2 - 'a';
                res[row, col] += 1;
            }

            double[,] normalizedFreqMatrix = NormalizeAllRow(res);
            return normalizedFreqMatrix;
        } //UNDONE: Paralize?
        /// <summary>
        /// Good helper function. 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        public static ImmutableDictionary<T1, T2> ImmuteDic<T1, T2>(IDictionary<T1, T2> d)
        {
            return ImmutableDictionary.ToImmutableDictionary<T1, T2>(d);
        }

        /// <summary>
        /// Good helper function. 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static IImmutableList<T1> ImmuteList<T1>(IList<T1> arg)
        {
            return ImmutableList.ToImmutableList<T1>(arg);
        }

        public static IImmutableSet<T1> ImmuteSet<T1>(HashSet<T1> arg)
        {
            return ImmutableHashSet.ToImmutableHashSet<T1>(arg);
        }

        public static IImmutableSet<T1> ImmuteSet<T1>(SortedSet<T1> arg)
        {
            return ImmutableSortedSet.ToImmutableSortedSet<T1>(arg);
        }

        public static IImmutableSet<T1> ImmuteSet<T1>(ISet<T1> arg)
        {
            return ImmutableHashSet.ToImmutableHashSet<T1>(arg);
        }

        /// <summary>
        ///     Vectorize the matrix and then take the Euclidean distance. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        ///     a positive float. 
        /// </returns>
        public static double Matrix2NormDistance(double[,] a, double[,] b)
        {
            double[] FlattenA = a.Cast<double>().ToArray(), FlattenB = b.Cast<double>().ToArray();
            return Distance.Euclidean(FlattenA, FlattenB);
        }

        public static double MatrixVectorizedOneNorm(double[,] a, double[,] b)
        {
            // dimension check: 
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new Exception("Matrix Dimension Mismatched. ");
            double sum = 0;

            for (int I = 0; I < a.GetLength(0); I++)
                for (int J = 0; J < a.GetLength(1); J++)
                {
                    sum += Math.Abs(a[I, J] - b[I, J]);
                }
            return sum;
        }

        /// <summary>
        ///     Matrix is assumed to be possitive. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// 
        /// </returns>
        public static double MatrixConsineDistance(double[,] a, double[,] b)
        {
            double[] FlattenedA = a.Cast<double>().ToArray(), FlattenedB = b.Cast<double>().ToArray();

            double dis = Distance.Cosine(FlattenedA, FlattenedB);
            return dis;
        }
        public static void Println(object o)
        {
            Console.WriteLine(o);
        }

        /// <summary>
        ///     Filter out files containing a certain postfix. 
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns>
        /// An array of files filtered by post fix. 
        /// </returns>
        static FileInfo[] FilterFilesByPostFix(FileInfo[] files, string postfix)
        {
            var filtered = new List<FileInfo>();
            foreach (FileInfo f in files)
            {
                if (f.Name.Substring(f.Name.Length - 3).Equals(postfix))
                {
                    filtered.Add(f);
                }
            }
            return filtered.ToArray();
        }

        static FileInfo[] ListFilesRecur(string dir)
        {
            List<FileInfo> fileinfo = new List<FileInfo>();
            ListFilesRecur(new DirectoryInfo(dir), fileinfo);
            FileInfo[] res = fileinfo.ToArray();
            return res;
        }

        /// <summary>
        /// Get all the files under all subdirectory. 
        /// </summary>
        /// <param name="Directory"></param>
        /// <returns></returns>
        static void ListFilesRecur(DirectoryInfo Directory, IList<FileInfo> bucket)
        {
            foreach (FileInfo fi in Directory.GetFiles())
            {
                bucket.Add(fi);
            }
            foreach (DirectoryInfo di in Directory.GetDirectories())
            {
                ListFilesRecur(di, bucket); 
            }
        }
        
        static double[,] NormalizeAllRow(int[,] a)
        {
            double[,] res = new double[a.GetLongLength(0), a.GetLength(1)]; 
            for (int I = 0; I < a.GetLength(0); I++)
            {
                int rowSum = 0;
                for (int J = 0; J < a.GetLength(1); J++)
                {
                    rowSum += a[I, J];
                }
                if (rowSum == 0) continue;
                for (int J = 0; J < a.GetLength(1); J++)
                {
                    res[I, J] = (a[I, J] + 0.0) / rowSum;
                }
            }
            return res;
        }
    }

    
    
}
