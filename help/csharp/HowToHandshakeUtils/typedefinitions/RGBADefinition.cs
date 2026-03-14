using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using System.Drawing;
using RCP.Parameters;
using Color = System.Drawing.Color;

namespace RCP.Types
{                           
    public class RGBADefinition : DefaultDefinition<Color>, IRGBADefinition
    {
        public RGBADefinition()
        : base(RcpTypes.Datatype.Rgba, Color.Black)
        {
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ValueParameter<Color>(id, manager, this);

        public override Color ReadValue(KaitaiStream input)
        {
            var a = input.ReadU1();
            var b = input.ReadU1();
            var g = input.ReadU1();
            var r = input.ReadU1();
            return Color.FromArgb(a, r, g, b);
        }

        public override void WriteValue(BinaryWriter writer, Color value)
        {
            writer.Write((byte)value.A);
            writer.Write((byte)value.B);
            writer.Write((byte)value.G);
            writer.Write((byte)value.R);
        }
    }
}
