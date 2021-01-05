using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunners
{

    /// <summary>
    ///     A task runner with return values. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITaskRunner<T>
    {
        /// <summary>
        ///     Run all the tasks in parallel. 
        ///     And stored the results, execution will be out of order. 
        /// </summary>
        void RunParallel();
        void AddResult(T result);
        Task<T> GetTask();
        void AddTask(Task<T> t);
        Queue<T> GetResult();
    }

    public interface ITaskRunner
    {
        void RunParallel();
        Task GetTask();
        void AddTask(Task t);
    }

    public abstract class AbstractTaskRnner
    {
        int _threadsAllowed = Environment.ProcessorCount;

        public void RunParallel()
        {

            Thread[] threads = new Thread[_threadsAllowed];
            for (int I = 0; I < threads.Length; I++)
            {
                threads[I] = GetThread();
                threads[I].Start();
            }
            for (int I = 0; I < threads.Length; I++)
            {
                threads[I].Join();
            }

        }

        protected abstract Thread GetThread();

    }

    /// <summary>
    ///     Queue Based Task Runner for tasks without return values. 
    /// </summary>
    public class QueueBaseTaskRunner : AbstractTaskRnner, ITaskRunner
    {

        Queue<Task> tasks_;

        public void QueueBasedTaskRunner(Queue<Task> tasksList)
        {
            tasks_ = tasksList;
        }

        public void AddTask(Task t)
        {
            lock (tasks_)
            {
                Console.WriteLine("adding tasks...");
                tasks_.Enqueue(t);
            }
        }

        public Task GetTask()
        {

            lock (tasks_)
            {
                if (tasks_.Count == 0)
                {
                    return null;
                }
                if (tasks_.Peek() is null) throw new Exception("Null task not allowed.");
                return tasks_.Dequeue();
            }
        }

        protected override Thread GetThread()
        {
            Thread t = new Thread(
                () => {
                    while (true)
                    {
                        Task task = GetTask();
                        if (task is null) break;
                        task.Start();
                    }

                }
            );
            return t;
        }
    }

    /// <summary>
    ///     T is the return type of the task. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueBasedTaskRunner<T> : AbstractTaskRnner, ITaskRunner<T>
    {
        int _threadsAllowed = Environment.ProcessorCount;
        Queue<Task<T>> tasks_;
        public Queue<T> results_;

        public QueueBasedTaskRunner(Queue<Task<T>> listOfTasks) : this(listOfTasks, new Queue<T>())
        { }

        public QueueBasedTaskRunner(Queue<Task<T>> listOfTasks, Queue<T> ListOfResults)
        {
            tasks_ = listOfTasks;
            results_ = ListOfResults;
        }

        override
        protected Thread GetThread()
        {
            Thread t = new Thread(
                    () => {
                        while (true)
                        {
                            Task<T> task = GetTask();
                            if (task is null) break;
                            task.Start();
                            AddResult(task.Result);
                        }

                    }
                );
            return t;
        }

        public Task<T> GetTask()
        {
            lock (tasks_)
            {
                if (tasks_.Count == 0)
                {
                    return null;
                }
                if (tasks_.Peek() is null) throw new Exception("Null task not allowed.");
                return tasks_.Dequeue();
            }
        }

        public void AddResult(T result)
        {
            lock (this)
                results_.Enqueue(result);
        }

        public void AddTask(Task<T> t)
        {
            lock (tasks_)
            {
                Console.WriteLine("adding tasks...");
                tasks_.Enqueue(t);
            }
        }

        public Queue<T> GetResult()
        {
            return results_;
        }
    }


}
