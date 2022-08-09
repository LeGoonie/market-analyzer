using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class HistoricalStockRecord : IEntity 
    {
        public Guid Id { get; set; }
        public string Timeframe { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public double Volume { get; set; }
        public DateTime StartDate { get; set; }
        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
