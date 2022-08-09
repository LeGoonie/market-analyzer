using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class MultiplierSetting : IEntity 
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public double? RoicMult { get; set; }
        public double? EquityMult { get; set; }
        public double? EpsMult { get; set; }
        public double? RevenueMult { get; set; }
        public double? PEMult { get; set; }
        public double? DebtToEquityMult { get; set; }
        public double? CurrentAssetsToLiabilitiesMult { get; set; }
    }
}
