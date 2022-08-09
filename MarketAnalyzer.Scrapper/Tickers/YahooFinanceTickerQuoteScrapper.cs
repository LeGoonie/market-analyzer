using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Tickers
{
    public class YahooFinanceTickerQuoteScrapper
    {
        public async Task<IEnumerable<Quote>> Scrape(params string[] parameters)
        {
            WebRequestHelper _webRequestHelper = new WebRequestHelper();

            int maxNumQuotes = 200;

            var results = new List<Quote>();

            for (int i = 0; i < parameters.Count(); i += maxNumQuotes)
            {
                var quoteParams = parameters.Skip(i).Take(maxNumQuotes).Aggregate((item1, item2) => $"{item1},{item2}");
                var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={quoteParams}";

                var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);

                var jsonResult = await _webRequestHelper.CallWebRequest(webRequest);

                var response = JsonConvert.DeserializeObject<Response>(jsonResult);
                results.AddRange(response.quoteResponse.result);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            return results;
        }

        private class Response
        {
            public QuoteResponse quoteResponse { get; set; }
        }

        private class QuoteResponse
        {
            public List<Quote> result { get; set; }
        }

        public class Quote
        {
            public string symbol { get; set; }
            public decimal regularMarketPrice { get; set; }

            //"language": "en-US",
            //    "region": "US",
            //    "quoteType": "EQUITY",
            //    "quoteSourceName": "Delayed Quote",
            //    "triggerable": true,
            //    "currency": "USD",
            //    "exchange": "NMS",
            //    "shortName": "Ulta Beauty, Inc.",
            //    "longName": "Ulta Beauty, Inc.",
            //    "messageBoardId": "finmb_35929",
            //    "exchangeTimezoneName": "America/New_York",
            //    "exchangeTimezoneShortName": "EDT",
            //    "gmtOffSetMilliseconds": -14400000,
            //    "market": "us_market",
            //    "esgPopulated": false,
            //    "tradeable": false,
            //    "firstTradeDateMilliseconds": 1193319000000,
            //    "trailingPE": 17.737448,
            //    "marketState": "CLOSED",
            //    "epsTrailingTwelveMonths": 12.15,
            //    "epsForward": 12.26,
            //    "sharesOutstanding": 57161300,
            //    "bookValue": 33.601,
            //    "fiftyDayAverage": 196.0397,
            //    "fiftyDayAverageChange": 19.470291,
            //    "fiftyDayAverageChangePercent": 0.0993181,
            //    "twoHundredDayAverage": 241.77745,
            //    "twoHundredDayAverageChange": -26.267456,
            //    "twoHundredDayAverageChangePercent": -0.108643115,
            //    "marketCap": 12135260160,
            //    "forwardPE": 17.578302,
            //    "priceToBook": 6.413797,
            //    "sourceInterval": 15,
            //    "exchangeDataDelayedBy": 0,
            //    "postMarketChangePercent": -0.25511086,
            //    "postMarketTime": 1587160001,
            //    "postMarketPrice": 214.9602,
            //    "postMarketChange": -0.5497894,
            //    "regularMarketChange": 6.5899963,
            //    "regularMarketChangePercent": 3.1543157,
            //    "regularMarketTime": 1587153602,
            //    "regularMarketPrice": 215.51,
            //    "regularMarketDayHigh": 222.94,
            //    "regularMarketDayRange": "208.82 - 222.94",
            //    "regularMarketDayLow": 208.82,
            //    "regularMarketVolume": 1095403,
            //    "regularMarketPreviousClose": 208.92,
            //    "bid": 214.96,
            //    "ask": 217.0,
            //    "bidSize": 9,
            //    "askSize": 9,
            //    "fullExchangeName": "NasdaqGS",
            //    "financialCurrency": "USD",
            //    "regularMarketOpen": 220.8,
            //    "averageDailyVolume3Month": 1257150,
            //    "averageDailyVolume10Day": 1093683,
            //    "fiftyTwoWeekLowChange": 91.45999,
            //    "fiftyTwoWeekLowChangePercent": 0.7372833,
            //    "fiftyTwoWeekRange": "124.05 - 368.83",
            //    "fiftyTwoWeekHighChange": -153.31999,
            //    "fiftyTwoWeekHighChangePercent": -0.41569287,
            //    "fiftyTwoWeekLow": 124.05,
            //    "fiftyTwoWeekHigh": 368.83,
            //    "dividendDate": 1337040000,
            //    "earningsTimestamp": 1584043380,
            //    "earningsTimestampStart": 1590710400,
            //    "earningsTimestampEnd": 1591056000,
            //    "priceHint": 2,
            //    "symbol": "ULTA"
        }
    }
}