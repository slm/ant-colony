using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ant_colony
{
    public class AntColony
    {

        Random random = new Random();

        static Item[] data = new Item[10];

        double[,] pheramones;

        Bag[] paths;

        int N;
        double alpha;
        double beta;
        int BagWeight;
        double Q = 0;

        public AntColony(string path, int N = 2, double Q = 100, double alpha = 1, double beta = 1)
        {
            this.Q = Q;
            this.N = N;
            this.alpha = alpha;
            this.beta = beta;
            this.paths = new Bag[N];
            readFile(path);

        }

        private void readFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            BagWeight = int.Parse(lines[0]);

            string[] weights = lines[2].Split(' ').Where(val => val != "").ToArray();
            string[] values = lines[4].Split(' ').Where(val => val != "").ToArray();

            data = new Item[weights.Count()];

            for (int i = 0; i < weights.Count(); i++) {
                data[i] = new Item(i, int.Parse(weights[i]), int.Parse(values[i]));
            }

        }

        public void initilize()
        {
            initilizePheramons();
            initilizePaths();
        }


        public void initilizePaths()
        {
            List<int> randomed = new List<int>();
            for (int i = 0; i < N; i++) {

                int r = random.Next(0, data.Length);

                randomed.Add(r);

                paths[i] = new Bag(data[r]);
            }
        }

        private void initilizePheramons()
        {
            pheramones = new double[data.Length, data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    if (i == j)
                    {
                        pheramones[i, j] = 0;
                    }
                    else
                    {
                        pheramones[i, j] = 1;
                    }

                }
            }
        }

        public void moveAnts()
        {
            double[,] posses = calcPossibility();
            foreach (Bag path in paths) {
                moveAnt(path, posses);
                //updatePheramones(path);
                Console.WriteLine(path);
            }

            foreach (Bag path in paths)
            {
                updatePheramones(path);
            }
        }


        private void updatePheramone(Item i, Item j, double value)
        {
            pheramones[i.pos, j.pos] = Q / value;
            pheramones[j.pos, i.pos] = Q / value;
        }

        private void updatePheramones(Bag path)
        {
            for (int i = 0; i < path.bag.Count; i++) {
                if (i == path.bag.Count - 1) {
                    updatePheramone(path.bag[i], path.bag[0], Q / cos(path.bag[i].pos, path.bag[0].pos));
                    continue;
                }
                updatePheramone(path.bag[i], path.bag[i + 1], Q / cos(path.bag[i].pos, path.bag[i + 1].pos));
            }
        }

        private void moveAnt(Bag bag, double[,] posses)
        {
            int step = 0;
            while (bag.bag.Count <  data.Length-1) {

                double rand = random.NextDouble();
                
                for (int i = 0; i <  data.Length; i++)
                {
                    if (bag.LastPos() == i || bag.bag.Contains(data[i]))
                    {
                        continue;
                    }
                    rand = rand - posses[bag.LastPos(), i];
                    if (rand <= 0)
                    {
                        step++;
                        bag.addItem(data[i]);
                        break;
                    }
                }
            }

        }


        double[,] calcPossibility()
        {
            double[,] posses = new double[data.Length, data.Length];
            double total = totalPoss();
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    if (i == j) {
                        continue;
                    }
                    posses[i, j] = possibility(i, j, total);
                }
            }
            return posses;

        }


        double totalPoss() {
            double totalPoss = 0;
            for (int i = 0; i < data.Length; i++) {
                for (int j = 0; j < data.Length; j++)
                {
                    if (i == j) {
                        continue;
                    }
                    double tij = Math.Pow(pheramones[i, j], alpha);
                    double nij = Math.Pow(1.0 / cos(i,j), beta);
                    double pab = (tij * nij);
                    totalPoss = totalPoss + pab;
                }
            }
            return totalPoss;
        }

        double possibility(int a, int b, double totalPoss)
        {
            double tij = Math.Pow(pheramones[a, b], alpha);
            double nij = Math.Pow(1.0 / cos(a, b), beta);
            double pab = (tij * nij);
            return pab / (totalPoss - pab);
        }

        double cos(int i , int j)
        {
            return data[i].value + data[j].value;
        }

        class Bag
        {
            public List<Item> bag = new List<Item>();
            private int value;
            private int weight;

            public Bag(Item item){
                bag.Add(item);
                value = item.value;
                weight = item.weight;
            }



            public int LastPos()
            {
                return bag.Last().pos;
            }

            public Item Last()
            {
                return bag.Last();
            }

            public bool addItem(Item item)
            {
                if (bag.Contains(item))
                {
                    return false;
                }else{
                    value = value + item.value;
                    weight = weight + item.weight;
                    bag.Add(item);
                    return true;
                }
            }

            public int getWeight() {
                return weight;
            }

            public int getValue()
            {
                return value;
            }

            public override string ToString()
            {
                string path = "";
                foreach(Item i in bag)
                {
                    path = path + i.pos + "-";
                }

                path = path + "| weight: " + getWeight() + " value: "+getValue();
                
                return path;
            }

        }

        class Item {
            public int pos;
            public int value;
            public int weight;
            public double ratio;

            public Item(int pos,int value,int weight)
            {
                this.pos = pos;
                this.value = value;
                this.weight = weight;
                this.ratio = (value * 1.0) / weight;
            }

            public override int GetHashCode()
            {
                return pos;
            }

        }

    }
}
