using System.Threading.Tasks;
using Quartz;
using MarketAnalyzer.Scrapper.Tickers;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class Top2000Job : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new Top2000CompaniesScrapper();
            await scrapper.Run();
        }
    }

}