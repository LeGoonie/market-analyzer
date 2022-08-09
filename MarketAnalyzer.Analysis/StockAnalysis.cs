using MarketAnalyzer.Data;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public class StockAnalysis
    {
        public int Position { get; set; }
        public Company Company { get; set; }

        public double TenYearMarketCapGrowth { get; set; }
        public double FiveYearsMarketCapGrowth { get; set; }

        public List<ExtractedIncomeStatement> IncomeStatements { get; set; }
        public List<ExtractedCashFlowStatement> CashFlows { get; set; }

        public List<ExtractedBalanceSheet> BalanceSheets { get; set; }

        public List<ExtractedKeyRatio> KeyRatios { get; set; }

        public List<HistoricalStockRecord> HistoricalStockRecords { get; set; }

        public SlopeInfo RevenueSlopeInfo { get; set; }

        public SlopeInfo EquitySlopeInfo { get; set; }

        public SlopeInfo EpsSlopeInfo { get; set; }

        public SlopeInfo RoicSlopeInfo { get; set; }

        public SlopeInfo MarketCapSlopeInfo { get; set; }

        public SlopeInfo DividendYieldSlopeInfo { get; set; }

        public ExtractedIncomeStatementTtm IncomeStatementTtm { get; set; }

        public ExtractedCashFlowStatementTtm CashFlowStatementTtm { get; set; }

        public StockFitness StockFitness { get; set; }

        public StockAnalysis()
        {
        }

        public StockAnalysis(Company company, double buyMarginOfSafety,
            List<ExtractedIncomeStatement> incomeStatements,
            List<ExtractedBalanceSheet> balanceSheets,
            List<ExtractedCashFlowStatement> cashFlows,
            List<ExtractedKeyRatio> keyRatios, ExtractedIncomeStatementTtm incomeStatementTTM, ExtractedCashFlowStatementTtm cashFlowStatementTTM, List<HistoricalStockRecord> historicalStockRecords)
        {
            Company = company;
            if (Company.Ticker == "TI.A")
            {
                int a = 0;
            }

            IncomeStatements = incomeStatements.OrderBy(s => s.Year).ToList();
            BalanceSheets = balanceSheets.OrderBy(s => s.Year).ToList();
            CashFlows = cashFlows.OrderBy(s => s.Year).ToList();
            KeyRatios = keyRatios.OrderBy(s => s.Year).ToList();
            HistoricalStockRecords = historicalStockRecords.OrderBy(s => s.StartDate).ToList();

            IncomeStatementTtm = incomeStatementTTM;
            CashFlowStatementTtm = cashFlowStatementTTM;

            var financialAnalysis = new FinancialAnalysis();

            RevenueSlopeInfo = new SlopeInfo(financialAnalysis.CalculateRevenueValues(IncomeStatements));
            EquitySlopeInfo = new SlopeInfo(financialAnalysis.CalculateEquityValues(BalanceSheets));
            EpsSlopeInfo = new SlopeInfo(financialAnalysis.CalculateEpsValues(IncomeStatements));
            RoicSlopeInfo = new SlopeInfo(financialAnalysis.CalculateRoicValues(KeyRatios));
            MarketCapSlopeInfo = new SlopeInfo(financialAnalysis.CalculateMarketCap(KeyRatios));
            DividendYieldSlopeInfo = new SlopeInfo(financialAnalysis.CalculateDividendYields(KeyRatios, IncomeStatements));

            FiveYearsMarketCapGrowth = financialAnalysis.CalculateSimpleGrowth(MarketCapSlopeInfo, 5);
            TenYearMarketCapGrowth = financialAnalysis.CalculateSimpleGrowth(MarketCapSlopeInfo, 10);

            StockFitness = new StockFitness(this, buyMarginOfSafety);
        }
    }
}