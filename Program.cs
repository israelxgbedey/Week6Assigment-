using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json.Linq;

// Interface for all file parsers
public interface IFileParser
{
    void ParseFile(string filePath);
}

// Abstract class for the file parser engine
public abstract class FileParserEngine
{
    protected List<IFileParser> parsers;

    public FileParserEngine()
    {
        parsers = new List<IFileParser>();
    }

    // Process a list of files using the registered parsers
    public void ProcessFiles(List<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                // Iterate over the registered parsers
                foreach (var parser in parsers)
                {
                    if (parser is ICanParseExtensions parserWithExtensions)
                    {
                        if (parserWithExtensions.CanParseExtensions.Contains(fileExtension))
                        {
                            // Use the parser to process the file
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

// Interface for parsers that can specify the file extensions they can handle
public interface ICanParseExtensions
{
    List<string> CanParseExtensions { get; }
}

// JSON file parser
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

// XML file parser
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

// Custom file parser engine
public class myFileParserEngine : FileParserEngine
{
    public myFileParserEngine()
    {
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

        var parserEngine = new myFileParserEngine();

        for (int i = 0; i < filesToProcess.Count; i++)
        {
            filesToProcess[i] = Path.Combine(AppContext.BaseDirectory, filesToProcess[i]);
        }

        parserEngine.ProcessFiles(filesToProcess);
    }
}
