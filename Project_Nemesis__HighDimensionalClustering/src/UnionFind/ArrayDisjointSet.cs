using System;
using static System.Array;
using MyDatastructure.Maps;

namespace MyDatastructure.UnionFind
{

    /// <summary>
    /// Disjoint set implemented by forest of reverse tree in the array. 
    /// </summary>
    public class ArrayDisjointSet<T> : IDisjointSet<T>
    {
        /// <summary>
        /// The object and their perspective index in the array. 
        /// </summary>
        protected IMap<T, int> IndexMap;
        /// <summary>
        /// Storing the forest reverse tree, index 0 is dummy. 
        /// </summary>
        protected int[] Forest; // more than one inverse tree. index 0 is a dummy. 
        /// <summary>
        /// Total number of elements. 
        /// </summary>
        protected int Size;

        /// <summary>
        /// The number of distinct sets there are after merging. 
        /// </summary>
        protected int disjointset_count;

        /// <summary>
        /// Create an instance of ArrayDisjointset.
        /// </summary>
        public ArrayDisjointSet()
        {
            IndexMap = new SysDefaultMap<T, int>();
            Forest = new int[16];
        }

        /// <summary>
        /// Element can not be null. 
        /// </summary>
        /// <param name="a"></param>
        public void CreateSet(T a)
        {
            if (IndexMap.ContainsKey(a))
            {
                return;
            }
            if (Object.ReferenceEquals(a, null))
            {
                throw new InvalidArgumentException();
            }
            AutoMaticResize();
            Forest[Size + 1] = 0;
            IndexMap[a] = Size + 1;
            Size++;

            disjointset_count++; 
        }

        /// <summary>
        /// Find a set this object belongs to. 
        /// </summary>
        /// <param name="a">
        /// The set object. 
        /// </param>
        /// <returns>
        /// An integer representative of the set it is belonged to. 
        /// </returns>
        public int FindSet(T a)
        {
            if (!IndexMap.ContainsKey(a))
            {
                throw new InvalidArgumentException();
            }
            return FindSet(IndexMap[a]);
        }

        /// <summary>
        /// Internally used. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int FindSet(int index)
        {
            //We are at root.
            if (Forest[index] <= 0)
            {
                return index;
            }
            int newparentindex = FindSet(Forest[index]);
            Forest[index] = newparentindex;
            return newparentindex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// True if both of the element are joined to the same set. 
        /// </returns>
        public bool IsSameSet(T a, T b)
        {
            if (!(IndexMap.ContainsKey(a) && IndexMap.ContainsKey(b)))
            {
                throw new InvalidArgumentException();
            }
            return FindSet(a) == FindSet(b);
        }

        /// <summary>
        /// Let these 2 element belongs to the same set. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Join(T a, T b)
        {
            if (!(IndexMap.ContainsKey(a) && IndexMap.ContainsKey(b)))
            {
                throw new InvalidArgumentException();
            }
            if (a.Equals(b))
            {
                return;
            }
            int rootindexA = FindSet(a);
            int rootindexB = FindSet(b);
            if (rootindexA == rootindexB)
            {
                return;
            }
            int setArank = - Forest[rootindexA];
            int setBrank = - Forest[rootindexB];
            if (setArank == setBrank)
            {
                if (rootindexA < rootindexB)
                    Forest[rootindexB] = rootindexA;
                else
                    Forest[rootindexA] = rootindexB;

                Forest[rootindexA]--;
                return;
            }
            if (setArank > setBrank)
            {
                Forest[rootindexB] = rootindexA;
                return;
            }
            Forest[rootindexA] = rootindexB;
            
            
            disjointset_count--; // UNDONE: regression notes. 
        }

        /// <summary>
        /// Used internally for resizing the array. 
        /// </summary>
        protected void AutoMaticResize()
        {
            if (Size == Forest.Length - 1)
            {
                var newforest = new int[Forest.Length * 2];
                Copy(Forest, 0, newforest, 0, Forest.Length);
                Forest = newforest;
            }
        }

        /// <summary>
        /// Get the integers representing the set where element a is in. 
        /// </summary>
        /// <param name="a"></param>
        /// <returns>
        /// Integer. 
        /// </returns>
        public int GetRepresentative(T a)
        {
            return FindSet(a);
        }


        public int DisjointSetCount() 
        {
            throw new NotImplementedException();
        }
    }
}

