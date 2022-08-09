using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MarketAnalyzer.WebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Data;
using MarketAnalyzer.Business;

namespace MarketAnalyzer.WebApp.Controllers
{
    public class SettingsController : Controller
    {

        [Authorize]
        public IActionResult Index()
        {
            return View(new SettingsViewModel());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangeSettings(SettingsViewModel settingsViewModel)
        {
            var settings = new MultiplierSetting();
            settings.Id = Guid.NewGuid();
            settings.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            settings.RoicMult = settingsViewModel.roicMulti;
            settings.EquityMult = settingsViewModel.equityMulti;
            settings.EpsMult = settingsViewModel.epsMulti;
            settings.RevenueMult = settingsViewModel.revenueMulti;
            settings.PEMult = settingsViewModel.peMulti;
            settings.DebtToEquityMult = settingsViewModel.dToEMulti;
            settings.CurrentAssetsToLiabilitiesMult = settingsViewModel.aToLMulti;

            var business = new UserSettingsBO();
            await business.SetMultiplierSettings(settings);


            return RedirectToAction("Index", "Home", new { roicMult = settings.RoicMult, equityMult = settings.EquityMult, epsMult = settings.EpsMult, revenueMult = settings.RevenueMult, peMult = settings.PEMult, dToEMult = settings.DebtToEquityMult, aToLMult = settings.CurrentAssetsToLiabilitiesMult });


        }
    }
}