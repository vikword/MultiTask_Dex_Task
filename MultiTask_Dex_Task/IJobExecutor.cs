using System;

namespace MultiTask_Dex_Task
{
    interface IJobExecutor
    {
        // Кол-во задач в очереди на обработку
        int Amount { get; }

        // Запустить обработку очереди и установить максимальное кол-во параллельных задач
        void Start(int maxConcurrent);

        void Stop();

        // Добавить задачу в очередь
        void Add(Action action);

        // Очистить очередь задач
        void Clear();
    }
}
