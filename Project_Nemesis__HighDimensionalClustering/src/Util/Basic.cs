using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Collections.Immutable;

namespace Chaos.src.Util
{
    /// <summary>
    /// Stores all the fragments of functions that are going to be needed 
    /// for computing stuff. 
    /// </summary>
    public class Basic
    {

        public static bool Verbalize = false;

        private Basic()
        {
            // Don't use instances. 
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
        public static IList<char> FilterByAlphebeticalChars(string arg)
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
        /// 
        /// </summary>
        /// <param name="Directory">
        /// </param>
        /// <returns></returns>
        public static IDictionary<String, String> GetContentForAllFiles(String Directory, bool recursive = false)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory);
            if (!dirInfo.Exists) return null;
            FileInfo[] files = recursive? ListFilesRecur(Directory):dirInfo.GetFiles();
            IDictionary<String, String> content = new SortedDictionary<String, String>();
            try 
            {
                foreach (FileInfo eachFile in files)
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
                return null;
            }
            return content;
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
            for(int I = 1; I < charList.Count; I++)
            {
                char c1 = charList[I - 1], c2 = charList[I]; 
                int row = c1 == ' '? 26: c1 - 'a';
                int col = c2 == ' ' ? 26 : c2 - 'a';
                res[row, col] += 1;
            }

            double[,] normalizedFreqMatrix = NormalizeAllRow(res);
            return normalizedFreqMatrix;
        }

        public static double Matrix2NormDistance(double[,] a, double[,] b)
        {
            // dimension check: 
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new Exception("Matrix Dimension Mismatched. "); // UNDONE: SPECIFY EXCEPTION TYPE. 
            double sum = 0;
           
            for (int I = 0; I < a.GetLength(0); I++)
            for (int J = 0; J < a.GetLength(1); J++)
            {
                sum += Math.Pow(a[I, J] - b[I, J], 2.0);
            }

            return Math.Sqrt(sum); 
        }

        public static double MatrixVectorizedOneNorm(double[,] a, double[,] b)
        {
            // dimension check: 
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                throw new Exception("Matrix Dimension Mismatched. "); // UNDONE: SPECIFY EXCEPTION TYPE.
            double sum = 0;

            for (int I = 0; I < a.GetLength(0); I++)
            for (int J = 0; J < a.GetLength(1); J++)
            {
                sum += Math.Abs(a[I, J] - b[I, J]);
            }
            return sum;
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


        public static void Println(object o)
        {
            Console.WriteLine(o);
        }

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
        public static ImmutableList<T1> ImmuteList<T1>(IList<T1> arg)
        {
            return ImmutableList.ToImmutableList<T1>(arg);
        }


    }

    
    
}
