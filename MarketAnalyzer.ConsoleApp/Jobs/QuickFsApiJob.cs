using MarketAnalyzer.Business;
using System.Threading.Tasks;
using Quartz;

namespace MarketAnalyzer.ConsoleApp.Jobs
{
    public class QuickFsApiJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scrapper = new QuickFsApiScrapperBO();
            await scrapper.Run();
            await scrapper.GetDividendsData();
            await scrapper.LoadAndSaveData();
        }
    }

}