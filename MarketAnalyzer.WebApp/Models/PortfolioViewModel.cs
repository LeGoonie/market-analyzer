using MarketAnalyzer.Data.Pocos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.WebApp.Models
{
    public class PortfolioViewModel
    {
        public IEnumerable<PortfolioAssistant> ListOfPortfolios { get; set; }
        public decimal? TotalInvested { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalGainLoss { get; set; }

        public decimal? TotalWithdrawed { get; set; }
        public double TotalGainLossPercentage { get; set; }

        public List<(double,double, int)> TotalInvestedYearValue { get; set; }

        [HiddenInput]
        public Guid companyId { get; set; }

        [DataType(DataType.Date)]
        public DateTime dateOfInvestment { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double amountOfStocks { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double priceOfStock { get; set; }

    }
}
