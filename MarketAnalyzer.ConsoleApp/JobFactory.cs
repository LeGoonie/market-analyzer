using MarketAnalyzer.ConsoleApp.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAnalyzer.ConsoleApp
{
    class JobFactory
    {

        public async void CreateJobAndTrigger(Type type,string name, string cronSchedule, IScheduler scheduler)
        {

            IJobDetail job = JobBuilder.Create(type)
               .WithIdentity("job"+name+"", "group"+name+"")
               .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger"+name, "group"+name+"")
                .StartNow()
                .WithCronSchedule(cronSchedule, x => x
                    .InTimeZone(TimeZoneInfo.Local))
                .ForJob("job"+name+"", "group"+name+"")
                .Build();

            TriggerKey key = new TriggerKey("triggerKey"+name, "group"+name);

            if (await scheduler.GetTriggerState(key) == TriggerState.Complete)
            {
                Console.WriteLine("Job "+name+" completed");
            }

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
