using MathNet.Numerics;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian
{
    class RecursiveParallel2
    {

        public static int BranchDepth = 4;
        
        [Test]
        public static void TestUselessThingy()
        {
            BranchDepth = 2;
            UselessParallelRecursion(); 
        }

        /// <summary>
        ///     This is a demonstration that async programming won't use all cores. 
        /// </summary>
        /// <param name="depth"></param>
        private static void UselessParallelRecursion(int depth = 0)
        {
            if (depth < BranchDepth)
            {
                Task t1 = new Task(
                    () =>
                    {
                        UselessParallelRecursion(depth + 1);
                    }
                );
                Task t2 = new Task(
                    () => {
                        UselessParallelRecursion(depth + 1);          
                    }
                );
                t1.Start(); 
                t2.Start();
                Task.WaitAll(t1, t2);

                return;
            }

            int sum = 0; 
            for (int I = 0; I < int.MaxValue; I++)
            {
                sum += I; 
                // do weird things 
            }
            return;
        
        }

        [Test]
        public static void TestPrimeNumberFilter()
        {
            Console.WriteLine("Test is running....");
            int[] arr = new int[0b1_000000_000000_000000_000000_0000];
            for (int I = 0; I < arr.Length; I++) arr[I] = I + 2;
            
            int[] isPrime = new int[arr.Length];
            
            var p = new PrimeNumberFilter(arr, isPrime);
            Console.WriteLine("Threads in the pool: " + p._ListOfThreads.Count);
            p.StartThreadsAndJoin();

            for (int I = 0; I < isPrime.Length; I++)
            {
                if (isPrime[I] == 1) Console.WriteLine(arr[I]);
            }
        }

    }

    /// <summary>
    ///     Split task, joint tasks, and then outputs results in the node. 
    /// </summary>
    class PrimeNumberFilter {

        public List<Thread> _ListOfThreads; // threads from all the base compute nodes. 

        // inputs and outputs, all nodes in the compute tree share the same reference. 
        int[] _arr;
        int[] _isPrime; // 0 means it's not, 1 means it is. 
       
        private int _start;
        private int _end;
        private int _depth; 

        // Subtasks. 
        PrimeNumberFilter _left = null;
        PrimeNumberFilter _right = null;

        protected PrimeNumberFilter(int[] arr, int[] isPrime, int start, int end, int depth, List<Thread> listOfThread)
        {
            this._arr = arr;
            _start = start;
            _end = end;
            _depth = depth;
            _isPrime = isPrime;
            _ListOfThreads = listOfThread; 
            SpawnSubTask();
        }

        /// <summary>
        ///     This constructor sets up the root node for computations. 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="isPrime"></param>
        public PrimeNumberFilter(int[] arr, int[] isPrime): this(arr,isPrime, 0, arr.Length, 4, new List<Thread>())
        {
        }

        protected void SpawnSubTask()
        {
            if (_depth > 0)
            {
                int mid = (_start + _end) / 2;
                _left = new PrimeNumberFilter(_arr, _isPrime, _start, mid, _depth - 1, _ListOfThreads);
                _right = new PrimeNumberFilter(_arr, _isPrime, mid, _end, _depth - 1, _ListOfThreads);
            }
            else
            {
                Console.WriteLine($"Constructing computing leaf node; start:{_start}, end: {_end}");
                _ListOfThreads.Add(
                    new Thread(
                            () =>
                            {
                                Console.WriteLine($"A thread is running, start:{_start}, end: {_end}");
                                FilterPrimes();
                            }
                        )
                    );
            }
        }

        /// <summary>
        ///     For all threads in the compute nodes, compute them all, join then all at the end, 
        ///     and then clear them all. 
        ///     *
        ///         Function halts until all the computations in the leaves nodes are done. 
        /// </summary>
        /// <returns></returns>
        public void StartThreadsAndJoin()
        {
            foreach (Thread T in _ListOfThreads)
            {
                T.Start();
            }

            foreach (Thread T in _ListOfThreads)
            {
                T.Join(); 
            }
            // clear all threads in the static field. 
            _ListOfThreads = new List<Thread>();
        }


        /// <summary>
        ///     This is a base task that doesn't need paralization. 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        void FilterPrimes()
        {
            for (int I = _start; I < _end; I++)
            {
                if (BruteForcePrimeTest(_arr[I]))
                    _isPrime[I] = 1;
            } 
        }

        static bool BruteForcePrimeTest(int n)
        {
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            for (int I = 3; I < Math.Sqrt(n) + 1; I++)
            {
                if (n % I == 0) return false; 
            }
            return true; 
        }
    }


}
