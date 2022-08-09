using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class CashFlowStatement : IEntity , ICreatedTime, IUpdatedTime, IFInfo
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public decimal? DepreciationAmortization { get; set; }
        public decimal? ChangeInWorkCapital { get; set; }
        public decimal? ChangeInDeferredTax { get; set; }
        public decimal? StockBasedCompensation { get; set; }
        public decimal? OperationsOther { get; set; }
        public decimal? PropertyPlanEquipment { get; set; }
        public decimal? Acquisitions { get; set; }
        public decimal? Investments { get; set; }
        public decimal? InvestingOther { get; set; }
        public decimal? IssuanceCommonStockNet { get; set; }
        public decimal? IssuancePreferredStockNet { get; set; }
        public decimal? IssuanceDebtNet { get; set; }
        public decimal? Other { get; set; }
        public decimal? CashFromFinancing { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
    }
}
