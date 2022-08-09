using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.BalanceSheets
{
    public class BalanceSheetsScrapper : BaseScrapper
    {
        public async Task<List<ExtractedBalanceSheet>> GetBalanceSheets(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedBalanceSheet, BalanceSheetParser>(requestToken, ticker, "bs");
        }
    }
}