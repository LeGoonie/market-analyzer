using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class BalanceSheet : IEntity , ICreatedTime, IUpdatedTime, IFInfo
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public decimal? CashEquivalents { get; set; }
        public decimal? ShortTermInvestments { get; set; }
        public decimal? AccountsReceivable { get; set; }
        public decimal? OtherCurrentAssets { get; set; }
        public decimal? Investments { get; set; }
        public decimal? PropertyPlanEquipmentNet { get; set; }
        public decimal? Goodwill { get; set; }
        public decimal? OtherIntangibleAssets { get; set; }
        public decimal? OtherAssets { get; set; }
        public decimal? AccountsPayable { get; set; }
        public decimal? TaxPayable { get; set; }
        public decimal? AccruedLiabilities { get; set; }
        public decimal? ShortTermDebt { get; set; }
        public decimal? DefferedRevenue { get; set; }
        public decimal? OtherCurrentLiabilities { get; set; }
        public decimal? LongTermDebt { get; set; }
        public decimal? CapitalLeases { get; set; }
        public decimal? DeferredRevenue { get; set; }
        public decimal? OtherLiabilities { get; set; }
        public decimal? RetainedEarnings { get; set; }
        public decimal? PaidInCapital { get; set; }
        public decimal? CommonStock { get; set; }
        public decimal? Aoci { get; set; }
        public decimal? Other { get; set; }
        public decimal? ShareholdersEquity { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
    }
}
