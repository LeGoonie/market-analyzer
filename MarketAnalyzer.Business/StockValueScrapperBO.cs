using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using MarketAnalyzer.Scrapper.Tickers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class StockValueScrapperBO
    {
        public async Task ScrapeTickersStockValue()
        {
            var scrapper = new SAndP500TickerScrapper();
            var companyTickers = await scrapper.GetTickers();

            GenericDao<Company> genericDao = new GenericDao<Company>();

            var companies = await genericDao.GetAllAsync();


            foreach (var ticker in companyTickers)
            {
                var company = companies.SingleOrDefault(c => c.Ticker == ticker.Ticker);
                if (company != null)
                {
                    Console.WriteLine(company.Name);
                    company.StockPrice = ticker.StockPrice;
                    company.SandPrank = ticker.SandPrank;
                }
            }

            await genericDao.UpdateRangeAsync(companies, 20);
            
        }
    }
}