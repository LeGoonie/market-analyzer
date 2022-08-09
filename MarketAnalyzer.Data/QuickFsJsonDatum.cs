using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class QuickFsJsonDatum : IEntity , ICreatedTime, IUpdatedTime
    {
        public Guid Id { get; set; }
        public string IncomeStatements { get; set; }
        public string BalanceSheets { get; set; }
        public string CashFlows { get; set; }
        public string KeyRatios { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
    }
}
