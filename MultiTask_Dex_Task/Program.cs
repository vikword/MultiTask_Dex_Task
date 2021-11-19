using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask_Dex_Task
{
    class Program
    {
        private const int maxConcurrent = 3;
        static void Main()
        {
            var job = new JobExecutor();

            for (int i = 1; i <= 20; i++)
            {
                job.Add(() => new TestClass().Test());
            }

            Task.Factory.StartNew(() => job.Start(maxConcurrent));
            Thread.Sleep(100);
            
            job.Clear();
            Thread.Sleep(2000);

            job.Stop();

            Console.ReadKey();
        }
    }
}
