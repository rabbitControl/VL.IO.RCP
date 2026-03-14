using System.IO;

using Kaitai;
using RCP.IO;
using RCP.Protocol;
using RCP.Parameters;

namespace RCP.Types
{                           
    public class ImageDefinition : DefaultDefinition<byte[]>, IImageDefinition
    {
        public ImageDefinition()
            : base(RcpTypes.Datatype.Image, new byte[0])
        {
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ImageParameter(id, manager, this);

        public override byte[] ReadValue(KaitaiStream input)
        {
            var size = input.ReadS4be();
            return input.ReadBytes(size);
        }

        public override void WriteValue(BinaryWriter writer, byte[] value)
        {
            writer.Write(value.Length, ByteOrder.BigEndian);
            writer.Write(value);
        }
    }
}