using System.IO;
using System.Numerics;

using Kaitai;
using RCP.IO;
using Vector3 = System.Numerics.Vector3;

namespace RCP.Types
{
    public class Vector3f32Definition : NumberDefinition<Vector3>
    {
        protected override Vector3 DefaultMinimum => new Vector3(float.MinValue);
        protected override Vector3 DefaultMaximum => new Vector3(float.MaxValue);
        protected override Vector3 DefaultMulitpleOf => new Vector3(0.01f);

        public override Vector3 ReadValue(KaitaiStream input)
        {
            return new Vector3(input.ReadF4be(), input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
            writer.Write(value.Z, ByteOrder.BigEndian);
        }
    }
}