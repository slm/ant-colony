using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extensions;

namespace ant_colony
{
    class MainClass
    {


       
        public static void Main(string[] args)
        {
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
            ACO colony = new ACO("Testbed/test"+k+".txt");
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("------ start running of file test"+k+".txt -----");
            for (int i = 0; i < 100; i++)
            {
                colony.solve();
            }
            
            Console.WriteLine("Max:"+colony.result.Max()+" "+stopwatch.ElapsedMilliseconds);
            Console.WriteLine("------ stop running of file test"+k+".txt -----");
        }

    }

}
