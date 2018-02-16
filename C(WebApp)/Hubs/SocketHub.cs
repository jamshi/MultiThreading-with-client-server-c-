using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using JBP.NetworkUtilities.Client;
using System.Net;
using System.Threading;
using System.Web.Configuration;

namespace C_WebApp_.Hubs
{
    public class SocketHub : Hub
    {
        public static List<WebClient> webClients = new List<WebClient>();

        public override Task OnConnected()
        {
            var _recordType = Context.QueryString["type"];
            Client _client = new Client(_recordType);
            WebClient _webClient = new WebClient();
            _webClient.client = Context.ConnectionId;
            _webClient.cancelToken = new CancellationTokenSource();
            try
            {
                var _listener_address = WebConfigurationManager.AppSettings["ListenerAddress"];
                var _parts = _listener_address.Trim().ToLower().Split(':');
                var _ipaddress = IPAddress.Parse(_parts[0]);
                int _port = int.Parse(_parts[1]);
                Task.Factory.StartNew(() => _client.ConnectServer(_ipaddress, _port,
                _webClient.OnMessage, _webClient.cancelToken.Token));
                webClients.Add(_webClient);
            }
            catch (Exception ex)
            {

            }
            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                var _webClient = webClients.FirstOrDefault(i => i.client == Context.ConnectionId);
                _webClient.cancelToken.Cancel();
                webClients.Remove(_webClient);
            }
            catch(Exception ex)
            {

            }
            return base.OnDisconnected(stopCalled);
        }

    }

    public class WebClient
    {
        public string client { get; set; }

        public CancellationTokenSource cancelToken { get; set; }
        public void OnMessage(string message, string type)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SocketHub>();
            if(message.TrimStart().StartsWith("<LOG>"))
                context.Clients.Client(client).AddLogToPage(message.Replace("<LOG>", ""), type);
            else
                context.Clients.Client(client).AddNewMessageToPage(message, type);
        }
    }

}