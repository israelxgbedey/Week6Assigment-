using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace week5Assigment
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the directory where your files are located
            string directoryPath = AppContext.BaseDirectory; // You can change this to your desired directory

            // Get a list of all files in the specified directory
            string[] filesToProcess = Directory.GetFiles(directoryPath);

            var parserEngine = new MyFileParserEngine();

            foreach (string filePath in filesToProcess)
            {
                // Process each file in the directory
                parserEngine.ProcessFile(filePath);
            }
        }
    }
}
