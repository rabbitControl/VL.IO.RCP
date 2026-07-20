using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class Integer64Definition : NumberDefinition<long>
    {
        protected override long DefaultMinimum => long.MinValue;
        protected override long DefaultMaximum => long.MaxValue;
        protected override long DefaultMulitpleOf => 1L;

        public override long ReadValue(KaitaiStream input)
        {
            return input.ReadS8be();
        }

        public override void WriteValue(BinaryWriter writer, long value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}