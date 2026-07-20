using System;
using System.IO;
using System.Text;

using RCP.IO;

namespace RCP.Protocol
{
    public partial class RcpTypes
    {
        public partial class TinyString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    var length = (byte)Math.Min(byte.MaxValue, bytes.Length);
                    writer.Write(length);
                    writer.Write(bytes, 0, length);
                }
                else
                    writer.Write((byte)0);
            }
        }
    }

    public partial class RcpTypes
    {
        public partial class ShortString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    var length = (ushort)Math.Min(ushort.MaxValue, bytes.Length);
                    writer.Write(length, ByteOrder.BigEndian);
                    writer.Write(bytes, 0, length);
                }
                else
                    writer.Write((ushort)0);
            }
        }
    }
	
	public partial class RcpTypes
    {
        public partial class LongString
        {
            public static void Write(string text, BinaryWriter writer)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    var length = (int)Math.Min(int.MaxValue, bytes.Length);
                    writer.Write(length, ByteOrder.BigEndian);
                    writer.Write(bytes, 0, length);
                }
                else
                    writer.Write((int)0);
            }
        }
    }
}
