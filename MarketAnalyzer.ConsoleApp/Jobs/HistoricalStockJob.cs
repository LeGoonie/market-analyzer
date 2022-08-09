using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class HistoricalStockJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new HistoricalStockPerfScrapperBO();
            await scrapper.ScrapeDataAndAddOrUpdate();
        }
    }

}