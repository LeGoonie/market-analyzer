using System;
using System.Collections.Generic;
using System.Linq;
using MarketAnalyzer.Analysis;
using MarketAnalyzer.Business;
using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Pocos;
using MarketAnalyzer.DataAccess;

namespace MarketAnalyzer.Algorithm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //MultidimensionalGraphApp multidimensionalGraphApp = new MultidimensionalGraphApp();
            //multidimensionalGraphApp.Run();
            GradientDescentApp gradientDescentApp = new GradientDescentApp();
            gradientDescentApp.Run();
        }
    }
}
