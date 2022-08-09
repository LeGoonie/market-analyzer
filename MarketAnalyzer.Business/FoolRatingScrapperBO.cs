using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using MarketAnalyzer.Data.Interfaces;
using MarketAnalyzer.Scrapper.Fool;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class FoolRatingScrapperBO
    {
        

        public async Task ScrapeDataAndUpdate()
        {
            GenericDao<Company> genericDao = new GenericDao<Company>();

            var companies = await genericDao.GetAllAsync();

            List<Company> updatedCompanies = new List<Company>();

            foreach (var company in companies)
            {
                var foolRating = await GetFoolRating(company.Ticker);
                        
                if(foolRating != null) {
                    company.DateUpdated = DateTime.UtcNow;
                    company.CapsRating = foolRating.CapsRating;
                    company.OutPerformRatio = foolRating.OutPerformRatio;
                    company.AllStarRatio = foolRating.AllStarRatio;
                    updatedCompanies.Add(company);
                } else
                {
                    continue;
                }
            }


            var changesMade = await genericDao.UpdateRangeAsync(updatedCompanies);

        }
                

        /*private void UpdateFoolRating(FoolRating foolRating, Company company, MarketAnalyzerDBContext context) {
            company.CapsRating = foolRating.CapsRating;
            company.OutPerformRatio = foolRating.OutPerformRatio;
            company.AllStarRatio = foolRating.AllStarRatio;

            int batchSize = 20;

            for (int i = 0; i < companies.Count; i += batchSize)
            {
                context.UpdateRange(companies.Skip(i).Take(batchSize));
                context.SaveChanges();
            }
        }*/

        private async Task<FoolRating> GetFoolRating(string ticker)
        {
            var scrapper = new FoolRatingsScrapper();
            return (await scrapper.GetFoolRating($"{ticker}"));
        }

    }
}
