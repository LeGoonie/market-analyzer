using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class YahooJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new YahooFinanceTickerQuoteScrapperBO();
            await scrapper.Run();
        }
    }

}