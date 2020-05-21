using MyDatastructure.UnionFind;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Chaos.src.Util;
using System.Threading.Tasks;

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
    ///     A class designed for ranking the clusters by their sizes. 
    ///     * A collection of points will be put into a set, and order by their sizes. 
    ///     * The top ranking 2 collection will be regarded as the potential centroids 
    ///     for 2 of the clusters we want to identify. 
    /// </summary>
    public class PointCollection : IComparable
    {

        protected HashSet<Point> PointSet;

        public PointCollection(Point p)
        {
            PointSet = new HashSet<Point>();
            PointSet.Add(p);
        }

        /// <summary>
        ///     Default constructor for static methods of the class. 
        /// </summary>
        protected PointCollection()
        { 
            
        }

        public int CompareTo(object obj)
        {
            if (object.ReferenceEquals(obj, null) || obj.GetType() != typeof(PointCollection))
            {
                throw new ArgumentException(); 
            }
            
            PointCollection that = (PointCollection)obj;
            if (object.ReferenceEquals(this, that)) return 0;
            if (DeepEqual(this, that)) return 0;
            int SizeDiff = this.PointSet.Count - that.PointSet.Count;
            if (SizeDiff != 0) return Math.Sign(SizeDiff);
            return Math.Sign(this.GetHashCode() - that.GetHashCode()); 
            // Having the same size doesn't mean they are equal.  
        }

        public static bool DeepEqual(PointCollection p1, PointCollection p2)
        {
            return p1.PointSet.Equals(p2.PointSet); 
        }

        /// <summary>
        ///     Union 2 collection together into one point collection. 
        /// </summary>
        /// <param name="pc1">
        ///     Point collection 1
        /// </param>
        /// <param name="pc2">
        ///     Point collection 2 
        /// </param>
        /// <returns>
        ///     The union of 2 of the point collection. 
        /// </returns>
        public static PointCollection operator + (PointCollection pc1, PointCollection pc2)
        {
            PointCollection Merged = new PointCollection();
            HashSet<Point> MergedSet = new HashSet<Point>();

            MergedSet.UnionWith(pc1.PointSet);
            MergedSet.UnionWith(pc2.PointSet);
            Merged.PointSet = MergedSet; 
            return Merged; 
        }

        public Point[] ToArray()
        {
            Point[] res = new Point[PointSet.Count];
            PointSet.CopyTo(res);
            return res; 
        }

        override
            public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Pointcollection: {");

            foreach (Point p in PointSet)
            {
                sb.Append(p.ToString() + ", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString(); 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FullGraph
    {
        protected SortedSet<Edge> E; // Edges soted by weight
        protected SortedSet<Edge> ChosenE;  // Edges Chosen by Kruskal sorted by weight. 
        // Maximum partition size during each iteration of cruskal 
        protected IList<int> max_partition = new List<int>();
        // The index 0 is always one, because before we run kruskal, all partitions are 1 vertex with size 1. 
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

            // Non parallel 
            {
                // for (int I = 0; I < points.Length; I++)
                //     for (int J = I + 1; J < points.Length; J++)
                //     {
                //         E.Add(new Edge(points[I], points[J]));
                //     }
            }
            // Parallel 
            {
                EdgesBuffers Buffer = new EdgesBuffers(E);
                Parallel.For(0, points.Length, I => {
                     for(int J = I + 1; J < points.Length; J++)
                     {
                         Buffer.LockAdd(new Edge(points[I], points[J])); 
                     }
                 });
            }

            max_partition = EstablishMST();
        }
        /// <summary>
        ///     A Recources holder for computing full graphs in parallel. 
        ///     * Lock the sorted set. 
        /// </summary>
        protected class EdgesBuffers
        {
            SortedSet<Edge> Holder;
            public EdgesBuffers(SortedSet<Edge> arg)
            {
                Holder = arg; 
            }

            public void LockAdd(Edge e)
            {
                lock(Holder)
                Holder.Add(e);
            }
        
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
        public virtual IImmutableList<int> MaxPartitionSize
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
        ///     * This method gives an index where this unionization happens of 2 large cluster happened. 
        ///     * Pause right before the returned index, that is the point where 2 of the largest unionizations
        ///     is about to happen, like, right on the next iteration. 
        /// </summary>
        /// <returns>
        ///     positive integer. 
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
        public Point Centroid1 { get; protected set;}
        public Point Centroid2 { get; protected set;}
        protected ISet<Point> TopCluster1;
        protected ISet<Point> TopCluters2;
        // A sorted set of collections of points. 
        protected SortedSet<PointCollection> EvolvingClusters;

        /// <summary>
        ///     2 of the partition identified by the kruskal algorithm. 
        ///     Will be established by the ClassifyByCentroid method. 
        /// </summary>
        protected ISet<Point> IdentifiedCluster1;
        protected ISet<Point> IdentifiedCluster2;

        virtual public IImmutableSet<PointCollection> Clusters
        {
            get {
                return Basic.ImmuteSet<PointCollection>(EvolvingClusters);
            }
        }

        virtual public IImmutableSet<Point> ClusterMajor {
            get {
                return Basic.ImmuteSet<Point>(IdentifiedCluster1); 
            }
        }

        virtual public IImmutableSet<Point> ClusterMinor
        {
            get
            {
                return Basic.ImmuteSet<Point>(IdentifiedCluster2);
            }
        }

        public FullGraphClustering(Point[] points): base(points)
        {
            if (points.Length <= 4) throw new Exception("too small to cluster. ");
            EvolvingClusters = KrusktalAagain();
            IdentifyCentroids();
            ClassifyByCentroid();
        }

        /// <summary>
        ///     * Identify 2 points from the collection that are mostly liekly to be the center 
        ///     of 2 of the cluters. 
        ///     * By default, it's going to be the top 2 clusters, which are basically 2 of the largest 
        ///     clusters. 
        /// </summary>
        protected virtual void IdentifyCentroids()
        {
            PointCollection MaxClusters = EvolvingClusters.Max;
            Centroid1 = Point.CentroidOf(MaxClusters.ToArray());
            EvolvingClusters.Remove(EvolvingClusters.Max);
            Centroid2 = Point.CentroidOf(EvolvingClusters.Max.ToArray());
            EvolvingClusters.Add(MaxClusters); 
        }

        /// <summary>
        ///     Given the established centroids, this will classfy then based on the distance. 
        /// </summary>
        protected virtual void ClassifyByCentroid()
        {
            ISet<Point> C1 = new HashSet<Point>(), C2 = new HashSet<Point>(); 
            foreach(Point v in V.Values)
            {
                if (Point.Dis(v, Centroid1) <= Point.Dis(v, Centroid2))
                {
                    C1.Add(v);
                }
                else C2.Add(v); 
            }
            IdentifiedCluster1 = C1;
            IdentifiedCluster2 = C2;
        }

        /// <summary>
        ///     * Run Kruskal again with the information about the max breaking edge 
        ///     in the graph. 
        /// </summary>
        protected virtual SortedSet<PointCollection> KrusktalAagain()
        {
            int TerminateIndex = base.IdentifyMaxBreakingEdge();
            ArrayDisjointSet<Point> ds = new ArrayDisjointSet<Point>();
            SortedDictionary<int, PointCollection> Partitions = new SortedDictionary<int, PointCollection>();
            SortedSet<PointCollection> ClustersSet = new SortedSet<PointCollection>();

            for (int I = 0; I < V.Count; I++)
            {
                ds.CreateSet(V[I]);
                PointCollection pc = new PointCollection(V[I]);
                Partitions[I] = pc;
                ClustersSet.Add(pc);
            }

            foreach (Edge e in E)
            {
                Point v = e.a, u = e.b;
                if (ChosenE.Contains(e))
                {
                    PointCollection P1 = Partitions[ds.FindSet(v) - 1], P2 =  Partitions[ds.FindSet(u) - 1];
                    PointCollection JoinedPartition = P1 + P2;
                    ds.Join(v, u);
                    Partitions[ds.FindSet(u) - 1] = JoinedPartition;
                    ClustersSet.Remove(P1);
                    ClustersSet.Remove(P2);
                    ClustersSet.Add(JoinedPartition); 
                }
                if (--TerminateIndex == 1) break; // Gives it some room. 
            }

            return ClustersSet; 
        }


    }
}
