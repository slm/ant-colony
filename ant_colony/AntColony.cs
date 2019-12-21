using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ant_colonyy
{
    public class AntColony
    {

        Random random = new Random(12);

        Item[] data = new Item[10];

        double[,] pheramones;

        Bag[] paths;

        int N;
        double alpha;
        double beta;
        static int BagWeight;
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

            Array.Sort(data);

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
            double[] posses = calcPossibility();
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
            pheramones[i.pos, j.pos] = pheramones[i.pos, j.pos] + (Q/value);
            pheramones[j.pos, i.pos] = pheramones[j.pos, i.pos] + (Q/value);
        }

        private void updatePheramones(Bag path)
        {
            for (int i = 0; i < path.bag.Count; i++) {
                if (i == path.bag.Count - 1) {
                    updatePheramone(path.GetItem(i), path.GetItem(0), cos(path.GetItem(i).pos, path.GetItem(0).pos));
                    continue;
                }
                updatePheramone(path.GetItem(i), path.GetItem(i+1),cos(path.GetItem(i).pos, path.GetItem(i+1).pos));
            }
        }

        private void moveAnt(Bag bag, double[] posses)
        {

            while (bag.getWeight() <= BagWeight && !bag.isVisitedAll(data)) {

                double rand = random.NextDouble();
                
                for (int i = 0; i <  data.Length; i++)
                {
                    rand = rand - posses[i];
                    if (rand <= 0)
                    {
                        bag.addItem(data[i]);
                        break;
                    }
                }
              
            }

        }


        double[] calcPossibility(int lastPos)
        {
            double[] posses = new double[data.Length];
   
            for (int i = 0; i < data.Length; i++)
            {
                posses[i] = possibility(i, lastPos, total);
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
                    double cs = cos(i, j);
                    double tij = Math.Pow(pheramones[i, j], alpha);//
                    double nij = Math.Pow(cs, beta);
                    double pab = (tij * nij);
                    totalPoss = totalPoss + pab;
                }
            }
            return totalPoss;
        }

        double possibility(int a ,int lastPos double totalPoss)
        {
            double cs = cos(lastPos, a);
            double tij = Math.Pow(pheramones[, b], alpha);
            double nij = Math.Pow(cs, beta);
            double pab = (tij * nij);
            return pab / (totalPoss - pab);
        }

        double cos(int i , int j)
        {
            return (data[i].value/ (data[i].weight * data[i].weight )) - (data[j].value / (data[j].weight * data[j].weight));
        }


        class Bag
        {
            public List<Item> bag = new List<Item>();
            private List<int> shadow = new List<int>();
            private HashSet<int> visited = new HashSet<int>();
            private int value;
            private int weight;

            public Bag(Item item){
                bag.Add(item);
                value = item.value;
                weight = item.weight;
                visited.Add(item.pos);
            }

            public Item GetItem(int pos)
            {
                return bag.ToList()[pos];
            }
            

            public int LastPos()
            {
                return bag.Last().pos;
            }

            public Item Last()
            {
                return bag.Last();
            }

            public bool isVisited(Item item) {
                return visited.Contains(item.pos);
            }

            public bool isVisitedAll(Item[] data) {
                return data.Length == visited.Count;
            }

            public bool canAdd(Item item,int limit) {
                return item.weight + weight < limit;
            }

            public bool addItem(Item item)
            {

                if (shadow.Contains(item.pos)) {
                    return false;
                }

                shadow.Add(item.pos);
                bag.Add(item);
                value = value + item.value;
                weight = weight + item.weight;
                
                return true;
                
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

        class Item: IComparable{
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

            public override bool Equals(object obj)
            {
                return pos == (obj as Item).pos;
            }

            public int CompareTo(object obj)
            {
                return ratio.CompareTo((obj as Item).ratio);
            }
        }

    }
}
