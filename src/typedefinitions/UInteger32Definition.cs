using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class UInteger32Definition : NumberDefinition<uint>
    {
        protected override uint DefaultMinimum => uint.MinValue;
        protected override uint DefaultMaximum => uint.MaxValue;
        protected override uint DefaultMulitpleOf => 1;

        public override uint ReadValue(KaitaiStream input)
        {
            return input.ReadU4be();
        }

        public override void WriteValue(BinaryWriter writer, uint value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}