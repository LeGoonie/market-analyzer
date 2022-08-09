using MarketAnalyzer.Analysis;
using MarketAnalyzer.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.ConsoleApp
{
    public static class Printer
    {
        public static void Print(StockAnalysis s)
        {
            P($"++++++++++++++++++++++++++++++++++++++++++++++++++++");
            P($"Company {s.Company.Name} *** Ticker {s.Company.Ticker}");
            P($"----------------------------------------------------");
            P($"Roic: ", false); PGrowthSlope(s.RoicSlopeInfo);
            P($"Raw values: ", false);
            PrintFInfos(s.KeyRatios.Select(i => new PointValue { Year = i.Year, Value = i.ReturnOnInvestedCapital.HasValue ? i.ReturnOnInvestedCapital.Value : 0 }));
            P($"Growth values: ", false);
            PrintFInfos(s.RoicSlopeInfo.ValueInfos.Select(i => new PointValue { Year = i.EndYear, Value = i.Cagr }));

            P($"----------------------------------------------------");
            P($"Revenue: ", false); PGrowthSlope(s.RevenueSlopeInfo);
            P($"Raw values: ", false);
            PrintFInfos(s.IncomeStatements.Select(i => new PointValue { Year = i.Year, Value = (double)(i.Revenue.HasValue ? i.Revenue.Value : 0) }));
            P($"Growth values: ", false);
            PrintFInfos(s.RevenueSlopeInfo.ValueInfos.Select(i => new PointValue { Year = i.EndYear, Value = i.Cagr }));

            P($"----------------------------------------------------");
            P($"Equity: ", false); PGrowthSlope(s.EquitySlopeInfo);
            P($"Raw values: ", false);
            PrintFInfos(s.BalanceSheets.Select(i => new PointValue { Year = i.Year, Value = (double)(i.ShareholdersEquity.HasValue ? i.ShareholdersEquity.Value : 0) }));
            P($"Growth values: ", false);
            PrintFInfos(s.EquitySlopeInfo.ValueInfos.Select(i => new PointValue { Year = i.EndYear, Value = i.Cagr }));

            P($"----------------------------------------------------");
            P($"Eps: ", false); PGrowthSlope(s.EpsSlopeInfo);
            P($"Raw values: ", false);
            PrintFInfos(s.IncomeStatements.Select(i => new PointValue { Year = i.Year, Value = (double)(i.EpsDiluted.HasValue ? i.EpsDiluted.Value : 0) }));
            P($"Growth values: ", false);
            PrintFInfos(s.EpsSlopeInfo.ValueInfos.Select(i => new PointValue { Year = i.EndYear, Value = i.Cagr }));
            P($"----------------------------------------------------");
            P($"Equity to Liabilities ratio: {Round(s.StockFitness.DebtToEquityRatio)}");
            P($"----------------------------------------------------");
            P($"Fitness: Roic {Round(s.StockFitness.RoicGrowthFitness)} | Revenue {Round(s.StockFitness.RevenueGrowthFitness)} | Equity {Round(s.StockFitness.EquityGrowthFitness)} | Eps {Round(s.StockFitness.EpsGrowthFitness)} | Total {Round(s.StockFitness.RoicGrowthFitness + s.StockFitness.RevenueGrowthFitness + s.StockFitness.EquityGrowthFitness + s.StockFitness.EpsGrowthFitness)}");
            P($"----------------------------------------------------");
            P($"price {s.Company.StockPrice} fairvalue {s.StockFitness.FairValueStockPrice} buyvalue {s.StockFitness.BuyValueStockPrice}");

            P($"Buy? {s.StockFitness.BuyValueStockPrice >= s.Company.StockPrice}");
            P($"----------------------------------------------------");
            P($"----------------------------------------------------");
            P($"oooooooooooooooooooooooooooooooooooooooooooooooooooo");
            P(""); P(""); P("");
        }

        private static void PrintFInfos(IEnumerable<PointValue> finfos)
        {
            foreach (var f in finfos) P($"Y: {f.Year} V: {Round(f.Value)} | ", false);
            P("");
        }

        private static void PGrowthSlope(SlopeInfo g) => P($"Slope: {Round(g.GrowthSlope)} Median Growth: {Round(g.MedianGrowth)} Deviation: {Round(g.GrowthPercentageDeviation)}");

        private static void P(object obj, bool newLine = true)
        {
            if (newLine) Console.WriteLine(obj); else Console.Write(obj);
        }

        private static double Round(double t)
        {
            return Math.Round(t, 2);
        }

        private static decimal Round(decimal t)
        {
            return Math.Round(t, 2);
        }
    }
}