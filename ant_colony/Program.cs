using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace ant_colony
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string startupPath = Environment.CurrentDirectory;


            string path = @"startupPath\Testresults";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("olustu");
            }


            var stopwatch = Stopwatch.StartNew();
            Thread[] array = new Thread[10];
            for (int k = 0; k < 10; k++)
            {
                runCase(k);
            }

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        public static void runCase(int k)
        {
            k++;

            ACO.Bag best = null;
            float bestDuration = 99999999;
            double bestAvarage = 0;
            Console.WriteLine("test" + k + ".txt");
            for (int i = 0; i < 1; i++)
            {
                ACO colony = new ACO("Testbed/test" + k + ".txt");
                var stopwatch = Stopwatch.StartNew();

                colony.solve();

                var bag = colony.getBestBag();
                var avarageValue = colony.getAvarage();
                var duration = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("" + bag.getValue() + " " + avarageValue + " " + duration);
                if (best == null)
                {
                    best = bag;
                    bestDuration = duration;
                    bestAvarage = avarageValue;
                }
                else if (bag.getValue() > best.getValue())
                {
                    best = bag;
                    bestDuration = duration;
                    bestAvarage = avarageValue;
                }
            }


            string paths = @"startupPath\Testresults\test" + k + "result.txt";
            StreamWriter sw = new StreamWriter(paths);
            String x = "" + best.getValue() + "  " + bestAvarage + "  " + bestDuration;
            sw.Write(x);
            sw.Close();
        }
    }
}