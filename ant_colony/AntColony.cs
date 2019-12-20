using System;
using System.Collections.Generic;
using System.Linq;

namespace ant_colony
{
    public class AntColony
    {

        Random random = new Random();

        static int[,] data = { { 0, 745, 665, 929 }, { 745, 0, 80, 337 }, { 665, 80, 0, 380 }, { 929, 337, 380, 0 } };

        double[,] pheramones;

        Path[] paths;

        int N;
        double alpha;
        double beta;
        int CityCount;
        double Q = 0;

        public AntColony(int N = 2, double Q = 100, double alpha = 1, double beta = 1)
        {
            this.Q = Q;
            this.N = N;
            this.alpha = alpha;
            this.beta = beta;
            this.CityCount = data.GetLength(0);
            this.paths = new Path[N];

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

                int r = random.Next(0, CityCount);

                while(randomed.Contains(r)) {
                    r = random.Next(0, CityCount);
                }

                randomed.Add(r);
             
            
                paths[i] = new Path(r);
            }
        }

        private void initilizePheramons()
        {
            pheramones = new double[CityCount, CityCount];

            for (int i = 0; i < CityCount; i++)
            {
                for (int j = 0; j < CityCount; j++)
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
            foreach (Path path in paths) {
                moveAnt(path, posses);
                //updatePheramones(path);
                Console.WriteLine(path);
            }

            foreach (Path path in paths)
            {
                updatePheramones(path);
            }
        }


        private void updatePheramone(int i, int j, double value)
        {
            pheramones[i, j] = Q / value;
            pheramones[j, i] = Q / value;
        }

        private void updatePheramones(Path path)
        {
            for (int i = 0; i < path.next.Count; i++) {
                if (i == path.next.Count - 1) {
                    updatePheramone(path.next[i], path.next[0], Q / data[path.next[i], path.next[0]]);
                    continue;
                }
                updatePheramone(path.next[i], path.next[i + 1], Q / data[path.next[i], path.next[i + 1]]);
            }
        }


        private bool isAddedBefore(int current, Path path,int step)
        {
            int checkIndex = step+1;

            foreach (Path p in paths) {
                if (p == path) {
                    continue;
                }

                if (p.next.Count > checkIndex && p.next[checkIndex] == current) {
                    if (p.next.Count == CityCount - 1) { }
                    return true;
                }
            }
            return false;
        }

        private void moveAnt(Path path, double[,] posses)
        {
            int step = 0;
            while (path.next.Count < CityCount-1) {

                double rand = random.NextDouble();
                
                for (int i = 0; i < CityCount; i++)
                {
                    if (path.getCurrent() == i || path.next.Contains(i) || isAddedBefore(i,path,step))
                    {
                        continue;
                    }
                    rand = rand - posses[path.getCurrent(), i];
                    if (rand <= 0)
                    {
                        step++;
                        path.addNext(i);
                        break;
                    }
                }

            }
            List<int> diffValue = Enumerable.Range(0, CityCount).ToList().Except(path.next).ToList();
            path.addNext(diffValue[0]);

        }


        double[,] calcPossibility()
        {
            double[,] posses = new double[CityCount, CityCount];
            double total = totalPoss();
            for (int i = 0; i < CityCount; i++)
            {
                for (int j = 0; j < CityCount; j++)
                {
                    if (i == j) {
                        continue;
                    }
                    posses[i,j] = possibility(i, j, total);
                }
            }
            return posses;

        }

        double totalPoss() {
            double totalPoss = 0;
            for (int i = 0; i < CityCount; i++) {
                for (int j = 0; j < CityCount; j++)
                {
                    if (i == j) {
                        continue;
                    }
                    double tij = Math.Pow(pheramones[i, j], alpha);
                    double nij = Math.Pow(1.0 / data[i, j], beta);
                    double pab = (tij * nij);
                    totalPoss = totalPoss + pab;
                }
            }
            return totalPoss;
        }

        double possibility(int a,int b,double totalPoss)
        {
            double tij = Math.Pow(pheramones[a, b], alpha);
            double nij = Math.Pow(1.0 / data[a, b], beta);
            double pab = (tij * nij);
            return pab / (totalPoss - pab);
        }

  
        class Path
        {
            public List<int> next = new List<int>();
            private int distance;

            public Path(int initialPoint)
            {
                this.next.Add(initialPoint);
            }

            public int getCurrent()
            {
                return next[next.Count - 1];
            }

            public bool addNext(int city)
            {
                if (next.Contains(city))
                {
                    return false;
                }else{
                    distance = distance + data[getCurrent(), city];
                    next.Add(city);
                    return true;
                }
            }

            public int getDistance() {
                return distance + data[getCurrent(), next[0]];
            }

            public override string ToString()
            {
                string path = "";
                foreach(int i in next)
                {
                    path = path + i + " -> ";
                }

                path = path + "| distance: " + getDistance();
                
                return path;
            }


        }

    }
}
