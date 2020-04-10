using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace Chaos.src.Util
{
    public class Basic
    {
        private Basic()
        {
            // Don't use instances. 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static string FilterByAlphebeticalChars(string arg)
        {
            IList<char> charStream = new List<char>();


            return null;
        }

    }
}
