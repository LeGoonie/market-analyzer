using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.KeyRatios
{
    public class KeyRatiosScrapper : BaseScrapper
    {
        public async Task<List<ExtractedKeyRatio>> GetKeyRatios(string requestToken, string ticker)
        {
            return await GetFinancialInfo<ExtractedKeyRatio, KeyRatioParser>(requestToken, ticker, "ratios");
        }
    }
}