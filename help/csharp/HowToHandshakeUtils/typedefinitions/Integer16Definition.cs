using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class Integer16Definition : NumberDefinition<short>
    {
        protected override short DefaultMinimum => short.MinValue;
        protected override short DefaultMaximum => short.MaxValue;
        protected override short DefaultMulitpleOf => 1;

        public override short ReadValue(KaitaiStream input)
        {
            return input.ReadS2be();
        }

        public override void WriteValue(BinaryWriter writer, short value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}