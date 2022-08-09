using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class ExtractedIncomeStatementTtm : IEntity , ICreatedTime, IUpdatedTime, IFScrappedInfo
    {
        public Guid Id { get; set; }
        public Guid DataSourceId { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? CostOfGoodsSold { get; set; }
        public decimal? GrossProfit { get; set; }
        public decimal? Sales { get; set; }
        public decimal? Rd { get; set; }
        public decimal? SpecialCharges { get; set; }
        public decimal? OtherExpenses { get; set; }
        public decimal? TotalOperatingExpenses { get; set; }
        public decimal? OperatingProfit { get; set; }
        public decimal? NetInterestIncome { get; set; }
        public decimal? OtherNonOperatingIncome { get; set; }
        public decimal? PreTaxIncome { get; set; }
        public decimal? IncomeTax { get; set; }
        public decimal? NetIncome { get; set; }
        public decimal? EpsBasic { get; set; }
        public decimal? EpsDiluted { get; set; }
        public decimal? SharesBasic { get; set; }
        public decimal? SharesDiluted { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
        public virtual DataSource DataSource { get; set; }
    }
}
