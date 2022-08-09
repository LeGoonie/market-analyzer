using MarketAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketAnalyzer.DataAccess
{
    public class JoinQueries
    {
        public MarketAnalyzerDB2Context CreateContext()
        {
            return new MarketAnalyzerDB2Context();
        }

        public IEnumerable<HistoricalStockRecord> GetHistoricalStockRecords(Guid companyId)
        {
            using (var context = CreateContext())
            {
                return context.HistoricalStockRecords.Where(h => h.CompanyId == companyId).ToList();
            }
        }

    }
}
