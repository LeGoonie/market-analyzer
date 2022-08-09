using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Business;
using MarketAnalyzer.Data;
using System.Security.Claims;
using MarketAnalyzer.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using MarketAnalyzer.Data.Pocos;
using System.Collections.Concurrent;

namespace MarketAnalyzer.WebApp.Controllers
{
    public class PortfoliosController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var bo = new PortfolioBO();
            var portfoliosAssistants = bo.GetPortfolioAssistants(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(portfoliosAssistants.Item1.Count() == 0)
            {
                return RedirectToAction("Error","Home");
            }

            var portfolioViewModel = new PortfolioViewModel
            {
                ListOfPortfolios = portfoliosAssistants.Item1.OrderByDescending(d => d.DateOfLastInvestment),
                TotalInvested = portfoliosAssistants.Item2,
                TotalValue = portfoliosAssistants.Item3,
                TotalWithdrawed = portfoliosAssistants.Item4,
                TotalGainLoss = portfoliosAssistants.Item5,
                TotalGainLossPercentage = portfoliosAssistants.Item6,
                TotalInvestedYearValue = portfoliosAssistants.Item7
            };
            return View(portfolioViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddInvestment(DetailViewModel detailViewModel)
        {
            var bo = new PortfolioBO();

            await bo.AddInvestment(detailViewModel.amountOfStocks, detailViewModel.priceOfStock, detailViewModel.dateOfInvestment, detailViewModel.companyId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddInvestmentDefault(PortfolioViewModel portfolioViewModel)
        {
            var bo = new PortfolioBO();
            var company = bo.GetCompany(portfolioViewModel.companyId);
            await bo.AddInvestment(portfolioViewModel.amountOfStocks, (double)company.StockPrice, DateTime.UtcNow, portfolioViewModel.companyId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddWithdraw(PortfolioViewModel portfolioViewModel)
        {
            var bo = new PortfolioBO();

            await bo.AddWithdraw(portfolioViewModel.amountOfStocks, portfolioViewModel.priceOfStock, portfolioViewModel.dateOfInvestment, portfolioViewModel.companyId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddWithdrawDefault(PortfolioViewModel portfolioViewModel)
        {
            var bo = new PortfolioBO();
            var company = bo.GetCompany(portfolioViewModel.companyId);
            await bo.AddWithdraw(portfolioViewModel.amountOfStocks, (double)company.StockPrice, DateTime.UtcNow, portfolioViewModel.companyId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return RedirectToAction("Index");
        }
    }
}