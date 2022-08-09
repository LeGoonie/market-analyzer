using System;
using System.Collections.Generic;
using MarketAnalyzer.Analysis;
using MarketAnalyzer.ConsoleApp.Jobs;
using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace MarketAnalyzer.ConsoleApp
{
    public class Program
    {
        

        static void Main(string[] args)
        {
            //Run();
            Console.ReadLine();
        }

        static async void Run()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            var jobFactory = new JobFactory();

            jobFactory.CreateJobAndTrigger(typeof(StockValueJob),"StockValue", "0,24,48 * * ? * * *", scheduler);


            // some sleep to show what's happening
            //await Task.Delay(TimeSpan.FromSeconds(60));

            // and last shut down the scheduler when you are ready to close your program
            //await scheduler.Shutdown();

            //Console.WriteLine("Press any key to close the application");
            //Console.ReadKey();

        }

        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }

    }

}