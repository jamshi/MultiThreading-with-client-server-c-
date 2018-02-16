using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace B.HelperFunctions
{
    public class Utility
    {
        /// <summary>
        /// Print the help to console for this App
        /// </summary>
        public static void PrintHelper()
        {
            Console.WriteLine(" >> Supported Command line Arguments ");
            Console.WriteLine(" >> 1. Address of the server to connect <IP>:<PORT>");
            Console.WriteLine("       This will connect to the server with the address provided.");

            Console.WriteLine(" >> Usage: B 127.0.0.1:8888");

        }

        public static void OnMessage(string message, string _recordType)
        {
            _recordType = _recordType.First().ToString().ToUpper() + _recordType.Substring(1).ToLower().Trim();
            message = message.Replace("<LOG>", "");
            Console.ResetColor();
            Console.Write(" >> ");
            Type type = typeof(ConsoleColor);
            ConsoleColor color;
            if (Enum.TryParse<ConsoleColor>(_recordType, out color))
                Console.ForegroundColor = color;
            Console.WriteLine(message);

        }

        

    }
}
