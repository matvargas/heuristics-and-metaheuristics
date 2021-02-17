using System;
using System.IO;

namespace heuristics_and_metaheuristics
{
    class Program
    {
        private static string PARENT_FOLDER_NAME = @"heuristics-and-metaheuristics";
        private static string INSTANCES_FOLDER_NAME = @"heu_e_met_tsp_instances/EUC_2D/";
        
        static void Main(string[] args)
        {
            string currPath = Directory.GetCurrentDirectory();
            currPath = currPath.Substring(0, currPath.LastIndexOf(PARENT_FOLDER_NAME) + PARENT_FOLDER_NAME.Length + 1);
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
                string[] instanceFiles = Directory.GetFiles(instancesPath);
                foreach (string s in instanceFiles)
                {
                    Console.WriteLine(s);
                }
            }
            
        }
    }
}