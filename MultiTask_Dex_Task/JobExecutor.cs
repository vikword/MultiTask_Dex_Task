using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask_Dex_Task
{
    class JobExecutor : IJobExecutor
    {
        private class JobItem
        {
            private readonly Action _action;
            private readonly JobExecutor _jobExecutor;

            public JobItem(Action action, JobExecutor jobExecutor)
            {
                _action = action;
                _jobExecutor = jobExecutor;
            }

            public void ExecuteJob()
            {
                try
                {
                    _jobExecutor._semaphore.Wait();

                    if (_jobExecutor._cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Задача была отменена, дальнейшая работа не будет произведена.");
                        return;
                    }
                    _action.Invoke();
                }
                finally
                {
                    _jobExecutor._semaphore.Release();
                }
            }
        }

        public int Amount => _tasks.Count;
        private uint _runningTaskCounter;
        private SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<JobItem> _tasks = new();
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _cancellationToken;

        public JobExecutor()
        {
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }
        
        public void Add(Action action)
        {
            _tasks.Enqueue(new JobItem(action, this));
        }

        public void Clear()
        {
            Console.WriteLine($"\nОчередь из {Amount} задач очищена. Выполняются запущенные ранее {_runningTaskCounter} задач.\n");
            _tasks.Clear();
        }

        public void Start(int maxConcurrent)
        {
            _semaphore = new SemaphoreSlim(maxConcurrent);

            while (_tasks.TryDequeue(out var task))
            {
                ++_runningTaskCounter;
                Thread.Sleep(1);
                Task.Factory.StartNew(() => task.ExecuteJob());
            }
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _runningTaskCounter = 0;
            Console.WriteLine($"\nОбработка очереди остановлена.\n");
        }
    }
}
