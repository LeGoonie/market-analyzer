namespace MarketAnalyzer.Analysis
{
    public class ValueInfo
    {
        public int StartYear { get; set; }
        public int NextYear { get; set; }
        public int EndYear { get; set; }

        public int NumYears { get { return EndYear - StartYear; } }
        public double Cagr { get; set; }
        public double GrowthToNextYear { get; set; }

        public double StartValue { get; set; }
        public double EndCagrValue { get; set; }
        public double NextValue { get; set; }
    }
}