using Kaitai;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text;
using static RCP.Protocol.RcpTypes;

namespace RCP.Protocol;

public static class Parser
{
    public static RcpTypes.PacketTypes ReadPacketType(KaitaiStream stream)
    { 
        stream.ReadBitsInt(3);
        return (RcpTypes.PacketTypes)stream.ReadBitsInt(5);
    }
    public static Version ReadVersion(KaitaiStream stream)
    {
        var v = new Version();
        v.Major = stream.ReadByte();
        v.Minor = stream.ReadByte();
        return v;
    }

    public static InfoData ReadInfoData(KaitaiStream stream)
    {
        var nfo = new InfoData();
        nfo.RCPVersion = ReadVersion(stream);
        nfo.HandshakeVersion = ReadVersion(stream);
        while (true)
        {
            var b = stream.ReadByte();
            if (b == 128)
                break;

            var option = (InfodataOptions)(b & ~128);
            switch (option)
            {
                case InfodataOptions.Applicationid: nfo.AppId = ReadString(stream); break;
                case InfodataOptions.Applicationversion: nfo.AppVersion = ReadString(stream); break;
                default:
                    {
                        //raise error
                        break;
                    }
            }

            if ((b & 128) > 0)
                break;
        }
        return nfo;
    }
    //public static KaitaiStream ParseInt(KaitaiStream stream, out int value)
    //{
    //    value = 0;
    //    for (int i = 0; i < 4; i++)
    //    {
    //        var b = stream.ReadByte();

    //        value += (b & ~128) << (i * 7);
    //        if ((b & 128) > 0)
    //            break;
    //    }

    //    return stream;
    //}

    //"big endian"
    public static int ReadInt(KaitaiStream stream)
    {
        var value = 0;
        for (int i = 0; i < 4; i++)
        {
            var b = stream.ReadByte();

            value = value << 7;
            value += b & ~128;
            if ((b & 128) > 0)
                break;
        }

        return value;
    }

    public static string ReadString(KaitaiStream stream)
    {
        var count = ReadInt(stream);
        return Encoding.UTF8.GetString(stream.ReadBytes(count));
    }

    public static byte[] AddVersionBytes(Version version)
    {
        return [ version.Major, version.Minor ];
    }

    public static byte[] AddInfoDataBytes(InfoData info)
    {
        var bytes = new List<byte>();
        bytes.AddRange(AddVersionBytes(info.RCPVersion));
        bytes.AddRange(AddVersionBytes(info.HandshakeVersion));

        var hasAppId = !string.IsNullOrEmpty(info.AppId);
        var hasAppVersion = !string.IsNullOrEmpty(info.AppVersion);
        if (!hasAppId && !hasAppVersion)
            bytes.Add(128);
        else
        {
            if (hasAppId)
                bytes.AddRange(AddStringOptionBytes(InfodataOptions.Applicationid, !hasAppVersion, info.AppId));
            if (hasAppVersion)
                bytes.AddRange(AddStringOptionBytes(InfodataOptions.Applicationversion, true, info.AppVersion));
        }

        return bytes.ToArray();
    }

    //public static List<byte> AddIntBytes(List<byte> bytes, int value)
    //{
    //    var v = value;
    //    var i = 0;
    //    while (i < 4)
    //    {
    //        byte b = (byte)(v & 127); //take 7 bits
    //        v = v >> 7; //shift them right 

    //        if (v == 0)
    //        {
    //            bytes.Add((byte)(b | 128));
    //            break;
    //        }
    //        else
    //            bytes.Add(b);

    //        i++;
    //    }

    //    return bytes;
    //}

    public static byte AddOptionId(ParameterOptions optionId, bool optionsFollow)
    {
        var oid = (byte)optionId;
        if (!optionsFollow)
            oid |= 128;
        return oid;
    }

    //"big endian"
    public static byte[] AddIntBytes(int value)
    {
        var bytes = new List<byte>();
        if (value < 0x4000)
        {
            if (value < 0x80)
            {
                // 1 byte
                bytes.Add((byte)((value & 0x7F) | 0x80));
            }
            else
            {
                // 2 bytes
                bytes.Add((byte)((value >> 7) & 0x7F));
                bytes.Add((byte)((value & 0x7F) | 0x80));
            }
        }
        else
        {
            if (value < 0x200000)
            {
                //3 bytes
                bytes.Add((byte)((value >> 14) & 0x7F));
                bytes.Add((byte)((value >> 7) & 0x7F));
                bytes.Add((byte)((value & 0x7F) | 0x80));
            }
            else
            {
                // 4 bytes
                bytes.Add((byte)((value >> 21) & 0x7F));
                bytes.Add((byte)((value >> 14) & 0x7F));
                bytes.Add((byte)((value >> 7) & 0x7F));
                bytes.Add((byte)((value & 0x7F) | 0x80));
            }
        }
        
        return bytes.ToArray();
    }

    public static byte[] AddStringBytes(string value)
    {
        var bytes = new List<byte>();
        bytes.AddRange(AddIntBytes(value.Length));
        bytes.AddRange(Encoding.UTF8.GetBytes(value));

        return bytes.ToArray();
    }

    public static byte[] AddStringOptionBytes(InfodataOptions option, bool isLastOption, string value)
    {
        if (isLastOption)
            option = (InfodataOptions)((byte)option | 128);

        var bytes = new List<byte>();
        bytes.Add((byte)option);
        bytes.AddRange(AddStringBytes(value));

        return bytes.ToArray();
    }

    public static byte[] AddPacketBytes(PacketTypes type)
    {
        return [ (byte)type ];
    }

    public static bool IsVersionValid(Version serverHandshakeVersion, Version clientRCPVersion, Version clientHandshakeVersion)
    {
        return serverHandshakeVersion.ToFloat() <= clientRCPVersion.ToFloat() && serverHandshakeVersion.ToFloat() >= clientHandshakeVersion.ToFloat();
    }
}
