using System;
using System.Collections.Generic;

namespace ant_colony
{
    class MainClass
    {


       
        public static void Main(string[] args)
        {
            AntColony colony = new AntColony();
            colony.initilize();
            for (int i = 0; i < 10; i++) {
                Console.WriteLine("---------" + i + "---------");
                colony.initilizePaths();
                colony.moveAnts();
                Console.WriteLine("-----------");

            }
           


        }


    }

}
