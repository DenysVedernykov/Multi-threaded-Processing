using System.Collections.Concurrent;

namespace Multi_threaded_Processing
{
    public class MultithreadedQueueProcessingService<T>
    {
        private readonly List<Thread> _threads = new List<Thread>();
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        private int _countEmptyQueue = 0;

        public MultithreadedQueueProcessingService()
        {
            InitThreads();
        }

        #region -- IMultithreadedQueueProcessingService implementation --

        public int ThreadCount { get; set; } = 3;

        public delegate void WorkerType(T value);

        public WorkerType Worker { get; set; }

        public void AddToQueue(T value)
        {
            _queue.Enqueue(value);
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

            while (true)
            {
                if (_queue.TryDequeue(out value))
                {
                    _countEmptyQueue = 0;

                    Worker(value);
                }
                else
                {
                    _countEmptyQueue++;

                    var delay = _countEmptyQueue >= 20000 ? 600 : 100 + (int)(500 * _countEmptyQueue / 20000f);

                    Task.Delay(delay).Wait();
                }
            }
        }

        #endregion
    }
}
