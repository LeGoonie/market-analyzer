using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MarketAnalyzer.Algorithm
{
    public class GradientDescentApp
    {
        public void Run()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var rangeMultiplier = new List<double>() { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7 };//, 0.8, 0.9, 1 };//, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2 };
            var gd = new GradientDescent();
            var minCoordinatesZValue = gd.GradientDescentNoLearning(rangeMultiplier.Last());
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.ReadLine();
        }
    }
}
