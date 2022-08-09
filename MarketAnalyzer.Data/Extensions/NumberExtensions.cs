using System;

namespace MarketAnalyzer.Data.Extensions
{
    public static class NumberExtensions
    {
        public static double Round(this double value)
        {
            return Math.Round(value, 2);
        }

        public static decimal Round(this decimal value)
        {
            return Math.Round(value, 2);
        }

        public static string ToMoney(this decimal value)
        {
            return $"{Round(value)}$";
        }

        public static string ToPerc(this double value)
        {
            return $"{Round(value * 100)}%";
        }

        public static double ToPercNum(this double value)
        {
            return Round(value * 100);
        }

        public static decimal ToPercNum(this decimal value)
        {
            return Round(value * 100);
        }
    }
}