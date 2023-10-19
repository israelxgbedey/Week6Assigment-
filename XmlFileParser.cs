using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace week5Assigment
{
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
}
