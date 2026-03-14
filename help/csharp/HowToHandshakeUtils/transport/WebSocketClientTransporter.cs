using System;
using System.Threading;
using WatsonWebsocket;

namespace RCP.Transporter
{
    public class WebsocketClientTransporter: IClientTransporter
    {
        private WatsonWsClient FClient;
        private SynchronizationContext FContext;

        public Action<byte[]> Received { get; set; }
        public Action Connected { get; set; }
        public Action Disconnected { get; set; }

        public bool IsConnected => FClient?.Connected ?? false;

        public WebsocketClientTransporter()
        {
            FContext = SynchronizationContext.Current;
        }

        public void Dispose()
        {
            DestroyClient();
        }

        private void DestroyClient()
        {
            if (FClient != null)
            {
                FClient.ServerConnected -= FClient_ServerConnected;
                FClient.ServerDisconnected -= FClient_ServerDisconnected;
                FClient.MessageReceived -= FClient_MessageReceived;
                FClient.Dispose();
            }
        }

        private void CreateClient(string remoteHost, int port)
        {
            
        }

        private void FClient_ServerDisconnected(object sender, EventArgs e)
        {
            FContext.Post((b) => Disconnected?.Invoke(), null);
        }

        private void FClient_ServerConnected(object sender, EventArgs e)
        {
            FContext.Post((b) => Connected?.Invoke(), null);
        }

        private void FClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Data.Count > 0)
            {
                FContext.Post((b) => Received?.Invoke(b as byte[]), e.Data.ToArray());
            }
        }

        public void Connect(string remoteIP, int port, bool ssl)
        {
            DestroyClient();
            FClient = new WatsonWsClient(remoteIP, port, ssl);
            FClient.MessageReceived += FClient_MessageReceived;
            FClient.ServerConnected += FClient_ServerConnected;
            FClient.ServerDisconnected += FClient_ServerDisconnected;
            FClient.Start();
        }

        public void Connect(Uri url)
        {
            DestroyClient();
            FClient = new WatsonWsClient(url);
            FClient.MessageReceived += FClient_MessageReceived;
            FClient.ServerConnected += FClient_ServerConnected;
            FClient.ServerDisconnected += FClient_ServerDisconnected;
            FClient.Start();
        }

        public void Disconnect()
        {
            DestroyClient();
        }

        public void Send(byte[] bytes)
        {
            if (FClient != null)
                FClient.SendAsync(bytes);
        }
    }
}
