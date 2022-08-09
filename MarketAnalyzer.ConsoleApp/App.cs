using MarketAnalyzer.Analysis;
using MarketAnalyzer.Business;
using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using MarketAnalyzer.Scrapper.Tickers;

namespace MarketAnalyzer.ConsoleApp
{
    public class App : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            try { 
                var scrapper = new FoolRatingScrapperBO();
                await scrapper.ScrapeDataAndUpdate();
     
            } catch(AggregateException ae)
            {
                Console.WriteLine(ae);
            }

        }

        private void Analysis()
        {
            using (var context = new MarketAnalyzerDBContext())
            {
                var companies = context.Companies.ToList();

                var analysis = new FinancialAnalysis();

                var stocksAnalysis = new List<StockAnalysis>();

                foreach (var company in companies)
                {
                    var incomeStatements = context.ExtractedIncomeStatements.Where(i => i.CompanyId == company.Id).ToList();
                    var balanceSheets = context.ExtractedBalanceSheets.Where(i => i.CompanyId == company.Id).ToList();
                    var cashFlows = context.ExtractedCashFlowStatements.Where(i => i.CompanyId == company.Id).ToList();
                    var keyRatios = context.ExtractedKeyRatios.Where(i => i.CompanyId == company.Id).ToList();
                    var historicalStockRecords = context.HistoricalStockRecords.Where(i => i.CompanyId == company.Id).ToList();

                    var incomeStatementTtm = context.ExtractedIncomeStatementTtms.SingleOrDefault(i => i.CompanyId == company.Id);
                    var cashFlowTtm = context.ExtractedCashFlowStatementTtms.SingleOrDefault(i => i.CompanyId == company.Id);

                    if (incomeStatements.Count < 6 || balanceSheets.Count < 6 || cashFlows.Count < 6 || keyRatios.Count < 6 || incomeStatementTtm == null || incomeStatementTtm.EpsDiluted == null || cashFlowTtm == null)
                    {
                        Console.WriteLine("Not enough info: " + company.Ticker);
                        continue;
                    }

                    var stockAnalysis = new StockAnalysis(company, 0.4, incomeStatements, balanceSheets, cashFlows, keyRatios, incomeStatementTtm, cashFlowTtm, historicalStockRecords);

                    stocksAnalysis.Add(stockAnalysis);
                }

                foreach (var stock in stocksAnalysis.OrderBy(f => f.Company.StockPrice <= f.StockFitness.BuyValueStockPrice).ThenBy(f => f.StockFitness.FitnessTotal))
                {
                    Printer.Print(stock);

                    Console.WriteLine();
                }
                Console.ReadLine();
            }
        }

        //private void ScrapperOld()
        //{
        //    var scrapper = new KeyRatiosScrapper();

        //    ////var login = await requestHelper.GetLogin("grL0gNYoMoLUB1ZoAKLfhXkoMoLODiO1WoL9.grLtk3PoMoLmqFEsMasbNK9fkXudkNBtR2jpkr5dINZoAKLtRNZoMlG1MQZ3PJD3PJRcOpEfqXGoMwcoqNWaka9tIKO6OlD2PlOoAKLahSVthKO6Olqph3BfIGHskp92INqfyNPdgFa6OosokXVik3qbkpZoMoO3Ip91RSEYkFLbqpB3RSaiAwf5goOcOwHryNIthXBwICO6PKsokpBwyS9dDFLtqoO6grLBDrO6PCsoZ0GoMlH9vN0.aQcaHtidvlDwLfMuUy9ggmqJCbGeQYAZTAB5HnONrCG");
        //    string tempLogin = "grL0gNYoMoLUB1ZoAKLfhXkoMoLODiO1WoL9.grLtk3PoMoLmqFEsMasbNK9fkXudkNBtR2jpkr5dINZoAKLtRNZoMlG1MQZ3PJk5MJPcOpEfqXGoMwcoqNWaka9tIKO6OlD2PlOoAKLahSVthKO6Olqph3BfIGHskp92INqfyNPdgFa6OosokXVik3qbkpZoMoO3Ip91RSEYkFLbqpB3RSaiAwf5goOcOwHryNIthXBwICO6PKsokpBwyS9dDFLtqoO6grLBDrO6PCsoZ0GoMlH9vN0.u16E-Z3bS-Yhjd3RjLm4yl0XCZuIx7lgDmnsK4Qv2w8";

        //    //var result = await scrapper.GetIncomeStatements(tempLogin, "MSFT:US");

        //    //var scrapper = new TickerScrapper();

        //    using (var context = new MarketAnalyzerDBContext())
        //    {
        //        var companies = context.Companies.ToList();

        //        var datasource = context.DataSources.Single(d => d.Name == "quickfs");

        //        foreach (var company in companies)
        //        {
        //            try
        //            {
        //                var result = await scrapper.GetKeyRatios(tempLogin, $"{company.Ticker}:US");

        //                foreach (var ic in result)
        //                {
        //                    ic.CompanyId = company.Id;
        //                    ic.Id = Guid.NewGuid();
        //                    ic.DataSourceId = datasource.Id;
        //                }

        //                context.ExtractedKeyRatios.AddRange(result);
        //                context.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }
        //}
    }
}