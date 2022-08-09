using MarketAnalyzer.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Historical
{
    public class HistoricalStockScrapper
    {
        private readonly WebRequestHelper _webRequestHelper = new WebRequestHelper();

        private int apikeyIndex = 0;

        private List<string> apikeyList = new List<string> { "Z1HCNSS5EBUQ5NQN", "2W65OCHGYNQWNWLK", "IHPG0Z4BV8LG8WRP", "AQ65W9EJ3OQAFYKD", "ZJEOUCOC0SF44KJ1", "Y66XW30JDCNWNHQR" };

        public async Task<IEnumerable<HistoricalStockRecord>> GetHistoricalData(Guid companyId, string ticker)
        {
            var apikey = apikeyList[apikeyIndex];
            apikeyIndex = apikeyIndex + 1 >= apikeyList.Count() ? 0 : apikeyIndex + 1;

            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY_ADJUSTED&symbol={ticker}&apikey={apikey}&outputsize=full";
            var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
            var jsonResult = await _webRequestHelper.CallWebRequest(webRequest);

            var historicalRecords = ParseJson(jsonResult);

            var results = ConvertRecordsToYearly(companyId, historicalRecords);
            return results;
        }

        private IEnumerable<HistoricalStockRecord> ConvertRecordsToYearly(Guid companyId, IEnumerable<HistoricalStockRecord> records)
        {
            return (from record in records
                    group record by record.StartDate.Year into groupedRecords
                    select new HistoricalStockRecord
                    {
                        Low = groupedRecords.Min(r => r.Low),
                        High = groupedRecords.Max(r => r.High),
                        Open = groupedRecords.OrderBy(r => r.StartDate).First().Open,
                        Close = groupedRecords.OrderBy(r => r.StartDate).Last().Close,
                        StartDate = DateTime.Parse($"{groupedRecords.Key}-01-01"),
                        Timeframe = "Y",
                        Volume = groupedRecords.Sum(r => r.Volume),
                        Id = Guid.NewGuid(),
                        CompanyId = companyId
                    }).ToList();
        }

        private IEnumerable<HistoricalStockRecord> ParseJson(string json)
        {
            var jObject = JObject.Parse(json);

            var series = jObject.Last.First.Children();

            var records = new List<HistoricalStockRecord>();

            foreach (Newtonsoft.Json.Linq.JProperty item in series)
            {
                var date = item.Name;

                var keyValues = item.First().Children();

                var record = new HistoricalStockRecord();

                Map(record, "startDate", date);

                foreach (JProperty keyValue in keyValues)
                {
                    var key = keyValue.Name;
                    var value = ((Newtonsoft.Json.Linq.JValue)keyValue.Value).Value;
                    Map(record, key, value.ToString());
                }

                record.Timeframe = "M";
                records.Add(record);
            }

            if (records.Count == 0) Console.WriteLine(json);

            return records;
        }

        private HistoricalStockRecord Map(HistoricalStockRecord record, string key, string value)
        {
            switch (key)
            {
                case "startDate":
                    record.StartDate = DateTime.Parse(value);
                    break;

                case "1. open":
                    record.Open = decimal.Parse(value);
                    break;

                case "2. high":
                    record.High = decimal.Parse(value);
                    break;

                case "3. low":
                    record.Low = decimal.Parse(value);
                    break;

                case "4. close":
                    record.Close = decimal.Parse(value);
                    break;

                case "6. volume":
                    record.Volume = double.Parse(value);
                    break;
            }

            return record;
        }
    }
}