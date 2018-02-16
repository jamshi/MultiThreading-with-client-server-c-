using B.HelperFunctions;
using JBP.ConnectionUtilities;
using JBP.NetworkUtilities.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace B
{
    class Program
    {
        
        private static string _recordType = string.Empty;
        private static IPAddress _ipaddress;
        private static int _port;
        public static int Main(String[] args)
        {
            Console.Clear();
            Console.WriteLine(" >> Client Console Application (B)");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(exitHandler);
            #region "Parse Command Line Parameters"
            //args[0] is the address of the server to connect
            if (args.Length > 0  && args[0].Trim().ToLower() == "--help")
            {
                Utility.PrintHelper();
                return 0 ;
            }
            try
            {
                //args = new string[] { "127.0.0.1:8888" };
                var _parts = args[0].Trim().ToLower().Split(':');
                _ipaddress = IPAddress.Parse(_parts[0]);
                _port = int.Parse(_parts[1]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(" >> Error occured while parsing command line arguments, please check help [Run B --help]");
                Console.ReadKey();
                return 1;
            }
            #endregion

            #region "Choice Menu For Record Type"
            int choice = 0;
            Console.WriteLine(" >> Please Choose A Record Type To Listen:");
            while (!(choice > 0 && choice <= 3))
            {
                Console.WriteLine(" >> 1. RED" +
                    Environment.NewLine + " >> 2. BLUE" +
                    Environment.NewLine + " >> 3. GREEN");
                Console.Write(" << ");
                var ans = Console.ReadLine();

                if (int.TryParse(ans, out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            _recordType = "Red";
                            break;
                        case 2:
                            _recordType = "Blue";
                            break;
                        case 3:
                            _recordType = "Green";
                            break;
                        //something for option 3
                        default:
                            Console.WriteLine("Wrong selection!!! Please make a valid choice.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("You must type numeric value only!!!");
                }
            }
            #endregion

            Client _this = new Client(_recordType);
            _this.ConnectServer(_ipaddress, _port, Utility.OnMessage);
            Console.ReadKey();
            return 0;
        }

        protected static void exitHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            Console.WriteLine("Application interupted, Press any key to exit...");
            Console.ReadKey();

        }

    }
}
