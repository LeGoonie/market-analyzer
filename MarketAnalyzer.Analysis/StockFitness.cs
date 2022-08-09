using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public class StockFitness
    {
        public decimal DebtToEquityRatio { get; set; }

        public decimal CurrentAssetsToLiabilitiesRatio { get; set; }

        public decimal PriceToEarningsRatio { get; set; }

        public decimal MedianPeRatio { get; set; }

        public double CurrentHighPERatio { get; set; }
        public double CurrentLowPERatio { get; set; }

        public double AverageHighPERatio { get; set; }
        public double AverageLowPERatio { get; set; }

        public double AveragePERatioNumYears { get; set; }

        public decimal FairValueStockPrice { get; set; }

        public decimal BuyValueStockPrice { get; set; }

        public double BuyMarginOfSafety { get; set; }

        public double RevenueGrowthFitness { get; set; }
        public double EquityGrowthFitness { get; set; }
        public double EpsGrowthFitness { get; set; }

        public double RoicGrowthFitness { get; set; }

        public double PERatioFitness { get; set; }
        public double DebtToEquityFitness { get; set; }
        public double CurrentAssetsToLiabilitiesFitness { get; set; }

        public double RoicMult { get; set; }
        public double RevenueMult { get; set; }
        public double EquityMult { get; set; }
        public double EpsMult { get; set; }
        public double PeMult { get; set; }
        public double DToEMult { get; set; }
        public double AToLMult { get; set; }

        public double NextYearEstimatedGrowthBasedOnEPS { get; set; }
        public double NextYearEstimatedSustainableGrowth { get; set; }
        public double NextYearEstimatedGrowth { get; set; }

        public (double,double) PotentialOutcome5Years { get; set; }

        public (double, double) PotentialOutcome3Years { get; set; }

        public (double, double) PotentialOutcome10Years { get; set; }

        public List<(double, double, int)> PeRatiosDeltaPerYear { get; set; }

        public List<(int year, double fairValue, double buyValue)> HistoricFairValues { get; set; }

        public double FitnessTotal
        {
            get { return RoicMult * RoicGrowthFitness + EquityMult * EquityGrowthFitness + EpsMult * EpsGrowthFitness + RevenueMult * RevenueGrowthFitness + PeMult * PERatioFitness + DToEMult * DebtToEquityFitness + AToLMult * CurrentAssetsToLiabilitiesFitness; }
        }

        public StockFitness(StockAnalysis stockAnalysis, double buyMarginOfSafety)
        {
            RoicMult = 1;
            EquityMult = 1;
            EpsMult = 1;
            RevenueMult = 1;
            PeMult = 1;
            DToEMult = 1;
            AToLMult = 1;

            var financialAnalysis = new FinancialAnalysis();

            BuyMarginOfSafety = buyMarginOfSafety;

            var latestBalanceSheet = stockAnalysis.BalanceSheets.Last();
            var totalCurrentAssets = latestBalanceSheet.TotalCurrentAssets.HasValue ? latestBalanceSheet.TotalCurrentAssets.Value : 0;
            var totalCurrentLiabilities = latestBalanceSheet.TotalCurrentLiabilities.HasValue ? latestBalanceSheet.TotalCurrentLiabilities.Value : 0;

            var currentEquity = latestBalanceSheet.ShareholdersEquity.HasValue ? latestBalanceSheet.ShareholdersEquity.Value : 0;

            var currentShortTermDebts = latestBalanceSheet.ShortTermDebt.HasValue ? latestBalanceSheet.ShortTermDebt.Value : 0;
            var currentLongTermDebts = latestBalanceSheet.LongTermDebt.HasValue ? latestBalanceSheet.LongTermDebt.Value : 0;
            var currentCapitalLeases = latestBalanceSheet.CapitalLeases.HasValue ? latestBalanceSheet.CapitalLeases.Value : 0;

            var totalCurrentDebt = currentShortTermDebts + currentLongTermDebts + currentCapitalLeases;

            DebtToEquityRatio = currentEquity == 0 ? 1 : Math.Abs(totalCurrentDebt / currentEquity);
            CurrentAssetsToLiabilitiesRatio = totalCurrentLiabilities == 0 ? 1 : totalCurrentAssets / totalCurrentLiabilities;

            var epsDiluted = stockAnalysis.IncomeStatementTtm.EpsDiluted.HasValue ? stockAnalysis.IncomeStatementTtm.EpsDiluted.Value : 0;
            PriceToEarningsRatio = epsDiluted != 0 ? stockAnalysis.Company.StockPrice / epsDiluted : 0;

            var stockCagrs = stockAnalysis.EquitySlopeInfo.ValueInfos.Where(v => !double.IsNaN(v.Cagr)).Select(v => v.Cagr);
            double estimatedGrowth = 0;

            if (stockCagrs.Count() >= 5)
            {
                estimatedGrowth = financialAnalysis.CalculateEstimatedGrowth(stockCagrs);
            }
            NextYearEstimatedGrowthBasedOnEPS = financialAnalysis.CalculateNextYearGrowthByHistoricalEPS(stockAnalysis, 5);

            NextYearEstimatedSustainableGrowth = financialAnalysis.CalculateSustainableNextYearGrowth(stockAnalysis, 5);

            NextYearEstimatedGrowth = financialAnalysis.CalculateEstimatedGrowth(stockCagrs);

            PotentialOutcome5Years = financialAnalysis.CalculateMinMaxPotentialOutcome(stockAnalysis, 5);

            PotentialOutcome3Years = financialAnalysis.CalculateMinMaxPotentialOutcome(stockAnalysis, 3);

            PotentialOutcome10Years = financialAnalysis.CalculateMinMaxPotentialOutcome(stockAnalysis, 10);

            FairValueStockPrice = (decimal)financialAnalysis.CalculateStockFairValue((double)stockAnalysis.IncomeStatementTtm.EpsDiluted.Value, 10, 0.15, estimatedGrowth,
                            financialAnalysis.CalculatePriceEarningsRatioAverage(stockAnalysis.KeyRatios, 5));

            HistoricFairValues = financialAnalysis.CalculateStockHistoricFairAndBuyValues(stockAnalysis, 10, 0.15, 0.4);

            BuyValueStockPrice = (decimal)financialAnalysis.CalculateStockBuyPrice((double)FairValueStockPrice, BuyMarginOfSafety);

            RoicGrowthFitness = financialAnalysis.CalculateRoicFitness(stockAnalysis.RoicSlopeInfo);
            EquityGrowthFitness = financialAnalysis.CalculateGrowthFitness(stockAnalysis.EquitySlopeInfo);
            EpsGrowthFitness = financialAnalysis.CalculateGrowthFitness(stockAnalysis.EpsSlopeInfo);
            RevenueGrowthFitness = financialAnalysis.CalculateGrowthFitness(stockAnalysis.RevenueSlopeInfo);
            PERatioFitness = financialAnalysis.CalculatePERatioFitness(PriceToEarningsRatio);
            DebtToEquityFitness = financialAnalysis.CalculateDebtToEquityFitness(DebtToEquityRatio);
            CurrentAssetsToLiabilitiesFitness = financialAnalysis.CalculateAssetsToLiabilitiesFitness(CurrentAssetsToLiabilitiesRatio);

            (double avgLowPeRatio, double avgHighPeRatio, int numYears) avgPeRatios = financialAnalysis.CalculatePeRatioHistoricalDelta(stockAnalysis.IncomeStatements, stockAnalysis.HistoricalStockRecords, stockAnalysis.IncomeStatements.Count());
            (double lowPeRatio, double highPeRatio) currentPeRatios = financialAnalysis.CalculatePeRatioCurrentDelta(stockAnalysis.IncomeStatementTtm, stockAnalysis.HistoricalStockRecords.LastOrDefault());

            AverageLowPERatio = avgPeRatios.avgLowPeRatio;
            AverageHighPERatio = avgPeRatios.avgHighPeRatio;
            AveragePERatioNumYears = avgPeRatios.numYears;

            CurrentLowPERatio = currentPeRatios.lowPeRatio;
            CurrentHighPERatio = currentPeRatios.highPeRatio;

            PeRatiosDeltaPerYear = new List<(double, double, int)>();
            foreach (var incomeStatement in stockAnalysis.IncomeStatements)
            {
                var record = stockAnalysis.HistoricalStockRecords.SingleOrDefault(r => r.StartDate.Year == incomeStatement.Year);
                if (record != null)
                {
                    PeRatiosDeltaPerYear.Add(financialAnalysis.CalculatePeRatioDelta(incomeStatement, record));
                }
            }
        }
    }
}