using Kaitai;
using Microsoft.Extensions.Logging;
using RCP.Parameters;
using RCP.Protocol;
using RCP.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using VL.Core;

namespace RCP
{
    public class RCPServer: ClientServerBase
    {
        List<int> FParamsToRemove = new List<int>();
		List<IServerTransporter> FTransporters = new List<IServerTransporter>();
        int FIdCounter = 1;
        ILogger FLog;

        public IReadOnlyDictionary<int, Parameter> Parameters => FParams;

        public string ApplicationId { get; }

        public RCPServer(string applicationId = "")
            : base()
        {
            ApplicationId = applicationId;
        }

        public RCPServer(IServerTransporter transporter, ILogger logger = null,  string applicationId = "")
            : this()
        {
            FLog = logger;
            AddTransporter(transporter);

            ApplicationId = applicationId;
        }

		public override void Dispose()
		{
            foreach (var transporter in FTransporters)
            {
                //transporter.Received = null; //.Dispose();
                transporter.Dispose();
            }
			
			FTransporters.Clear();
		}

        #region Parameters
        public Parameter CreateParameter(RcpTypes.Datatype type, string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, type);
            return AddAndReturn(param, label, group);
        }

        //public ArrayParameter<T> CreateArrayParameter<T>(string label = "", params int[] structure) => CreateArrayParameter<T>(label, null, structure);

        //public ArrayParameter<T> CreateArrayParameter<T>(string label = "", GroupParameter group = null, int[] structure = null)
        //{
        //    var elementType = TypeDefinition.GetDatatype(typeof(T));
        //    var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, elementType, structure) as ArrayParameter<T>;
        //    return AddAndReturn(param, label, group);
        //}

        public Parameter CreateBangParameter(string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Bang);
            return AddAndReturn(param, label, group);
        }

        //public Parameter CreateRangeParameter(RcpTypes.Datatype elementType, string label = "", GroupParameter group = null)
        //{
        //    var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Range, elementType);
        //    return AddAndReturn(param, label, group);
        //}

        public NumberParameter<T> CreateNumberParameter<T>(string label = "", GroupParameter group = null) /*where T : struct*/
        {
            var datatype = TypeDefinition.GetDatatype(typeof(T));
            var param = (NumberParameter<T>)CreateParameter(datatype, label, group);
            return AddAndReturn(param, label, group);
        }

        public ValueParameter<T> CreateValueParameter<T>(string label = "", GroupParameter group = null)
        {
            var datatype = TypeDefinition.GetDatatype(typeof(T));
            var param = CreateParameter(datatype, label, group) as ValueParameter<T>;
            return AddAndReturn(param, label, group);
        }

        public StringParameter CreateStringParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.String, label, group) as StringParameter;
            return AddAndReturn(param, label, group);
        }

        public UriParameter CreateUriParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.Uri, label, group) as UriParameter;
            return AddAndReturn(param, label, group);
        }

        //public ImageParameter CreateImageParameter(string label = "", GroupParameter group = null)
        //{
        //    var param = CreateParameter(RcpTypes.Datatype.Image, label, group) as ImageParameter;
        //    return AddAndReturn(param, label, group);
        //}

        //public ArrayParameter<string> CreateUriArrayParameter(string label, params int[] structure)
        //{
        //    var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, RcpTypes.Datatype.Uri, structure) as ArrayParameter<string>;
        //    param.Label = label;
        //    AddParameter(param);
        //    return param;
        //}

        public EnumParameter CreateEnumParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.Enum, label, group) as EnumParameter;
            return AddAndReturn(param, label, group);
        }

        //public ArrayParameter<string> CreateEnumArrayParameter(string label, params int[] structure)
        //{
        //    var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, RcpTypes.Datatype.Enum, structure) as ArrayParameter<string>;
        //    param.Label = label;
        //    AddParameter(param);
        //    return param;
        //}

        public GroupParameter CreateGroup(string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Group) as GroupParameter;
            return AddAndReturn(param, label, group);
        }

        TParameter AddAndReturn<TParameter>(TParameter param, string label, GroupParameter group) where TParameter : Parameter
        {
            param.Label = label;
            AddParameter(param, group);
            return param;
        }

        public void AddParameter(Parameter param, GroupParameter group)
        {
            base.AddParameter(param);

            if (group == null)
                group = Root;

            group.AddParameter(param);
        }

        public override void RemoveParameter(Parameter param)
        {
            FParams.Remove(param.Id);
            FParamsToRemove.Add(param.Id);
        }

        public override void Update()
        {
            foreach (var id in FParamsToRemove)
                SendToMultiple(Pack(RcpTypes.PacketTypes.Remove, id));
            FParamsToRemove.Clear();

            foreach (var parameter in FParams.Values)
                if (parameter.OnlyValueChanged)
                    SendToMultiple(Pack(RcpTypes.PacketTypes.Updatevalue, parameter));
                else if (parameter.IsDirty)
                    SendToMultiple(Pack(RcpTypes.PacketTypes.Update, parameter));
        }
        #endregion

		//public IEnumerable<IParameter> GetParametersByParent(Int16 id)
		//{
		//	return FParams.Values.Where(p => p.Parent.HasValue ? p.Parent.Value == id : false);
		//}

        public int ConnectionCount => FTransporters.Sum(t => t.ConnectionCount);

        #region Transporter
        public bool AddTransporter(IServerTransporter transporter)
        {
            if (!FTransporters.Contains(transporter))
            {
                transporter.Received = ReceiveFromClientCB;
                FTransporters.Add(transporter);
                return true;
            }

            return false;
        }

        public bool RemoveTransporter(IServerTransporter transporter)
        {
            if (FTransporters.Contains(transporter))
            {
                FTransporters.Remove(transporter);
                return true;
            }

            return false;
        }

        void ReceiveFromClientCB(byte[] bytes, string senderId)
		{
            FLog.LogDebug(senderId);
			try
            {
			    var packet = Packet.Parse(new KaitaiStream(bytes), this);
		        switch (packet.PacketType)
		        {
                    case RcpTypes.PacketTypes.Info:
                        {
                            var clientInfo = (packet.Data as InfoData);
                            FLog?.LogInformation(clientInfo.ToString());

                            //answer with an info data packet
                            var serverInfo = new InfoData();
                            serverInfo.AppId = ApplicationId;
                            serverInfo.AppVersion = "0.0";
                            serverInfo.RCPVersion = new Protocol.Version();
                            serverInfo.RCPVersion.Major = 1;
                            serverInfo.RCPVersion.Minor = 0;
                            serverInfo.HandshakeVersion = new Protocol.Version();
                            serverInfo.HandshakeVersion.Major = 1;
                            serverInfo.HandshakeVersion.Minor = 0;

                            var p = new Packet(RcpTypes.PacketTypes.Info);
                            p.Data = serverInfo;
                            SendToOne(p, senderId);

                            //check version 
                            if (Parser.IsVersionValid(serverInfo.HandshakeVersion, clientInfo.RCPVersion, clientInfo.HandshakeVersion))
                            {
                                FLog?.LogInformation("version good!");
                            }
                            else
                                FLog?.LogInformation("version no good!");

                            //shutdown or move on 

                            break;
                        }

                    //case RcpTypes.Command.Update:
                    //    {
                    //        Log?.Invoke("received: update");
                    //        var param = packet.Data as Parameter;
                    //        if (FParams.ContainsKey(param.Id))
                    //            SendToMultiple(bytes, senderId);
                    //        param.RaiseEvents();
                    //        break;
                    //    }


                    case RcpTypes.PacketTypes.Initialize:
                        {
                            var count = (int)packet.Data;
                            FLog?.LogInformation("init requests: {0}", [ count ]);

                            var initPack = new Packet(RcpTypes.PacketTypes.Initialize);
                            initPack.Data = Parameters.Count();
                            SendToOne(initPack, senderId);
                            
                            if (count == 0)
                            {
                                //client requests all parameters
                                foreach (var param in FParams.Values)
                                {
                                    param.ResetForInitialize();
                                    var updatePack = new Packet(RcpTypes.PacketTypes.Update);
                                    updatePack.Data = param;
                                    SendToOne(updatePack, senderId);
                                }
                            }

                            break;
                        }

                    case RcpTypes.PacketTypes.Updatevalue:
                        {
                            //TODO: actually only set the parameters value
                            FLog?.LogInformation("received: update value");
                            var param = packet.Data as Parameter;
                            if (FParams.ContainsKey(param.Id))
                                SendToMultiple(bytes, senderId);
                            param.RaiseEvents();
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        void SendToMultiple(Packet packet, string exceptClientId = "")
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                //Log?.Invoke("sending to multiple");
                packet.Write(writer);
                var bytes = stream.ToArray();
                foreach (var transporter in FTransporters)
                    transporter.SendToAll(bytes, exceptClientId);
            }
        }

        void SendToMultiple(byte[] bytes, string exceptClientId = "")
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                FLog?.LogInformation("sending to multiple");
                foreach (var transporter in FTransporters)
                    transporter.SendToAll(bytes, exceptClientId);
            }
        }

        void SendToOne(Packet packet, string clientId)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                //Log?.Invoke("sending to one");
                packet.Write(writer);
                var bytes = stream.ToArray();
                foreach (var transporter in FTransporters)
                    transporter.SendToOne(bytes, clientId);
            }
        }
        #endregion

    }
}