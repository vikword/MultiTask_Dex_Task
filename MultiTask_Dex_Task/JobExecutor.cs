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
        public bool IsEnabled { get; private set; }

        private void ExecuteAction(Action action)
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
                if (IsEnabled)
                {
                    Interlocked.Decrement(ref _runningTaskCounter);
                }
                _semaphore.Release();
            }
        }

        public void Add(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (IsEnabled)
            {
                Task.Factory.StartNew(() => ExecuteAction(action));
                return;
            }

            _tasks.Enqueue(new Task(() => ExecuteAction(action)));
        }

        public void Clear()
        {
            Console.WriteLine($"\nОчередь из {Amount} задач очищена. Выполняются запущенные ранее {_runningTaskCounter} задач.\n");
            _tasks.Clear();
        }

        public void Start(int maxConcurrent)
        {
            if (IsEnabled)
            {
                return;
            }

            _semaphore = new SemaphoreSlim(maxConcurrent);
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            while (_tasks.TryDequeue(out var task))
            {
                task.Start();
                Interlocked.Increment(ref _runningTaskCounter);
            }
            IsEnabled = true;
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            IsEnabled = false;
            _runningTaskCounter = 0;
            Console.WriteLine($"\nОбработка очереди остановлена.\n");
        }
    }
}
