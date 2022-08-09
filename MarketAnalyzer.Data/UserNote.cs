using System;
using System.Collections.Generic;
using MarketAnalyzer.Data.Interfaces;

namespace MarketAnalyzer.Data
{
    public partial class UserNote : IEntity, ICreatedTime, IUpdatedTime
    {
        public UserNote()
        {
        }
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public virtual Company Company { get; set; }
    }
}
