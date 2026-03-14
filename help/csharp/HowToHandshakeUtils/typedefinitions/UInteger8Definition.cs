using Kaitai;
using System.IO;

namespace RCP.Types
{
    public class UInteger8Definition : NumberDefinition<byte>
    {
        protected override byte DefaultMinimum => byte.MinValue;
        protected override byte DefaultMaximum => byte.MaxValue;
        protected override byte DefaultMulitpleOf => 1;

        public override byte ReadValue(KaitaiStream input)
        {
            return input.ReadU1();
        }

        public override void WriteValue(BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }
    }
}