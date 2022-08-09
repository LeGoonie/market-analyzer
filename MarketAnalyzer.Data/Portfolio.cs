using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class Portfolio : IEntity , ICreatedTime, IUpdatedTime
    {
        public Portfolio()
        {
            TransactionRecords = new HashSet<TransactionRecord>();
        }

        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
        public DateTime LastInvestment { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<TransactionRecord> TransactionRecords { get; set; }
    }
}
