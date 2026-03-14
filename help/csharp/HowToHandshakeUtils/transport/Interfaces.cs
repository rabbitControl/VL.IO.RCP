using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
        void Bind(string host, int port);
        void Unbind();
        int ConnectionCount { get; }
		void SendToAll(byte[] bytes, string exceptId);
        void SendToOne(byte[] bytes, string id);
        Action<byte[], string> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
        void Connect(string ip, int port, bool ssl);
        void Connect(Uri uri);
        void Disconnect();
        bool IsConnected { get; }
		void Send(byte[] bytes);

        Action Connected { get; set; }
        Action Disconnected { get; set; }
        Action<byte[]> Received {get; set;}
    }
}