using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAnalyzer.Data.Pocos
{
    public class PortfolioAssistant
    {
        public Portfolio Portfolio { get; set; }

        public Company Company { get; set; }

        public decimal? TotalValue { get; set; }

        public decimal? TotalInvested { get; set; }

        public decimal? TotalWithdrawed { get; set; }

        public double? TotalStocks { get; set; }

        public decimal? TotalGainLoss { get; set; }

        public double? TotalGainLossPercentage { get; set; }

        public DateTime DateOfLastInvestment { get; set; }

        public IEnumerable<TransactionRecord> TransactionRecords { get; set; }
    }
}
