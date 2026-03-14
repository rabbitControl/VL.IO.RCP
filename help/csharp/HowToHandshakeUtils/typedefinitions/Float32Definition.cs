using System.IO;

using Kaitai;
using RCP.IO;

namespace RCP.Types
{
    public class Float32Definition : NumberDefinition<float>
    {
        protected override float DefaultMinimum => float.MinValue;
        protected override float DefaultMaximum => float.MaxValue;
        protected override float DefaultMulitpleOf => 0.01f;

        public override float ReadValue(KaitaiStream input)
        {
            return input.ReadF4be();
        }

        public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}