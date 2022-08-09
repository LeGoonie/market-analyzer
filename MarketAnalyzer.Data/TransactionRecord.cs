using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class TransactionRecord : IEntity 
    {
        public Guid Id { get; set; }
        public Guid PortfolioId { get; set; }
        public string Type { get; set; }
        public double AmountOfStocks { get; set; }
        public decimal? DollarAtTimeOfTransaction { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public virtual Portfolio Portfolio { get; set; }
    }
}
