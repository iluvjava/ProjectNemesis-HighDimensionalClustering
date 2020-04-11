using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace Chaos.src.Util
{

    /// <summary>
    /// Stores all the fragments of functions that are going to be needed 
    /// for computing stuff. 
    /// </summary>
    public class Basic
    {
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
                    if (charStream.Count == 0 || charStream[charStream.Count - 1] == ' ')
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
        public static IDictionary<String, String> GetContentForAllFiles(String Directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory);
            if (!dirInfo.Exists) return null;
            FileInfo[] files = dirInfo.GetFiles();
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
                int row = c1 == ' '? 27: c1 - 'a';
                int col = c2 == ' ' ? 27 : c2 - 'a';
                res[row, col] += 1;
            }

            double[,] normalizedFreqMatrix = NormalizeAllRow(res);
            return normalizedFreqMatrix;
        }

        public static double Matrix2NormDistance(double[,] a, double[,] b)
        {
            double sum = 0;
            for (int I = 0; I < a.GetLength(0); I++)
                for (int J = 0; J < a.GetLength(1); J++)
                {
                    sum += Math.Pow(a[I, J] - b[I, J], 2.0);
                }
            return Math.Sqrt(sum); // TODO: TEST THIS
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
