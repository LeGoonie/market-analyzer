using MarketAnalyzer.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Tickers
{
    public class FullQuickFsTickerScrapper
    {
        public async Task Run()
        {
            WebRequestHelper _webRequestHelper = new WebRequestHelper();
            var companiesJson = File.ReadAllText("CompanyNamesList.txt");
            var companiesData = Newtonsoft.Json.JsonConvert.DeserializeObject<CompaniesJsonObject>(companiesJson);

            var numItemsToSkip = 1858;
            foreach (var item in companiesData.data.Skip(numItemsToSkip))
            {
                var url = $"https://api.quickfs.net/search?q={item}";
                var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
                var results = JsonConvert.DeserializeObject<List<QuickFsCompanySearchResult>>(await _webRequestHelper.CallWebRequest(webRequest));

                var firstResult = results.FirstOrDefault();
                if (firstResult != null)
                {
                    AddOrUpdateCompanyData(firstResult);
                    Console.WriteLine($"Add or updated index {companiesData.data.IndexOf(item)} {firstResult.name}");
                }
                else
                {
                    int a = 0;
                }
            }
        }

        private void AddOrUpdateCompanyData(QuickFsCompanySearchResult searchResult)
        {
            var data = searchResult.symbol.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var ticker = data[0];
            var country = data[1];
            var name = searchResult.name;
            var industryName = searchResult.industry;

            using (var context = new MarketAnalyzerDBContext())
            {
                var company = context.Companies.SingleOrDefault(c => c.Ticker == ticker && c.Country == country);
                var industry = context.Industries.SingleOrDefault(i => i.Name == industryName);

                if (industry == null)
                {
                    industry = new Industry
                    {
                        Id = Guid.NewGuid(),
                        Name = industryName
                    };

                    context.Industries.Add(industry);
                }

                if (company == null)
                {
                    company = new Company
                    {
                        Id = Guid.NewGuid(),
                        Country = country,
                        IndustryId = industry.Id,
                        Name = name,
                        Ticker = ticker
                    };
                    context.Companies.Add(company);
                }
                else
                {
                    company.IndustryId = industry.Id;
                    company.Name = name;
                }
                context.SaveChanges();
            }
        }

        private class CompaniesJsonObject
        {
            public List<string> data { get; set; }
        }

        private class QuickFsCompanySearchResult
        {
            public string label { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public string exch_sym { get; set; }
            public string industry { get; set; }
        }

        //[{"label":"The Coca-Cola Company (KO:US)","symbol":"KO:US","name":"The Coca-Cola Company","exch_sym":"NYSE: KO","industry":"Food, Beverage & Tobacco"}]
    }
}