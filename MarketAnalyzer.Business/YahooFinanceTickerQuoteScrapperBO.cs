using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Extensions;
using MarketAnalyzer.DataAccess;
using MarketAnalyzer.Scrapper.Tickers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class YahooFinanceTickerQuoteScrapperBO
    {
        public async Task Run()
        {
            GenericDao<Company> genericDao = new GenericDao<Company>();

            var companies = await genericDao.GetAllAsync();

            var tickers = companies.Select(c => c.Ticker).ToList();

            var scrapper = new YahooFinanceTickerQuoteScrapper();

            var results = await scrapper.Scrape(tickers.ToArray());

            int batchSize = 10;

            for (int i = 0; i < companies.Count(); i += batchSize)
            {
                var batchResults = companies.Skip(i).Take(batchSize).ToList();

                foreach (var company in batchResults)
                {
                    var result = results.SingleOrDefault(r => r.symbol.ToLower() == company.Ticker.ToLower());
                    if (result != null)
                    {
                        company.StockPrice = result.regularMarketPrice.Round();
                        //context.Entry(company).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
                await genericDao.UpdateRangeAsync(batchResults);
            }
            
        }
    }
}