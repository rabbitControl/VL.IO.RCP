using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatsonWebsocket;
using System.Threading;
//using VVVV.Core.Logging;

namespace RCP.Transporter
{
    public class WebsocketServerTransporter: IServerTransporter
    {
        private SynchronizationContext FContext;
        private WatsonWsServer FServer;

    	public Action<byte[], string> Received {get; set;}
    	public int ConnectionCount => FServer.ListClients().Count();
    	
        public WebsocketServerTransporter(string remoteHost, int port)
        {
            FContext = SynchronizationContext.Current;

            CreateServer(remoteHost, port);
        }

        public void Dispose()
        {
            Unbind();
        }

        private void CreateServer(string remoteHost, int port)
        {
            FServer = new WatsonWsServer(remoteHost, port, false);
            FServer.MessageReceived += FServer_MessageReceived;

            FServer.Start();
        }

        private void FServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Data.Count > 0)
                FContext.Post((b) => Received?.Invoke(b as byte[], e.Client.Guid.ToString()), e.Data.ToArray());
        }

        public void Bind(string remoteHost, int port)
        {
            Unbind();
            CreateServer(remoteHost, port);
        }

        public void Unbind()
        {
            if (FServer != null)
            {
                foreach (var client in FServer.ListClients())
                    FServer.DisconnectClient(client.Guid);
                try
                {
                    FServer.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendToAll(byte[] bytes, string exceptId)
        {
            foreach (var client in FServer.ListClients())
                if (string.IsNullOrEmpty(exceptId) || client.Guid.ToString() != exceptId)
                    FServer.SendAsync(client.Guid, bytes);
        }

        public void SendToOne(byte[] bytes, string id)
        {
            var targetClient = FServer.ListClients().FirstOrDefault(c => c.Guid.ToString() == id);
            if (targetClient != null)
                FServer.SendAsync(targetClient.Guid, bytes);
        }
    }
}
