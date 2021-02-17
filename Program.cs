using System;
using System.IO;
using System.Text.RegularExpressions;

namespace heuristics_and_metaheuristics
{
    class Program
    {
        private static string PARENT_FOLDER_NAME = @"heuristics-and-metaheuristics";
        private static string INSTANCES_FOLDER_NAME = @"heu_e_met_tsp_instances/EUC_2D/";

        static Tuple<double, double> handleInstance(string instanceFile)
        {
            int dim = 0;
            bool isEUC2D = false;
            int headerLines = 6;
            string[] lines = System.IO.File.ReadAllLines(instanceFile);

            for (int i = 0; i < headerLines; i++)
            {
                if (lines[i].Contains("DIMENSION"))
                {
                    dim = int.Parse(Regex.Split(lines[i], @"\D+")[1]);
                    Console.WriteLine(dim);
                }
                else if (lines[i].Contains("EDGE_WEIGHT_TYPE"))
                {
                    isEUC2D = lines[i].Contains("EUC_2D");
                }
            }

            for (int i = headerLines; i < dim; i++)
            {
                Console.Write(Regex.Split(lines[i], @"\D+")[0], 
                    Regex.Split(lines[i], @"\D+")[1], 
                    Regex.Split(lines[i], @"\D+")[2]);
            }
            return null;
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
                foreach (var instanceFile in instanceFiles)
                {
                    if (instanceFile.EndsWith(".tsp"))
                    {
                        var pair = handleInstance(instanceFile);
                    }
                }
            }
            
        }
    }
}