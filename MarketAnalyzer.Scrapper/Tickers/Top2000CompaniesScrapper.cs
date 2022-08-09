using MarketAnalyzer.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Tickers
{
    public class Top2000CompaniesScrapper
    {
        public async Task Run()
        {
            WebRequestHelper _webRequestHelper = new WebRequestHelper();

            var companiesJson = File.ReadAllText("Forbes2000List.txt");

            var companiesResults = JsonConvert.DeserializeObject<List<Forbes2000Company>>(companiesJson);

            var numItemsToSkip = 1375;
            foreach (var item in companiesResults.Skip(numItemsToSkip))
            {
                var url = $"https://api.quickfs.net/search?q={item.name}";
                var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
                var results = JsonConvert.DeserializeObject<List<QuickFsCompanySearchResult>>(await _webRequestHelper.CallWebRequest(webRequest));

                if (results.Count() > 0)
                {
                    var selectedResults = SelectMostProbableResult(results, item);

                    foreach (var selectedResult in selectedResults)
                    {
                        AddOrUpdateCompanyData(selectedResult, item.position);
                        Console.WriteLine($"Add or updated index {companiesResults.IndexOf(item)} {item.name} | match {selectedResult.name}");
                    }
                }
                else
                {
                    Console.WriteLine($"Skipped index {companiesResults.IndexOf(item)} {item.name}");
                }
            }
        }

        private List<QuickFsCompanySearchResult> SelectMostProbableResult(List<QuickFsCompanySearchResult> items, Forbes2000Company company)
        {
            var orderedItems = items
                //this assumes they have to be listed in the US
                .Where(i => i.symbol.EndsWith(":US"))
                .Select(i => new { Distance = Fastenshtein.Levenshtein.Distance(i.name.ToLower(), company.name.ToLower()), Item = i }).OrderBy(i => i.Distance).ToList();

            var bestMatch = orderedItems.FirstOrDefault();

            if (bestMatch == null || bestMatch.Distance > 10)
                return new List<QuickFsCompanySearchResult>();

            var filteredItems = orderedItems.Where(i => i.Distance == orderedItems.First().Distance).ToList();

            return filteredItems.Select(i => i.Item).ToList();
        }

        private void AddOrUpdateCompanyData(QuickFsCompanySearchResult searchResult, int forbesRank)
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
                        Ticker = ticker,
                        Forbes2000Rank = forbesRank
                    };
                    context.Companies.Add(company);
                }
                else
                {
                    company.IndustryId = industry.Id;
                    company.Name = name;
                    company.Forbes2000Rank = forbesRank;
                }
                context.SaveChanges();
            }
        }

        private class CompaniesJsonObject
        {
            public List<string> data { get; set; }
        }

        private class Forbes2000Company
        {
            public int position { get; set; }
            public string name { get; set; }
            public string country { get; set; }
            public double marketvalue { get; set; }
        }

        private class QuickFsCompanySearchResult
        {
            public string label { get; set; }
            public string symbol { get; set; }
            public string name { get; set; }
            public string exch_sym { get; set; }
            public string industry { get; set; }
        }
    }
}