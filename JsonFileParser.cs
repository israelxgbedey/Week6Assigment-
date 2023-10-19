using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace week5Assigment
{
    public class JsonFileParser : IFileParser, ICanParseExtensions
    {
        public List<string> CanParseExtensions => new List<string> { ".json" };

        public void ParseFile(string filePath)
        {
            try
            {
                string outputFilePath = Path.ChangeExtension(filePath, "_out.txt");

                using (StreamWriter writer = new StreamWriter(outputFilePath, false)) // Overwrite the existing file
                {
                    writer.WriteLine("This is the JSON file processing output:");
                    string jsonText = File.ReadAllText(filePath);
                    JToken json = JToken.Parse(jsonText);
                    ProcessJson(json, writer, 1);
                }

                Console.WriteLine($"JSON file processed successfully: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing JSON file: {ex.Message}");
            }
        }

        private void ProcessJson(JToken json, StreamWriter writer, int lineCount, string prefix = "Field#")
        {
            if (json is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string output = $"Line#{lineCount} :{prefix}{property.Name}={property.Value}";
                    writer.WriteLine(output);
                    lineCount++;
                }
            }
            else if (json is JArray array)
            {
                foreach (var item in array)
                {
                    ProcessJson(item, writer, lineCount, prefix);
                }
            }
        }
    }
}
