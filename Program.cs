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
                    }
                }
                Console.WriteLine(pairs);
            }
            
        }
    }
}