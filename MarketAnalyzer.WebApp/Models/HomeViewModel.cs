using MarketAnalyzer.Analysis;
using System.Collections.Generic;

namespace MarketAnalyzer.WebApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<StockAnalysis> StocksAnalysis { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public double RoicMult { get; set; }
        public double EquityMult { get; set; }
        public double EpsMult { get; set; }
        public double RevenueMult { get; set; }
        public double PEMult { get; set; }
        public double DebtToEquityMult { get; set; }
        public double CurrentAssetsToLiabilitiesMult { get; set; }

        public bool FilterOnlyFairStocks { get; set; }
        public bool FilterOnlyBuyStocks { get; set; }
    }
}