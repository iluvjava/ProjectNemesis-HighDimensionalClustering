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
        public Edge(Point a, Point b)
        {
            this.a = a;
            this.b = b;
            this.cost = Point.Dis(a, b);
        }

        public Point a { get; protected set; }
        public Point b { get; protected set; }
        public static int CompareWeight(Edge x, Edge y)
        {
            return Math.Sign(x.cost - y.cost);
        }

        public int CompareTo(object obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new ArgumentException();
            if (obj.GetType() != typeof(Edge)) throw new ArgumentException(); // Strict Type equal. 

            return CompareWeight(this, (Edge)obj);
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

                for (int J = I + 1; J < points.Length; J++)
                {
                    E.Add(new Edge(points[I], points[J]));
                }
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
                PartitionSizes[I] = 1;
            }

            SortedSet<Edge> ChosenEdges = new SortedSet<Edge>();
            foreach (Edge e in E)
            {
                Point u = e.a, v = e.b;
                if (ds.FindSet(u) != ds.FindSet(v))
                {
                    int MergedSize = PartitionSizes[W[u]] + PartitionSizes[W[v]];
                    ds.Join(u, v);
                    PartitionSizes[ds.FindSet(u)] = MergedSize;
                    CompSizes.Add(MergedSize);
                    MaxSize.Add(CompSizes.Max);
                    ChosenEdges.Add(e);
                }
                
            }

            // Establish Field. 
            ChosenE = ChosenEdges; 
            return MaxSize;
        }
    }
}
