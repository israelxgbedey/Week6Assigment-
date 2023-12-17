using System;
using System.Collections.Generic;

namespace week5Assigment
{
    public class MyFileParserEngine : FileParserEngine
    {
        public MyFileParserEngine()
        {
            parsers.Add(new JsonFileParser());
            parsers.Add(new XmlFileParser());
        }

        public void ProcessFile(string filePath)
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
                Console.WriteLine($"Files not found: {filePath}");
            }
        }

    }
}
