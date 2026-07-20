using System.IO;
using System.Numerics;

using Kaitai;
using RCP.IO;
using Vector2 = System.Numerics.Vector2;

namespace RCP.Types
{
    public class Vector2f32Definition : NumberDefinition<Vector2>
    {
        protected override Vector2 DefaultMinimum => new Vector2(float.MinValue);
        protected override Vector2 DefaultMaximum => new Vector2(float.MaxValue);
        protected override Vector2 DefaultMulitpleOf => new Vector2(0.01f);

        public override Vector2 ReadValue(KaitaiStream input)
        {
            return new Vector2(input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
        }
    }
}