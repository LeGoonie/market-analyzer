using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public class StatisticalAnalysis
    {
        public (double valueSlopeIntersection, double valueSlope, double rateSlopeIntersection, double rateSlope) CalculateSlopes(IEnumerable<ValueInfo> valueInfos)
        {
            double i = 0;
            var xData = valueInfos.Select(v => i++).ToArray();
            var rates = valueInfos.Select(g => g.Cagr).ToArray();
            var values = valueInfos.Select(g => g.StartValue).ToArray();
            var rateSlope = Fit.Line(xData, rates);
            var valueSlope = Fit.Line(xData, values);
            return (valueSlope.Item1, valueSlope.Item2, rateSlope.Item1, rateSlope.Item2); //slope
        }

        public (double valueError, double rateError) CalculateGoodnessOfFit(SlopeInfo slopeInfo)
        {
            return (CalculateGoodnessOfFit(slopeInfo.ValueInfos.Select(v => v.StartValue), slopeInfo.ValueSlopeIntersection, slopeInfo.ValueSlope),
                    CalculateGoodnessOfFit(slopeInfo.ValueInfos.Select(v => v.Cagr), slopeInfo.GrowthSlopeIntersection, slopeInfo.GrowthSlope, true));
        }

        public double CalculateGoodnessOfFit(IEnumerable<double> observedValues, double slopeIntersection, double slopeValue, bool percentageFit = false)
        {
            Func<double, double, double, double> func = (double b, double m, double x) => { return m * x + b; };

            var modelledValues = new List<double>();

            for (int i = 0; i < observedValues.Count(); i++)
            {
                modelledValues.Add(func(slopeIntersection, slopeValue, i));
            }

            var rsquared = MathNet.Numerics.GoodnessOfFit.RSquared(modelledValues, observedValues);
            if (!percentageFit) return 1 - rsquared;

            var stdErr = MathNet.Numerics.GoodnessOfFit.StandardError(modelledValues, observedValues, observedValues.Count() - 1);

            return Math.Min(stdErr, 1) / 1;
        }

        public (double valueDeviation, double rateDeviation) CalculatePercentageDeviations(IEnumerable<ValueInfo> growths)
        {
            return (CalculatePercentageDeviations(growths.Select(g => g.StartValue)), CalculatePercentageDeviations(growths.Select(g => g.Cagr)));
        }

        public double CalculatePercentageDeviations(IEnumerable<double> values)
        {
            var meanStandard = values.MeanStandardDeviation();
            var mean = meanStandard.Item1;
            var std = meanStandard.Item2;

            var summaries = Statistics.FiveNumberSummary(values);
            //{min, lower-quantile, median, upper-quantile, max}

            var count = 0;

            foreach (var value in values)
            {
                //if (value < summaries[1] || value > summaries[3]) count++;
                if (value < mean - std || value > mean + std) count++;
            }

            return (double)count / values.Count();
        }
    }
}