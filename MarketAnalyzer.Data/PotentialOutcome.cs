using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class PotentialOutcome : IEntity 
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public double? ThreeYearMinOutcome { get; set; }
        public double? ThreeYearMaxOutcome { get; set; }
        public double? FiveYearMinOutcome { get; set; }
        public double? FiveYearMaxOutcome { get; set; }
        public double? TenYearMinOutcome { get; set; }
        public double? TenYearMaxOutcome { get; set; }

        public virtual Company Company { get; set; }
    }
}
