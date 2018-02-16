using A.HelperFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace A
{
    class Program
    {
        static int count = 1000;
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        public static int Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine(" >> Server Application (A)");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(exitHandler);
            bool fast_mode = false;

            #region "Parse Command Line Parameters and Create XML File"
            //args[0] is Run Mode (supported values fast / slow ) default slow mode
            //args[1] is XML Records count, default is 1000
            if (args.Length > 0 &&  args[0] != null)
            {
                if (args[0].Trim().ToLower() == "--help")
                {
                    Utility.PrintHelper();
                    return 0 ;
                }
                if (args[0].Trim().ToLower() == "fast")
                    fast_mode = true;
                if (args.Length > 1 && args[1] != null)
                {
                    bool result = int.TryParse(args[1], out count);
                    if (!result)
                    {
                        count = 1000;
                        Console.WriteLine(" >> Parse Error For XML Record Count Parameter, Using default value(1000).");
                    }
                }
            }
            string input_file = Utility.CreateXMLFile(count);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(string.Format(" >> XML file with {0} records created at {1}.", count, input_file));
            Console.ResetColor();
            #endregion

            #region Start TCP Listener For the client(B & C) to connect
            Console.WriteLine(" >> Run Mode => " + (fast_mode ? " FAST" : "SLOW"));
            ThreadPool.QueueUserWorkItem(o => Server.StartListening());
            Console.WriteLine(" >> Press CTRL+C to stop the server...");
            #endregion

            #region Read Generated XML File and Spawn a new thread for each type of record
            XmlDocument xml = new XmlDocument();
            xml.Load(input_file);
            
            foreach(var type in Utility.RecordTypes)
            {
                XmlNodeList list = xml.SelectNodes("data/record");
                var records = list.Cast<XmlNode>().Where(node => node.Attributes["type"].Value.ToUpper().Trim() == type)
                                               .Select(x => x.Attributes["value"].Value.ToString().Trim()).ToList();
                Server.TypeResetEvents.Add(type, new AutoResetEvent(false));
                Server.RecordProcessFinished.Add(type, false);
                //Spawning new thread for each record type
                ThreadPool.QueueUserWorkItem(o => Server.ProcessRecords(type, records, fast_mode));
            }
            #endregion

            autoResetEvent.WaitOne();
            Console.ReadKey();
            return 0;

        }

        protected static void exitHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            Console.WriteLine("CancelKey Pressed, Stoping server...");
            autoResetEvent.Set();


        }
    }
}
