using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json.Linq;

public interface IFileParser
{
    void ParseFile(string filePath);
}

public abstract class FileParserEngine
{
    protected List<IFileParser> parsers;

    public FileParserEngine()
    {
        parsers = new List<IFileParser>();
    }

    public void ProcessFiles(List<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                foreach (var parser in parsers)
                {
                    if (parser is ICanParseExtensions parserWithExtensions)
                    {
                        if (parserWithExtensions.CanParseExtensions.Contains(fileExtension))
                        {
                            parser.ParseFile(filePath);
                            Console.WriteLine($"File processed successfully: {filePath}");
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
    }
}

public interface ICanParseExtensions
{
    List<string> CanParseExtensions { get; }
}

public class CsvFileParser : IFileParser, ICanParseExtensions
{
    public List<string> CanParseExtensions => new List<string> { ".csv" };

    public void ParseFile(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                int lineCount = 1;
                string outputFilePath = Path.ChangeExtension(filePath, "_out.txt");

                using (StreamWriter writer = new StreamWriter(outputFilePath, false)) // Overwrite the existing file
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] fields = line.Split(',');

                        if (fields.Length >= 6)
                        {
                            string output = $"Line#{lineCount} :Field#1={fields[0]} ==> Field#2={fields[1]} ==> Field#3={fields[2]} ==> Field#4={fields[3]} ==> Field#5={fields[4]} ==> Field#6={fields[5]}";
                            writer.WriteLine(output);
                            lineCount++;
                        }
                    }
                }
            }
            Console.WriteLine($"CSV file processed successfully: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing CSV file: {ex.Message}");
        }
    }
}

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

public class XmlFileParser : IFileParser, ICanParseExtensions
{
    public List<string> CanParseExtensions => new List<string> { ".xml" };

    public void ParseFile(string filePath)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            string outputFilePath = Path.ChangeExtension(filePath, "_out.txt");

            using (StreamWriter writer = new StreamWriter(outputFilePath, false)) // Overwrite the existing file
            {
                int lineCount = 1;

                foreach (XmlNode itemNode in xmlDoc.SelectNodes("/menu/item"))
                {
                    string name = itemNode.SelectSingleNode("name").InnerText;
                    string price = itemNode.SelectSingleNode("price").InnerText;
                    string uom = itemNode.SelectSingleNode("uom").InnerText;

                    string output = $"Line#{lineCount} :Field#1={name} ==> Field#2={price} ==> Field#3={uom}";
                    writer.WriteLine(output);
                    lineCount++;
                }
            }

            Console.WriteLine($"XML file processed successfully: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing XML file: {ex.Message}");
        }
    }
}

public class YourFileParserEngine : FileParserEngine
{
    public YourFileParserEngine()
    {
        parsers.Add(new CsvFileParser());
        parsers.Add(new JsonFileParser());
        parsers.Add(new XmlFileParser());
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<string> filesToProcess = new List<string>
        {
            "SampleXML.xml",
            "SampleJSON.json"
        };

        var parserEngine = new YourFileParserEngine();

        for (int i = 0; i < filesToProcess.Count; i++)
        {
            filesToProcess[i] = Path.Combine(AppContext.BaseDirectory, filesToProcess[i]);
        }

        parserEngine.ProcessFiles(filesToProcess);
    }
}
