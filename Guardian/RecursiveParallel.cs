using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian
{
    internal class RecursiveParallel
    {
        public static int BaseTaskSize = 0b1_0000_0000_0000_0000_0000;

        public static int BranchDepth = 0; 

        [Test]
        public void TestSimpleSums()
        {
            int[] arr = new int[1073741824];
            for (int I = 0; I < arr.Length; I++) arr[I] = 1;

            Task<long> t = SimpleSum(arr, 0, arr.Length);
            Console.WriteLine($"The sum is: {t.Result}");
        }

        [Test]
        public void TestJustSumIt()
        {
            int[] arr = new int[2147483647/2];
            for (int I = 0; I < arr.Length; I++) arr[I] = 1;

            long t = JustSumIt(arr, 0, arr.Length);

            Console.WriteLine($"The sum is: {t}");
        }

        [Test]
        public void TestSumItWithThreads()
        {
            int[] arr = new int[1073741824];
            for (int I = 0; I < arr.Length; I++) arr[I] = 1;

            long t = SumItWithThreads(arr);

            Console.WriteLine($"TestSumItWithThreads: {t}");
        }

        [Test]
        public void TestSumItWithThreadsBetter()
        {
            int[] arr = new int[1073741824];
            for (int I = 0; I < arr.Length; I++) arr[I] = 1;
        }

        [Test]
        public void TestTotalSubsetSumUpTo()
        {
            int[] arr = new int[26];
            for (int I = 0; I < arr.Length; I++)
            {
                arr[I] = 1;
            }
            for (int branchDepth = 0; branchDepth <= 4; branchDepth++)
            {
                BranchDepth = branchDepth;
                Console.WriteLine($"Branch Depth Limit: {branchDepth}");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Console.WriteLine($"TestSubsetSumUPTo: {TotalSubsetSumUpTo(8, arr)}");
                sw.Stop();
                Console.WriteLine($"Time it takes: {sw.Elapsed}");
            }
        }

        /// <summary>
        ///    Sum numbers up.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static Task<long> SimpleSum(int[] arr, int start, int end)
        {
            if (end - start <= BaseTaskSize)
            {
                Task<long> baseTask = Task<long>.Factory.StartNew(() =>
                {
                    long sum = 0;
                    for (int I = start; I < end; I++)
                    {
                        sum += arr[I];
                    }
                    return sum;
                });

                return baseTask;
            }

            int mid = (start + end) / 2;
            Task<long> t1 = SimpleSum(arr, start, mid);
            Task<long> t2 = SimpleSum(arr, mid, end);

            return Task<long>.Factory.StartNew(() =>
            {
                return t1.Result + t2.Result;
            });
        }

        /// <summary>
        ///    Sum numbers up.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static long JustSumIt(int[] arr, int start, int end)
        {
            if (end - start <= BaseTaskSize)
            {
                long sum = 0;
                for (int I = start; I < end; I++)
                {
                    sum += arr[I];
                }
                return sum;
            }

            int mid = (start + end) / 2;
            Task<long> t1 = Task<long>.Factory.StartNew(() =>
                {
                    return JustSumIt(arr, start, mid);
                }
            );

            Task<long> t2 = Task<long>.Factory.StartNew(() =>
                {
                    return JustSumIt(arr, mid, end);
                }
            );

            return t1.Result + t2.Result;
        }

        /// <summary>
        ///     using Thread for branching.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="threadCanUse"></param>
        /// <returns></returns>
        private static long SumItWithThreads(int[] arr)
        {
            ArraySumThreadBranching tt = new ArraySumThreadBranching(arr, 0, arr.Length);
            tt.Run();
            return tt.sum;
        }

        private class ArraySumThreadsManager
        {
            private int _threadsAvailable;

            public ArraySumThreadsManager()
            {
                _threadsAvailable = 8;
            }

            public bool GetThread(int threads)
            {
                lock (this)
                {
                    if (_threadsAvailable - threads >= 0)
                    {
                        _threadsAvailable -= threads;
                        Console.WriteLine($"Take Thread/s:{threads}");

                        return true;
                    }
                    else return false;
                }
            }

            public void PutBackThread()
            {
                lock (this) { _threadsAvailable++; }
            }
        }

        /// <summary>
        ///     This seems to be the best way of doing things...
        /// </summary>
        private class ArraySumThreadBranching
        {
            public long sum { get; protected set; }
            private int[] _arr;
            private int _start;
            private int _end;
            private static ArraySumThreadsManager _mgr = null;

            public ArraySumThreadBranching(int[] arr, int start, int end)
            {
                if (_mgr is null) _mgr = new ArraySumThreadsManager();
                this._arr = arr;
                this._start = start;
                this._end = end;
            }

            public void Run()
            {
                if (_mgr.GetThread(2)) // Bracnch and merge.
                {
                    int mid = (_start + _end) / 2;
                    ArraySumThreadBranching task1 = new ArraySumThreadBranching(_arr, _start, mid);
                    ArraySumThreadBranching task2 = new ArraySumThreadBranching(_arr, mid, _end);

                    Thread t1 = new Thread(task1.Run);
                    Thread t2 = new Thread(task2.Run);
                    t1.Start(); t2.Start();
                    t1.Join(); t2.Join();
                    this.sum = task1.sum + task2.sum;
                    _mgr.PutBackThread();
                    _mgr.PutBackThread();

                    return;
                }

                // No branching just do it.
                for (int I = _start; I < _end; I++) sum += _arr[I];
            }
        }

        private class ArraySumThreadBranchingBetter
        {
            public long _sum { get; protected set; }
            public ArraySumThreadBranchingBetter _left { get; protected set; }
            public ArraySumThreadBranchingBetter _right { get; protected set; }
            private int[] _arr;
            private int _start;
            private int _end;
            private int _depth;

            public ArraySumThreadBranchingBetter(int[] arr, int start, int end, int depth = 3)
            {
                this._arr = arr;
                this._start = start;
                this._end = end;
                this._depth = depth;
                Branch();
            }

            public void Branch()
            {
                if (_depth >= 1)
                {
                    int mid = (_start + _end) / 2;
                    _left = new ArraySumThreadBranchingBetter(_arr, _start, mid, _depth - 1);
                    _right = new ArraySumThreadBranchingBetter(_arr, mid, _end, _depth - 1);
                }
            }

            public Task Compute()
            {
                return null;
            }
        }

        /// <summary>
        ///     The number of subset that sum up to a certain targeted number, in a stupid way, for the purpose
        ///     of demonstrating parallel programming.
        /// </summary>
        /// <param name="targetSum"></param>
        /// <param name="arr"></param>
        /// <param name="curIndex"></param>
        /// <param name="runningSum"></param>
        /// <returns></returns>
        private static long TotalSubsetSumUpTo(
                int targetSum,
                int[] arr,
                int curIndex = 0,
                long runningSum = long.MaxValue
            )
        {
            if (curIndex == arr.Length)
            {
                if (runningSum == long.MaxValue) return 0; // Empty set is not counted!

                return targetSum == runningSum ? 1 : 0;
            }

            // Branch, limits the total number of tasks spawned. 
            if (curIndex < BranchDepth)
            {
                Task<long> t1 =new Task<long>
                    (
                        () =>
                        {
                            runningSum = runningSum == long.MaxValue ? 0 : runningSum;
                            return TotalSubsetSumUpTo(targetSum, arr, curIndex + 1, runningSum + arr[curIndex]);
                        }
                    );

                Task<long> t2 = new Task<long>
                   (
                       () =>
                       {
                           return TotalSubsetSumUpTo(targetSum, arr, curIndex + 1, runningSum);
                       }
                   );
                t1.Start(); t2.Start();
                Task.WaitAll(t1, t2);
                return t1.Result + t2.Result;
            }

            // No branching 
            long leftSum = 
                TotalSubsetSumUpTo(
                        targetSum, 
                        arr,
                        curIndex + 1,
                        runningSum == long.MaxValue? arr[curIndex]: runningSum + arr[curIndex]
                    );
            long rightSum = TotalSubsetSumUpTo(targetSum, arr, curIndex + 1, runningSum);

            return leftSum + rightSum;
        }

    }
}