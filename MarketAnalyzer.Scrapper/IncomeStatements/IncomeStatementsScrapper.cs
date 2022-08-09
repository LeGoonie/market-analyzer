using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.IncomeStatements
{
    public class IncomeStatementsScrapper : BaseScrapper
    {
        public async Task<List<ExtractedIncomeStatement>> GetIncomeStatements(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedIncomeStatement, IncomeStatementParser>(requestToken, ticker, "is");
        }

        public async Task<List<ExtractedIncomeStatementTtm>> GetIncomeStatementTTM(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedIncomeStatementTtm, IncomeStatementTTMParser>(requestToken, ticker, "is");
        }
    }
}