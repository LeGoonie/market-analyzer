using System;

namespace MarketAnalyzer.Data.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}