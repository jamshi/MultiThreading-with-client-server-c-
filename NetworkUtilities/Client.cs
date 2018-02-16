using JBP.ConnectionUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JBP.NetworkUtilities.Client
{
    public class Client
    {
        private  ManualResetEvent connectDone = new ManualResetEvent(false);
        private  string recordType { get; set; }
        private  TcpClient clientSocket = new TcpClient();

        public Client(string _recordType)
        {
            recordType = _recordType;

        }

        /// <summary>
        /// Establish a connection with the server
        /// </summary>
        /// <param name="_ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="_msgCallBack"></param>
        public  void ConnectServer(IPAddress _ipAddress, int port, Action<string, string> _msgCallBack, [Optional]CancellationToken _cancelToken)
        {
            _msgCallBack(string.Format("<LOG>Attempting to connect {0}:{1}...", _ipAddress.ToString(), port.ToString()), "Green");
           
            int _retryCount = 0;
            while (true)
            {
                
                try
                {
                    if (_cancelToken != null && _cancelToken.IsCancellationRequested)
                        break;
                    clientSocket.Connect(_ipAddress, port);
                    ConnectionUtility.WriteClientStream(clientSocket, recordType);
                    _msgCallBack("<LOG>Connection established with server", "Green");
                    ThreadPool.QueueUserWorkItem(o => readStream(clientSocket, _msgCallBack, _cancelToken));
                    connectDone.WaitOne();
                    break;
                        
                }
                catch (Exception ex)
                {
                    _retryCount++;
                    if (_retryCount > 10)
                    {
                        _msgCallBack("<LOG>Failed to connect the server specified, Retrying...", "Red");
                        _retryCount = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Read the record from server stream
        /// </summary>
        /// <param name="clientSocket">Connected Socket</param>
        /// <param name="_msgCallBack">Callback function on new message</param>
        public  void readStream(object clientSocket, Action<string, string> _msgCallBack, [Optional]CancellationToken _cancelToken)
        {
            _msgCallBack("<LOG>Fetching Records...", recordType);
            try
            {
                while (true)
                {
                    if (_cancelToken != null && _cancelToken.IsCancellationRequested)
                    {
                        connectDone.Set();
                        ((TcpClient)clientSocket).Close();
                        return;
                    }
                        
                    var msg = ConnectionUtility.ReadClientStream((TcpClient)clientSocket);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        _msgCallBack(msg, recordType);
                        ConnectionUtility.WriteClientStream((TcpClient)clientSocket, "<ACK>");
                    }
                    else
                        break;
                }
                _msgCallBack(((TcpClient)clientSocket).Connected ? "<LOG>Finished Processing records." : "<LOG>Connection Terminated.", recordType);
            }
            catch (Exception ex)
            {
                
            }
            finally{
                connectDone.Set();
                connectDone.Reset();
            }

        }
    }
}
