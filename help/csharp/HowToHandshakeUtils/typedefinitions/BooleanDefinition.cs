using System;
using System.IO;

using Kaitai;
using RCP.IO;
using RCP.Parameters;
using RCP.Protocol;

namespace RCP.Types
{                           
    public class BooleanDefinition : DefaultDefinition<bool>, IBoolDefinition
    {
        public BooleanDefinition()
            : base(RcpTypes.Datatype.Boolean, false)
        {
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new ValueParameter<bool>(id, manager, this);

        public override bool ReadValue(KaitaiStream input)
        {
            return input.ReadU1() > 0;
        }

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}
