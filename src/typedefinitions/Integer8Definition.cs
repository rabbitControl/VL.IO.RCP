using Kaitai;
using System.IO;

namespace RCP.Types
{
    public class Integer8Definition : NumberDefinition<sbyte>
    {
        protected override sbyte DefaultMinimum => sbyte.MinValue;
        protected override sbyte DefaultMaximum => sbyte.MaxValue;
        protected override sbyte DefaultMulitpleOf => 1;

        public override sbyte ReadValue(KaitaiStream input)
        {
            return input.ReadS1();
        }

        public override void WriteValue(BinaryWriter writer, sbyte value)
        {
            writer.Write(value);
        }
    }
}