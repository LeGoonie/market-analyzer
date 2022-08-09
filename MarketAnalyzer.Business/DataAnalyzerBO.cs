using MarketAnalyzer.Analysis;
using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MarketAnalyzer.Business
{
    public class DataAnalyzerBO
    {
        public IEnumerable<StockAnalysis> AnalyzeStocks()
        {
            GenericDao<Company> genericDao = new GenericDao<Company>();

            using (var context = new MarketAnalyzerDB2Context())
            {
                var companies = genericDao.GetAllSync();

                var analysis = new FinancialAnalysis();

                var stocksAnalysis = new List<StockAnalysis>();


                var results = (from c in context.Companies
                               select new
                               {
                                   IncomeStatements = c.ExtractedIncomeStatements,
                                   BalanceSheets = c.ExtractedBalanceSheets,
                                   CashFlows = c.ExtractedCashFlowStatements,
                                   KeyRatios = c.ExtractedKeyRatios,
                                   IncomeStatementTtm = c.ExtractedIncomeStatementTtms,
                                   CashFlowTtm = c.ExtractedCashFlowStatementTtms,
                                   HistoricalStockRecords = c.HistoricalStockRecords,
                                   Company = c,
                               }).ToList();

                foreach (var result in results)
                {
                    //var incomeStatements = context.ExtractedIncomeStatements.Where(i => i.CompanyId == company.Id).ToList();
                    //var balanceSheets = context.ExtractedBalanceSheets.Where(i => i.CompanyId == company.Id).ToList();
                    //var cashFlows = context.ExtractedCashFlowStatements.Where(i => i.CompanyId == company.Id).ToList();
                    //var keyRatios = context.ExtractedKeyRatios.Where(i => i.CompanyId == company.Id).ToList();

                    //var incomeStatementTtm = context.ExtractedIncomeStatementTtms.SingleOrDefault(i => i.CompanyId == company.Id);
                    //var cashFlowTtm = context.ExtractedCashFlowStatementTtms.SingleOrDefault(i => i.CompanyId == company.Id);

                    var incomeStatements = result.IncomeStatements.ToList();
                    var balanceSheets = result.BalanceSheets.ToList();
                    var cashFlows = result.CashFlows.ToList();
                    var keyRatios = result.KeyRatios.ToList();
                    var historicalStockRecords = result.HistoricalStockRecords.ToList();
                    var incomeTtm = result.IncomeStatementTtm.SingleOrDefault();
                    var cashTtm = result.CashFlowTtm.SingleOrDefault();
                    var company = result.Company;

                    if (incomeStatements.Count < 6 || balanceSheets.Count < 6 || cashFlows.Count < 6 || keyRatios.Count < 6 || incomeTtm == null || incomeTtm.EpsDiluted == null || cashTtm == null)
                    {
                        Debug.WriteLine("Not enough info: " + company.Ticker);
                        continue;
                    }

                    var stockAnalysis = new StockAnalysis(company, 0.4, incomeStatements, balanceSheets, cashFlows, keyRatios, incomeTtm, cashTtm, historicalStockRecords);

                    stocksAnalysis.Add(stockAnalysis);
                }

                return stocksAnalysis;
            }
        }
    }
}