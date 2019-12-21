﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ant_colony
{
	public class ACO
    {
		public List<int> result = new List<int>();
		List<int> time = new List<int>();
		
        Random random = new Random();

        List<Item> data = new List<Item>();

        double[,] pheramones;

        Bag[] paths;

        double alpha;
        double beta;
        static int BagWeight;
        int N;
        double Q = 0;

        public ACO(string path, int N = 10, double Q = 100, double alpha = 1, double beta = 1)
        {
            this.Q = Q;
            this.alpha = alpha;
            this.beta = beta;
            this.N = N;
            this.paths = new Bag[N];
            readFile(path);
            initilize(N);
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
            pheramones = new double[data.Count(), data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                for (int j = 0; j < data.Count(); j++)
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

        private void readFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            BagWeight = int.Parse(lines[0]);

            string[] weights = lines[2].Split(' ').Where(val => val != "").ToArray();
            string[] values = lines[4].Split(' ').Where(val => val != "").ToArray();


            for (int i = 0; i < weights.Count(); i++)
            {
                data.Add(new Item(i, int.Parse(weights[i]), int.Parse(values[i])));
            }
        }

        public void solve()
        {
            initilizePaths(N);
            double max = 0;
            double[,] posses = calcPossibility(out max);
            Thread[] array = new Thread[paths.Length];
            int i = 0;
            foreach (Bag path in paths) {
	            array[i] = new Thread(moveAntT);
	            array[i].Start(new mvP(path,posses,max));
	            //moveAnt(path,posses,max);
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

      

        private void updatePheramone(Item i, Item j, double value)
        {
            pheramones[i.pos, j.pos] = pheramones[i.pos, j.pos] + (Q / value);
        }

        private void updatePheramones(Bag path)
        {
            for (int i = 0; i < path.bag.Count; i++)
            {
                if (i == path.bag.Count - 1)
                {
                    continue;
                }
                updatePheramone(path.GetItem(i), path.GetItem(i + 1), cos(path.GetItem(i).pos, path.GetItem(i + 1).pos));
            }
        }

        class mvP
        {
	        public Bag bag;
	        public double[,] posses;
	        public double maxPosses;
	        public mvP(Bag bag,double[,] posses,double maxPosses)
	        {
		        this.bag = bag;
		        this.posses = posses;
		        this.maxPosses = maxPosses;
	        }
        }
        
        private void moveAntT(object obj)
        {
	        var mvp = (mvP)obj;
	        moveAnt(mvp.bag,mvp.posses,mvp.maxPosses);
        }
        
        private void moveAnt(Bag bag, double[,] posses,double maxPoss)
        {
            var available = bag.AvailableItems();
            while (available.Count > 0)
            {
         
                double rand = random.NextDouble() * maxPoss;
                Item lastItem = bag.Last();
                foreach (var item in available) {
                    rand = rand - posses[lastItem.pos, item.pos];
                    if (rand <= 0)
                    {
	                    lastItem = item;
	                    bag.addItem(item);
                        break;
                    }
                }
                available = bag.AvailableItems();
            }
            result.Add(bag.getValue());
        }

        double[,] calcPossibility(out double m)
        {
            double max = 0;
            double[,] posses = new double[data.Count, data.Count];
            double total = 0;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    posses[i, j] = possibility(i, j);
                    if (posses[i, j] > max) {
                        max = posses[i, j];
                    }
                    total = total + posses[i, j];
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    posses[i, j] = posses[i,j]/(total-posses[i,j]);
                }
            }
            m = max;
            return posses;
        }

        double possibility(int a, int b)
        {
            double cs = cos(a, b);
            double tij = Math.Pow(pheramones[a, b], alpha);
            double nij = Math.Pow(cs, beta);
            double pab = (tij * nij);
            return pab;
        }

        double cos(int i, int j)
        {
	        return Math.Pow(data[j].ratio, 2);
        }

        class Bag
		{
			public List<Item> bag = new List<Item>();
			private List<int> shadowBag = new List<int>();
			private List<Item> availableItems;
			public Item last;

			private int value;
			private int weight;
			private int maxSize;

			public Bag(Item firstItem,List<Item> data,int maxSize)
			{
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

			public bool canAdd(Item item)
			{
				return item.weight + weight < maxSize;
			}

			public bool addItem(Item item)
			{

				if (shadowBag.Contains(item.pos))
				{
					return false;
				}

				shadowBag.Add(item.pos);
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

			public int getWeight()
			{
				return weight;
			}

			public int getValue()
			{
				return value;
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

		class Item : IComparable
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