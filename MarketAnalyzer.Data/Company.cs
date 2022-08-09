using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class Company : IEntity , ICreatedTime, IUpdatedTime
    {
        public Company()
        {
            BalanceSheets = new HashSet<BalanceSheet>();
            CashFlowStatements = new HashSet<CashFlowStatement>();
            ExtractedBalanceSheets = new HashSet<ExtractedBalanceSheet>();
            ExtractedCashFlowStatementTtms = new HashSet<ExtractedCashFlowStatementTtm>();
            ExtractedCashFlowStatements = new HashSet<ExtractedCashFlowStatement>();
            ExtractedIncomeStatementTtms = new HashSet<ExtractedIncomeStatementTtm>();
            ExtractedIncomeStatements = new HashSet<ExtractedIncomeStatement>();
            ExtractedKeyRatios = new HashSet<ExtractedKeyRatio>();
            HistoricalStockRecords = new HashSet<HistoricalStockRecord>();
            IncomeStatements = new HashSet<IncomeStatement>();
            KeyRatios = new HashSet<KeyRatio>();
            Portfolios = new HashSet<Portfolio>();
            UserNotes = new HashSet<UserNote>();
            PotentialOutcomes = new HashSet<PotentialOutcome>();
            QuickFsJsonData = new HashSet<QuickFsJsonDatum>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public decimal StockPrice { get; set; }
        public string Country { get; set; }
        public bool? BuyR { get; set; }
        public bool? BuyL { get; set; }
        public string Notes { get; set; }
        public int? SandPrank { get; set; }
        public int? Forbes2000Rank { get; set; }
        public Guid? IndustryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public double? CapsRating { get; set; }
        public double? OutPerformRatio { get; set; }
        public double? AllStarRatio { get; set; }

        public virtual Industry Industry { get; set; }
        public virtual ICollection<BalanceSheet> BalanceSheets { get; set; }
        public virtual ICollection<CashFlowStatement> CashFlowStatements { get; set; }
        public virtual ICollection<ExtractedBalanceSheet> ExtractedBalanceSheets { get; set; }
        public virtual ICollection<ExtractedCashFlowStatementTtm> ExtractedCashFlowStatementTtms { get; set; }
        public virtual ICollection<ExtractedCashFlowStatement> ExtractedCashFlowStatements { get; set; }
        public virtual ICollection<ExtractedIncomeStatementTtm> ExtractedIncomeStatementTtms { get; set; }
        public virtual ICollection<ExtractedIncomeStatement> ExtractedIncomeStatements { get; set; }
        public virtual ICollection<ExtractedKeyRatio> ExtractedKeyRatios { get; set; }
        public virtual ICollection<HistoricalStockRecord> HistoricalStockRecords { get; set; }
        public virtual ICollection<IncomeStatement> IncomeStatements { get; set; }
        public virtual ICollection<KeyRatio> KeyRatios { get; set; }
        public virtual ICollection<Portfolio> Portfolios { get; set; }
        public virtual ICollection<UserNote> UserNotes { get; set; }
        public virtual ICollection<PotentialOutcome> PotentialOutcomes { get; set; }
        public virtual ICollection<QuickFsJsonDatum> QuickFsJsonData { get; set; }
    }
}
