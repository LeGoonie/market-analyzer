using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.WebApp.Models
{
    public class SettingsViewModel
    {
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double roicMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double equityMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double epsMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double revenueMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double peMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double dToEMulti { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid doubleNumber")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "must be numeric")]
        public double aToLMulti { get; set; }
    }
}
