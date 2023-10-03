using System.Collections.Concurrent;

namespace Multi_threaded_Processing
{
    public class MultithreadedQueueProcessingService<T>
    {
        private readonly List<Thread> _threads = new List<Thread>();
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        private readonly Semaphore _semaphore = new Semaphore(0, 100000);

        public MultithreadedQueueProcessingService()
        {
            InitThreads();
        }

        #region -- IMultithreadedQueueProcessingService implementation --

        public int ThreadCount { get; set; } = 4;

        public delegate void WorkerType(T value);

        public WorkerType Worker { get; set; }

        public void AddToQueue(T value)
        {
            _queue.Enqueue(value);

            _semaphore.Release();
        }

        public void RunThreads()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        #endregion

        #region -- Private helpers --

        private void InitThreads()
        {
            _threads.Clear();

            for (int i = 0; i < ThreadCount; i++)
            {
                _threads.Add(new Thread(ThreadWorker));
            }
        }

        private void ThreadWorker()
        {
            T value;

            while (_semaphore.WaitOne())
            {
                if (_queue.TryDequeue(out value))
                {
                    Worker(value);
                }
            }
        }

        #endregion
    }
}
