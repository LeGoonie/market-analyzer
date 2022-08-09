using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarketAnalyzer.Algorithm
{

    public class GradientDescent
    {
        Dictionary<(double x, double y, double w, double p, double r, double a, double b), decimal?> multiDimensionalGraph;
        int graphCount;
        public GradientDescent()
        {
            ReadCsv("dictionary0.7-OnlyNetProfit5Years");
            graphCount = multiDimensionalGraph.Count;
        }

        

        string line;

        public void ReadCsv(string file)
        {
            FileStream aFile = new FileStream("../" + file + ".csv", FileMode.Open);
            StreamReader sr = new StreamReader(aFile);

            string[] row = new string[8];
            multiDimensionalGraph = new Dictionary<(double x, double y, double w, double p, double r, double a, double b), decimal?>();
            int counter = 0;
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(';');
                multiDimensionalGraph.Add((double.Parse(row[0]), double.Parse(row[1]), double.Parse(row[2]), double.Parse(row[3]), double.Parse(row[4]), double.Parse(row[5]), double.Parse(row[6])), -decimal.Parse(row[7]));
                counter++;
            }
            sr.Close();
        }

        public KeyValuePair<(double, double, double, double, double, double, double), decimal?> GradientDescentNoLearning(double maxDouble)
        {
            var i = 0;
            Random rand = new Random();
            var allMinValues = new Dictionary<(double, double, double, double, double, double, double), decimal?>();
            var minValuesOcur = new Dictionary<(double, double, double, double, double, double, double), int>();
            while (i < 1000)
            {
                i++;
                var randomCoordinate = multiDimensionalGraph.ElementAt(rand.Next(0, graphCount)).Key;
                var recursiveMinValue = RecursiveMinCoordinate(randomCoordinate, maxDouble);
                if (!allMinValues.ContainsKey(recursiveMinValue.Key))
                {
                    allMinValues.Add(recursiveMinValue.Key, recursiveMinValue.Value);
                    minValuesOcur.Add(recursiveMinValue.Key, 1);
                }
                else
                {
                    minValuesOcur[recursiveMinValue.Key] += 1;
                }
            }
            var minValuesOcurTimesZ = new Dictionary<(double, double, double, double, double, double, double), decimal?>();
            foreach (var item in minValuesOcur)
            {
                minValuesOcurTimesZ.Add(item.Key, Math.Round(item.Value * (decimal)multiDimensionalGraph[item.Key],2));
            }
            var lowestValue = allMinValues.Aggregate((l, r) => l.Value < r.Value ? l : r);
            //var mostOcur = minValuesOcur.Aggregate((l, r) => l.Value > r.Value ? l : r);
            var mostOcur = minValuesOcurTimesZ.Aggregate((l, r) => l.Value < r.Value ? l : r);
            var orderedMinValues = minValuesOcur.OrderBy(x => x.Value);
            var result = minValuesOcurTimesZ.OrderBy(z => z.Value);
            var idealCombination = NormalizeIdealCombination(result);
            var mostOcurMinValue = new KeyValuePair<(double, double, double, double, double, double, double), decimal?>(mostOcur.Key, allMinValues[mostOcur.Key]);
            return mostOcurMinValue;
        }

        private (double, double, double, double, double, double, double) NormalizeIdealCombination(IOrderedEnumerable<KeyValuePair<(double, double, double, double, double, double, double), decimal?>> result)
        {
            double roicValue = 0.0; double equityValue = 0.0; double epsValue = 0.0; double revenueValue = 0.0; double peValue = 0.0; double debtValue = 0.0; double assetsValue = 0.0;

            foreach (var item in result)
            {
                roicValue += (item.Key.Item1) * -(double)item.Value;
                equityValue += (item.Key.Item2) * -(double)item.Value;
                epsValue += (item.Key.Item3) * -(double)item.Value;
                revenueValue += (item.Key.Item4) * -(double)item.Value;
                peValue += (item.Key.Item5) * -(double)item.Value;
                debtValue += (item.Key.Item6) * -(double)item.Value;
                assetsValue += (item.Key.Item7) * -(double)item.Value;
            }
            double[] values = new double[] { roicValue, equityValue, epsValue, revenueValue, peValue, debtValue, assetsValue };

            var idealCombination = new ValueTuple<double, double, double, double, double, double, double>
            {
                Item1 = NormalizeNumber(roicValue, Highest(values), Lowest(values)),
                Item2 = NormalizeNumber(equityValue, Highest(values), Lowest(values)),
                Item3 = NormalizeNumber(epsValue, Highest(values), Lowest(values)),
                Item4 = NormalizeNumber(revenueValue, Highest(values), Lowest(values)),
                Item5 = NormalizeNumber(peValue, Highest(values), Lowest(values)),
                Item6 = NormalizeNumber(debtValue, Highest(values), Lowest(values)),
                Item7 = NormalizeNumber(assetsValue, Highest(values), Lowest(values))
            };

            return idealCombination;
        }

        public double Highest(params double[] inputs)
        {
            return inputs.Max();
        }

        public double Lowest(params double[] inputs)
        {
            return inputs.Min();
        }

        public double NormalizeNumber(double val, double max, double min) { return (val - min) / (max - min); }

        public KeyValuePair<(double, double, double, double, double, double, double), decimal?> RecursiveMinCoordinate((double, double, double, double, double, double, double) startCoordinate, double maxDouble)
        {
            var adjacentCoordinates = GetAdjacentCoordinates(startCoordinate,  maxDouble);
            var minValue = adjacentCoordinates.Aggregate((l, r) => l.Value < r.Value ? l : r);
            if(multiDimensionalGraph[minValue.Key] < multiDimensionalGraph[startCoordinate])
            {
                return RecursiveMinCoordinate(minValue.Key,maxDouble);
            }
            return minValue;
        }

        public Dictionary<(double, double, double, double, double, double, double), decimal?> GetAdjacentCoordinates((double, double, double, double, double, double, double) startCoordinate
                                                                                                                     , double maxDouble)
        {
            var adjacentCoordinates = new Dictionary<(double, double, double, double, double, double, double), decimal?>();

            var x = new double[] { 0, 0.1, -0.1 };

            var roicCombinations = new List<double>() { startCoordinate.Item1 };
            if (startCoordinate.Item1 > 0) roicCombinations.Add(Math.Round(startCoordinate.Item1 - 0.1,1)); else roicCombinations.Add(startCoordinate.Item1);
            if (startCoordinate.Item1 < maxDouble) roicCombinations.Add(Math.Round(startCoordinate.Item1 + 0.1, 1)); else roicCombinations.Add(startCoordinate.Item1);
            var equityCombinations = new List<double>() { startCoordinate.Item2 };
            if (startCoordinate.Item2 > 0) equityCombinations.Add(Math.Round(startCoordinate.Item2 - 0.1, 1)); else equityCombinations.Add(startCoordinate.Item2);
            if (startCoordinate.Item2 < maxDouble) equityCombinations.Add(Math.Round(startCoordinate.Item2 + 0.1, 1)); else equityCombinations.Add(startCoordinate.Item2);
            var epsCombinations = new List<double>() { startCoordinate.Item3 };
            if (startCoordinate.Item3 > 0) epsCombinations.Add(Math.Round(startCoordinate.Item3 - 0.1, 1)); else epsCombinations.Add(startCoordinate.Item3);
            if (startCoordinate.Item3 < maxDouble) epsCombinations.Add(Math.Round(startCoordinate.Item3 + 0.1, 1)); else epsCombinations.Add(startCoordinate.Item3);
            var revenueCombinations = new List<double>() { startCoordinate.Item4 };
            if (startCoordinate.Item4 > 0) revenueCombinations.Add(Math.Round(startCoordinate.Item4 - 0.1, 1)); else revenueCombinations.Add(startCoordinate.Item4);
            if (startCoordinate.Item4 < maxDouble) revenueCombinations.Add(Math.Round(startCoordinate.Item4 + 0.1, 1)); else revenueCombinations.Add(startCoordinate.Item4);
            var peCombinations = new List<double>() { startCoordinate.Item5 };
            if (startCoordinate.Item5 > 0) peCombinations.Add(Math.Round(startCoordinate.Item5 - 0.1, 1)); else peCombinations.Add(startCoordinate.Item5);
            if (startCoordinate.Item5 < maxDouble) peCombinations.Add(Math.Round(startCoordinate.Item5 + 0.1, 1)); else peCombinations.Add(startCoordinate.Item5);
            var debtCombinations = new List<double>() { startCoordinate.Item6 };
            if (startCoordinate.Item6 > 0) debtCombinations.Add(Math.Round(startCoordinate.Item6 - 0.1, 1)); else debtCombinations.Add(startCoordinate.Item6);
            if (startCoordinate.Item6 < maxDouble) debtCombinations.Add(Math.Round(startCoordinate.Item6 + 0.1, 1)); else debtCombinations.Add(startCoordinate.Item6);
            var assetsCombinations = new List<double>() { startCoordinate.Item7 };
            if (startCoordinate.Item7 > 0) assetsCombinations.Add(Math.Round(startCoordinate.Item7 - 0.1, 1)); else assetsCombinations.Add(startCoordinate.Item7);
            if (startCoordinate.Item7 < maxDouble) assetsCombinations.Add(Math.Round(startCoordinate.Item7 + 0.1, 1)); else assetsCombinations.Add(startCoordinate.Item7);

            foreach (var a in GetCombinations(x, 7))
            {
                var list = a.ToList();
                var aux = (Math.Round(startCoordinate.Item1 + list[0],1), Math.Round(startCoordinate.Item2 + list[1],1), Math.Round(startCoordinate.Item3 + list[2],1)
                                        , Math.Round(startCoordinate.Item4 + list[3],1), Math.Round(startCoordinate.Item5 + list[4],1), Math.Round(startCoordinate.Item6 + list[5],1)
                                        , Math.Round(startCoordinate.Item7 + list[6],1));
                if (!roicCombinations.Contains(aux.Item1)) aux.Item1 = startCoordinate.Item1;
                if (!equityCombinations.Contains(aux.Item2)) aux.Item2 = startCoordinate.Item2;
                if (!epsCombinations.Contains(aux.Item3)) aux.Item3 = startCoordinate.Item3;
                if (!revenueCombinations.Contains(aux.Item4)) aux.Item4 = startCoordinate.Item4;
                if (!peCombinations.Contains(aux.Item5)) aux.Item5 = startCoordinate.Item5;
                if (!debtCombinations.Contains(aux.Item6)) aux.Item6 = startCoordinate.Item6;
                if (!assetsCombinations.Contains(aux.Item7)) aux.Item7 = startCoordinate.Item7;
                if (adjacentCoordinates.ContainsKey(aux) || aux == startCoordinate) continue;
                adjacentCoordinates.Add(aux, multiDimensionalGraph[aux]);
            }
            return adjacentCoordinates;
        }

        static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetCombinations(list, length - 1)
                .SelectMany(t => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
