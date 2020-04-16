using MyDatastructure.UnionFind;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Chaos.src.Util;

namespace Chaos.src.Ethena
{
    public class Edge: IComparer<Edge>
    {
        public Point a { get; protected set; }
        public Point b { get; protected set; }
        double cost; 

        public Edge(Point a, Point b)
        {
            this.a = a;
            this.b = b;
            this.cost = Point.Dis(a, b);
        }

        public int Compare([AllowNull] Edge x, [AllowNull] Edge y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                throw new Exception(); // UNDONE: SPECIFIED ERROR TYPE.

            return Math.Sign(x.cost - y.cost);
        }
    }

    /// <summary>
    /// TODO: This whole class is not tested yet. 
    /// </summary>
    public class FullGraph
    {
        IDictionary<int, Point> V;
        public virtual ImmutableDictionary<int, Point> IndexToVertices
        {
            get {
                
                return Basic.ImmuteDic<int, Point>(V);
            }        
        }

        IDictionary<Point, int> Vee;
        public virtual ImmutableDictionary<Point, int> VertexToIndex
        {
            get
            {
                return Basic.ImmuteDic<Point,  int>(Vee);
            }
        }

        IDictionary<Edge, bool> E;
        public virtual ImmutableDictionary<Edge, bool> ChosedForMST
        {
            get {
                return Basic.ImmuteDic<Edge, bool>(E);
            }
        }

        // Maximum partition size during each iteration of cruskal 
        IList<int> max_partition = new List<int>();

        public virtual ImmutableList<int> MaxPartitionSize
        {
            get
            {
                return Basic.ImmuteList<int>(max_partition);
            }
        }

        public FullGraph(Point[] points)
        {
            V = new SortedDictionary<int, Point>();
            E = new SortedDictionary<Edge, bool>(); // Whther the edge is included in the MSt or not. 
            for(int I = 0; I < points.Length; I++)
            {
                V[I] = points[I]; // index to vertex
                Vee[points[I]] = I; // reverse map

                for (int J = I + 1; J < points.Length; J++)
                {
                    E[new Edge(points[I], points[J])] = false; 
                }
            }

            max_partition = EstablishMST();
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

            foreach (KeyValuePair<Edge, bool> kvp in E)
            {
                Point u = kvp.Key.a, v = kvp.Key.b;
                if (ds.FindSet(u) != ds.FindSet(v))
                {
                    int MergedSize = PartitionSizes[Vee[u]] + PartitionSizes[Vee[v]];
                    ds.Join(u, v);
                    PartitionSizes[ds.FindSet(u)] = MergedSize;
                    CompSizes.Add(MergedSize);
                    MaxSize.Add(CompSizes.Max);
                    this.E[kvp.Key] = true;
                }
            }

            return MaxSize;
        }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"full graph with: |V|={V.Count}\n");
            return sb.ToString();
        }

    }
}
