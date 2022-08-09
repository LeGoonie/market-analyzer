using HtmlAgilityPack;
using MarketAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Tickers
{
    public class SAndP500TickerScrapper
    {
        private readonly WebRequestHelper _webRequestHelper = new WebRequestHelper();

        public async Task<List<Company>> GetTickers()
        {
            var url = $"https://www.slickcharts.com/sp500";
            var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
            webRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            var result = await _webRequestHelper.CallWebRequest(webRequest);

            var document = new HtmlDocument();

            document.LoadHtml(result);

            var rootNode = document.DocumentNode.Descendants().Where(n => n.Name == "tbody").Single();

            var rows = rootNode.Descendants().Where(n => n.Name == "tr");

            var companies = new List<Company>();

            foreach (var row in rows)
            {
                var sAndPRank = row.Descendants().Where(c => c.Name == "td").First().InnerHtml;
                var infos = row.Descendants().Where(n => n.Name == "a");

                var companyName = infos.ElementAt(0).InnerHtml;
                var companyTicker = infos.ElementAt(1).InnerHtml;
                var stockPrice = row.Descendants().Where(n => n.Name == "img").Single().NextSibling.InnerHtml.Replace("&nbsp;&nbsp;", "").Trim();

                companies.Add(new Company { Id = Guid.NewGuid(), Name = companyName, Ticker = companyTicker, StockPrice = decimal.Parse(stockPrice, CultureInfo.InvariantCulture), SandPrank = int.Parse(sAndPRank) });
            }

            return companies;
            
        }
    }
}