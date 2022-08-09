using ChartJSCore.Helpers;
using ChartJSCore.Models;
using MarketAnalyzer.Analysis;
using MarketAnalyzer.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.WebApp.Common
{
    public static class ChartUtils
    {
        public static Chart BuildPEChart(IEnumerable<(double, double, int)> records, decimal currentPeRatio)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            var labels = records.Select(r => r.Item3.ToString()).ToList();

            data.Labels = labels;

            data.Datasets = new List<Dataset>();

            var lowValues = records.Select(r => (double?)r.Item1.Round()).ToList();
            var highValues = records.Select(r => (double?)r.Item2.Round()).ToList();

            var currentPeValues = labels.Select(l => (double?)currentPeRatio.Round());

            data.Datasets.Add(BuildLineDataSet("Low", lowValues, ChartColor.FromHexString("#F4D00C")));
            data.Datasets.Add(BuildLineDataSet("High", highValues, ChartColor.FromHexString("#F4D00C")));
            data.Datasets.Add(BuildLineDataSet("current", currentPeValues, ChartColor.FromHexString("#F2635F")));
            chart.Data = data;
            return chart;
        }

        public static Chart BuildInvestedValueChart(IEnumerable<(double, double, int)> records, decimal currentValue)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            Options options = new Options();

            var labels = records.Select(r => r.Item3.ToString()).ToList();

            data.Labels = labels;
            options.Scales = new Scales();
            options.Scales.YAxes = new List<Scale>();
            var scale1 = new Scale
            {
                Weight = 5
            };
            options.Scales.YAxes.Add(scale1);

            data.Datasets = new List<Dataset>();

            var growthPercentageValues = records.Select(r => (double?)r.Item1.Round()).ToList();
            var growthValues = records.Select(r => (double?)r.Item2.Round()).ToList();

            var currentValues = labels.Select(l => (double?)currentValue.Round());

            data.Datasets.Add(BuildLineDataSet("Growth Percentage", growthPercentageValues, ChartColor.FromHexString("#F4D00C")));
            data.Datasets.Add(BuildLineDataSet("Gain/Loss", growthValues, ChartColor.FromHexString("#8BB63C"),true));
            data.Datasets.Add(BuildLineDataSet("Current Value", currentValues, ChartColor.FromHexString("#F2635F"),true));
            chart.Data = data;
            chart.Options = options;
            return chart;
        }

        public static Chart BuildMarketCapChart(StockAnalysis stockAnalysis)
        {
            Chart chart = new Chart();
            chart.Type = Enums.ChartType.Line;
            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Datasets = new List<Dataset>();

            var currentYear = DateTime.UtcNow.Year;

            //select all except for current year because we'll add that data from the ttm
            IEnumerable<(int year, decimal? marketCap)> marketCaps = stockAnalysis.KeyRatios.Where(kr => kr.Year < currentYear).Select(kr => (kr.Year, kr.MarketCapitalization));
            var years = marketCaps.Select(m => m.year).ToList();

            var fairAndBuyValues = stockAnalysis.StockFitness.HistoricFairValues.Where(h => years.Contains(h.year)).ToList();

            var marketCapAdjustedFairAndBuyValues = new List<(double?, double?)>();

            foreach (var fairAndBuyValue in fairAndBuyValues)
            {
                var sharesBasic = stockAnalysis.IncomeStatements.FirstOrDefault(ist => ist.Year == fairAndBuyValue.year)?.SharesBasic;
                sharesBasic = sharesBasic.HasValue ? sharesBasic.Value : 0;

                var adjustedFairValue = fairAndBuyValue.fairValue * (double)sharesBasic.Value;
                var adjustedBuyValue = fairAndBuyValue.buyValue * (double)sharesBasic.Value;
                marketCapAdjustedFairAndBuyValues.Add((adjustedFairValue.Round(), adjustedBuyValue.Round()));
            }

            data.Labels = years.Select(y => y.ToString()).ToList();

            var marketCapValues = marketCaps.Select(v => (double?)(v.marketCap.HasValue ? v.marketCap.Value.Round() : 0)).ToList();

            //TTM SECTION
            data.Labels.Add(currentYear.ToString());
            var ttmSharesBasic = stockAnalysis.IncomeStatementTtm.SharesBasic;
            ttmSharesBasic = ttmSharesBasic.HasValue ? ttmSharesBasic.Value.Round() : 0;

            var ttmMarketCap = stockAnalysis.Company.StockPrice * ttmSharesBasic.Value;
            marketCapValues.Add((double?)ttmMarketCap);

            var ttmAdjustedFairValue = (double?)(stockAnalysis.StockFitness.FairValueStockPrice * ttmSharesBasic.Value);
            var ttmAdjustedBuyValue = (double?)(stockAnalysis.StockFitness.BuyValueStockPrice * ttmSharesBasic.Value);

            marketCapAdjustedFairAndBuyValues.Add((ttmAdjustedFairValue, ttmAdjustedBuyValue));

            data.Datasets.Add(BuildLineDataSet("MarketCap", marketCapValues, ChartColor.FromHexString("#F2635F")));
            data.Datasets.Add(BuildLineDataSet("Fair Value", marketCapAdjustedFairAndBuyValues.Select(m => m.Item1), ChartColor.FromHexString("#F4D00C")));
            data.Datasets.Add(BuildLineDataSet("Buy Value", marketCapAdjustedFairAndBuyValues.Select(m => m.Item2), ChartColor.FromHexString("#8BB63C")));

            chart.Data = data;
            return chart;
        }

        public static Chart BuildValueChart(string chartName, SlopeInfo slopeInfo, bool toPercentage = false)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            var labels = slopeInfo.ValueInfos.TakeYears(9).Select(v => v.StartYear.ToString()).ToList();
            labels.Add(slopeInfo.ValueInfos.Last().NextYear.ToString());

            data.Labels = labels;

            data.Datasets = new List<Dataset>();

            var values = slopeInfo.ValueInfos.TakeYears(9).Select(v => (double?)(toPercentage ? v.StartValue.ToPercNum().Round() : v.StartValue.Round())).ToList();

            var lastValue = slopeInfo.ValueInfos.Last();

            values.Add(toPercentage ? lastValue.NextValue.ToPercNum().Round() : lastValue.NextValue.Round());

            data.Datasets.Add(BuildLineDataSet(chartName, values, ChartColor.FromHexString("#F2635F")));
            chart.Data = data;
            return chart;
        }

        public static Chart BuildGrowthChart(string chartName, SlopeInfo slopeInfo)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Labels = slopeInfo.ValueInfos.TakeYears(9).Select(v => v.StartYear.ToString()).ToList();

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(BuildLineDataSet(chartName, slopeInfo.ValueInfos.TakeYears(9).Select(v => (double?)v.Cagr.ToPercNum().Round()), ChartColor.FromHexString("#F4D00C")));
            chart.Data = data;
            return chart;
        }

        private static LineDataset BuildLineDataSet(string name, IEnumerable<double?> values, ChartColor lineColor, bool isHidden = false)
        {
            if (!isHidden)
            {
                return new LineDataset()
                {
                    Label = name,
                    Data = values.ToList(),
                    Fill = "false",
                    //LineTension = 0.1,
                    //BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                    BorderColor = lineColor,
                    //BorderCapStyle = "butt",
                    //BorderDash = new List<int> { },
                    //BorderDashOffset = 0.0,
                    //BorderJoinStyle = "miter",
                    //PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                    //PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                    //PointBorderWidth = new List<int> { 1 },
                    //PointHoverRadius = new List<int> { 5 },
                    //PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                    //PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                    //PointHoverBorderWidth = new List<int> { 2 },
                    //PointRadius = new List<int> { 1 },
                    //PointHitRadius = new List<int> { 10 },
                    //SpanGaps = false
                };
            }
            else
            {
                return new LineDataset()
                {
                    Label = name,
                    Data = values.ToList(),
                    Fill = "false",
                    Hidden = true,
                    BorderColor = lineColor,
                };
            }
            
        }
    }
}