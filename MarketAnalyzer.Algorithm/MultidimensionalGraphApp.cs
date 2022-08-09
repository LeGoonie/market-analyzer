using MarketAnalyzer.Analysis;
using MarketAnalyzer.Business;
using MarketAnalyzer.Data;
using MarketAnalyzer.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketAnalyzer.Algorithm
{
    public class MultidimensionalGraphApp
    {
        public void Run()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var dataAnalyzerBO = new DataAnalyzerBO();

            var stockAnalysis = dataAnalyzerBO.AnalyzeStocks();

            var rangeMultiplier = new List<double>() { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };//, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2 };
            IEnumerable<IEnumerable<double>> result = GetCombinations(rangeMultiplier, 7);

            var listMultipliers = new List<Multiplier>();

            foreach (var item in result)
            {
                listMultipliers.Add(new Multiplier(item.ToList()));
            }

            var listOfListMultipliers = splitData(listMultipliers, 7);

            
            TimeSpan tsCombinations = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTimeCombinations = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                tsCombinations.Hours, tsCombinations.Minutes, tsCombinations.Seconds,
                tsCombinations.Milliseconds / 10);
            Console.WriteLine("RunTime Combinations " + elapsedTimeCombinations);
            var multiDimensionalGraph = new Dictionary<(double x, double y, double w, double p, double r, double a, double b), decimal?>();

            List<Task> tasks = new List<Task>();

            foreach(var item in listOfListMultipliers)
            {
                tasks.Add(Task.Run(() => { foreach(var item in GetPartOfGraph(stockAnalysis, item)) multiDimensionalGraph.Add(item.Key,item.Value); }));
            }

            Task.WaitAll(tasks.ToArray());
            string filePath = @"C:\Users\Bruno\Desktop\ExcelData\dictionary.csv";
            DictToCsvStreamWriter(multiDimensionalGraph, filePath);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.ReadLine();
        }

        public List<List<Multiplier>> splitData(List<Multiplier> fullList, int numberOfLists)
        {
            List<List<Multiplier>> partitionLists = new List<List<Multiplier>>();

            int width = (fullList.Count / numberOfLists);

            for (int i = 0; i <= numberOfLists; i++)
            {
                List<Multiplier> newList;
                newList = fullList.Skip(i * width).Take(width).ToList();
                partitionLists.Add(newList);
            }

            return partitionLists;
        }


        static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetCombinations(list, length - 1)
                .SelectMany(t => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }


        public Dictionary<(double, double, double, double, double, double, double), decimal?> GetPartOfGraph(IEnumerable<StockAnalysis> stockAnalysis, List<Multiplier> combinationMultiplier)
        {
            var algorithmBO = new AlgorithmBO();
            var newMultiDimensionalGraph = new Dictionary<(double x, double y, double w, double p, double r, double a, double b), decimal?>();
            
            for (int i = 0; i < combinationMultiplier.Count(); i++)
            {
                var orderedCompaniesDict = algorithmBO.GetCompaniesOrderedWithMultiplier(stockAnalysis, combinationMultiplier[i]);
                
                var topCompaniesWithSpecificMultiplier = orderedCompaniesDict
                                                        .OrderByDescending(x => x.Value)
                                                        .ToDictionary(x => x.Key, x => x.Value);
                var top100Companies = topCompaniesWithSpecificMultiplier.Keys.Take(100);
                
                var listStockAnalysis = new List<StockAnalysis>();

                var top100Tickers = top100Companies.Select(c => c.Ticker);
                listStockAnalysis.AddRange(stockAnalysis.Where(x => top100Tickers.Contains(x.Company.Ticker)));

                //foreach (var item in top100Companies)
                //{
                //    listStockAnalysis.Add(stockAnalysis.Where(x => item.Ticker == x.Company.Ticker).SingleOrDefault());
                //}

                var growthOfCompaniesWithSpecificMultiplier = algorithmBO.GetCompaniesGrowth(top100Companies, listStockAnalysis);

                decimal? zValue = growthOfCompaniesWithSpecificMultiplier.ToList().Sum(x => x.Item2);

                if(zValue > -1000000)
                {
                    newMultiDimensionalGraph.Add((combinationMultiplier[i].roicMulti,
                                                    combinationMultiplier[i].equityMulti,
                                                    combinationMultiplier[i].epsMulti,
                                                    combinationMultiplier[i].revenueMulti,
                                                    combinationMultiplier[i].peMulti,
                                                    combinationMultiplier[i].dToEMulti,
                                                    combinationMultiplier[i].aToLMulti), zValue);
                }

            }
            return newMultiDimensionalGraph;
        }

        public static void DictToCsv(Dictionary<(double, double, double, double, double, double, double), decimal?> dict, string filePath)
        {
            try
            {
                var csvLines = String.Join(Environment.NewLine,
                       dict);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, csvLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void DictToCsvStreamWriter(Dictionary<(double, double, double, double, double, double, double), decimal?> dict, string filePath)
        {
            FileStream stream = null;
            try
            {
                // Create a FileStream with mode CreateNew  
                stream = new FileStream(filePath, FileMode.OpenOrCreate);
                // Create a StreamWriter from FileStream  
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    foreach(var item in dict)
                    {
                        writer.WriteLine(item.Key.Item1 + ";" + item.Key.Item2 + ";" + item.Key.Item3 + ";" + item.Key.Item4 + ";" + item.Key.Item5 + ";" + item.Key.Item6 + ";" + item.Key.Item7 + ";" + item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
