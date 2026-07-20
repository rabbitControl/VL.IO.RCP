using System.IO;
using System.Numerics;

using Kaitai;
using RCP.IO;
using Vector4 = System.Numerics.Vector4;

namespace RCP.Types
{
    public class Vector4f32Definition : NumberDefinition<Vector4>
    {
        protected override Vector4 DefaultMinimum => new Vector4(float.MinValue);
        protected override Vector4 DefaultMaximum => new Vector4(float.MaxValue);
        protected override Vector4 DefaultMulitpleOf => new Vector4(0.01f);

        public override Vector4 ReadValue(KaitaiStream input)
        {
            return new Vector4(input.ReadF4be(), input.ReadF4be(), input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector4 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
            writer.Write(value.Z, ByteOrder.BigEndian);
            writer.Write(value.W, ByteOrder.BigEndian);
        }
    }
}