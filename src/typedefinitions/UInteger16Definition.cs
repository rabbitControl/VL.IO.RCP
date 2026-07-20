using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class UInteger16Definition : NumberDefinition<ushort>
    {
        protected override ushort DefaultMinimum => ushort.MinValue;
        protected override ushort DefaultMaximum => ushort.MaxValue;
        protected override ushort DefaultMulitpleOf => 1;

        public override ushort ReadValue(KaitaiStream input)
        {
            return input.ReadU2be();
        }

        public override void WriteValue(BinaryWriter writer, ushort value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}