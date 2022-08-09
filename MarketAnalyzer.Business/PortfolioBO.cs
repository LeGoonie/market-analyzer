using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Pocos;
using MarketAnalyzer.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class PortfolioBO
    {
        public async Task<TransactionRecord> AddInvestment(double amountOfStocks, double priceOfStock, DateTime dateOfInvestment, Guid companyId, string userId)
        {
            var genericDao = new GenericDao<Portfolio>();

            var portfolio = genericDao.GetSingleBySync(x => x.UserId == userId && x.CompanyId == companyId);

            if(portfolio == null)
            {
                var newPortfolio = await CreatePortfolio(companyId, userId);
                portfolio = newPortfolio;
            }

            var transactionDao = new GenericDao<TransactionRecord>();

            var newTransaction = new TransactionRecord();

            newTransaction.Id = Guid.NewGuid();
            newTransaction.DateOfTransaction = dateOfInvestment;
            newTransaction.PortfolioId = portfolio.Id;
            newTransaction.DollarAtTimeOfTransaction = (decimal)(priceOfStock* amountOfStocks);
            newTransaction.AmountOfStocks = amountOfStocks;
            newTransaction.Type = "Investment";

            await transactionDao.AddAsync(newTransaction);

            return newTransaction;
        }

        public (List<PortfolioAssistant>, decimal?, decimal, decimal?,decimal, double, List<(double, double, int)>) GetPortfolioAssistants(string userId)
        {
            var portfolios = GetUsersPortfolios(userId);
            var portfolioAssistants = new List<PortfolioAssistant>();
            if (portfolios.Count() == 0)
            {
                return (new List<PortfolioAssistant>(), null, 0,null, 0, 0, null);
            }

            decimal? totalInvested = 0;
            decimal totalValue = 0;
            decimal? totalWithdrawed = 0;
            var totalInvestedYearValue = new List<(double, double, int)>();
            var dictInvestedPerYear = new Dictionary<int, double>();
            var dictWithdraw = new Dictionary<int, double>();
            var dictValues = new Dictionary<int, double>();
            dictValues.Add(DateTime.UtcNow.Year, 0);
            var dicSumInvestedPerYear = new ConcurrentDictionary<int, double>();

            foreach (var p in portfolios)
            {
                var dictValue = new Dictionary<int, double>();
                var dictStocks = new ConcurrentDictionary<int, double>();
                var portfolioAssistant = new PortfolioAssistant();
                portfolioAssistant.Portfolio = p;
                portfolioAssistant.Company = GetPortfolioCompany(p);
                dictValue = CompanyStockValuesOverYears(portfolioAssistant.Company);
                portfolioAssistant.TransactionRecords = GetPortfolioTransactions(p.Id);
                portfolioAssistant.TotalWithdrawed = 0;
                portfolioAssistant.TotalValue = 0;
                portfolioAssistant.TotalStocks = 0;
                portfolioAssistant.TotalInvested = 0;
                portfolioAssistant.DateOfLastInvestment = new DateTime();
                foreach (var t in portfolioAssistant.TransactionRecords)
                {
                    //Transactions that are investments, => sums investment values and stock amounts.
                    if (t.Type.Equals("Investment", StringComparison.OrdinalIgnoreCase))
                    {
                        var listYears = EveryYearTillNow(t.DateOfTransaction.Year);
                        foreach (var year in listYears)
                        {
                            dicSumInvestedPerYear.AddOrUpdate(year, (double)t.DollarAtTimeOfTransaction, (key, oldValue) => oldValue + (double)t.DollarAtTimeOfTransaction);
                        }
                        if (dictInvestedPerYear.ContainsKey(t.DateOfTransaction.Year))
                        {
                            dictInvestedPerYear[t.DateOfTransaction.Year] += (double)t.DollarAtTimeOfTransaction;
                        }
                        else
                        {
                            dictInvestedPerYear.Add(t.DateOfTransaction.Year, (double)t.DollarAtTimeOfTransaction);
                        }
                        if (dictStocks.ContainsKey(t.DateOfTransaction.Year))
                        {
                            dictStocks[t.DateOfTransaction.Year] += t.AmountOfStocks;
                        }
                        else
                        {
                            foreach (var year in listYears)
                            {
                                dictStocks.AddOrUpdate(year, t.AmountOfStocks, (key, oldValue) => oldValue + t.AmountOfStocks);
                            }
                        }
                        portfolioAssistant.TotalStocks += t.AmountOfStocks;
                        totalInvested += t.DollarAtTimeOfTransaction;
                        portfolioAssistant.TotalInvested += t.DollarAtTimeOfTransaction;
                        int result = DateTime.Compare(t.DateOfTransaction, portfolioAssistant.DateOfLastInvestment);
                        if (result > 0)
                        {
                            portfolioAssistant.DateOfLastInvestment = t.DateOfTransaction;
                        }
                    }
                    //Transactions that are withdraws, => stores the withdraw amount, and stocks that were withdrawed.
                    else if (t.Type.Equals("Withdraw", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dictWithdraw.ContainsKey(t.DateOfTransaction.Year))
                        {
                            dictWithdraw[t.DateOfTransaction.Year] += (double)t.DollarAtTimeOfTransaction;
                        }
                        else
                        {
                            dictWithdraw.Add(t.DateOfTransaction.Year, (double)t.DollarAtTimeOfTransaction);
                        }
                        if (dictStocks.ContainsKey(t.DateOfTransaction.Year))
                        {
                            dictStocks[t.DateOfTransaction.Year] -= t.AmountOfStocks;
                        }
                        else
                        {
                            dictStocks.AddOrUpdate(t.DateOfTransaction.Year, -t.AmountOfStocks, (key, oldValue) => oldValue - t.AmountOfStocks);
                        }
                        totalWithdrawed += t.DollarAtTimeOfTransaction;
                        portfolioAssistant.TotalWithdrawed += t.DollarAtTimeOfTransaction;
                        portfolioAssistant.TotalStocks -= t.AmountOfStocks;
                    }
                }
                //Iterates dictValue (may contain 2001-2020) and all amount stocks owned per year. Stores it in new dictionary.
                foreach (var item in dictValue)
                {
                    foreach (var j in dictStocks)
                    {
                        if (item.Key == j.Key && item.Key != DateTime.UtcNow.Year)
                        {
                            if (dictValues.ContainsKey(item.Key))
                            {
                                dictValues[item.Key] += (item.Value * j.Value);
                                //dictValues[item.Key] += item.Value * (double)portfolioAssistant.TotalStocks;
                            }
                            else
                            {
                                dictValues.Add(item.Key, item.Value * j.Value);
                                //dictValues.Add(item.Key, item.Value * (double)portfolioAssistant.TotalStocks);
                            }
                        }

                    }
                }
                
                dictValues[DateTime.UtcNow.Year] += (double)portfolioAssistant.Company.StockPrice * dictStocks[DateTime.UtcNow.Year]; //Update value of current year with current stock price.
                portfolioAssistant.TotalValue += (portfolioAssistant.Company.StockPrice * (decimal)portfolioAssistant.TotalStocks);
                totalValue += (portfolioAssistant.Company.StockPrice * (decimal)portfolioAssistant.TotalStocks);
                portfolioAssistant.TotalGainLoss = (portfolioAssistant.TotalValue + portfolioAssistant.TotalWithdrawed) - portfolioAssistant.TotalInvested;
                portfolioAssistant.TotalGainLossPercentage = ((((double)portfolioAssistant.TotalValue + (double)portfolioAssistant.TotalWithdrawed) - (double)portfolioAssistant.TotalInvested) / Math.Abs((double)portfolioAssistant.TotalInvested)) * 100;
                portfolioAssistants.Add(portfolioAssistant);
            }

            var dictGrowth = new Dictionary<int, double>();
            var dictAux = new Dictionary<int, double>();

            // Iterates all values and adds the withdraw amount to them in a new dictionary.
            foreach (var item in dictValues.OrderBy(x => x.Key))
            {
                var aux = 0;
                foreach (var j in dictWithdraw.OrderBy(x => x.Key))
                {
                    if (item.Key == j.Key)
                    {
                        aux = 1;
                        dictAux.Add(item.Key, item.Value + j.Value);
                    }
                }

                if (aux == 0)
                {
                    dictAux.Add(item.Key, item.Value);
                }
            }

            var dicGrowthPercentage = new Dictionary<int, double>();
            //Iterates the previous dictionary and sees if in those years something what was invested. If it was the growth that year is (ValueOfStocksOwnedThatYear - InvestedThatYear - lastYearGrowth)
            //it a previous year exists.
            var outerAux = 0;
            foreach (var item in dictAux.OrderBy(x => x.Key))
            {
                var innerAux = 0;
                foreach (var j in dictInvestedPerYear.OrderBy(x => x.Key))
                {
                    if (j.Key == item.Key)
                    {
                        innerAux = 1;
                        outerAux = 1;
                        if(dictGrowth.ContainsKey(item.Key-1))
                        {
                            var lastYearGrowth = dictGrowth[item.Key - 1];
                            var lastYearValue = dictAux[item.Key - 1];
                            dicGrowthPercentage.Add(item.Key, ((((item.Value - j.Value)) / Math.Abs(lastYearValue) - 1) * 100));
                            if (lastYearGrowth < 0)
                            {
                                dictGrowth.Add(item.Key, (item.Value - j.Value));
                            } else
                            {
                                dictGrowth.Add(item.Key, (item.Value - j.Value - lastYearGrowth));
                            }
                            
                        } else
                        {
                            dictGrowth.Add(item.Key, (item.Value - j.Value));
                            dicGrowthPercentage.Add(item.Key, ((item.Value / j.Value)-1)*100);
                        }
                    }
                }
                if (outerAux == 1 && innerAux == 0)
                {
                    if (dictGrowth.ContainsKey(item.Key - 1))
                    {
                        var lastYearValue = dictAux[item.Key - 1];
                        dicGrowthPercentage.Add(item.Key, ((item.Value / Math.Abs(lastYearValue)-1) * 100));
                    }
                    dictGrowth.Add(item.Key, item.Value);
                }
            }


            //Stores the growth and investment every year in model to present to a view.
            foreach (var item in dictGrowth.OrderBy(x => x.Key))
            {
                var aux = 0;
                foreach (var j in dicGrowthPercentage.OrderBy(x => x.Key))
                {
                    if (j.Key == item.Key)
                    {
                        aux = 1;
                        totalInvestedYearValue.Add((j.Value, item.Value, item.Key));
                    }
                }
                if (aux == 0)
                {
                    totalInvestedYearValue.Add((0, item.Value, item.Key));
                }
            }


            decimal totalGainLoss = 0;
            double totalGainLossPercentage = 0;

            totalGainLoss = (totalValue + (decimal)totalWithdrawed) - (decimal)totalInvested;
            totalGainLossPercentage = ((((double)totalValue + (double)totalWithdrawed) - (double)totalInvested) / Math.Abs((double)totalInvested)) * 100;
            return (portfolioAssistants, totalInvested, totalValue, totalWithdrawed, totalGainLoss, totalGainLossPercentage, totalInvestedYearValue);
        }
        public async Task<TransactionRecord> AddWithdraw(double amountOfStocks, double priceOfStock, DateTime dateOfTransaction, Guid companyId, string userId)
        {
            var genericDao = new GenericDao<Portfolio>();

            var portfolio = genericDao.GetSingleBySync(x => x.UserId == userId && x.CompanyId == companyId);

            if (portfolio == null)
            {
                var newPortfolio = await CreatePortfolio(companyId, userId);
                portfolio = newPortfolio;
            }

            var transactionDao = new GenericDao<TransactionRecord>();

            var newTransaction = new TransactionRecord();

            newTransaction.Id = Guid.NewGuid();
            newTransaction.DateOfTransaction = dateOfTransaction;
            newTransaction.PortfolioId = portfolio.Id;
            newTransaction.DollarAtTimeOfTransaction = (decimal)(priceOfStock * amountOfStocks);
            newTransaction.AmountOfStocks = amountOfStocks;
            newTransaction.Type = "Withdraw";

            await transactionDao.AddAsync(newTransaction);

            return newTransaction;
        }

        public Dictionary<int, double> CompanyStockValuesOverYears(Company company)
        {
            var joinDao = new JoinQueries();
            var dictValue = new Dictionary<int, double>();
            var historicalRecords = joinDao.GetHistoricalStockRecords(company.Id);
            foreach (var item in historicalRecords)
            {
                double[] doubles = { (double)item.Low, (double)item.High };
                if (dictValue.ContainsKey(item.StartDate.Year))
                {
                    dictValue[item.StartDate.Year] += doubles.Average();
                }
                else
                {
                    dictValue.Add(item.StartDate.Year, doubles.Average());
                }
            }
            return dictValue;
        }

        public async Task<Portfolio> CreatePortfolio(Guid companyId, string userId)
        {
            var genericDao = new GenericDao<Portfolio>();

            var newPortfolio = new Portfolio();
            newPortfolio.Id = Guid.NewGuid();
            newPortfolio.UserId = userId;
            newPortfolio.CompanyId = companyId;
            newPortfolio.DateCreated = DateTime.UtcNow;
            newPortfolio.DateUpdated = DateTime.UtcNow;

            await genericDao.AddAsync(newPortfolio);

            return newPortfolio;
        }


        public IEnumerable<Portfolio> GetUsersPortfolios(string userId)
        {
            var genericDao = new GenericDao<Portfolio>();
            var portfolios = genericDao.GetListBySync(x => x.UserId == userId);
            return portfolios;
        }

        public IEnumerable<TransactionRecord> GetPortfolioTransactions(Guid portfolioId)
        {
            var genericDao = new GenericDao<TransactionRecord>();
            var transactionRecords = genericDao.GetListBySync(x => x.PortfolioId == portfolioId);
            return transactionRecords;
        }

        public Company GetPortfolioCompany(Portfolio portfolio)
        {
            var genericDao = new GenericDao<Company>();
            var company = genericDao.GetSingleBySync(x => x.Id == portfolio.CompanyId);
            return company;
        }

        public Company GetCompany(Guid companyId)
        {
            var genericDao = new GenericDao<Company>();
            var company = genericDao.GetSingleBySync(x => x.Id == companyId);
            return company;
        }

        public List<int> EveryYearTillNow(int startYear)
        {
            var listYears = new List<int>();
            listYears.Add(startYear);
            while(startYear < DateTime.UtcNow.Year)
            {
                listYears.Add(startYear + 1);
                startYear++;
            }
            return listYears;
        }
    }
}
