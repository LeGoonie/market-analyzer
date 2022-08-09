using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class Industry : IEntity 
    {
        public Industry()
        {
            Companies = new HashSet<Company>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
    }
}
