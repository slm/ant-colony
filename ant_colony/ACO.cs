using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Extensions;

namespace ant_colony
{
    public class ACO
    {
        public List<Bag> results = new List<Bag>();
        

        Random random = new Random();

        List<Item> data = new List<Item>();

        double[] pheramones;

        Bag[] paths;

        double alpha;
        double beta;
        static int BagWeight;
        int N;
        double Q;
        private double p;
        private int iteration;

        public ACO(string path, int N = 200, double Q = 100, double alpha = 0.5, double beta = 0.5,double p = 0.2,int iteration=500)
        {
            this.Q = Q;
            this.alpha = alpha;
            this.beta = beta;
            this.N = N;
            this.paths = new Bag[N];
            this.iteration = iteration;
            this.p = p;
            readFile(path);
            initilize(N);
        }

        public Bag getBestBag()
        {
            return results.MaxBy(t => t.getValue());
        }
        
        public double getAvarage()
        {
            return results.Average(t => t.getValue());
        }
        
        
        
   

        public void initilize(int N)
        {
            initilizePheramons();
        }

        public void initilizePaths(int N)
        {
            List<int> randomed = new List<int>();
            for (int i = 0; i < N; i++)
            {
                int r = random.Next(0, data.Count());

                randomed.Add(r);

                paths[i] = new Bag(data[r], data, BagWeight);
            }
        }

        private void initilizePheramons()
        {
            pheramones = new double[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                pheramones[i] = 1;
            }
        }

        private void readFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            BagWeight = int.Parse(lines[0]);

            string[] weights = lines[4].Split(' ').Where(val => val != "").ToArray();
            string[] values = lines[2].Split(' ').Where(val => val != "").ToArray();


            for (int i = 0; i < weights.Count(); i++)
            {
                data.Add(new Item(i, int.Parse(weights[i]), int.Parse(values[i])));
            }
        }

        public void solve()
        {
            for (int o = 0; o < iteration; o++)
            {
                initilizePaths(N);
                double max = 0;
                double[] posses = calcPossibility(out max);
                Thread[] array = new Thread[paths.Length];
                int i = 0;
                foreach (Bag path in paths)
                {
                    array[i] = new Thread(moveAntT);
                    array[i].Start(new mvP(path, posses, max));
                    i++;
                }

                for (int k = 0; k < paths.Length; k++)
                {
                    array[k].Join();
                }

                foreach (Bag path in paths)
                {
                    updatePheramones(path);
                }
            }

         
        }


        private void updatePheramone(Item i, double value)
        {
            pheramones[i.pos] = pheramones[i.pos] + (1-p)*(Q/value);
        }

        private void updatePheramones(Bag path)
        {
            for (int i = 0; i < path.bag.Count; i++)
            {
                if (i == path.bag.Count - 1)
                {
                    continue;
                }

                updatePheramone(path.GetItem(i), cos(path.GetItem(i).pos));
            }

            //Console.WriteLine(path.id + " -> update pheramones");
        }

        class mvP
        {
            public Bag bag;
            public double[] posses;
            public double maxPosses;

            public mvP(Bag bag, double[] posses, double maxPosses)
            {
                this.bag = bag;
                this.posses = posses;
                this.maxPosses = maxPosses;
            }
        }

        private void moveAntT(object obj)
        {
            var mvp = (mvP) obj;
            moveAnt(mvp.bag, mvp.posses, mvp.maxPosses);
        }

        private void moveAnt(Bag bag, double[] posses, double maxPoss)
        {
            var available = bag.AvailableItems();
            while (available.Count > 0)
            {
                double rand = random.NextDouble() * maxPoss;
                foreach (var item in available)
                {
                    rand = rand - posses[item.pos];
                    if (rand <= 0)
                    {
                        bag.addItem(item);
                        break;
                    }
                }
                available = bag.AvailableItems();
            }

            results.Add(bag);
        }

        double[] calcPossibility(out double m)
        {
            double max = 0;
            double[] posses = new double[data.Count];
            double total = 0;
            for (int i = 0; i < data.Count; i++)
            {
                posses[i] = possibility(i);
                total = total + posses[i];
            }

            for (int i = 0; i < data.Count; i++)
            {
                posses[i] = posses[i] / (total - posses[i]);
                if (posses[i] > max)
                {
                    max = posses[i];
                }
            }

            m = max;
            return posses;
        }

        double possibility(int a)
        {
            double cs = cos(a);
            double tij = Math.Pow(pheramones[a], alpha);
            double nij = Math.Pow(cs, beta);
            double pab = (tij * nij);
            return pab;
        }

        double cos(int i)
        {
            return data[i].value/Math.Pow(data[i].weight,2);
        }

        public class Bag
        {
            public int id;
            public List<Item> bag = new List<Item>();
            private List<Item> availableItems;
            public Item last;

            private int value;
            private int weight;
            private int maxSize;

            public Bag(Item firstItem, List<Item> data, int maxSize)
            {
                id = DateTime.Now.Millisecond;
                last = firstItem;
                bag.Add(firstItem);
                value = firstItem.value;
                weight = firstItem.weight;
                availableItems = data.ToList();
                availableItems.Remove(firstItem);
                this.maxSize = maxSize;
            }

            public Item GetItem(int pos)
            {
                return bag[pos];
            }

            public int LastPos()
            {
                return Last().pos;
            }

            public Item Last()
            {
                return last;
            }

            public bool addItem(Item item)
            {
                
                bag.Add(item);
                last = item;
                
                value = value + item.value;
                weight = weight + item.weight;
                
                availableItems.Remove(item);
                updateAvailableItems();
                
                return true;
            }

            private void updateAvailableItems()
            {
                int empty = maxSize - weight;
                availableItems = availableItems.Where(val => val.weight <= empty).ToList();
            }

            public List<Item> AvailableItems()
            {
                return availableItems;
            }

            
            public int getEmptySpace()
            {
                return maxSize-weight;
            }
            
            public int getWeight()
            {
                return weight;
            }

            public int getValue()
            {
                return value;
            }

            public int getRawValue()
            {
                return bag.Sum(x=>x.value);
            }
            
            public int getRawWeight()
            {
                return bag.Sum(x=>x.weight);
            }

            public List<int> getDuplicates()
            {
                return bag.FindDuplicates(x=>x.pos);
            }

            public override string ToString()
            {
                string path = "";
                foreach (Item i in bag)
                {
                    path = path + i.pos + "-";
                }

                path = path + "| weight: " + getWeight() + " value: " + getValue();

                return path;
            }
        }

        public class Item : IComparable
        {
            public int pos;
            public int value;
            public int weight;
            public double ratio;

            public Item(int pos, int value, int weight)
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