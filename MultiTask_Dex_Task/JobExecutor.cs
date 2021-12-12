using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask_Dex_Task
{
    class JobExecutor : IJobExecutor
    {
        public int Amount => _tasks.Count;
        private int _runningTaskCounter;
        private SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<Task> _tasks = new();
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private bool _isEnabled;

        public void Add(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _tasks.Enqueue(new Task(() =>
            {
                try
                {
                    _semaphore.Wait();

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Задача была отменена, дальнейшая работа не будет произведена.");
                        return;
                    }
                    action.Invoke();
                }
                finally
                {
                    if (_isEnabled)
                    {
                        Interlocked.Decrement(ref _runningTaskCounter);
                    }
                    _semaphore.Release();
                }
            }));
        }

        public void Clear()
        {
            Console.WriteLine($"\nОчередь из {Amount} задач очищена. Выполняются запущенные ранее {_runningTaskCounter} задач.\n");
            _tasks.Clear();
        }

        public void Start(int maxConcurrent)
        {
            _semaphore = new SemaphoreSlim(maxConcurrent);
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
            _isEnabled = true;

            while (_tasks.TryDequeue(out var task))
            {
                task.Start();
                Interlocked.Increment(ref _runningTaskCounter);
            }

            Task.Factory.StartNew(() =>
            {
                while (_isEnabled)
                {
                    if (_tasks.TryDequeue(out var task))
                    {
                        task.Start();
                        Interlocked.Increment(ref _runningTaskCounter);
                    }
                }
            });
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _isEnabled = false;
            _runningTaskCounter = 0;
            Console.WriteLine($"\nОбработка очереди остановлена.\n");
        }
    }
}
