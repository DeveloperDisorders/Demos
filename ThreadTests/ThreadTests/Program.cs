using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTests
{
    class Program
    {
        const int _sleep = 1;

        static void Main(string[] args)
        {
            var bag = new ConcurrentBag<string>();
            var data = new List<string>();
            var newList = new List<string>();
            var waitBag = new ConcurrentBag<string>();

            //Fill data with junk
            for (int i = 0; i < 10000; i++) data.Add(Guid.NewGuid().ToString("N"));

            //Test using a basic list fill
            var startDate = DateTime.Now;
            foreach (var s in data)
            {
                newList.Add(s);
                Thread.Sleep(_sleep);
            }
            Console.WriteLine("Normal fill");
            Console.WriteLine(DateTime.Now.Subtract(startDate).TotalMilliseconds);

            //Reset the data and test with a Parallel foreach and concurrent bag
            startDate = DateTime.Now;
            Parallel.ForEach(data, s =>
            {
                bag.Add(s);
                Thread.Sleep(_sleep);
            });
            Console.WriteLine("Parallel fill");
            Console.WriteLine(DateTime.Now.Subtract(startDate).TotalMilliseconds);

            //Reset the data and test with an async task and concurrent bag
            startDate = DateTime.Now;
            var taskList = new List<Task>();
            foreach (var s in data)
            {
                taskList.Add(Task.Factory.StartNew(() =>
                {
                    waitBag.Add(s);
                    Thread.Sleep(_sleep);
                }));
            }
            Task.WaitAll(taskList.ToArray(), new TimeSpan(0, 5, 0));

            Console.WriteLine("Async fill");
            Console.WriteLine(DateTime.Now.Subtract(startDate).TotalMilliseconds);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey((true));
        }
    }
}
