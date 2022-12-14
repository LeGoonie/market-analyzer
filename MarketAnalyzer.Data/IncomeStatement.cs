using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class IncomeStatement : IEntity , ICreatedTime, IUpdatedTime, IFInfo
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? CostOfGoodsSold { get; set; }
        public decimal? Sales { get; set; }
        public decimal? Rd { get; set; }
        public decimal? SpecialCharges { get; set; }
        public decimal? OtherExpenses { get; set; }
        public decimal? NetInterestIncome { get; set; }
        public decimal? OtherNonOperatingIncome { get; set; }
        public decimal? IncomeTax { get; set; }
        public decimal? EpsBasic { get; set; }
        public decimal? EpsDiluted { get; set; }
        public decimal? SharesBasic { get; set; }
        public decimal? SharesDiluted { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
    }
}
