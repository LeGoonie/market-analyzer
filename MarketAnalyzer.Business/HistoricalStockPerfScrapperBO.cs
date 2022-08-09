using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Historical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Business
{
    public class HistoricalStockPerfScrapperBO
    {
        public async Task ScrapeDataAndAddOrUpdate()
        {
            List<Company> companies = null;

            using (var context = new MarketAnalyzerDB2Context())
            {
                companies = (from c in context.Companies
                             where !(from h in context.HistoricalStockRecords
                                     where h.CompanyId == c.Id
                                     select h.CompanyId).Contains(c.Id)
                             select c).ToList();
            }

            var scrapper = new HistoricalStockScrapper();

            foreach (var company in companies)
            {
                try
                {
                    using (var context = new MarketAnalyzerDB2Context())
                    {
                        var hist = context.HistoricalStockRecords.Where(r => r.CompanyId == company.Id).ToList();
                        if (hist.Count() > 0)
                        {
                            Console.WriteLine($"Already scrapped ticker {company.Ticker} index {companies.IndexOf(company)}");
                            continue;
                        }
                    }

                    var results = await scrapper.GetHistoricalData(company.Id, company.Ticker);
                    if (results.Count() == 0)
                    {
                        Console.WriteLine($"No results ticker {company.Ticker} index {companies.IndexOf(company)}");
                        Thread.Sleep(TimeSpan.FromSeconds(12));
                        continue;
                    }
                    AddAndUpdateIfNotExists(company.Id, results);
                    Console.WriteLine($"Added ticker {company.Ticker} index {companies.IndexOf(company)}");
                    Thread.Sleep(TimeSpan.FromSeconds(12));
                }
                catch (Exception ex)
                {
                    int a = 0;
                }
            }
        }

        private void AddAndUpdateIfNotExists(Guid companyId, IEnumerable<HistoricalStockRecord> records)
        {
            using (var context = new MarketAnalyzerDB2Context())
            {
                var existing = context.HistoricalStockRecords.Where(r => r.CompanyId == companyId).ToList();

                var recordsToAdd = records.Where(r => !existing.Select(e => e.StartDate).Contains(r.StartDate));
                var existingToCheckToUpdate = records.Where(r => existing.Select(e => e.StartDate).Contains(r.StartDate));

                var recordsToUpdate = FilterAndUpdateRecordsAsNeeded(records, existingToCheckToUpdate);

                context.HistoricalStockRecords.AddRange(recordsToAdd);
                context.HistoricalStockRecords.UpdateRange(recordsToUpdate);
                context.SaveChanges();
            }
        }

        private IEnumerable<HistoricalStockRecord> FilterAndUpdateRecordsAsNeeded(IEnumerable<HistoricalStockRecord> records, IEnumerable<HistoricalStockRecord> existingRecords)
        {
            var recordsToUpdate = new List<HistoricalStockRecord>();
            foreach (var record in records)
            {
                var existing = existingRecords.SingleOrDefault(e => e.StartDate == record.StartDate);
                if (existing != null && !AreRecordsEqual(record, existing))
                {
                    existing.Low = record.Low;
                    existing.High = record.High;
                    existing.Open = record.Open;
                    existing.Close = record.Close;
                    existing.Volume = record.Volume;
                    recordsToUpdate.Add(existing);
                }
            }
            return recordsToUpdate;
        }

        private bool AreRecordsEqual(HistoricalStockRecord record1, HistoricalStockRecord record2)
        {
            if (record1.Low != record2.Low
                || record1.High != record2.High
                || record1.Open != record2.Open
                || record1.Close != record2.Close
                || record1.Volume != record2.Volume)
                return false;
            return true;
        }
    }
}