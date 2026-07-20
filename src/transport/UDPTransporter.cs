using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RCP.Transporter
{
    public abstract class UDPTransporter
    {
        protected UdpClient FUDPSender;
        private int FListeningPort;
        private bool FListening;

        public UDPTransporter(string remoteHost, int remotePort, int listeningPort)
        {
            SetRemoteHostAndPort(remoteHost, remotePort);
            FListeningPort = listeningPort;
            StartListening();
        }

        private void StartListening()
        {
            FListening = true;
            var uiContext = SynchronizationContext.Current;

            Task.Run(async () =>
            {
                using (var udpClient = new UdpClient(FListeningPort))
                {
                    while (FListening)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var result = await udpClient.ReceiveAsync();// .Receive(ref remoteEndPoint);

                        if (result.Buffer.Length > 0)
                            uiContext.Post((b) => OnReceived(b as byte[]), result.Buffer);
                    }
                }
            });
        }

        private void StopListening()
        {
            FListening = false;
        }

        protected abstract void OnReceived(byte[] bytes);

        public void Dispose()
        {
            StopListening();

            if (FUDPSender != null)
            {
                FUDPSender.Close();
                FUDPSender.Dispose();
            }
        }

        public void SetRemoteHostAndPort(string host, int port)
        {
            if (FUDPSender != null)
            {
                FUDPSender.Close();
                FUDPSender.Dispose();
            }

            FUDPSender = new UdpClient(host, port);
        }

        public void SetListeningPort(int port)
        {
            StopListening();
            FListeningPort = port;
            StartListening();
        }
    }

    public class UDPServerTransporter: UDPTransporter, IServerTransporter
	{
        private string FServerId;
        public Action<byte[], string> Received { get; set; }

        public int ConnectionCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UDPServerTransporter(string remoteHost, int remotePort, int listeningPort):
            base(remoteHost, remotePort, listeningPort)
		{ 
            FServerId = Guid.NewGuid().ToString();
        }

        protected override void OnReceived(byte[] bytes)
        {
            Received?.Invoke(bytes, FServerId);
        }
		
		public void SendToAll(byte[] bytes, string exceptId)
		{
            FUDPSender.Send(bytes, bytes.Length);
		}

        public void SendToOne(byte[] bytes, string id)
        {
            if (id == FServerId)
                FUDPSender.Send(bytes, bytes.Length);
        }

        public void Bind(int port)
        {
            throw new NotImplementedException();
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }

        public void Bind(string host, int port)
        {
            throw new NotImplementedException();
        }
    }
	
	public class UDPClientTransporter: UDPTransporter, IClientTransporter
	{
		public Action<byte[]> Received {get; set;}

        public bool IsConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Action Connected
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Action Disconnected
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnReceived(byte[] bytes)
        {
            Received?.Invoke(bytes);
        }

        public UDPClientTransporter(string remoteHost, int remotePort, int listeningPort) :
            base(remoteHost, remotePort, listeningPort)
        { }
		
		public void Send(byte[] bytes)
		{
			var r = FUDPSender.Send(bytes, bytes.Length);
		}

        public void Connect(string host, int port, bool ssl)
        {
            throw new NotImplementedException();
        }

        public void Connect(Uri url)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}