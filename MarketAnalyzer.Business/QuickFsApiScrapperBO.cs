using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.QuickFsApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MarketAnalyzer.Scrapper.QuickFsApi.BalanceSheetApi;
using static MarketAnalyzer.Scrapper.QuickFsApi.CashFlowApi;
using static MarketAnalyzer.Scrapper.QuickFsApi.KeyRatioApi;

namespace MarketAnalyzer.Business
{
    public class QuickFsApiScrapperBO
    {
        public async Task Run()
        {
            var incomeStatementApi = new IncomeStatementApi();
            var balanceSheetApi = new BalanceSheetApi();
            var cashFlowApi = new CashFlowApi();
            var keyRatiosApi = new KeyRatioApi();

            using (var context = new MarketAnalyzerDBContext())
            {
                var companies = (from c in context.Companies
                                 where !(from q in context.QuickFsJsonData
                                         select q.CompanyId).Contains(c.Id)
                                 select c).ToList();

                foreach (var company in companies)
                {
                    try
                    {
                        var incomeStatements = await incomeStatementApi.GetIncomeStatements(company.Ticker, 20);
                        var balanceSheets = await balanceSheetApi.GetBalanceSheets(company.Ticker, 20);
                        var cashFlows = await cashFlowApi.GetCashFlows(company.Ticker, 20);
                        var keyRatios = await keyRatiosApi.GetKeyRatios(company.Ticker, 20);

                        var data = new QuickFsJsonDatum
                        {
                            Id = Guid.NewGuid(),
                            CompanyId = company.Id,
                            IncomeStatements = incomeStatements,
                            BalanceSheets = balanceSheets,
                            CashFlows = cashFlows,
                            KeyRatios = keyRatios,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow
                        };

                        Console.WriteLine("--------------------------");
                        Console.WriteLine($"ticker {company.Ticker} | index {companies.IndexOf(company)}");
                        Console.WriteLine("--------------------------");
                        Console.WriteLine(incomeStatements.Substring(0, incomeStatements.Length > 100 ? 100 : incomeStatements.Length - 1));
                        Console.WriteLine(balanceSheets.Substring(0, balanceSheets.Length > 100 ? 100 : balanceSheets.Length - 1));
                        Console.WriteLine(cashFlows.Substring(0, cashFlows.Length > 100 ? 100 : cashFlows.Length - 1));
                        Console.WriteLine(keyRatios.Substring(0, keyRatios.Length > 100 ? 100 : keyRatios.Length - 1));

                        context.QuickFsJsonData.Add(data);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        int a = 0;
                    }
                }
            }
        }

        public async Task GetDividendsData()
        {
            var keyRatiosApi = new KeyRatioApi();

            int skip = 0;

            using (var context = new MarketAnalyzerDBContext())
            {
                var records = (from c in context.Companies
                               join q in context.QuickFsJsonData
                               on c.Id equals q.CompanyId
                               select new { Company = c, QuickFsJsonDatum = q }).Skip(skip).ToList();

                foreach (var record in records)
                {
                    try
                    {
                        var errors = new List<string>();

                        var jsonSerializerSettings = new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            }
                        };

                        var jsonObj = JsonConvert.DeserializeObject<KeyRatiosData>(record.QuickFsJsonDatum.KeyRatios, jsonSerializerSettings);

                        if (jsonObj != null) Console.WriteLine(records.IndexOf(record));

                        if (jsonObj == null)
                        {
                            var jsonString = await keyRatiosApi.GetKeyRatios(record.Company.Ticker, 20);
                            jsonObj = JsonConvert.DeserializeObject<KeyRatiosData>(jsonString, jsonSerializerSettings);
                            if (jsonObj != null)
                            {
                                record.QuickFsJsonDatum.KeyRatios = jsonString;

                                context.SaveChanges();
                            }
                            continue;
                        }

                        if (jsonObj.data.dividends != null) continue;

                        var dividendJson = await keyRatiosApi.GetDividends(record.Company.Ticker, 20);

                        var dividendObj = JsonConvert.DeserializeObject<DividendData>(dividendJson);

                        jsonObj.data.dividends = dividendObj.data.dividends;

                        record.QuickFsJsonDatum.KeyRatios = JsonConvert.SerializeObject(jsonObj);

                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        int a = 0;
                    }
                }
            }
        }

        public async Task LoadAndSaveData()
        {
            using (var context = new MarketAnalyzerDBContext())
            {
                var batchsize = 10;
                var dateUpdatedLimit = DateTime.Parse("2020-04-19");

                int counter = 0;

                for (int i = 0; i < context.QuickFsJsonData.Count(); i += batchsize)
                {
                    var companiesRecords = (from c in context.Companies
                                            join q in context.QuickFsJsonData
                                            on c.Id equals q.CompanyId
                                            where c.DateUpdated < dateUpdatedLimit
                                            select new { Company = c, QuickFsJsonDatum = q }).Skip(i).Take(batchsize).ToList();

                    //var quickFsJsonDatas = context.QuickFsJsonData.Skip(i).Take(batchsize).ToList();

                    foreach (var companyRecord in companiesRecords)
                    {
                        var errors = new List<string>();

                        var jsonSerializerSettings = new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            }
                        };

                        try
                        {
                            var incomeStatementsData = JsonConvert.DeserializeObject<IncomeStatementsData>(companyRecord.QuickFsJsonDatum.IncomeStatements, jsonSerializerSettings);

                            var balanceSheetsData = JsonConvert.DeserializeObject<BalanceSheetsData>(companyRecord.QuickFsJsonDatum.BalanceSheets, jsonSerializerSettings);

                            var cashFlowsData = JsonConvert.DeserializeObject<CashFlowsData>(companyRecord.QuickFsJsonDatum.CashFlows, jsonSerializerSettings);

                            var keyRatiosData = JsonConvert.DeserializeObject<KeyRatiosData>(companyRecord.QuickFsJsonDatum.KeyRatios, jsonSerializerSettings);

                            if (incomeStatementsData == null || balanceSheetsData == null || cashFlowsData == null || keyRatiosData == null)
                            {
                                counter++;
                                Console.WriteLine("Found " + counter);
                                continue;
                            }

                            var incomeStatements = ConvertToIncomeStatements(incomeStatementsData.data, companyRecord.Company.Id, Guid.Parse("323E6036-3087-41B1-AA84-39466AC6900E"));
                            var balanceSheets = ConvertToBalanceSheets(balanceSheetsData.data, companyRecord.Company.Id, Guid.Parse("323E6036-3087-41B1-AA84-39466AC6900E"));
                            var cashFlows = ConvertToCashFlows(cashFlowsData.data, companyRecord.Company.Id, Guid.Parse("323E6036-3087-41B1-AA84-39466AC6900E"));
                            var keyRatios = ConvertToKeyRatios(keyRatiosData.data, companyRecord.Company.Id, Guid.Parse("323E6036-3087-41B1-AA84-39466AC6900E"));
                        }
                        catch (Exception ex)
                        {
                            int a = 0;
                        }
                    }
                }
            }
        }

        private class DividendData
        {
            public DividendJson data { get; set; }
            public List<dynamic> errors { get; set; }
        }

        private class DividendJson
        {
            public List<double> dividends { get; set; }
        }

        private class IncomeStatementsData
        {
            public IncomeStatementJson data { get; set; }
            public List<dynamic> errors { get; set; }
        }

        private class BalanceSheetsData
        {
            public BalanceSheetJson data { get; set; }
            public List<dynamic> errors { get; set; }
        }

        private class CashFlowsData
        {
            public CashFlowJson data { get; set; }
            public List<dynamic> errors { get; set; }
        }

        private class KeyRatiosData
        {
            public KeyRatioJson data { get; set; }
            public List<dynamic> errors { get; set; }
        }

        private IEnumerable<ExtractedIncomeStatement> ConvertToIncomeStatements(IncomeStatementJson data, Guid companyId, Guid dataSourceId)
        {
            var list = new List<ExtractedIncomeStatement>();

            var year = DateTime.UtcNow.Year - 20;

            Func<List<decimal>, int, decimal> pr = (arr, i) => { return arr.Count() > i ? arr[i] : 0; };

            for (int i = 0; i < 20; i++, year++)
            {
                var ist = new ExtractedIncomeStatement();
                ist.Revenue = pr(data.revenue, i);
                ist.CostOfGoodsSold = pr(data.cogs, i);
                ist.EpsBasic = pr(data.eps_basic, i);
                ist.EpsDiluted = pr(data.eps_diluted, i);
                ist.GrossProfit = pr(data.gross_profit, i);
                ist.IncomeTax = pr(data.income_tax, i);
                ist.NetIncome = pr(data.net_income, i);
                ist.NetInterestIncome = pr(data.net_interest_income, i);
                //ist.OperatingProfit = //has to be calculated I guess
                ist.OtherExpenses = pr(data.other_opex, i);
                ist.OtherNonOperatingIncome = pr(data.other_nonoperating_income, i);
                ist.PreTaxIncome = pr(data.pretax_income, i);
                ist.Rd = pr(data.rnd, i);
                ist.Sales = pr(data.sga, i);
                ist.SharesBasic = pr(data.shares_basic, i);
                ist.SharesDiluted = pr(data.shares_diluted, i);
                ist.SpecialCharges = pr(data.special_charges, i);
                ist.TotalOperatingExpenses = pr(data.total_opex, i);

                ist.Id = Guid.NewGuid();
                ist.CompanyId = companyId;
                ist.DataSourceId = dataSourceId;
                ist.DateCreated = DateTime.UtcNow;
                ist.DateUpdated = DateTime.UtcNow;
                ist.Year = year;

                list.Add(ist);
            }

            return list;
        }

        private IEnumerable<ExtractedBalanceSheet> ConvertToBalanceSheets(BalanceSheetJson data, Guid companyId, Guid dataSourceId)
        {
            var list = new List<ExtractedBalanceSheet>();

            var year = DateTime.UtcNow.Year - 20;

            Func<List<decimal>, int, decimal> pr = (arr, i) => { return arr.Count() > i ? arr[i] : 0; };

            for (int i = 0; i < 20; i++, year++)
            {
                var bal = new ExtractedBalanceSheet();
                bal.ShareholdersEquity = pr(data.total_equity, i);
                bal.AccountsPayable = pr(data.accounts_payable, i);
                bal.AccountsReceivable = pr(data.receivables, i);
                bal.AccruedLiabilities = pr(data.current_accrued_liabilities, i);
                bal.Aoci = pr(data.aoci, i);
                bal.CapitalLeases = pr(data.noncurrent_capital_leases, i);
                bal.CashEquivalents = pr(data.cash_and_equiv, i);
                bal.CommonStock = pr(data.common_stock, i);
                bal.DeferedRevenue1 = pr(data.noncurrent_deferred_revenue, i);
                bal.Goodwill = pr(data.goodwill, i);
                bal.Investments = pr(data.equity_and_other_investments, i);
                bal.LiabilitiesAndEquity = pr(data.total_liabilities_and_equity, i);
                bal.LongTermDebt = pr(data.lt_debt, i);
                bal.Other = pr(data.other_equity, i);
                bal.OtherAssets = pr(data.other_lt_assets, i);
                bal.OtherCurrentAssets = pr(data.other_current_assets, i);
                bal.OtherCurrentLiabilities = pr(data.other_current_liabilities, i);
                bal.OtherIntangibleAssets = pr(data.intangible_assets, i);
                bal.OtherLiabilities = pr(data.other_lt_liabilities, i);
                bal.PaidInCapital = pr(data.apic, i);
                bal.PropertyPlanEquipmentNet = pr(data.ppe_net, i);
                bal.RetainedEarnings = pr(data.retained_earnings, i);
                bal.ShortTermDebt = pr(data.st_debt, i);
                bal.ShortTermInvestments = pr(data.st_investments, i);
                bal.TaxPayable = pr(data.tax_payable, i);
                bal.TotalAssets = pr(data.total_assets, i);
                bal.TotalCurrentAssets = pr(data.total_current_assets, i);
                bal.TotalCurrentLiabilities = pr(data.total_current_liabilities, i);
                bal.TotalLiabilities = pr(data.total_liabilities, i);
                bal.TreasuryStock = pr(data.treasury_stock, i);

                bal.Id = Guid.NewGuid();
                bal.CompanyId = companyId;
                bal.DataSourceId = dataSourceId;
                bal.DateCreated = DateTime.UtcNow;
                bal.DateUpdated = DateTime.UtcNow;
                bal.Year = year;

                list.Add(bal);
            }

            return list;
        }

        private IEnumerable<ExtractedCashFlowStatement> ConvertToCashFlows(CashFlowJson data, Guid companyId, Guid dataSourceId)
        {
            var list = new List<ExtractedCashFlowStatement>();

            var year = DateTime.UtcNow.Year - 20;

            Func<List<decimal>, int, decimal> pr = (arr, i) => { return arr.Count() > i ? arr[i] : 0; };

            for (int i = 0; i < 20; i++, year++)
            {
                var cfo = new ExtractedCashFlowStatement();

                cfo.DepreciationAmortization = pr(data.cfo_da, i);
                cfo.Acquisitions = pr(data.cfi_acquisitions_net, i);
                cfo.CashFromFinancing = pr(data.cf_cff, i);
                cfo.CashFromInvesting = pr(data.cf_cfi, i);
                cfo.CashFromOperations = pr(data.cf_cfo, i);
                cfo.CashPaidForDividends = pr(data.cff_dividend_paid, i);
                cfo.ChangeInDeferredTax = pr(data.cfo_deferred_tax, i);
                cfo.ChangeInWorkCapital = pr(data.cfo_change_in_working_capital, i);
                //cfo.FinancingOther = data.
                cfo.Intangibles = pr(data.cfi_intangibles_net, i);
                //cfo.InvestingOther =
                cfo.Investments = pr(data.cfi_investment_net, i);
                cfo.IssuanceCommonStockNet = pr(data.cff_common_stock_net, i);
                cfo.IssuanceDebtNet = pr(data.cff_debt_net, i);
                //cfo.IssuancePreferredStockNet = data.
                //cfo.OperationsOther =
                cfo.PropertyPlanEquipment = pr(data.cfi_ppe_net, i);
                cfo.StockBasedCompensation = pr(data.cfo_stock_comp, i);

                cfo.Id = Guid.NewGuid();
                cfo.CompanyId = companyId;
                cfo.DataSourceId = dataSourceId;
                cfo.DateCreated = DateTime.UtcNow;
                cfo.DateUpdated = DateTime.UtcNow;

                cfo.Year = year;

                list.Add(cfo);
            }

            return list;
        }

        private IEnumerable<ExtractedKeyRatio> ConvertToKeyRatios(KeyRatioJson data, Guid companyId, Guid dataSourceId)
        {
            var list = new List<ExtractedKeyRatio>();

            var year = DateTime.UtcNow.Year - 20;

            Func<List<double>, int, double> pr = (arr, i) => { return arr != null ? (arr.Count() > i ? arr[i] : 0) : 0; };
            Func<List<decimal>, int, decimal> pr1 = (arr, i) => { return arr != null ? (arr.Count() > i ? arr[i] : 0) : 0; };

            for (int i = 0; i < data.roic.Count(); i++, year++)
            {
                var kr = new ExtractedKeyRatio();

                kr.ReturnOnInvestedCapital = pr(data.roic, i);
                kr.AssetsToEquity = pr(data.assets_to_equity, i);
                kr.BookValue = pr1(data.book_value, i);
                kr.DebtToAssets = pr(data.debt_to_assets, i);
                kr.DebtToEquity = pr(data.debt_to_equity, i);

                kr.DividendsPerShare = pr(data.dividends, i);

                kr.EbitdaMargin = pr(data.ebitda_margin, i);
                kr.EquityToAssets = pr(data.equity_to_assets, i);
                kr.FreeCashFlow = pr1(data.fcf, i);
                kr.FreeCashMargin = pr(data.fcf_margin, i);
                kr.GrossMargin = pr(data.gross_margin, i);
                kr.MarketCapitalization = pr1(data.market_cap, i);
                kr.NetMargin = pr(data.net_income_margin, i);
                kr.OperatingMargin = pr(data.operating_margin, i);
                kr.PayoutRatio = pr(data.payout_ratio, i);
                kr.PretaxMargin = pr(data.pretax_margin, i);
                kr.PriceToBook = pr(data.price_to_book, i);
                kr.PriceToEarnings = pr(data.price_to_earnings, i);
                kr.PriceToSales = pr(data.price_to_sales, i);
                kr.ReturnOnAssets = pr(data.roa, i);
                kr.ReturnOnCapitalEmployed = pr(data.roce, i);
                kr.ReturnOnEquity = pr(data.roe, i);
                kr.ReturnOnTangibleCapitalEmployed = pr(data.rotce, i);
                kr.TangibleBookValue = pr1(data.tangible_book_value, i);

                kr.Id = Guid.NewGuid();
                kr.CompanyId = companyId;
                kr.DataSourceId = dataSourceId;
                kr.DateCreated = DateTime.UtcNow;
                kr.DateUpdated = DateTime.UtcNow;

                kr.Year = year;

                list.Add(kr);
            }

            return list;
        }
    }
}