using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class DataSource : IEntity 
    {
        public DataSource()
        {
            ExtractedBalanceSheets = new HashSet<ExtractedBalanceSheet>();
            ExtractedCashFlowStatementTtms = new HashSet<ExtractedCashFlowStatementTtm>();
            ExtractedCashFlowStatements = new HashSet<ExtractedCashFlowStatement>();
            ExtractedIncomeStatementTtms = new HashSet<ExtractedIncomeStatementTtm>();
            ExtractedIncomeStatements = new HashSet<ExtractedIncomeStatement>();
            ExtractedKeyRatios = new HashSet<ExtractedKeyRatio>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExtractedBalanceSheet> ExtractedBalanceSheets { get; set; }
        public virtual ICollection<ExtractedCashFlowStatementTtm> ExtractedCashFlowStatementTtms { get; set; }
        public virtual ICollection<ExtractedCashFlowStatement> ExtractedCashFlowStatements { get; set; }
        public virtual ICollection<ExtractedIncomeStatementTtm> ExtractedIncomeStatementTtms { get; set; }
        public virtual ICollection<ExtractedIncomeStatement> ExtractedIncomeStatements { get; set; }
        public virtual ICollection<ExtractedKeyRatio> ExtractedKeyRatios { get; set; }
    }
}
