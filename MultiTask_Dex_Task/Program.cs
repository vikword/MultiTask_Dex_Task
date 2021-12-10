using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask_Dex_Task
{
    class Program
    {
        private const int maxConcurrent = 7;
        static void Main()
        {
            var job = new JobExecutor();

            for (int i = 1; i <= 20; i++)
            {
                job.Add(() => new TestClass().Test());
            }

            Task.Factory.StartNew(() => job.Start(maxConcurrent));
            Thread.Sleep(1);
            
            job.Clear();
            Thread.Sleep(22000);
            Console.WriteLine("добавление новых задач");

            for (int i = 1; i <= 20; i++)
            {
                job.Add(() => new TestClass().Test());
            }


            Thread.Sleep(2000);
            Console.WriteLine("Stop");
            job.Stop();

            Thread.Sleep(2000);
            Console.WriteLine("Ещё задачи");
            for (int i = 1; i <= 20; i++)
            {
                job.Add(() => new TestClass().Test());
            }

            Thread.Sleep(4000);
            Console.WriteLine("Start");
            job.Start(2);


            Console.ReadKey();
        }
    }
}
