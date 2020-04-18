using MyDatastructure.UnionFind;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Chaos.src.Util;

namespace Chaos.src.Ethena
{
    public class Edge: IComparable
    {
        double cost;
        double ID; 
        public Edge(Point a, Point b)
        {
            this.a = a;
            this.b = b;
            this.cost = Point.Dis(a, b);
            this.ID = (new Random()).NextDouble();
        }
        public Point a { get; protected set; }
        public Point b { get; protected set; }
        public static int CompareWeight(Edge x, Edge y)
        {
            return Math.Sign(x.cost - y.cost);
        }

        /// <summary>
        ///     Edges equal <==> Points are equaled. 
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns></returns>
        public static bool DeepEquals(Edge x, Edge y)
        {
            return x.a.Equals(y.a) && x.b.Equals(y.b); 
        }

        public static double CompareID(Edge x, Edge y)
        {
            return x.ID - y.ID; 
        }

        public int CompareTo(object obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new ArgumentException();
            if (obj.GetType() != typeof(Edge)) throw new ArgumentException(); // Strict Type equal. 
            if (object.ReferenceEquals(obj, this)) return 0;
            if (DeepEquals(this, (Edge)obj)) return 0; 
            int dw = CompareWeight(this, (Edge)obj);
            if (dw != 0) return dw;
            return Math.Sign(CompareID(this, (Edge)obj));
        }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{{ {this.a}, { this.b} }}");
            return sb.ToString();
        }

    }

    /// <summary>
    /// TODO: This whole class is not tested yet. 
    /// </summary>
    public class FullGraph
    {
        protected SortedSet<Edge> E; // Edges soted by weight
        protected SortedSet<Edge> ChosenE;  // Edges Chosen by Kruskal sorted by weight. 
        // Maximum partition size during each iteration of cruskal 
        protected IList<int> max_partition = new List<int>();
        protected IDictionary<int, Point> V;
        protected IDictionary<Point, int> W;

        public FullGraph(Point[] points)
        {
            V = new SortedDictionary<int, Point>();
            E = new SortedSet<Edge>();        // Whther the edge is included in the MSt or not. 
            W = new Dictionary<Point, int>(); // hashed, order is lost in the reverse map. 
            // Chosen E established in EstablisheMST method. 

            for (int I = 0; I < points.Length; I++)
            {
                V[I] = points[I]; // index to vertex
                W[points[I]] = I; // reverse map
            }

            for (int I = 0; I < points.Length; I++)
            for (int J = I + 1; J < points.Length; J++)
            {
                E.Add(new Edge(points[I], points[J]));
            }

            max_partition = EstablishMST();
        }

        public virtual IImmutableSet<Edge> ChosedForMST
        {
            get
            {
                return Basic.ImmuteSet<Edge>(ChosenE);
            }
        }

        public virtual ImmutableDictionary<int, Point> IndexToVertices
        {
            get {
                
                return Basic.ImmuteDic<int, Point>(V);
            }        
        }
        public virtual ImmutableList<int> MaxPartitionSize
        {
            get
            {
                return Basic.ImmuteList<int>(max_partition);
            }
        }

        public virtual ImmutableDictionary<Point, int> VertexToIndex
        {
            get
            {
                return Basic.ImmuteDic<Point,  int>(W);
            }
        }
        override 
            public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"full graph with: |V|={V.Count}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Run Kruskal and establish the boolean edges selection. 
        /// * return the size of max partition while running the kruskal. 
        /// </summary>
        /// <returns>
        /// Max Partitions. 
        /// </returns>
        protected virtual IList<int> EstablishMST()
        {
            // A list of maximum size for each iteration. 
            IList<int> MaxSize = new List<int>();
            
            // Disjoint set for kruskal.
            IDisjointSet<Point> ds = new ArrayDisjointSet<Point>();
            
            // Keep track of max partition.
            SortedSet<int> CompSizes = new SortedSet<int>();
            
            // mapping integer representative to the partition size. 
            IDictionary<int, int> PartitionSizes = new SortedDictionary<int, int>();

            for (int I = 0; I < V.Count; I++)
            {
                ds.CreateSet(V[I]);
                PartitionSizes[I] = 1; // Integer representative starts indexing from 1. 
            }

            SortedSet<Edge> ChosenEdges = new SortedSet<Edge>();
            MaxSize.Add(1);
            int TotalComponentCount = V.Count - 1; 
            foreach (Edge e in E)
            {
                Point u = e.a, v = e.b;
                if (ds.FindSet(u) != ds.FindSet(v))
                {
                    int MergedSize = PartitionSizes[ds.FindSet(u) - 1] + PartitionSizes[ds.FindSet(v) - 1];
                    ds.Join(u, v);
                    PartitionSizes[ds.FindSet(u) - 1] = MergedSize;
                    CompSizes.Add(MergedSize);
                    ChosenEdges.Add(e);
                    --TotalComponentCount;
                }

                MaxSize.Add(CompSizes.Max);
                if (TotalComponentCount == 0) break; 
            }

            // Establish Field. 
            ChosenE = ChosenEdges; 
            return MaxSize;
        }

        /// <summary>
        ///     * During the runtime of kruskal, there is one step where joining the 
        ///     components gives the maximum changes in size of the maximum partition
        ///         ** This point is the point the identify 2 clusters in the graph. 
        ///     * This method gives an index where this unionization happens. 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        protected virtual int IdentifyMaxBreakingEdge()
        {
            int MaxChange = 0, MaxChangeIndex = 0; 
            for (int I = 1; I < max_partition.Count; I++)
            {
                // HACK: This part, a >= > can make huge difference. 
                int delta = max_partition[I] - max_partition[I - 1];
                if ( delta > MaxChange)
                {
                    MaxChange = delta;
                    MaxChangeIndex = I;
                }

            }
            return MaxChangeIndex; 
        }

    }

    /// <summary>
    ///     TODO: IMPLEMENT THIS SHIT. 
    ///     * Should not accept fewer than 5 points, that is just too small to determine. 
    /// </summary>
    public class FullGraphClustering : FullGraph
    {
        protected Point Centroid1;
        protected Point centroid2;
        protected ISet<Point> Cluster1;
        protected ISet<Point> Cluters2;

        public FullGraphClustering(Point[] points): base(points)
        {
            
        }

        /// <summary>
        ///     Identify 2 points from the collection that are mostly liekly to be the center 
        ///     of 2 of the cluters. 
        /// </summary>
        protected virtual void IdentifyCentroids()
        { 
            
        }

        /// <summary>
        ///     Given the established centroids, this will classfy then based on the distance. 
        ///     
        /// </summary>
        protected virtual void ClassifyByCentroid()
        {

        }


        /// <summary>
        ///     * Run Kruskal again with the information about the max breaking edge 
        ///     in the graph. 
        /// </summary>
        protected virtual void KrusktalAagain()
        { 
        
        }


    }
}
