using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Analysis
{
    public static class Utils
    {
        public static IEnumerable<ValueInfo> TakeYears(this IEnumerable<ValueInfo> valueInfos, int numYears, int? lastYear = null)
        {
            var lastYearDate = lastYear.HasValue ? lastYear.Value : DateTime.Now.Year - 1;
            var firstYearDate = lastYearDate - (numYears); //- 1);

            var outValueInfos = valueInfos.Where(v => v.StartYear >= firstYearDate && v.StartYear <= lastYearDate).ToList();

            //while (outValueInfos.First().StartYear > firstYearDate)
            //    outValueInfos.Insert(0, new ValueInfo { StartYear = outValueInfos.First().StartYear - 1, Rate = 0 });

            //while (outValueInfos.Last().StartYear < lastYearDate)
            //    outValueInfos.Add(new ValueInfo { StartYear = outValueInfos.Last().StartYear + 1, Rate = 0 });

            return outValueInfos;
        }
    }
}