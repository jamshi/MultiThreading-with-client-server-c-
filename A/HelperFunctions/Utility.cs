using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace A.HelperFunctions
{
    public class Utility
    {
        private static Random rnd = new Random();
        public static readonly List<string> RecordTypes = new List<string>() { "RED", "BLUE", "GREEN" };


        /// <summary>
        /// Function to create XML file with the specifed number of records
        /// </summary>
        /// <param name="totalNodes">Total Number of random nodes needed for the XML file</param>
        /// <returns>returns the path of XML file created</returns>
        public static string CreateXMLFile(int totalNodes) {
            string file_path = Environment.CurrentDirectory + "/input_data.xml";
            XmlTextWriter writer = new XmlTextWriter("input_data.xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("data");
            for (var i = 0; i < totalNodes; i++) { 
                writer.WriteStartElement("record");
                writer.WriteAttributeString("value", i.ToString());
                writer.WriteAttributeString("type", RecordTypes[rnd.Next(RecordTypes.Count)]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return file_path;
        }


        /// <summary>
        /// Print the help to console for this App
        /// </summary>
        public static void PrintHelper()
        {
            Console.WriteLine(" >> Supported Optional Command line Arguments ");
            Console.WriteLine(" >> 1. Run Mode, Possible values \"fast\" or \"slow\"");
            Console.WriteLine("       This will configure app to run in fast or slow mode provided (Default mode is slow).");

            Console.WriteLine(" >> 2. XML Record Count, Provide a numeric value. eg: 1200");
            Console.WriteLine("       This will genreate the XML file with specified number of records (Default value is 1000)");
            Console.WriteLine(" >> Usage: A fast 1200");

        }

        

    }
}
