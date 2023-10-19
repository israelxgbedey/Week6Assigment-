using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week5Assigment
{
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
}
