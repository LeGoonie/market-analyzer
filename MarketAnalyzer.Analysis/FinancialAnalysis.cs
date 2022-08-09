using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Interfaces;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public class FinancialAnalysis
    {
        public IEnumerable<ValueInfo> CalculateEquityValues(IEnumerable<ExtractedBalanceSheet> fInfos)
        {
            var tt = new ExtractedBalanceSheet();
            return CalculateGrowthValues<decimal>(fInfos.Select(i => (IFInfo)i), nameof(tt.ShareholdersEquity));
        }

        public IEnumerable<ValueInfo> CalculateEpsValues(IEnumerable<ExtractedIncomeStatement> fInfos)
        {
            var tt = new ExtractedIncomeStatement();
            return CalculateGrowthValues<decimal>(fInfos.Select(i => (IFInfo)i), nameof(tt.EpsDiluted));
        }

        public IEnumerable<ValueInfo> CalculateRevenueValues(IEnumerable<ExtractedIncomeStatement> fInfos)
        {
            var tt = new ExtractedIncomeStatement();
            return CalculateGrowthValues<decimal>(fInfos.Select(i => (IFInfo)i), nameof(tt.Revenue));
        }

        public IEnumerable<ValueInfo> CalculateRoicValues(IEnumerable<ExtractedKeyRatio> fInfos)
        {
            var tt = new ExtractedKeyRatio();
            return CalculateGrowthValues<double>(fInfos.Select(i => (IFInfo)i), nameof(tt.ReturnOnInvestedCapital));
        }

        public IEnumerable<ValueInfo> CalculateMarketCap(IEnumerable<ExtractedKeyRatio> fInfos)
        {
            var tt = new ExtractedKeyRatio();
            return CalculateGrowthValues<decimal>(fInfos.Select(i => (IFInfo)i), nameof(tt.MarketCapitalization));
        }

        public (double avgLowPeRatio, double avgHighPeRatio, int numYears) CalculatePeRatioHistoricalDelta(IEnumerable<ExtractedIncomeStatement> incomeStatements, IEnumerable<HistoricalStockRecord> stockRecords, int numYears)
        {
            var items = new List<(double, double)>();
            foreach (var inc in incomeStatements.OrderByDescending(i => i.Year).Take(numYears))
            {
                var year = inc.Year;
                var stockRecord = stockRecords.SingleOrDefault(s => s.StartDate.Year == year);

                if (stockRecord == null || inc.EpsBasic == null || inc.EpsBasic.Value == 0)
                    continue;

                var lowPE = (double)(stockRecord.Low / inc.EpsBasic.Value);

                var highPE = (double)(stockRecord.High / inc.EpsBasic.Value);

                items.Add((lowPE, highPE));
            }

            return (items.Select(i => i.Item1).Median(), items.Select(i => i.Item2).Median(), items.Count);
        }

        public (double lowPeRatio, double highPeRatio, int year) CalculatePeRatioDelta(ExtractedIncomeStatement incomeStatement, HistoricalStockRecord stockRecord)
        {
            if (incomeStatement == null || incomeStatement.EpsBasic == null || incomeStatement.EpsBasic.Value == 0 || stockRecord == null) return (0, 0, 0);
            return ((double)(stockRecord.Low / incomeStatement.EpsBasic.Value),
                    (double)(stockRecord.High / incomeStatement.EpsBasic.Value), incomeStatement.Year);
        }

        public (double lowPeRatio, double highPeRatio) CalculatePeRatioCurrentDelta(ExtractedIncomeStatementTtm incomeStatement, HistoricalStockRecord stockRecord)
        {
            if (incomeStatement == null || incomeStatement.EpsBasic == null || incomeStatement.EpsBasic.Value == 0 || stockRecord == null) return (0, 0);
            return ((double)(stockRecord.Low / incomeStatement.EpsBasic.Value),
                    (double)(stockRecord.High / incomeStatement.EpsBasic.Value));
        }

        public IEnumerable<ValueInfo> CalculateDividendYields(IEnumerable<ExtractedKeyRatio> keyRatios, IEnumerable<ExtractedIncomeStatement> incomeStatements)
        {
            var tt = new ExtractedKeyRatio();
            var tt2 = new ExtractedIncomeStatement();
            var marketCaps = CalculateGrowthValues<decimal>(keyRatios.Select(i => (IFInfo)i), nameof(tt.MarketCapitalization));
            var dividendsPerShare = CalculateGrowthValues<double>(keyRatios.Select(i => (IFInfo)i), nameof(tt.DividendsPerShare));
            var sharesBasic = CalculateGrowthValues<decimal>(incomeStatements.Select(i => (IFInfo)i), nameof(tt2.SharesBasic));

            var lastYear = new List<int> { marketCaps.Last().NextYear, dividendsPerShare.Last().NextYear, sharesBasic.Last().NextYear }.Min();

            var marketCaps9Years = marketCaps.TakeYears(9, lastYear).ToList();
            var dividendsPerShare9Years = dividendsPerShare.TakeYears(9, lastYear).ToList();
            var sharesBasic9Years = sharesBasic.TakeYears(9, lastYear).ToList();

            var tuples = new List<(int, double)>();

            for (int i = 0; i < marketCaps9Years.Count(); i++)
            {
                tuples.Add(CalculateDividendYield(marketCaps9Years[i], dividendsPerShare9Years[i], sharesBasic9Years[i]));
            }

            var values = new List<ValueInfo>();

            for (int i = 1; i < tuples.Count(); i++)
            {
                var startPos = i - 1;
                var endPos = tuples.Count() - 1;
                //if (i > 3) startPos = i - 3;

                if (tuples.Count() <= i) return values;

                (var startValue, var nextValue, var endValue, var nextYearGrowth, var cagrGrowth) = CalculateCagrValues(tuples.Select(t => t.Item2), startPos, endPos);

                values.Add(
                    new ValueInfo
                    {
                        StartYear = tuples[startPos].Item1,
                        EndYear = tuples[endPos].Item1,
                        NextYear = tuples[startPos + 1].Item1,
                        StartValue = startValue,
                        NextValue = nextValue,
                        EndCagrValue = endValue,
                        Cagr = cagrGrowth,
                        GrowthToNextYear = nextYearGrowth
                    });
            }

            return values;
        }

        public (int year, double dividends) CalculateDividendYield(ValueInfo marketCap, ValueInfo dividendsPerShare, ValueInfo sharesBasic)
        {
            if (marketCap.StartYear != dividendsPerShare.StartYear || marketCap.StartYear != sharesBasic.StartYear) throw new ArgumentException("CalculateDividends error because parameters belong to different years");

            var yield = marketCap.StartValue != 0 ? sharesBasic.StartValue * dividendsPerShare.StartValue / marketCap.StartValue : 0;
            return (marketCap.StartYear, yield);
        }

        public double CalculatePriceEarningsRatioAverage(IEnumerable<ExtractedKeyRatio> keyRatios, int lastNYears)
        {
            return keyRatios.Reverse().Take(lastNYears).Average(r => r.PriceToEarnings.HasValue ? r.PriceToEarnings.Value : 0);
        }

        public (double, double) CalculateMinMaxPotentialOutcome(StockAnalysis stockAnalysis, int basedOnPreviousYears)
        {
            var historicalEpsGrowth = CalculateNextYearGrowthByHistoricalEPS(stockAnalysis, basedOnPreviousYears);
            var sustainableGrowth = CalculateSustainableNextYearGrowth(stockAnalysis, basedOnPreviousYears);
            var stockGrowth = CalculateNextYearEstimatedGrowth(stockAnalysis, basedOnPreviousYears);

            if (historicalEpsGrowth > 0.2) historicalEpsGrowth = 0.2;
            if (sustainableGrowth > 0.3) sustainableGrowth = 0.3;
            if (stockGrowth > 0.2) stockGrowth = 0.2;

            var avg = (historicalEpsGrowth + sustainableGrowth + stockGrowth) / 3;

            return (avg / 2, avg);
            
        }

        public double CalculateNextYearGrowthByHistoricalEPS(StockAnalysis stockAnalysis, int basedOnPreviousYears)
        {
            var endYear = stockAnalysis.IncomeStatements.ToList().OrderByDescending(x => x.Year).FirstOrDefault().Year;

            var startYear = endYear - basedOnPreviousYears;
            var epsValues = CalculateEpsValues(stockAnalysis.IncomeStatements);
            double firstEpsValue = 0;
            double secondEpsValue = 0;
            foreach (var item in epsValues)
            {
                if (item.StartYear == startYear)
                {
                    firstEpsValue = item.StartValue;
                }
                else if (item.EndYear == endYear)
                {
                    secondEpsValue = item.StartValue;
                }
            }
            return ((Math.Pow((secondEpsValue / firstEpsValue), 0.1) - 1) * 100) / basedOnPreviousYears;
        }

        public double CalculateSustainableNextYearGrowth(StockAnalysis stockAnalysis, int basedOnPreviousYears)
        {
            var endYear = stockAnalysis.IncomeStatements.ToList().OrderByDescending(x => x.Year).FirstOrDefault().Year;

            var startYear = endYear - basedOnPreviousYears;

            var auxKeyRatios = stockAnalysis.KeyRatios.Where(x => x.Year > startYear).ToList();
            //var auxBalanceSheets = stockAnalysis.BalanceSheets.Where(x => x.Year > startYear).ToList();
            //var auxIncomeStatements = stockAnalysis.IncomeStatements.Where(x => x.Year > startYear).ToList();

            var listOfGrowthPercentages = new List<double>();

            /*for(int i = 0; i < auxBalanceSheets.Count(); i++)
            {
                for(int j =0; j < auxIncomeStatements.Count(); j++)
                {

                }
            }*/

            foreach (var item in auxKeyRatios)
            {
                if (item.PayoutRatio != null)
                {
                    listOfGrowthPercentages.Add((double)(item.ReturnOnEquity * (1 - item.PayoutRatio)));
                }
                else
                {
                    listOfGrowthPercentages.Add((double)(item.ReturnOnEquity * 1));
                }

            }

            return listOfGrowthPercentages.Average();


        }

        public double CalculateNextYearEstimatedGrowth(StockAnalysis stockAnalysis, int basedOnPreviousYears)
        {
            var endYear = stockAnalysis.BalanceSheets.ToList().OrderByDescending(x => x.Year).First().Year;

            var startYear = endYear - basedOnPreviousYears;

            var stockCagrs = stockAnalysis.EquitySlopeInfo.ValueInfos.Where(v => v.StartYear <= endYear).Select(v => !double.IsNaN(v.Cagr) ? v.Cagr : 0);

            double estimatedGrowth = CalculateEstimatedGrowth(stockCagrs);

            return estimatedGrowth;

        }

        public List<(int year, double fairValue, double buyValue)> CalculateStockHistoricFairAndBuyValues(StockAnalysis stockAnalysis, int timeFrame, double rateOfReturn, double marginOfSafety)
        {
            List<(int year, double fairValue, double buyValue)> yearlyFairBuyValues = new List<(int year, double fairValue, double buyValue)>();
            for (int i = 0; i < stockAnalysis.BalanceSheets.Count(); i++)
            {
                var endYear = stockAnalysis.BalanceSheets[i].Year;

                if (i < 2) //if you don't have at least two years history don't bother
                {
                    yearlyFairBuyValues.Add((endYear, 0, 0));
                    continue;
                }

                var startYear = stockAnalysis.BalanceSheets[0].Year;

                var stockCagrs = stockAnalysis.EquitySlopeInfo.ValueInfos.Where(v => v.StartYear <= endYear).Select(v => !double.IsNaN(v.Cagr) ? v.Cagr : 0);

                double estimatedGrowth = CalculateEstimatedGrowth(stockCagrs);

                var currentEps = stockAnalysis.IncomeStatements.FirstOrDefault(ist => ist.Year == endYear)?.EpsBasic;
                var keyRatioStartYear = endYear - 5 > startYear ? endYear - 5 : startYear;
                var fiveYearsPERatios = stockAnalysis.KeyRatios.Where(kr => kr.Year >= keyRatioStartYear && kr.Year <= endYear).Select(kr => kr.PriceToEarnings.HasValue && !double.IsNaN(kr.PriceToEarnings.Value) ? kr.PriceToEarnings.Value : 0);
                var avgPeRatio = fiveYearsPERatios.Average();
                var medianPeRatio = fiveYearsPERatios.Median();
                var adjustedAveragePeRatio = (avgPeRatio + medianPeRatio) / 2;

                if (!currentEps.HasValue || double.IsNaN((double)currentEps.Value))
                {
                    yearlyFairBuyValues.Add((endYear, 0, 0));
                    continue;
                }
                var fairValue = CalculateStockFairValue((double)currentEps.Value, timeFrame, rateOfReturn, estimatedGrowth, adjustedAveragePeRatio);
                yearlyFairBuyValues.Add((endYear, fairValue, fairValue * (1 - marginOfSafety)));
            }

            return yearlyFairBuyValues;
        }

        public double CalculateEstimatedGrowth(IEnumerable<double> stockCagrs)
        {
            double estimatedGrowth = 0;

            var midPoint = stockCagrs.Count() / 2;

            var halfStockCagrs = stockCagrs.Skip(midPoint).Take(stockCagrs.Count());
            var cagrTotalAvg = stockCagrs.Average();
            var cagrTotalMedian = stockCagrs.Median();
            var cagrTotalAvgMedian = Math.Round((cagrTotalAvg + cagrTotalMedian) / 2, 2);

            var cagrAvg = halfStockCagrs.Average();
            var cagrMedian = halfStockCagrs.Median();
            var cagrAvgMedian = Math.Round((cagrAvg + cagrMedian) / 2, 2);
            estimatedGrowth = cagrTotalAvgMedian < cagrAvgMedian ? cagrTotalAvgMedian : cagrAvgMedian;
            estimatedGrowth = estimatedGrowth <= 0.2 ? estimatedGrowth : 0.2;

            return estimatedGrowth;
        }

        public double CalculateStockFairValue(double epsTTM, int timeFrame, double rateOfReturn, double growthEstimate, double fiveYearAvgPERatio)
        {
            return (epsTTM * fiveYearAvgPERatio * Math.Pow((1 + growthEstimate), timeFrame)) / Math.Pow(1 + rateOfReturn, timeFrame);
        }

        public double CalculateRoicFitness(SlopeInfo slopeInfo)
        {
            //if (slopeInfo.ValueSlope < 0 && slopeInfo.MedianValue < 0.1) return 0;
            //if (slopeInfo.ValueSlope >= 0 && slopeInfo.MedianValue >= 0.1 && slopeInfo.ValuePercentageDeviation < 0.5) return 1;

            var slope = slopeInfo.ValueSlope < 0 ? 0 : 1;

            double periodGrowth = 0;

            if (slopeInfo.LastYearValue >= 0.1) periodGrowth += 2;
            if (slopeInfo.LastThreeYearsValueAvg >= 0.1) periodGrowth += 1;
            if (slopeInfo.LastFiveYearsValueAvg >= 0.1) periodGrowth += 1;
            if (slopeInfo.LastSevenYearsValueAvg >= 0.1) periodGrowth += 1;
            if (slopeInfo.LastNineYearsValueAvg >= 0.1) periodGrowth += 1;

            periodGrowth = periodGrowth / 6;

            var deviation = 1 - slopeInfo.ValuePercentageDeviation;

            return (slope + 4 * periodGrowth + 2 * deviation) / 7;
        }

        public double CalculateGrowthFitness(SlopeInfo growthSlopeInfo)
        {
            //if (growthSlopeInfo.GrowthSlope < 0 || growthSlopeInfo.MedianGrowth < 0.1) return 0;
            //if (growthSlopeInfo.GrowthSlope >= 0 && growthSlopeInfo.MedianGrowth >= 0.1 && growthSlopeInfo.ValuePercentageDeviation < 0.5) return 1;

            var slope = growthSlopeInfo.GrowthSlope < 0 ? 0 : 1;

            double periodGrowth = 0;

            if (growthSlopeInfo.LastYearGrowth >= 0.1) periodGrowth += 2;
            if (growthSlopeInfo.LastThreeYearsGrowthAvg >= 0.1) periodGrowth += 1;
            if (growthSlopeInfo.LastFiveYearsGrowthAvg >= 0.1) periodGrowth += 1;
            if (growthSlopeInfo.LastSevenYearsGrowthAvg >= 0.1) periodGrowth += 1;
            if (growthSlopeInfo.LastNineYearsGrowthAvg >= 0.1) periodGrowth += 1;

            periodGrowth = periodGrowth / 6;

            //var meanGrowth = growthSlopeInfo.MedianGrowth >= 0.1 ? 1 : 0;
            double precision = 1 - growthSlopeInfo.GrowthPercentageDeviation;
            if (double.IsNaN(precision)) precision = 0;

            return (slope + 4 * periodGrowth + 2 * precision) / 7;
        }

        public double CalculateSimpleGrowth(SlopeInfo slopeInfo, int numYears)
        {
            var lastValueInfo = slopeInfo.ValueInfos.Last();
            var firstYear = lastValueInfo.StartYear - numYears;
            var firstValueInfo = slopeInfo.ValueInfos.SingleOrDefault(v => v.StartYear == firstYear);
            if (firstValueInfo == null) firstValueInfo = slopeInfo.ValueInfos.First();

            var startValue = firstValueInfo.StartValue;
            var endValue = lastValueInfo.StartValue;

            if (startValue == 0) return 1;

            return (endValue - startValue) / Math.Abs(startValue);
        }

        public double CalculatePERatioFitness(decimal peRatio)
        {
            if (peRatio <= 15) return 1;
            if (peRatio <= 30) return 0.75;
            if (peRatio <= 45) return 0.5;
            if (peRatio <= 60) return 0.25;
            return 0;
        }

        public double CalculateDebtToEquityFitness(decimal debtoToEquity)
        {
            if (debtoToEquity == 0) return 1;
            if (debtoToEquity <= (decimal)0.25) return 0.75;
            if (debtoToEquity <= (decimal)0.5) return 0.5;
            if (debtoToEquity <= 1) return 0.25;
            return 0;
        }

        public double CalculateAssetsToLiabilitiesFitness(decimal assetsToLiabilities)
        {
            if (assetsToLiabilities >= 3) return 1;
            if (assetsToLiabilities >= 2) return 0.75;
            if (assetsToLiabilities >= (decimal)1.5) return 0.5;
            if (assetsToLiabilities >= 1) return 0.25;
            return 0;
        }

        public double CalculateStockBuyPrice(double fairValue, double marginOfSafety)
        {
            return fairValue * (1 - marginOfSafety);
        }

        //public IEnumerable<ValueInfo> CalculateValues<T>(IEnumerable<IFInfo> fInfos, string propertyName)
        //{
        //    var values = new List<ValueInfo>();

        //    var listFInfos = fInfos.ToList();

        //    for (int i = 1; i < fInfos.Count(); i++)
        //    {
        //        var startPos = 0;
        //        var endPos = i;
        //        //if (i > 3) startPos = i - 3;

        //        if (fInfos.Count() <= i) return values;

        //        (var startValue, var endValue, var growth) = CalculateValues<T>(fInfos, propertyName, startPos, endPos, CalculateValues);

        //        values.Add(
        //            new ValueInfo
        //            {
        //                StartYear = listFInfos[i - 1].Year,
        //                EndYear = listFInfos[i].Year,
        //                StartValue = startValue,
        //                EndValue = endValue,
        //                Rate = growth
        //            });
        //    }

        //    return values;
        //}

        public IEnumerable<ValueInfo> CalculateGrowthValues<T>(IEnumerable<IFInfo> fInfos, string propertyName)
        {
            var values = new List<ValueInfo>();

            var listFInfos = fInfos.ToList();

            for (int i = 1; i < fInfos.Count(); i++)
            {
                var startPos = i - 1;
                var endPos = fInfos.Count() - 1;
                //if (i > 3) startPos = i - 3;

                (var startValue, var nextValue, var endCagrValue, var nextYearGrowth, var cagrGrowth) = CalculateCagrValues<T>(fInfos, propertyName, startPos, endPos);

                values.Add(
                    new ValueInfo
                    {
                        StartYear = listFInfos[startPos].Year,
                        EndYear = listFInfos[endPos].Year,
                        NextYear = listFInfos[startPos + 1].Year,
                        StartValue = startValue,
                        EndCagrValue = endCagrValue,
                        NextValue = nextValue,
                        Cagr = cagrGrowth,
                        GrowthToNextYear = nextYearGrowth
                    });
            }

            return values;
        }

        public (double startValue, double nextValue, double endValue, double nextYearGrowth, double cagrGrowth) CalculateCagrValues<TOut>(IEnumerable<IFInfo> fInfos, string propertyName, int startPosition, int endPosition)
        {
            fInfos = fInfos.OrderBy(f => f.Year).ToList();
            var propertyInfo = fInfos.First().GetType().GetProperty(propertyName);
            var values = new List<TOut>();

            foreach (var fInfo in fInfos)
            {
                var value = propertyInfo.GetValue(fInfo);
                values.Add((TOut)(value != null ? value : default(TOut)));
            }

            return CalculateCagrValues(values, startPosition, endPosition);
        }

        public (double startValue, double nextValue, double endValue, double nextYearGrowth, double cagrGrowth) CalculateCagrValues<T>(IEnumerable<T> values, int startPosition, int endPosition)
        {
            int numValues = endPosition - startPosition;
            var valuesIn = values.ToList();
            if (startPosition < 0 || endPosition >= values.Count() || numValues < 1) throw new InvalidOperationException($"start year or end year wrong");

            dynamic endValue = valuesIn[endPosition];
            dynamic startValue = valuesIn[startPosition];
            dynamic nextValue = valuesIn[startPosition + 1];
            if (startValue == 0) return ((double)startValue, (double)nextValue, (double)endValue, 1, 1);

            double nextYearGrowth = ((double)endValue - (double)startValue) / (double)Math.Abs(startValue);

            if (numValues == 1)
            {
                return ((double)startValue, (double)nextValue, (double)endValue, nextYearGrowth, nextYearGrowth);
            }
            //CAGR calculation doesn't work when the startValue or endValue are negative, because it results in the negative square-root operation...
            else
            {
                if (startValue < 0 || endValue < 0) return ((double)startValue, (double)nextValue, (double)endValue, nextYearGrowth, double.NaN);
                return ((double)startValue, (double)nextValue, (double)endValue, nextYearGrowth, (Math.Pow((double)(endValue / startValue), 1.0 / numValues) - 1));
            }
        }

        //public (double startValue, double endValue, double growth) CalculateValues<T>(IEnumerable<T> values, int startPosition, int endPosition)
        //{
        //    int numValues = endPosition - startPosition;
        //    var valuesIn = values.ToList();
        //    if (startPosition < 0 || endPosition >= values.Count() || numValues < 1) throw new InvalidOperationException($"start year or end year wrong");
        //    if (numValues == 1)
        //    {
        //        dynamic endValue = valuesIn[endPosition];
        //        dynamic startValue = valuesIn[startPosition];

        //        return ((double)startValue, (double)endValue, CalculateGrowth((double)startValue, (double)endValue));
        //    }
        //    //CAGR calculation doesn't work when the startValue or endValue are negative, because it results in the negative square-root operation...
        //    //else
        //    //{
        //    //    dynamic endValue = valuesIn[endPosition];
        //    //    dynamic startValue = valuesIn[startPosition];
        //    //    if (startValue == 0) return ((double)startValue, (double)endValue, 1);
        //    //    return ((double)startValue, (double)endValue, (Math.Pow((double)(endValue / startValue), 1.0 / numValues) - 1));
        //    //}
        //    else //this is my take on an alternative calculation of growth, that uses the average of the previous 3 years to smooth out the values
        //    {
        //        dynamic endValue = valuesIn[endPosition];
        //        var startValues = valuesIn.Skip(startPosition).Take(endPosition - startPosition);
        //        double startValuesSum = 0;
        //        foreach (dynamic value in startValues) startValuesSum += (double)value;
        //        dynamic startValuesAvg = startValuesSum / startValues.Count();

        //        return ((double)startValuesAvg, (double)endValue, CalculateGrowth((double)startValuesAvg, (double)endValue));
        //    }
        //}

        public double CalculateCagrGrowth(double startValue, double endValue, int numValues)
        {
            if (startValue == 0) return 1;
            if (startValue < 0 || endValue < 0) return double.NaN;
            return (Math.Pow(endValue / startValue, 1.0 / numValues) - 1);
        }

        //public double CalculateGrowth(double startValue, double endValue)
        //{
        //    //old formula: return ((double)startValue, (double)endValue, (double)((endValue - startValue) / startValue));
        //    //there are lots of problems with this formula
        //    //alternative formula is return ((double)startValue, (double)endValue, (double)((endValue - startValue) / Math.Abs(startValue)));
        //    //but this formula shows wrong results when only one of the values is negative and abs(startValue) > abs(endValue)
        //    //there is no solution to this problem. So if one of them is negative and abs(startValue) > abs(endValue) will return 200% positive or negative.

        //    if (startValue == endValue) return 0;
        //    if (startValue == 0 && endValue > 0) return 1;
        //    if (startValue == 0 && endValue < 0) return -1;

        //    if (startValue > 0 && endValue >= 0 || startValue < 0 && endValue <= 0)
        //        return (endValue - startValue) / Math.Abs(startValue);

        //    if (startValue < 0 && endValue > 0)
        //    {
        //        var cappedStartValue = Math.Min(Math.Abs(startValue), endValue) * -1;
        //        return (endValue - cappedStartValue) / Math.Abs(cappedStartValue);
        //    }
        //    else if (startValue > 0 && endValue < 0)
        //    {
        //        var cappedEndValue = Math.Min(startValue, Math.Abs(endValue)) * -1;
        //        return (cappedEndValue - startValue) / Math.Abs(startValue);
        //    }

        //    throw new NotSupportedException("Missing option");
        //}
    }
}