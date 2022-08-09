using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public class SlopeInfo
    {
        private readonly IEnumerable<ValueInfo> _valueInfos;

        public int NumYears { get; }

        public double ValueSlopeIntersection { get; set; }
        public double ValueSlope { get; }
        public double GrowthSlopeIntersection { get; set; }
        public double GrowthSlope { get; }
        public double GrowthPercentageDeviation { get; }

        public double ValuePercentageDeviation { get; }

        public double MedianGrowth { get; set; }

        public double MedianValue { get; set; }

        public double LastYearValue { get; set; }

        public double LastThreeYearsValueAvg { get; set; }

        public double LastFiveYearsValueAvg { get; set; }

        public double LastSevenYearsValueAvg { get; set; }

        public double LastNineYearsValueAvg { get; set; }

        public double LastYearGrowth { get; set; }

        public double LastThreeYearsGrowthAvg { get; set; }

        public double LastFiveYearsGrowthAvg { get; set; }

        public double LastSevenYearsGrowthAvg { get; set; }

        public double LastNineYearsGrowthAvg { get; set; }

        public SlopeInfo()
        {
        }

        public SlopeInfo(IEnumerable<ValueInfo> valueInfos)
        {
            _valueInfos = valueInfos;

            NumYears = _valueInfos.Count();

            var analysis = new StatisticalAnalysis();

            if (valueInfos.Count() > 1)
            {
                (var valueSlopeIntersection, var valueSlope, var rateSlopeIntersection, var rateSlope) = analysis.CalculateSlopes(valueInfos);
                ValueSlopeIntersection = valueSlopeIntersection;
                GrowthSlopeIntersection = rateSlopeIntersection;
                GrowthSlope = rateSlope;
                ValueSlope = valueSlope;
            }
            else
            {
                GrowthSlope = 0;
                ValueSlope = valueInfos.Count() > 0 ? valueInfos.First().NextValue : 0;
            }

            MedianGrowth = valueInfos.Select(g => g.Cagr).Median();
            MedianValue = valueInfos.Select(g => g.StartValue).Median();

            (double valueDeviation, double rateDeviation) = analysis.CalculateGoodnessOfFit(this);

            ValuePercentageDeviation = valueDeviation;
            GrowthPercentageDeviation = rateDeviation;

            var lastYear = ValueInfos.Last().StartYear;

            LastYearValue = GrowthAvg(ValueInfos, lastYear, 0, v => v.StartValue);
            LastThreeYearsValueAvg = GrowthAvg(ValueInfos, lastYear - 2, 1, v => v.StartValue);
            LastFiveYearsValueAvg = GrowthAvg(ValueInfos, lastYear - 4, 1, v => v.StartValue);
            LastSevenYearsValueAvg = GrowthAvg(ValueInfos, lastYear - 6, 1, v => v.StartValue);
            LastNineYearsValueAvg = GrowthAvg(ValueInfos, lastYear - 8, 1, v => v.StartValue);

            LastYearGrowth = GrowthAvg(ValueInfos, lastYear, 0, v => v.Cagr);
            LastThreeYearsGrowthAvg = GrowthAvg(ValueInfos, lastYear - 2, 1, v => v.Cagr);
            LastFiveYearsGrowthAvg = GrowthAvg(ValueInfos, lastYear - 4, 1, v => v.Cagr);
            LastSevenYearsGrowthAvg = GrowthAvg(ValueInfos, lastYear - 6, 1, v => v.Cagr);
            LastNineYearsGrowthAvg = GrowthAvg(ValueInfos, lastYear - 8, 1, v => v.Cagr);
        }

        public List<ValueInfo> ValueInfos { get { return _valueInfos.ToList(); } }

        private double GrowthAvg(IEnumerable<ValueInfo> valueInfos, int referenceYear, int yearSpan, Func<ValueInfo, double> selector)
        {
            int leftPos = 0;
            int rightSpan = 0;

            var valueInfo = valueInfos.SingleOrDefault(v => v.StartYear == referenceYear);
            if (valueInfo == null) return 0;

            var pos = valueInfos.ToList().IndexOf(valueInfo);

            if (pos - yearSpan < 0) leftPos = 0;
            else leftPos = pos - yearSpan;

            if (pos + yearSpan >= valueInfos.Count()) rightSpan = valueInfos.Count() - 1;
            else rightSpan = pos + yearSpan;

            var selectedValueInfos = new List<ValueInfo>();

            for (int i = leftPos; i <= rightSpan; i++)
            {
                selectedValueInfos.Add(valueInfos.ElementAt(i));
            }

            return selectedValueInfos.Average(selector);
        }
    }
}