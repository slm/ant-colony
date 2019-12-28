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
            ACO colony = new ACO("Testbed/test"+k+".txt");
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("------ start running of file test"+k+".txt -----");
            for (int i = 0; i < 1; i++)
            {
                colony.solve();
            }


            var bestBag = colony.getBestBag();
            
            Console.WriteLine("Best value:"+bestBag.getValue()+"| Best weight: "+ bestBag.getWeight()+" - "+bestBag.getEmptySpace()+"| duration:"+stopwatch.ElapsedMilliseconds);
            string paths = @"startupPath\Testresults\test"+k+"result.txt";
            TextWriter writer = new StreamWriter(@"startupPath\Testresults\test" + k + "result.txt");
            writer.Close();
            if (!Directory.Exists(paths))
            {
                StreamWriter sw = new StreamWriter(paths);


                
                String x = ""+bestBag.getValue() + "  "  + colony.getAvarage()+"  "+ stopwatch.ElapsedMilliseconds;
                sw.Write(x);
                sw.Close();


            }
            else
            {

                StreamWriter sw = new StreamWriter(paths);

                sw.Flush();

                String x = "" + bestBag.getValue() + "  " + bestBag.getEmptySpace() + "  " + stopwatch.ElapsedMilliseconds;
                sw.Write(x);
                sw.Close();
                //         writer.WriteLine("Best value:" + bestBag.getValue() + "| Best weight: " + bestBag.getWeight() + " - " + bestBag.getEmptySpace() + "| duration:" + stopwatch.ElapsedMilliseconds);


            }
            //Console.WriteLine("Best bag:"+bestBag);

            Console.WriteLine("------ stop running of file test"+k+".txt -----");
        }


       

    }

}
