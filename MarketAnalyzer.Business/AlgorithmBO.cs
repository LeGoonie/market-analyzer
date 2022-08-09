using MarketAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MarketAnalyzer.Analysis;
using MarketAnalyzer.Data.Pocos;

namespace MarketAnalyzer.Business
{
    public class AlgorithmBO
    {
        public IEnumerable<(Company, decimal?)> GetTopGrowingCompanies(int numberOfCompanies, int basedOnLastYears)
        {
            var dataAnalyzerBO = new DataAnalyzerBO();

            var stockAnalysis = dataAnalyzerBO.AnalyzeStocks();
            var topCompaniesGrossProfit = new List<(Company, decimal?)>();
            var topCompaniesRevenues = new List<(Company, decimal?)>();
            var topCompaniesProfitTimesRevenue = new List<(Company, decimal?)>();
            var topCompaniesEquity = new List<(Company, decimal?)>();
            var topCompaniesEps = new List<(Company, decimal?)>();
            var endYear = DateTime.UtcNow.Year;
            var startYear = endYear - basedOnLastYears;

            /**
             * Still not taking companies with negative values on their revenue, equity etc.
             * If it doesn't have 2020 data, method only does till 2019
             * Must have first year data.
             */
            foreach (var item in stockAnalysis)
            {
                var firstIncomeStatement = item.IncomeStatements.Where(x => x.Year == startYear).SingleOrDefault();
                if (firstIncomeStatement == null)
                {
                    continue;
                }
                var lastIncomeStatement = item.IncomeStatements.Where(x => x.Year == endYear).SingleOrDefault();
                if(lastIncomeStatement == null)
                {
                    lastIncomeStatement = item.IncomeStatements.Where(x => x.Year == endYear - 1).FirstOrDefault();
                    if (lastIncomeStatement == null) continue;
                }
                var aux = (new Company(), (decimal?)new decimal());
                if (firstIncomeStatement.Revenue > 0 && lastIncomeStatement.Revenue > 0)
                {
                    aux = (item.Company, (lastIncomeStatement.Revenue - firstIncomeStatement.Revenue) / firstIncomeStatement.Revenue);
                    topCompaniesRevenues.Add((item.Company, (lastIncomeStatement.Revenue - firstIncomeStatement.Revenue) / firstIncomeStatement.Revenue));
                }
                if (firstIncomeStatement.GrossProfit > 0 && lastIncomeStatement.GrossProfit > 0)
                {
                    topCompaniesGrossProfit.Add((item.Company, (lastIncomeStatement.GrossProfit - firstIncomeStatement.GrossProfit) / firstIncomeStatement.GrossProfit));
                    /**
                     * Roic and EPS fitness are being accounted. companies can't have terrible fitness and stil grow
                     */
                    if (topCompaniesRevenues.Contains(aux) && item.StockFitness.RoicGrowthFitness >= 0.4 && item.StockFitness.EpsGrowthFitness >= 0.3)
                    {
                        topCompaniesProfitTimesRevenue.Add((item.Company, ((lastIncomeStatement.GrossProfit - firstIncomeStatement.GrossProfit) / firstIncomeStatement.GrossProfit) * aux.Item2));
                    }
                }
                if (firstIncomeStatement.EpsBasic > 0 && lastIncomeStatement.EpsBasic > 0)
                {
                    topCompaniesEps.Add((item.Company, (lastIncomeStatement.EpsBasic - firstIncomeStatement.EpsBasic) / firstIncomeStatement.EpsBasic));
                }
            }

            foreach (var item in stockAnalysis)
            {
                var firstBalanceSheet = item.BalanceSheets.Where(x => x.Year == startYear).SingleOrDefault();
                if (firstBalanceSheet == null)
                {
                    continue;
                }
                var lastBalanceSheet = item.BalanceSheets.Where(x => x.Year == endYear).SingleOrDefault();
                if (lastBalanceSheet == null)
                {
                    lastBalanceSheet = item.BalanceSheets.Where(x => x.Year == endYear - 1).FirstOrDefault();
                    if (lastBalanceSheet == null) continue;
                }
                if (firstBalanceSheet.ShareholdersEquity > 0 && lastBalanceSheet.ShareholdersEquity > 0)
                {
                    topCompaniesEquity.Add((item.Company, (lastBalanceSheet.ShareholdersEquity - firstBalanceSheet.ShareholdersEquity) / firstBalanceSheet.ShareholdersEquity));
                }
            }

            var listOfCompanies = new List<(Company, decimal?)>();

            topCompaniesRevenues = topCompaniesRevenues.OrderByDescending(x => x.Item2).ToList();
            topCompaniesEquity = topCompaniesEquity.OrderByDescending(x => x.Item2).ToList();
            topCompaniesEps = topCompaniesEps.OrderByDescending(x => x.Item2).ToList();
            topCompaniesProfitTimesRevenue = topCompaniesProfitTimesRevenue.OrderByDescending(x => x.Item2).ToList();


            foreach (var itemEquity in topCompaniesEquity.Take(numberOfCompanies*3))
            {
                foreach (var itemRevenueProfit in topCompaniesProfitTimesRevenue.Take(numberOfCompanies*4))
                {
                    //foreach (var itemEps in topCompaniesEps.Take(1000))
                    //{
                        if (itemEquity.Item1 == itemRevenueProfit.Item1)
                        {
                            listOfCompanies.Add((itemEquity.Item1,(itemRevenueProfit.Item2 + itemEquity.Item2)/2));
                            if (listOfCompanies.Count() == numberOfCompanies)
                            {
                                listOfCompanies = listOfCompanies.OrderByDescending(x => x.Item2).ToList();
                                return listOfCompanies;
                            }
                        }
                    //}
                }
            }


            listOfCompanies = listOfCompanies.OrderByDescending(x => x.Item2).ToList();
            return listOfCompanies;
        }

        public ExtractedIncomeStatement GetFirstIncomeStatement(StockAnalysis stockAnalysis, int startYear)
        {
            var firstIncomeStatement = stockAnalysis.IncomeStatements.Where(x => x.Year == startYear).SingleOrDefault();
            if (firstIncomeStatement == null)
            {
                return null;
            }
            return firstIncomeStatement;
        }

        public ExtractedIncomeStatement GetLastIncomeStatement(StockAnalysis stockAnalysis, int endYear)
        {
            var lastIncomeStatement = stockAnalysis.IncomeStatements.Where(x => x.Year == endYear).SingleOrDefault();
            if (lastIncomeStatement == null)
            {
                lastIncomeStatement = stockAnalysis.IncomeStatements.Where(x => x.Year == endYear - 1).FirstOrDefault();
                if (lastIncomeStatement == null) return null;
            }
            return lastIncomeStatement;
        }

        public ExtractedBalanceSheet GetFirstBalanceSheet(StockAnalysis stockAnalysis, int startYear)
        {
            var firstBalanceSheet = stockAnalysis.BalanceSheets.Where(x => x.Year == startYear).SingleOrDefault();
            if (firstBalanceSheet == null)
            {
                return null;
            }
            return firstBalanceSheet;
        }

        public ExtractedBalanceSheet GetLastBalanceSheet(StockAnalysis stockAnalysis, int endYear)
        {
            var lastBalanceSheet = stockAnalysis.BalanceSheets.Where(x => x.Year == endYear).SingleOrDefault();
            if (lastBalanceSheet == null)
            {
                lastBalanceSheet = stockAnalysis.BalanceSheets.Where(x => x.Year == endYear - 1).FirstOrDefault();
                if (lastBalanceSheet == null) return null;
            }
            return lastBalanceSheet;
        }

        /**
         * Revenue * net income + equity
         * net income + equity
         * Roic? + net income + equity
         * stock price?
         * Revenue * net income + equity + roic
         * market cap
         */
        public IEnumerable<(Company, decimal?)> GetCompaniesGrowth(IEnumerable<Company> top100Companies, IEnumerable<StockAnalysis> stockAnalysis)
        {

            var topCompaniesGrossProfit = new List<(Company, decimal?)>();
            var topCompaniesRevenues = new List<(Company, decimal?)>();
            var topCompaniesProfitTimesRevenue = new List<(Company, decimal?)>();
            var topCompaniesEquity = new List<(Company, decimal?)>();
            var topCompaniesEps = new List<(Company, decimal?)>();
            var endYear = DateTime.UtcNow.Year;
            var startYear = endYear - 5;

            /**
             * Still not taking companies with negative values on their revenue, equity etc.
             * If it doesn't have 2020 data, method only does till 2019
             * Must have first year data.
             */
            foreach (var item in stockAnalysis)
            {
                var firstIncomeStatement = GetFirstIncomeStatement(item, startYear);
                var lastIncomeStatement = GetLastIncomeStatement(item, endYear);
                if(firstIncomeStatement != null && lastIncomeStatement != null)
                {
                    var aux = (new Company(), (decimal?)new decimal());
                    if (firstIncomeStatement.Revenue > 0 && lastIncomeStatement.Revenue > 0)
                    {
                        aux = (item.Company, (lastIncomeStatement.Revenue - firstIncomeStatement.Revenue) / firstIncomeStatement.Revenue);
                        topCompaniesRevenues.Add((item.Company, (lastIncomeStatement.Revenue - firstIncomeStatement.Revenue) / firstIncomeStatement.Revenue));
                    }
                    if (firstIncomeStatement.NetIncome > 0 && lastIncomeStatement.NetIncome > 0)
                    {
                        topCompaniesGrossProfit.Add((item.Company, (lastIncomeStatement.NetIncome - firstIncomeStatement.NetIncome) / firstIncomeStatement.NetIncome));
                        /**
                         * Roic and EPS fitness are being accounted. companies can't have terrible fitness and stil grow
                         */
                        if (topCompaniesRevenues.Contains(aux))
                        {
                            topCompaniesProfitTimesRevenue.Add((item.Company, ((lastIncomeStatement.NetIncome
                                                                                - firstIncomeStatement.NetIncome) / firstIncomeStatement.NetIncome) * aux.Item2));
                        }
                    }
                }
                var firstBalanceSheet = GetFirstBalanceSheet(item, startYear);
                var lastBalanceSheet = GetLastBalanceSheet(item, endYear);
                if(firstBalanceSheet != null && lastBalanceSheet != null)
                {
                    if (firstBalanceSheet.ShareholdersEquity > 0 && lastBalanceSheet.ShareholdersEquity > 0)
                    {
                        topCompaniesEquity.Add((item.Company, (lastBalanceSheet.ShareholdersEquity
                                                               - firstBalanceSheet.ShareholdersEquity) / firstBalanceSheet.ShareholdersEquity));
                    }
                }

            }

            var listOfCompanies = new List<(Company, decimal?)>();
            var dict = new Dictionary<string, (Company, decimal?)>();

            //foreach (var item in topCompaniesGrossProfit)
            foreach (var item in topCompaniesProfitTimesRevenue) 
                dict.Add(item.Item1.Ticker, item);


            foreach (var itemEquity in topCompaniesEquity)
            {
                if (!dict.ContainsKey(itemEquity.Item1.Ticker))
                {
                    continue;
                }
                var item = dict[itemEquity.Item1.Ticker];
                listOfCompanies.Add((itemEquity.Item1, (item.Item2 + itemEquity.Item2) / 2));
            }
            listOfCompanies = listOfCompanies.OrderByDescending(x => x.Item2).ToList();
            return listOfCompanies;


        }

        public Dictionary<Company, (double roicFitness, double equityFitness, double epsFitness, double revenueFitness, double peFitness, double debtFitness, double assetsFitness, double totalFitness)> GetAllCompaniesRawFitness(List<StockAnalysis> stockAnalysis)
        {
            var mapCompanyFitness = new Dictionary<Company,(double roicFitness, double equityFitness, double epsFitness, double revenueFitness, double peFitness, double debtFitness, double assetsFitness, double totalFitness)>();
            foreach(var item in stockAnalysis)
            {
                var totalRawFitness = item.StockFitness.RoicGrowthFitness
                                      + item.StockFitness.EquityGrowthFitness
                                      + item.StockFitness.EpsGrowthFitness
                                      + item.StockFitness.RevenueGrowthFitness
                                      + item.StockFitness.PERatioFitness
                                      + item.StockFitness.DebtToEquityFitness
                                      + item.StockFitness.CurrentAssetsToLiabilitiesFitness;
                mapCompanyFitness.Add(item.Company, (item.StockFitness.RoicGrowthFitness, item.StockFitness.EquityGrowthFitness, item.StockFitness.EpsGrowthFitness, item.StockFitness.RevenueGrowthFitness,
                 item.StockFitness.PERatioFitness, item.StockFitness.DebtToEquityFitness, item.StockFitness.CurrentAssetsToLiabilitiesFitness, totalRawFitness));
            }

            return mapCompanyFitness;
        }

        public Dictionary<Company, double> GetCompaniesOrderedWithMultiplier(IEnumerable<StockAnalysis> stockAnalysis, Multiplier multipliers)
        {
            var mapCompanyFitness = new Dictionary<Company, double>();
            foreach (var item in stockAnalysis)
            {
                var totalFitness = (item.StockFitness.RoicGrowthFitness * multipliers.roicMulti)
                    + (item.StockFitness.EquityGrowthFitness * multipliers.equityMulti)
                    + (item.StockFitness.EpsGrowthFitness * multipliers.epsMulti)
                    + (item.StockFitness.RevenueGrowthFitness * multipliers.revenueMulti)
                    + (item.StockFitness.PERatioFitness * multipliers.peMulti)
                    + (item.StockFitness.DebtToEquityFitness * multipliers.dToEMulti)
                    + (item.StockFitness.CurrentAssetsToLiabilitiesFitness * multipliers.aToLMulti);
                mapCompanyFitness.Add(item.Company, totalFitness);
            }

            return mapCompanyFitness;
        }
    }
}
