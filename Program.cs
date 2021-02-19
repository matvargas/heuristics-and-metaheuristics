using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace heuristics_and_metaheuristics
{
    class Program
    {
        private static string PARENT_FOLDER_NAME = @"heuristics-and-metaheuristics";
        private static string INSTANCES_FOLDER_NAME = @"heu_e_met_tsp_instances/EUC_2D/";
        // private static string INSTANCES_FOLDER_NAME = @"teste/";
        static bool isEUC2D = false;

        static double calcLimit(double min, double max)
        {
            return min + 0.01*(max - min);
        }
        
        static double greedyRandomized(double[,] m, int[] path)
        {
            int currentCity = 0;
            int visitedCount = 0;
            double total = 0.0;
            double max;
            double min;
            double limit;
            bool[] visited = new bool[m.GetLength(0)];
            

            Random rdn = new Random();
            int start = rdn.Next() % m.GetLength(0);
            // Console.WriteLine(start);
            currentCity = start;

            visited[currentCity] = true;
            path[0] = currentCity;
            visitedCount++;

            List<int> aux = new List<int>();

            while (visitedCount < m.GetLength(0))
            {
                aux.Clear();
                min = 99999999999.99999999999;
                max = 0.0;
                for (int i = 0; i < m.GetLength(0); i++)
                {
                    if (!visited[i] && m[currentCity, i] < min)
                    {
                        min = m[currentCity, i];
                    }

                    if (!visited[i] && m[currentCity, i] > max)
                    {
                        max = m[currentCity, i];
                    }
                }
                // Console.Write(min);
                // Console.Write(" - ");
                // Console.Write(max);
                // Console.Write(" - ");
                limit = calcLimit(min, max);
                // Console.Write(limit);
                // Console.Write(" - ");
                for (int i = 0; i < m.GetLength(0); i++)
                {
                    if (!visited[i] && i != currentCity)
                    {
                        if (m[currentCity, i] <= limit)
                        {
                            aux.Add(i);
                        }
                    }
                }

                rdn = new Random();
                int rand = rdn.Next() % aux.Count;
                path[visitedCount] = aux[rand];
                total += m[currentCity, aux[rand]];
                // Console.Write(visitedCount);
                // Console.Write(" - ");
                // Console.WriteLine(total);
                currentCity = aux[rand];
                visited[currentCity] = true;
                visitedCount++;

            }
            path[visitedCount] = start;
            total += m[currentCity, start];
            return total;

        }

        static double GRASP(double[,]m)
        {
            double cgreedy = 0.0;
            double clocal = 0.0;
            double best = 0.0;

            int[] path = new int[m.GetLength(0) + 1];
            int[] newPath = new int[path.Length];
            best = greedyRandomized(m, path);
            int iter = 0;
            
            
            while (iter < 500)
            {
                path.CopyTo(newPath, 0);
                cgreedy = greedyRandomized(m, newPath);
                clocal = search2OptTSP(m, newPath, cgreedy);

                if(clocal < best){
                    path = newPath;
                    best = clocal;
                    iter = 0;
                }
                else
                {
                    iter++;
                }
            
            }

            Console.WriteLine(best);
            return best;
        }
        
        static double search2OptTSP(double[,] m, int[] newPath, double initialCost){
            double bestcost = initialCost;
            double curcost = 0.0;
            for (int i = 0; i < m.GetLength(0) - 1; i++){
                for (int k = i + 1; k < m.GetLength(0); k++)
                {
                    int[] localPath = new int[newPath.Length];
                    newPath.CopyTo(localPath, 0);
                    
                    reversePath(localPath, i, k);
                    
                    curcost = calculateTourCost(localPath, m);
                    
                    if(curcost < bestcost){
                        bestcost = curcost;
                        newPath = localPath;
                    }
                }
            }
            return bestcost;
        }
        
        static double calculateTourCost(int[] path, double[,] m){
            double totalcost = 0.0;
            for (int i = 0; i < path.Length - 1; i++){ 
                totalcost += m[path[i], path[i+1]];
            }
            return totalcost;
        }
        
        static void reversePath(int[] path, int i, int k){
            int swapaux = 0;
            int ncities = path.Length - 1;
            if (k < i)
            {
                int tmp = i;
                i = k;
                k = tmp;
            }
            if(i == 0){
                swapaux = path[i];
                path[i] = path[k];
                path[ncities] = path[k];
                path[k] = swapaux;
                i++;
                k--;
            }
            for (int index = i, revindex = k; index < revindex; index++, revindex--)
            {
                int tmp = path[index];
                path[index] = path[revindex];
                path[revindex] = tmp;
            }
        }
        
        static double euc2d(Tuple<double, double> c1, Tuple<double, double> c2)
        {
            return Math.Round(Math.Sqrt(Math.Pow((c1.Item1 - c2.Item1), 2) + Math.Pow((c1.Item2 - c2.Item2), 2)));
        }
        
        static double att(Tuple<double, double> c1, Tuple<double, double> c2)
        {
            return Math.Ceiling(Math.Sqrt((Math.Pow((c1.Item1 - c2.Item1), 2) + Math.Pow((c1.Item2 - c2.Item2), 2)) / 10.0));
        }
        
        static double calcDist(Tuple<double, double> c1, Tuple<double, double> c2)
        {
            return isEUC2D ? euc2d(c1, c2) : att(c1, c2);
        }
        
        static double[,] doMatrix(List<Tuple<double, double>> pairs)
        {
            double[,]matrix = new double[pairs.Count, pairs.Count];
            for (int i = 0; i < pairs.Count - 1; i++)
            {
                matrix[i, i] = 0.0;
                for (int j = i + 1; j < pairs.Count; j++)
                {
                    matrix[i, j] = calcDist(pairs[i], pairs[j]);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return matrix;
        }
        
        static List<Tuple<double, double>> handleInstance(string instanceFile)
        {
            int dim = 0;
            int headerLines = 6;
            string[] lines = System.IO.File.ReadAllLines(instanceFile);
            List<Tuple<double, double>> pairs = new List<Tuple<double, double>>();

            Console.WriteLine(lines[0]);
            
            for (int i = 0; i < headerLines; i++)
            {
                if (lines[i].Contains("DIMENSION"))
                {
                    dim = int.Parse(Regex.Split(lines[i], @"\D+")[1]);
                    // Console.WriteLine(dim);
                }
                else if (lines[i].Contains("EDGE_WEIGHT_TYPE"))
                {
                    isEUC2D = lines[i].Contains("EUC_2D");
                }
            }

            // Console.WriteLine(headerLines);
            
            for (int i = headerLines; i < dim + headerLines; i++)
            {
                var dataLine = lines[i].Split(' ');
                double [] tmp = new Double[3];
                int j = 0;
                foreach (var s in dataLine)
                {
                    if (s != "")
                        tmp[j++] = Convert.ToDouble(s);
                }

                pairs.Add(new Tuple<double, double>(tmp[1], tmp[2]));
            }
            return pairs;
        }

        static void Main(string[] args)
        {

            var currPath = Directory.GetCurrentDirectory();
            currPath = currPath.Substring(0, currPath.LastIndexOf(PARENT_FOLDER_NAME) + PARENT_FOLDER_NAME.Length) + "/";
            Console.WriteLine("Project path: " + currPath);
            
            string instancesPath = currPath + INSTANCES_FOLDER_NAME;
            Console.WriteLine("Instances path: " + instancesPath);
            
            if (!Directory.Exists(instancesPath))
            {
                Console.WriteLine("ERROR: Instances could not be found on the specified path");
                Environment.ExitCode = -1;
            }
            else
            {
                var instanceFiles = Directory.GetFiles(instancesPath);
                List<Tuple<double, double>> pairs = new List<Tuple<double, double>>();
                foreach (var instanceFile in instanceFiles)
                {
                    if (instanceFile.EndsWith(".tsp"))
                    {
                        pairs = handleInstance(instanceFile);
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        double[,]m = doMatrix(pairs);
                        GRASP(m);
                        watch.Stop();
                        var elapsedMs = watch.Elapsed.TotalSeconds;
                        Console.WriteLine(elapsedMs);
                    }
                }
                
            }

        }
    }
}