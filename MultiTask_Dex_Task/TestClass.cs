using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask_Dex_Task
{
    public class TestClass
    {
        public void Test()
        {
            Console.WriteLine($"Начало работы задачи. Id задачи {Task.CurrentId}, #Thread {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(2000);
            Console.WriteLine($"Конец работы задачи. Id задачи {Task.CurrentId}, #Thread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
