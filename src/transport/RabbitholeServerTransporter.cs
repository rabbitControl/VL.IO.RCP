using System;

using WatsonWebsocket;
using System.Threading;
using System.Linq;
//using VVVV.Core.Logging;

namespace RCP.Transporter
{
    public class RabbitholeServerTransporter: IServerTransporter
    {
        private SynchronizationContext FContext;
        private WatsonWsClient FClient;
        private string FClientId;
        private string FRemoteHost;
        private System.Timers.Timer FTimer = new System.Timers.Timer(2000);

    	public Action<byte[], string> Received {get; set;}
    	public int ConnectionCount => (FClient?.Connected ?? false) ? 1 : 0;

        public RabbitholeServerTransporter(string remoteHost)
        {
            FContext = SynchronizationContext.Current;

            FClientId = Guid.NewGuid().ToString();
            FRemoteHost = remoteHost;
            Bind(remoteHost, 0);
            
            FTimer.Elapsed += FTimer_Elapsed;
            FTimer.Start();
        }

        private void FTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (FClient != null && !FClient.Connected)
            {
                Bind(FRemoteHost, 0);
            }
        }

        public void Dispose()
        {
            FTimer.Stop();
            FTimer.Elapsed -= FTimer_Elapsed;
            Unbind();
        }

        private void CreateClient(string remoteHost)
        {
            if (Uri.TryCreate(remoteHost, UriKind.Absolute, out Uri uri))
            {
                FClient = new WatsonWsClient(uri);

                FClient.MessageReceived += FClient_MessageReceived;
                FClient.ServerConnected += FClient_ServerConnected;
                FClient.ServerDisconnected += FClient_ServerDisconnected;

                FClient.Start();
            }
        }

        private void FClient_ServerDisconnected(object sender, EventArgs e)
        {
            FTimer.Start();
        }

        private void FClient_ServerConnected(object sender, EventArgs e)
        {
            FTimer.Stop();
        }

        private void FClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Data.Count > 0)
                FContext.Post((b) => Received?.Invoke(b as byte[], FClientId), e.Data.ToArray());
        }

        public void Bind(string remoteHost, int port)
        {
            Unbind();
            CreateClient(remoteHost);
        }

        public void Unbind()
        {
            if (FClient != null)
            {
                try
                {
                    FClient.ServerConnected -= FClient_ServerConnected;
                    FClient.ServerDisconnected -= FClient_ServerDisconnected;
                    FClient.MessageReceived -= FClient_MessageReceived;
                    FClient.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendToAll(byte[] bytes, string exceptId)
        {
            if (FClient?.Connected ?? false)
                if (string.IsNullOrEmpty(exceptId) || FClientId != exceptId) 
                    FClient.SendAsync(bytes);
        }

        public void SendToOne(byte[] bytes, string id)
        {
            if (FClient?.Connected ?? false)
                if (id == FClientId)
                    FClient.SendAsync(bytes);
        }
    }
}
