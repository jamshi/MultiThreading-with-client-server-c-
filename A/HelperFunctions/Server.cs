using JBP.ConnectionUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A.HelperFunctions
{
    class Server : ConnectionUtility
    {
        public static Dictionary<string, List<ConnectedClient>> Clients = new Dictionary<string, List<ConnectedClient>>();
        public static Dictionary<string, AutoResetEvent> TypeResetEvents = new Dictionary<string, AutoResetEvent>();
        public static Dictionary<string, bool> RecordProcessFinished = new Dictionary<string, bool>();
        static TcpListener serverSocket;
        static int counter = 0;

        /// <summary>
        /// Start listening for new clients on the interface specified
        /// </summary>
        public static void StartListening()
        {
            Console.WriteLine(" >> " + "Please specify the address to bind the listener (<IP>:<Port>):");
            Console.WriteLine(" >> Example - \"127.0.0.1:8888\" ");
            bool _success = false;
            while (!_success)
            {
                try
                {
                    Console.Write(" << ");
                    string _interface = Console.ReadLine();
                    var _parts = _interface.Split(':');
                    serverSocket = new TcpListener(IPAddress.Parse(_parts[0]), int.Parse(_parts[1]));
                    serverSocket.Start();
                    Console.WriteLine(" >> " + "Server Started...");
                    Console.WriteLine(" >> Please configure clients to listen at " +
                                                 IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
                                                    ":" + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());
                    _success = true;
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" >> " + ex.ToString());
                    Console.ResetColor();
                }
            }

            try
            {
                while (true)
                {
                    counter += 1;
                    TcpClient _client = serverSocket.AcceptTcpClient();
                    RegisterClient(_client, counter);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Register the new connected client
        /// </summary>
        /// <param name="inClient"></param>
        /// <param name="clientNo"></param>
        public static void RegisterClient(TcpClient inClient, int clientNo)
        {
            ConnectedClient _client = new ConnectedClient();
            _client.tcpClient = inClient;
            _client.clientNo = clientNo;
            string clientMsg = ReadClientStream(inClient).ToUpper().Trim();
            if (clientMsg != null && Utility.RecordTypes.Contains(clientMsg))
            {
                if (RecordProcessFinished[clientMsg])
                {
                    WriteClientStream(inClient, "<LOG> Process for the requested type already completed.");
                    inClient.Close();
                    return;
                }
                lock (Clients)
                {
                    if (!Clients.ContainsKey(clientMsg))
                        Clients.Add(clientMsg, new List<ConnectedClient>());
                    Clients[clientMsg].Add(_client);
                    if(Clients[clientMsg].Count == 1)
                        TypeResetEvents[clientMsg].Set();
                }
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format(" >> Registered client from {0}", _client.clientAddress));
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format(" >> Failed to register client from {0}, Invalid Type Registered ({1})",
                    _client.clientAddress, clientMsg));
                Console.ResetColor();
                WriteClientStream(inClient, "<LOG> Unsupported record type, Accepeted types are " + 
                    string.Join(", " , Utility.RecordTypes.ToArray()));
                inClient.Close();
            }

        }

        

        /// <summary>
        /// Process XML Records and send to registered clients
        /// </summary>
        /// <param name="type"></param>
        /// <param name="values"></param>
        public static void ProcessRecords(string type, List<string> values, bool fast_mode)
        {
            int _clientpos = 0;
            int _recordpos = 0;
            bool _block = false;
            while (_recordpos < values.Count())
            {
                lock (Clients)
                {
                    if (!Clients.ContainsKey(type) || Clients[type].Count == 0)
                        _block = true;
                    else
                        _block = false;
                }
                if(_block)
                    TypeResetEvents[type].WaitOne();
                lock (Clients[type])
                {
                    var _tcpclient = Clients[type][_clientpos].tcpClient;
                    if (WriteClientStream(_tcpclient, values[_recordpos]) && ReadClientStream(_tcpclient) == "<ACK>")
                        _recordpos++;
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" >> Unregistered client at " + Clients[type][_clientpos].clientAddress);
                        Console.ResetColor();
                        Clients[type].RemoveAt(_clientpos);
                    }
                    if (_clientpos < Clients[type].Count - 1)
                        _clientpos++;
                    else
                        _clientpos = 0;
                }
                if(!fast_mode)
                    Thread.Sleep(1000);
            }

            lock (Clients[type])
            {
                foreach (var client in Clients[type]) {
                    WriteClientStream(client.tcpClient, string.Format("<LOG>Completed Record Processing for type {0}.", type));
                    client.tcpClient.Close();
                    
                }
                Console.WriteLine(" >> Finished Processing " + type + " Records.");
                Clients[type].Clear();
                RecordProcessFinished[type] = true;
            }

        }
    }

    /// <summary>
    /// General Class to hold connected client details
    /// </summary>
    public class ConnectedClient {
        public TcpClient tcpClient { get; set; }

        public int clientNo { get; set; }

        public string clientAddress {
            get {
                var endpoint = ((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                return string.Format("{0}:{1}", endpoint.Address.ToString(), endpoint.Port.ToString());
            }
        }

    }


    
}
