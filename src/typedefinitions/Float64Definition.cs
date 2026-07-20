using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class Float64Definition : NumberDefinition<double>
    {
        protected override double DefaultMinimum => double.MinValue;
        protected override double DefaultMaximum => double.MaxValue;
        protected override double DefaultMulitpleOf => 0.01d;

        public override double ReadValue(KaitaiStream input)
        {
            return input.ReadF8be();
        }

        public override void WriteValue(BinaryWriter writer, double value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}