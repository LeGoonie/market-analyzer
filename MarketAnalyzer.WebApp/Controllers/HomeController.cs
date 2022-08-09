using MarketAnalyzer.Analysis;
using MarketAnalyzer.Business;
using MarketAnalyzer.Core;
using MarketAnalyzer.Data;
using MarketAnalyzer.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace MarketAnalyzer.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public class ToggleBuyStockMessage
        {
            public string CompanyId { get; set; }
            public string CHeckBoxId { get; set; }
            public bool IsChecked { get; set; }
        }

        public class SaveNotesMessage
        {
            public string CompanyId { get; set; }
            public string Message { get; set; }
        }

        private Company GetCompanyFromCache(Guid companyId)
        {
            var dotnetCache = new DotNetCache();
            IEnumerable<StockAnalysis> results = dotnetCache.Get<IEnumerable<StockAnalysis>>("stocksAnalysis");
            if (results == null) return null;
            var stockAnalysis = results.SingleOrDefault(r => r.Company.Id == companyId);
            return stockAnalysis?.Company;
        }

        [HttpPost]
        public void ToggleBuyStock([FromBody]ToggleBuyStockMessage message)
        {
            var tickerBO = new TickerDataBO();
            tickerBO.SaveToggleBuyTicker(Guid.Parse(message.CompanyId), message.CHeckBoxId, message.IsChecked);

            var company = GetCompanyFromCache(Guid.Parse(message.CompanyId));
            if (company != null)
            {
                if (message.CHeckBoxId == "buyR")
                    company.BuyR = message.IsChecked;
                if (message.CHeckBoxId == "buyL")
                    company.BuyL = message.IsChecked;
            }
        }

        [Authorize]
        [HttpPost]
        public void SaveNotes([FromBody]SaveNotesMessage message)
        {
            var tickerBO = new TickerDataBO();
            tickerBO.SaveTickerNotes(Guid.Parse(message.CompanyId), message.Message);

            var company = GetCompanyFromCache(Guid.Parse(message.CompanyId));
            if (company != null)
            {
                company.Notes = message.Message;
            }

        }

        [Authorize]
        [HttpPost]
        public IActionResult AddNote(DetailViewModel detailViewModel)
        {
            var tickerBO = new TickerDataBO();
            tickerBO.AddNote(detailViewModel.companyId, User.FindFirstValue(ClaimTypes.NameIdentifier),detailViewModel.CompanyNote);

            var company = GetCompanyFromCache(detailViewModel.companyId);
            if (company != null)
            {
                company.Notes = detailViewModel.CompanyNote;
            }

            return RedirectToAction("CompanyDetail", new {ticker = company.Ticker});
        }

        public IActionResult Index(int skip = 0, int take = 10, double roicMult = 2, double equityMult = 1.7, double epsMult = 1.5, double revenueMult = 1.3, double peMult = 2, double dToEMult = 0.8, double aToLMult = 0.8, bool fairValue = false, bool buyValue = false, string ticker = "", bool buyr = false, bool buyl = false)
        {
            var stockAnalyzerBO = new DataAnalyzerBO();

            var dotnetCache = new DotNetCache();

            var settingsBO = new UserSettingsBO();

            if (User.Identity.IsAuthenticated)
            {
                var userSettings = settingsBO.GetMultiplierSetting(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userSettings != null) { 
                    roicMult = (double)userSettings.RoicMult;
                    equityMult = (double)userSettings.EquityMult;
                    epsMult = (double)userSettings.EpsMult;
                    revenueMult = (double)userSettings.RevenueMult;
                    peMult = (double)userSettings.PEMult;
                    dToEMult = (double)userSettings.DebtToEquityMult;
                    aToLMult = (double)userSettings.CurrentAssetsToLiabilitiesMult;
                }
            }

            IEnumerable<StockAnalysis> results = dotnetCache.Get<IEnumerable<StockAnalysis>>("stocksAnalysis");
            DateTime? lastUpdatedAt = dotnetCache.Get<DateTime?>("lastUpdatedAt");

            var timeInCache = TimeSpan.FromHours(48);
            if (results == null)
            {
                results = stockAnalyzerBO.AnalyzeStocks();
                dotnetCache.Set("stocksAnalysis", results, timeInCache);
                dotnetCache.Set("lastUpdatedAt", DateTime.UtcNow);
            }

            var tickers = new List<string>();

            if (ticker != null) { 
                tickers = ticker.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToUpper()).ToList();
            } 
            var filteredResults = results;

            foreach (var result in results)
            {
                result.StockFitness.RoicMult = roicMult;
                result.StockFitness.EquityMult = equityMult;
                result.StockFitness.EpsMult = epsMult;
                result.StockFitness.RevenueMult = revenueMult;
                result.StockFitness.PeMult = peMult;
                result.StockFitness.DToEMult = dToEMult;
                result.StockFitness.AToLMult = aToLMult;
            }

            filteredResults = filteredResults
                //.OrderByDescending(f => f.StockPrice <= f.BuyValueStockPrice)
                .OrderByDescending(
                        //.ThenByDescending(
                        f => f.StockFitness.FitnessTotal);

            int pos = 0;
            foreach (var filteredResult in filteredResults) filteredResult.Position = ++pos;

            if (tickers.Count() > 0) filteredResults = filteredResults.Where(s => tickers.Contains(s.Company.Ticker));
            if (buyr) filteredResults = filteredResults.Where(s => s.Company.BuyR == true);
            if (buyl) filteredResults = filteredResults.Where(s => s.Company.BuyL == true);
            if (fairValue) filteredResults = filteredResults.Where(s => s.Company.StockPrice <= s.StockFitness.FairValueStockPrice);
            if (buyValue) filteredResults = filteredResults.Where(s => s.Company.StockPrice <= s.StockFitness.BuyValueStockPrice);
            if (tickers.Count() == 0 && !buyr && !buyl) filteredResults = filteredResults.Skip(skip).Take(take);

            var homeViewModel = new HomeViewModel
            {
                StocksAnalysis = filteredResults,
                RoicMult = roicMult,
                EquityMult = equityMult,
                EpsMult = epsMult,
                RevenueMult = revenueMult,
                PEMult = peMult,
                DebtToEquityMult = dToEMult,
                CurrentAssetsToLiabilitiesMult = aToLMult,
                Total = results.Count(),
                Skip = skip,
                Take = take,
                FilterOnlyFairStocks = fairValue,
                FilterOnlyBuyStocks = buyValue,
            };

            return View(homeViewModel);
        }

        [Authorize]
        public IActionResult CompanyDetail(int skip = 0, int take = 1, double roicMult = 2, double equityMult = 1.7, double epsMult = 1.5, double revenueMult = 1.3, double peMult = 2, double dToEMult = 0.8, double aToLMult = 0.8, bool fairValue = false, bool buyValue = false, string ticker = "", bool buyr = false, bool buyl = false)
        {
            var stockAnalyzerBO = new DataAnalyzerBO();

            var dotnetCache = new DotNetCache();

            var settingsBO = new UserSettingsBO();

            if (User.Identity.IsAuthenticated)
            {
                var userSettings = settingsBO.GetMultiplierSetting(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userSettings != null)
                {
                    roicMult = (double)userSettings.RoicMult;
                    equityMult = (double)userSettings.EquityMult;
                    epsMult = (double)userSettings.EpsMult;
                    revenueMult = (double)userSettings.RevenueMult;
                    peMult = (double)userSettings.PEMult;
                    dToEMult = (double)userSettings.DebtToEquityMult;
                    aToLMult = (double)userSettings.CurrentAssetsToLiabilitiesMult;
                }
            }

            IEnumerable<StockAnalysis> results = dotnetCache.Get<IEnumerable<StockAnalysis>>("stocksAnalysis");
            DateTime? lastUpdatedAt = dotnetCache.Get<DateTime?>("lastUpdatedAt");

            var timeInCache = TimeSpan.FromHours(48);
            if (results == null)
            {
                results = stockAnalyzerBO.AnalyzeStocks();
                dotnetCache.Set("stocksAnalysis", results, timeInCache);
                dotnetCache.Set("lastUpdatedAt", DateTime.UtcNow);
            }

            var tickers = ticker.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToUpper()).ToList();

            var filteredResults = results;

            

            foreach (var result in results)
            {
                result.StockFitness.RoicMult = roicMult;
                result.StockFitness.EquityMult = equityMult;
                result.StockFitness.EpsMult = epsMult;
                result.StockFitness.RevenueMult = revenueMult;
                result.StockFitness.PeMult = peMult;
                result.StockFitness.DToEMult = dToEMult;
                result.StockFitness.AToLMult = aToLMult;
            }

            filteredResults = filteredResults
                //.OrderByDescending(f => f.StockPrice <= f.BuyValueStockPrice)
                .OrderByDescending(
                        //.ThenByDescending(
                        f => f.StockFitness.FitnessTotal);

            int pos = 0;
            foreach (var filteredResult in filteredResults) filteredResult.Position = ++pos;

            if (tickers.Count() > 0) filteredResults = filteredResults.Where(s => tickers.Contains(s.Company.Ticker));
            if (buyr) filteredResults = filteredResults.Where(s => s.Company.BuyR == true);
            if (buyl) filteredResults = filteredResults.Where(s => s.Company.BuyL == true);
            if (fairValue) filteredResults = filteredResults.Where(s => s.Company.StockPrice <= s.StockFitness.FairValueStockPrice);
            if (buyValue) filteredResults = filteredResults.Where(s => s.Company.StockPrice <= s.StockFitness.BuyValueStockPrice);
            if (tickers.Count() == 0 && !buyr && !buyl) filteredResults = filteredResults.Skip(skip).Take(take);
            var companyLastNote = "";
            var companyNotes = new List<UserNote>();
            var tickerBO = new TickerDataBO();
            companyLastNote = tickerBO.GetNoteFromUserCompany(filteredResults.SingleOrDefault().Company.Id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            companyNotes = tickerBO.GetAllNotesFromUserCompany(filteredResults.SingleOrDefault().Company.Id, User.FindFirstValue(ClaimTypes.NameIdentifier));

            var detailViewModel = new DetailViewModel
            {
                StocksAnalysis = filteredResults,
                RoicMult = roicMult,
                EquityMult = equityMult,
                EpsMult = epsMult,
                RevenueMult = revenueMult,
                PEMult = peMult,
                DebtToEquityMult = dToEMult,
                CurrentAssetsToLiabilitiesMult = aToLMult,
                Total = results.Count(),
                Skip = skip,
                Take = take,
                FilterOnlyFairStocks = fairValue,
                FilterOnlyBuyStocks = buyValue,
                CompanyNote = companyLastNote,
                UserNotes = companyNotes,
            };

            return View(detailViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}