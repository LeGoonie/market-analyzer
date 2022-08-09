using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class StockValueJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new StockValueScrapperBO();
            await scrapper.ScrapeTickersStockValue();
        }
    }

}