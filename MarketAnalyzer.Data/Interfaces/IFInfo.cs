using System;

namespace MarketAnalyzer.Data.Interfaces
{
    public interface IFInfo
    {
        Guid Id { get; set; }
        int Year { get; set; }

        Guid CompanyId { get; set; }
    }
}