using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;
using System;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    class FoolRatingJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var scrapper = new FoolRatingScrapperBO();
                await scrapper.ScrapeDataAndUpdate();

            }
            catch (AggregateException ae)
            {
                Console.WriteLine(ae);
            }

        }
    }
}
