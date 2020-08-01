using Chaos.src.Util;
using Project_Nemesis__HighDimensionalClustering.src.Util;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunners;
using static Chaos.src.Util.SettingsManager;
 

namespace Chaos.src.Ethena
{
    /// <summary>
    /// Text and the transition matrices generated from the text. 
    /// * Everything should be setted when the thing is initialized. 
    /// </summary>
    public class Text: Point
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
        public Text(string content, string name)
        {
            this.content = content;
            this.file_name = name;
            Util.MatrixGenFxn MgenFxn = DisPatchMatrixGenFxn();
            transition_matrix = MgenFxn(content);
        }

        /// <summary>
        ///     Constructor for internal use only 
        /// </summary>
        protected Text()
        { }

        public static double Dis (Text a, Text b)
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

        /// <summary>
        ///     Finds the distance between 2 instance of Text. 
        /// </summary>
        /// <returns>
        ///     A double representing the distance of the transition matrices. 
        /// </returns>
        public static Text GetCentroidAmong(Text[] l)
        {
            double[][,] Matrices = 
                new double[l.Length][,];
            for (int I = 0; I < l.Length; I++) Matrices[I] = l[I].transition_matrix;
            Text center = new Text();
            center.transition_matrix = Basic.EntrywiseAverage(Matrices);
            center.file_name = "Hypothetical Centroid";
            return center; 
        }

    }

    [Obsolete("Replaced by TextFileClassifier")]
    /// <summary>
    ///     This class cluster text files under a certain directory, and it 
    ///     will just print out the clustering in the end. 
    ///     
    ///     * This class will treat the list of instances of text as point and 
    ///     feed them to full graph class. 
    /// </summary>
    public class TextFileCluster
    {
        public FullGraphClustering ClusterIdentifier {get; protected set;}
        public string Dir { get; protected set; }
        Text[] Texts;

        public TextFileCluster(string directory)
        {
            DirectoryInfo dinfo = new DirectoryInfo(directory);
            if (dinfo.Exists)
            {
                throw new ArgumentException(); 
            }
            this.Dir = directory; 
            IDictionary<string, string> FilesAndContent = Basic.GetContentForAllFiles(directory, false , "txt");

            ConstructorHelper(FilesAndContent);
        }

        public TextFileCluster(IDictionary<string, string> FilesAndContent)
        {
            ConstructorHelper(FilesAndContent); 
        }

        protected void ConstructorHelper(IDictionary<string, string> FilesAndContent) // UNDONE: HOW TO Paralize???
        {

            Queue<Task<Point>> tasks = new Queue<Task<Point>>();

            foreach (KeyValuePair<string, string> kvp in FilesAndContent)
            {
                tasks.Enqueue(
                        new Task<Point>(
                                () => 
                                {
                                    return new Text(kvp.Value, kvp.Key);
                                }
                            )
                    );
            }

            QueueBasedTaskRunner<Point> runner = new QueueBasedTaskRunner<Point>(tasks);
            runner.RunParallel();
            ClusterIdentifier = new FullGraphClustering(runner.GetResult().ToArray());
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }


    /// <summary>
    ///     This class is for reporting the results found by the 
    ///     TextFileCluster in a more acceptable manner. 
    ///     
    ///     This class should be interfacing with the outside really. 
    /// </summary>
    [Obsolete("Replaced by TextFileClassifier")]
    public class TextFileClusterReporter
    {
        protected TextFileCluster FileCluster;

        public TextFileClusterReporter(IDictionary<string, string> FileAndContent)
        {
            FileCluster = new TextFileCluster(FileAndContent);
        }

        public TextFileClusterReporter(string directory)
        {
            FileCluster = new TextFileCluster(directory);
        }

        /// <summary>
        ///     Generate a report on the 2 clustering of files names. 
        /// </summary>
        /// <returns></returns>
        public string GetReport()
        {
            StringBuilder sb1 = new StringBuilder("The first clusters of files: \n");
            FullGraphClustering theFullGraph = FileCluster.ClusterIdentifier;
            IImmutableSet<Point> clusterMajor = theFullGraph.ClusterMajor;
            IImmutableSet<Point> clusterMinor = theFullGraph.ClusterMinor;

            // String for first cluster. 

            foreach (Point p in clusterMajor)
            {
                Text t = p as Text;
                sb1.AppendLine($"\t{t.file_name}");
            }
            sb1.AppendLine("This is the second cluster of files: ");
            foreach (Point p in clusterMinor)
            {
                Text t = p as Text;
                sb1.AppendLine($"\t{t.file_name}");
            }

            return sb1.ToString();
        }
    }

    /// <summary>
    ///     <para>
    ///      Class construct an instance with the following information: 
    ///     </para>
    ///    
    ///     TODO: Test this class. 
    /// </summary>
    public class TextFileClassifier {

        // TODO: Do these: 
        // 1. Construct a KMinSpanningTree Instance. 
        // 2. Run that instance
        // 3. Analyze the output 
        // 4. Produce report on it. 

        protected KMinSpanningTree ClusterIdentifier;
        // Store it for convenience of producing reports. 
        protected string Directory; 

        /// <summary>
        ///     For internal use only. 
        /// </summary>
        protected TextFileClassifier(){}

        /// <summary>
        ///     
        /// </summary>
        /// <param name="fileAndContent"></param>
        /// <param name="maxClusterSize"></param>
        /// <returns></returns>
        public static TextFileClassifier GetInstance(
            IDictionary<string, string> fileAndContent, 
            int maxClusterSize = -1)
        {

            Queue<Task<Point>> tasks = new Queue<Task<Point>>();

            foreach (KeyValuePair<string, string> kvp in fileAndContent)
            {
                tasks.Enqueue(
                        new Task<Point>(
                                () =>
                                {
                                    return new Text(kvp.Value, kvp.Key);
                                }
                            )
                    );
            }

            QueueBasedTaskRunner<Point> runner = new QueueBasedTaskRunner<Point>(tasks);
            runner.RunParallel();
            TextFileClassifier TheInstance = new TextFileClassifier();
            TheInstance.ClusterIdentifier = new KMinSpanningTree(runner.GetResult().ToArray(), maxClusterSize);
            return TheInstance; 
        }

        /// <summary>
        ///     Get the instance prividing the directory containing the text files you want to classfiy 
        /// </summary>
        /// <param name="directory">
        ///     A string that points to the directory of targeted files. 
        /// </param>
        /// <param name="maxClusterSize">
        ///     The maximum size you of the clusters you want to set. 
        /// </param>    
        /// <returns>
        ///     An instacne of TextFileClassifier.
        /// </returns>
        public static TextFileClassifier GetInstance(string directory, int maxClusterSize = -1)
        {
            DirectoryInfo dinfo = new DirectoryInfo(directory);
            if (!dinfo.Exists)
            {
                throw new ArgumentException("Directory doesn't exist.");
            }
            
            IDictionary<string, string> FilesAndContent = Basic.GetContentForAllFiles(
                directory, 
                SettingsManager.FileSearchRecursive, 
                "txt");
            TextFileClassifier TheInstance = GetInstance(FilesAndContent, maxClusterSize);
            TheInstance.Directory = directory;
            return TheInstance;
        }

        /// <summary>
        ///     Produce a report for all the files in the specified directory. 
        /// </summary>
        /// <returns>
        ///     A string that is ready to print. 
        /// </returns>
        public string GetReport()
        {

            // Evaluate and Digest Results -----------------------------------------------------------------------------
            ClusterIdentifier.KMinKruskal();
            IEnumerable<PointCollection> Step1 = ClusterIdentifier.KSpanningTree.AsEnumerable();
            IEnumerable<Point[]> Step2 = Step1.Select((pointArray) => pointArray.ToArray());
            IEnumerable<Text[]> Step3 = Step2.Select((textArray) =>
            {
                return Array.ConvertAll(
                    textArray, (eachText) => eachText as Text
                );
            });

            // To Return -----------------------------------------------------------------------------------------------
            StringBuilder Report = new StringBuilder();
            Report.Append(ConsoleLog.GetSeperator());
            Report.AppendLine("Identified Clusters");
            Report.Append(ConsoleLog.GetSeperator());
            foreach (Text[] Cluster in Step3)
            {
                foreach (Text T in Cluster)
                {
                    Report.AppendLine(T.file_name);
                }
                Report.Append(ConsoleLog.GetSeperator('-'));
            }
            return Report.ToString(); 
        }
    
    }

    
}
