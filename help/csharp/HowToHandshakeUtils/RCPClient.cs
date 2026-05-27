using System;
using System.IO;

using Kaitai;
using RCP.Protocol;
using RCP.Types;
using RCP.Parameters;
using System.Threading;
using System.ComponentModel;

namespace RCP
{
    public class RCPClient : ClientServerBase
	{
		private IClientTransporter FTransporter;

        public string ApplicationId { get; }
        public string ConnectedServerVersion { get; private set; }
        public string ConnectedServerApplicationId { get; private set; }

        public RCPClient(string applicationId = "")
        {
            ApplicationId = applicationId;
        }

        public RCPClient(IClientTransporter transporter, string applicationId = "") 
        {
            SetTransporter(transporter);

            ApplicationId = applicationId;
        }

		public override void Dispose()
		{
			if (FTransporter != null)
                FTransporter.Dispose();
		}

        public Action<Exception> OnError;
        //public Action<RcpTypes.ClientStatus, string> StatusChanged;

        public void Connect(string host, int port, bool ssl)
        {
            if (FTransporter.IsConnected)
                FTransporter.Disconnect();
            FTransporter.Connect(host, port, ssl);
        }

        public void Connect(Uri url)
        {
            if (FTransporter.IsConnected)
                FTransporter.Disconnect();
            FTransporter.Connect(url);
        }

        public void Disonnect()
        {
            FTransporter.Disconnect();
        }

        public void Initialize()
		{
            FParams.Clear();
            SendPacket(Pack(RcpTypes.PacketTypes.Initialize, 0));
		}

        public void SendInfo()
        {
            var clientInfo = new InfoData();
            clientInfo.AppId = ApplicationId;
            clientInfo.AppVersion = "0.0";
            clientInfo.RCPVersion = new Protocol.Version();
            clientInfo.RCPVersion.Major = 1;
            clientInfo.RCPVersion.Minor = 0;
            clientInfo.HandshakeVersion = new Protocol.Version();
            clientInfo.HandshakeVersion.Major = 1;
            clientInfo.HandshakeVersion.Minor = 0;
            SendPacket(Pack(RcpTypes.PacketTypes.Info, clientInfo));
        }

        public override void Update()
        {
            foreach (var parameter in FParams.Values)
                if (parameter.OnlyValueChanged)
                    SendPacket(Pack(RcpTypes.PacketTypes.Updatevalue, parameter));
                else if (parameter.IsDirty)
                    SendPacket(Pack(RcpTypes.PacketTypes.Update, parameter));
        }

        #region Transporter
        public void SetTransporter(IClientTransporter transporter)
        {
            var previousTransporter = Interlocked.Exchange(ref FTransporter, transporter);
            if (previousTransporter != null)
            {
                previousTransporter.Received = null;
                previousTransporter.Dispose();
            }
            if (transporter != null)
                transporter.Received = ReceiveCB;
        }

        void ReceiveCB(byte[] bytes)
		{
			//Logger.Log(LogType.Debug, "Client received: " + bytes.Length + "bytes");
			var packet = Packet.Parse(new KaitaiStream(bytes), this);
            if (packet == null)
                return;

			//Logger.Log(LogType.Debug, packet.Command.ToString());
			switch (packet.PacketType)
			{
                case RcpTypes.PacketTypes.Info:
                    {
                       //check if version matches
                       var serverInfo = (packet.Data as InfoData);
                       ConnectedServerVersion = serverInfo.AppVersion;
                       ConnectedServerApplicationId = serverInfo.AppId;
                       break;
                    }

                case RcpTypes.PacketTypes.Update:
                        {
                            var parameter = packet.Data as Parameter;
                            var id = parameter.Id;
                            
                            if (!FParams.ContainsKey(id))
                                AddParameter(parameter);
                            else
                                parameter.RaiseEvents();
				            break;
                        }

                case RcpTypes.PacketTypes.Updatevalue:
                        {
                            var parameter = packet.Data as Parameter;
                            var id = parameter.Id;

                            if (FParams.ContainsKey(id))
                                parameter.RaiseEvents();
                            break;
                        }

                case RcpTypes.PacketTypes.Remove:
                        {
                            var id = (short)packet.Data;

                            if (FParams.ContainsKey(id))
                                RemoveParameter(FParams[id]);
                            break;
                        }
			}
		}
		
		void SendPacket(Packet packet)
		{
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				packet.Write(writer);
				FTransporter.Send(stream.ToArray());
			}
		}
        #endregion
    }
}