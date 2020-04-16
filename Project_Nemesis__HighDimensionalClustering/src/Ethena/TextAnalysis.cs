using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static Chaos.src.Util.SettingsManager;
 

namespace Chaos.src.Ethena
{
    /// <summary>
    /// Text and the transition matrices generated from the text. 
    /// * Everything should be setted when the thing is initialized. 
    /// </summary>
    public class TextAnalysis: Point
    {
        public string file_name { get; protected set; } 
        public string content { get; protected set; }
        public double[,] transition_matrix { get; protected set; } 

        /// <summary>
        /// Establish the content of the document 
        /// Estalbish the settings for generating stuff. 
        /// * Type of transition matrix depends on the settings manager. 
        /// * Type of distance depends on settings manager. 
        /// </summary>
        /// <param name="content">
        /// A string, raw. 
        /// </param>
        /// <param name="name">
        /// A name for the text analysis instance. 
        /// </param>
        public TextAnalysis(string content, string name)
        {
            this.content = content;
            this.file_name = name;
            Util.MatrixGenFxn MgenFxn = DisPatchMatrixGenFxn();
            transition_matrix = MgenFxn(content);
        }


        public static double Dis (TextAnalysis a, TextAnalysis b)
        {
            return DispatchMatrixDisFxn()(a.transition_matrix, b.transition_matrix);
        }

        /// <summary>
        /// Print out the string for this text analysis instance.
        /// * The transition matrix. 
        /// * 
        /// </summary>
        /// <returns></returns>
        override 
        public string ToString()
        {
            // String formatting:
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings

            string res = $"Instance Type: TextAnalysis; Insance Name: {this.file_name}\n";
            res += "==========================================================\n";
            res += "Transition Matrix:\n";
            res += "==========================================================\n";
            StringBuilder sb = new StringBuilder();
            for (int I = 0; I < transition_matrix.GetLength(0); I++)
            {
                for (int J = 0; J < transition_matrix.GetLength(1); J++)
                {
                    sb.Append(String.Format("{0:0.00E+0}", transition_matrix[I, J]) + " "); 
                }
                sb.Append("\n");
            }
            return res + sb.ToString(); 
        }

    }

   
}
