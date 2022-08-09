using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketAnalyzer.Data.Pocos
{
    public class Multiplier
    {

        public Multiplier(IList<double> values)
        {
            if (values.Count() < 7) return;
            roicMulti = values[0];
            equityMulti = values[1];
            epsMulti = values[2];
            revenueMulti = values[3];
            peMulti = values[4];
            dToEMulti = values[5];
            aToLMulti = values[6];

        }

        public Multiplier(double roicMulti, double equityMulti, double epsMulti, double revenueMulti, double peMulti, double dToEMulti, double aToLMulti)
        {
            this.roicMulti = roicMulti;
            this.equityMulti = equityMulti;
            this.epsMulti = epsMulti;
            this.revenueMulti = revenueMulti;
            this.peMulti = peMulti;
            this.dToEMulti = dToEMulti;
            this.aToLMulti = aToLMulti;
        }

        public double roicMulti { get; set; }

        public double equityMulti { get; set; }
        public double epsMulti { get; set; }

        public double revenueMulti { get; set; }

        public double peMulti { get; set; }

        public double dToEMulti { get; set; }

        public double aToLMulti { get; set; }


    }
}
