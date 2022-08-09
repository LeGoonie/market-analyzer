using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class ExtractedKeyRatio : IEntity , ICreatedTime, IUpdatedTime, IFInfo, IFScrappedInfo
    {
        public Guid Id { get; set; }
        public Guid DataSourceId { get; set; }
        public int Year { get; set; }
        public double? ReturnOnAssets { get; set; }
        public double? ReturnOnEquity { get; set; }
        public double? ReturnOnInvestedCapital { get; set; }
        public double? ReturnOnCapitalEmployed { get; set; }
        public double? ReturnOnTangibleCapitalEmployed { get; set; }
        public double? GrossMargin { get; set; }
        public double? EbitdaMargin { get; set; }
        public double? OperatingMargin { get; set; }
        public double? PretaxMargin { get; set; }
        public double? NetMargin { get; set; }
        public double? FreeCashMargin { get; set; }
        public double? AssetsToEquity { get; set; }
        public double? EquityToAssets { get; set; }
        public double? DebtToEquity { get; set; }
        public double? DebtToAssets { get; set; }
        public decimal? FreeCashFlow { get; set; }
        public decimal? BookValue { get; set; }
        public decimal? TangibleBookValue { get; set; }
        public decimal? MarketCapitalization { get; set; }
        public double? PriceToEarnings { get; set; }
        public double? PriceToBook { get; set; }
        public double? PriceToSales { get; set; }
        public double? DividendsPerShare { get; set; }
        public double? PayoutRatio { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
        public virtual DataSource DataSource { get; set; }
    }
}
