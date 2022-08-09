using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.CashFlows
{
    public class CashFlowsScrapper : BaseScrapper
    {
        public async Task<List<ExtractedCashFlowStatement>> GetCashFlows(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedCashFlowStatement, CashFlowParser>(requestToken, ticker, "cf");
        }

        public async Task<List<ExtractedCashFlowStatementTtm>> GetCashFlowTT(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedCashFlowStatementTtm, CashFlowTTMParser>(requestToken, ticker, "cf");
        }
    }
}