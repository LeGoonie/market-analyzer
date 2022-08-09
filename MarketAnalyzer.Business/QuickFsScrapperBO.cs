using MarketAnalyzer.Data;
using MarketAnalyzer.DataAccess;
using MarketAnalyzer.Data.Interfaces;
using MarketAnalyzer.Scrapper.BalanceSheets;
using MarketAnalyzer.Scrapper.CashFlows;
using MarketAnalyzer.Scrapper.IncomeStatements;
using MarketAnalyzer.Scrapper.KeyRatios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class QuickFsScrapperBO
    {
        public async Task ScrapeDataAndAddOrUpdate()
        {
            GenericDao<Company> genericDao = new GenericDao<Company>();
            GenericDao<DataSource> genericDaoDataSource = new GenericDao<DataSource>();
            ////var login = await requestHelper.GetLogin("grL0gNYoMoLUB1ZoAKLfhXkoMoLODiO1WoL9.grLtk3PoMoLmqFEsMasbNK9fkXudkNBtR2jpkr5dINZoAKLtRNZoMlG1MQZ3PJD3PJRcOpEfqXGoMwcoqNWaka9tIKO6OlD2PlOoAKLahSVthKO6Olqph3BfIGHskp92INqfyNPdgFa6OosokXVik3qbkpZoMoO3Ip91RSEYkFLbqpB3RSaiAwf5goOcOwHryNIthXBwICO6PKsokpBwyS9dDFLtqoO6grLBDrO6PCsoZ0GoMlH9vN0.aQcaHtidvlDwLfMuUy9ggmqJCbGeQYAZTAB5HnONrCG");
            string tempLogin = "grL0gNYoMoLUB1ZoAKLfhXkoMoLODiO1WoL9.grLtk3PoMoLmqFEsMasbNK9fkXudkNBtR2jpkr5dINZoAKLtRNZoMlG1MQZ3PJk5MJPcOpEfqXGoMwcoqNWaka9tIKO6OlD2PlOoAKLahSVthKO6Olqph3BfIGHskp92INqfyNPdgFa6OosokXVik3qbkpZoMoO3Ip91RSEYkFLbqpB3RSaiAwf5goOcOwHryNIthXBwICO6PKsokpBwyS9dDFLtqoO6grLBDrO6PCsoZ0GoMlH9vN0.u16E-Z3bS-Yhjd3RjLm4yl0XCZuIx7lgDmnsK4Qv2w8";
            string rpintoLogin = "grL0gNYoMoLUB1ZoAKLfhXkoMoLODiO1WoL9.grLtk3PoMoLmqFEsMasbNK9fkXudkNBtR2jpkr5dINZoAKLtRNZoMlG1MJR4PQY1MQYcOpEfqXGoMwcoqNWaka9tIKO6OlGnWJY4OosoIS1fySsoMoLokwBdhr5wh29dySBYyX90hSVthK5lh20oAKLsRNWiq29rIKO6OwV3INL0qXfrh3koAKLskpa2ySnaI2DoMlYcOwLaI2abhaHryNRoMwcoBBPoMlGcOuWHOlmsvN19.t5R0sOanWJXj-PXJkjSnUhXL0feEEwkKQrkt99wzoM0";

            var companies = await genericDao.GetAllAsync();

            var datasource = await genericDaoDataSource.GetSingleByAsync(d => d.Name == "quickfs");

                foreach (var company in companies)
                {
                    try
                    {
                        company.DateUpdated = DateTime.UtcNow;

                        var incomeStatements = await GetIncomeStatements(tempLogin, company.Ticker);
                        AddOrUpdateEntites(incomeStatements, company, datasource);

                        var incomeStatementTtm = await GetIncomeStatementTtm(tempLogin, company.Ticker);
                        await AddOrUpdateIncomeStatementTtm(incomeStatementTtm, company, datasource);

                        var balanceSheets = await GetBalanceSheets(tempLogin, company.Ticker);
                        AddOrUpdateEntites(balanceSheets, company, datasource);

                        var cashFlows = await GetCashFlows(tempLogin, company.Ticker);
                        AddOrUpdateEntites(cashFlows, company, datasource);

                        var cashFlowTtm = await GetCashFlowTtm(tempLogin, company.Ticker);
                        await AddOrUpdateCashFlowTtm(cashFlowTtm, company, datasource);

                        var keyRatios = await GetKeyRatios(tempLogin, company.Ticker);
                        AddOrUpdateEntites(keyRatios, company, datasource);

                        await genericDao.UpdateAsync(company);

                        Console.WriteLine($"Scrapped {company.Ticker} index {companies.IndexOf(company)}");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            
        }

        private async Task AddOrUpdateIncomeStatementTtm(ExtractedIncomeStatementTtm isttm, Company company, DataSource dataSource)
        {
            isttm.CompanyId = company.Id;
            isttm.DataSourceId = dataSource.Id;
            isttm.Id = Guid.NewGuid();

            var genericDao = new GenericDao<ExtractedIncomeStatementTtm>();


            var existing = await genericDao.GetSingleByAsync(c => c.CompanyId == isttm.CompanyId);
            if (existing == null)
            {
                await genericDao.AddAsync(isttm);
            }
            else
            {
                Map(isttm, existing);
                await genericDao.UpdateAsync(existing);
            }
        }

        private async Task AddOrUpdateCashFlowTtm(ExtractedCashFlowStatementTtm cfttm, Company company, DataSource dataSource)
        {
            cfttm.CompanyId = company.Id;
            cfttm.DataSourceId = dataSource.Id;
            cfttm.Id = Guid.NewGuid();

            var genericDao = new GenericDao<ExtractedCashFlowStatementTtm>();

            var existing = await genericDao.GetSingleByAsync(c => c.CompanyId == cfttm.CompanyId);
            if (existing == null)
            {
                await genericDao.AddAsync(cfttm);
            }
            else
            {
                Map(cfttm, existing);
                await genericDao.UpdateAsync(existing);
            }
        }

        private void AddOrUpdateEntites<TEntity>(IEnumerable<TEntity> entities, Company company, DataSource dataSource) where TEntity : class, IFInfo, IFScrappedInfo, IEntity
        {
            var context = new MarketAnalyzerDB2Context();
            entities = SetCompanyIdAndDataSource(entities, company.Id, dataSource.Id);

            if (!AreYearsOrderedCorrectly(entities))
            {
                Console.WriteLine("Years not ordered correctly");
                return;
            }

            var toAdd = GetFInfosToAdd(entities, company.Id, context);
            var toUpdate = GetFInfosToUpdate(entities, company.Id, context);
            Update(entities, toUpdate);
            context.Set<TEntity>().AddRange(toAdd);

            foreach (var item in toUpdate) context.Entry<TEntity>(item).State = EntityState.Modified;
        }

        private bool AreYearsOrderedCorrectly<T>(IEnumerable<T> fInfos) where T : IFInfo
        {
            if (fInfos == null || fInfos.Count() == 0) return true;
            var orderedFInfos = fInfos.OrderBy(f => f.Year).ToList();
            var year = orderedFInfos.First().Year;

            for (int i = 1; i < orderedFInfos.Count(); i++)
            {
                var currYear = orderedFInfos[i].Year;
                if (currYear != year + 1) return false;
                year = currYear;
            }
            return true;
        }

        private void Update<T>(IEnumerable<T> source, IEnumerable<T> dest) where T : IFInfo
        {
            foreach (var itemSrc in source)
            {
                var itemDest = dest.SingleOrDefault(i => i.Year == itemSrc.Year && i.CompanyId == itemSrc.CompanyId);
                if (itemDest != null) Map(itemSrc, itemDest);
            }
        }

        private void Map(object source, object target)
        {
            var propertiesSource = source.GetType().GetProperties();
            var propertiesTarget = target.GetType().GetProperties();

            foreach (var propSource in propertiesSource)
            {
                if (propSource.Name == "Id") continue;
                if (propSource.Name == "Company") continue;
                if (propSource.Name == "DataSource") continue;
                var propTarget = propertiesTarget.SingleOrDefault(p => p.Name == propSource.Name && p.PropertyType == propSource.PropertyType);
                if (propTarget != null) propTarget.SetValue(target, propSource.GetValue(source));
            }
        }

        private IEnumerable<T> GetFInfosToAdd<T>(IEnumerable<T> fInfos, Guid companyId, MarketAnalyzerDB2Context context) where T : class, IFScrappedInfo, IFInfo
        {

            var allItems = context.Set<T>().Where(c => c.CompanyId == companyId).ToList();

            var existing = (from fInfo in fInfos
                            from item in allItems
                            where fInfo.Year == item.Year && fInfo.CompanyId == item.CompanyId
                            select fInfo).ToList();

            //var existing = context.Set<T>().Where(i => fInfos.Where(f => f.Year == i.Year && f.CompanyId == i.CompanyId && f.DataSourceId == i.DataSourceId).Any());

            return fInfos.Where(i => !existing.Where(f => f.Year == i.Year && f.CompanyId == i.CompanyId).Any()).ToList();
        }

        private IEnumerable<T> GetFInfosToUpdate<T>(IEnumerable<T> fInfos, Guid companyId, MarketAnalyzerDB2Context context) where T : class, IFScrappedInfo, IFInfo
        {
            var allItems = context.Set<T>().Where(c => c.CompanyId == companyId).ToList();

            return (from fInfo in fInfos
                    from item in allItems
                    where fInfo.Year == item.Year && fInfo.CompanyId == item.CompanyId && AreFInfosDifferent(fInfo, item)
                    select item).ToList();
            //return context.Set<T>().Where(i => fInfos.Where(f => f.Year == i.Year && f.CompanyId == i.CompanyId && f.DataSourceId == i.DataSourceId).Any()).ToList();
        }

        private bool AreFInfosDifferent<T>(T fInfo1, T fInfo2) where T : class, IFScrappedInfo, IFInfo
        {
            var properties1 = fInfo1.GetType().GetProperties();
            var properties2 = fInfo2.GetType().GetProperties();
            var diffCounter = 0;
            var nullCounter = 0;

            var areDifferent = false;

            foreach (var propSource in properties1)
            {
                if (propSource.Name == "Id") continue;
                if (propSource.Name == "Company") continue;
                if (propSource.Name == "DataSource") continue;
                if (propSource.Name == "DateCreated") continue;
                if (propSource.Name == "DateUpdated") continue;
                var propTarget = properties2.SingleOrDefault(p => p.Name == propSource.Name && p.PropertyType == propSource.PropertyType);
                if (propTarget == null)
                {
                    areDifferent = true;
                    diffCounter++;
                }
                var propSourceValue = propSource.GetValue(fInfo1);
                var propTargetValue = propTarget.GetValue(fInfo2);

                if (!AreValuesEqual(propSourceValue, propTargetValue))
                {
                    areDifferent = true;
                    diffCounter++;
                    if (propSourceValue == null) nullCounter++;
                }
            }

            if (diffCounter > properties1.Count() / 3)
            {
                Console.WriteLine($"Scrapping may be wrong. Found {diffCounter} from {properties1.Count()} values different");
            }
            if (nullCounter > properties1.Count() / 3)
            {
                //there may be a problem with scrapping
                Console.WriteLine($"Scrapping may be really wrong. Found {diffCounter} from {properties1.Count()} values null");
            }
            return areDifferent;
        }

        private bool AreValuesEqual(object obj1, object obj2)
        {
            if (obj1 == obj2) return true;
            if (obj1 == null && obj2 != null || obj1 != null && obj2 == null) return false;
            if (obj1.GetType() != obj2.GetType()) return false;

            var type = obj1.GetType();

            if (type == typeof(decimal))
                return (decimal)obj1 == (decimal)obj2;
            if (type == typeof(decimal?))
                return (decimal?)obj1 == (decimal?)obj2;
            if (type == typeof(double))
                return (double)obj1 == (double)obj2;
            if (type == typeof(double?))
                return (double?)obj1 == (double?)obj2;

            return obj1.ToString() == obj2.ToString();
        }

        private IEnumerable<T> SetCompanyIdAndDataSource<T>(IEnumerable<T> fInfos, Guid companyId, Guid datasourceId) where T : IFInfo, IFScrappedInfo
        {
            foreach (var ic in fInfos)
            {
                ic.CompanyId = companyId;
                ic.Id = Guid.NewGuid();
                ic.DataSourceId = datasourceId;
            }

            return fInfos;
        }

        private async Task<IEnumerable<ExtractedIncomeStatement>> GetIncomeStatements(string login, string ticker)
        {
            var scrapper = new IncomeStatementsScrapper();
            return await scrapper.GetIncomeStatements(login, $"{ticker}:US");
        }

        private async Task<ExtractedIncomeStatementTtm> GetIncomeStatementTtm(string login, string ticker)
        {
            var scrapper = new IncomeStatementsScrapper();
            return (await scrapper.GetIncomeStatementTTM(login, $"{ticker}:US")).Single();
        }

        private async Task<IEnumerable<ExtractedBalanceSheet>> GetBalanceSheets(string login, string ticker)
        {
            var scrapper = new BalanceSheetsScrapper();
            return await scrapper.GetBalanceSheets(login, $"{ticker}:US");
        }

        private async Task<IEnumerable<ExtractedCashFlowStatement>> GetCashFlows(string login, string ticker)
        {
            var scrapper = new CashFlowsScrapper();
            return await scrapper.GetCashFlows(login, $"{ticker}:US");
        }

        private async Task<ExtractedCashFlowStatementTtm> GetCashFlowTtm(string login, string ticker)
        {
            var scrapper = new CashFlowsScrapper();
            return (await scrapper.GetCashFlowTT(login, $"{ticker}:US")).Single();
        }

        private async Task<IEnumerable<ExtractedKeyRatio>> GetKeyRatios(string login, string ticker)
        {
            var scrapper = new KeyRatiosScrapper();
            return await scrapper.GetKeyRatios(login, $"{ticker}:US");
        }
    }
}