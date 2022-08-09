using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class QuickFsJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new QuickFsScrapperBO();
            await scrapper.ScrapeDataAndAddOrUpdate();
        }
    }

}