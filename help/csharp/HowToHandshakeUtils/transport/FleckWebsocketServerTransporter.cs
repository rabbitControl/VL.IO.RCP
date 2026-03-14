using System;
using System.Collections.Generic;
using System.Linq;

using Fleck;
using System.Threading;

namespace RCP.Transporter
{
    public class FleckWebsocketServerTransporter: IServerTransporter
    {
        private SynchronizationContext FContext;
        private WebSocketServer FServer;
        private Dictionary<string, IWebSocketConnection> FSockets = new Dictionary<string, IWebSocketConnection>();

    	public Action<byte[], string> Received {get; set;}
    	public int ConnectionCount => FSockets.Count;
    	
        public FleckWebsocketServerTransporter(string remoteHost, int port)
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
            FServer = new WebSocketServer("ws://" + remoteHost + ":" + port.ToString());
            FServer.Start(socket => {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    FSockets.Add(socket.ConnectionInfo.Id.ToString(), socket);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    FSockets.Remove(socket.ConnectionInfo.Id.ToString());
                };

                socket.OnMessage = message =>
                {
                    //this shouldn't be necessary
                    //if (message.Length > 0)
                    //    FContext.Post((m) => Received?.Invoke(Encoding.Default.GetBytes(m as string), socket.ConnectionInfo.Id), message);
                };

                socket.OnBinary = bytes =>
                {
                    if (bytes.Length > 0)
                        FContext.Post((b) => Received?.Invoke(b as byte[], socket.ConnectionInfo.Id.ToString()), bytes);
                };
            });
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
                FSockets.Keys.ToList().ForEach(k => {
                    FSockets[k].Close();
                });
                FSockets.Clear();
                FServer.Dispose();
            }
        }

        public void SendToAll(byte[] bytes, string exceptId)
        {
            FSockets.Keys.ToList().ForEach(k => {
            	if (string.IsNullOrEmpty(exceptId) || k != exceptId)
                    FSockets[k].Send(bytes);
            });
        }

        public void SendToOne(byte[] bytes, string id)
        {
            IWebSocketConnection socket;
            if (FSockets.TryGetValue(id, out socket))
                socket.Send(bytes);
        }
    }
}
