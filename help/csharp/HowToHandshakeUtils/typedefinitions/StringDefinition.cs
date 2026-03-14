using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameters;

namespace RCP.Types
{                           
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        string FRegEx = "";

        public StringDefinition()
            : base(RcpTypes.Datatype.String, string.Empty)
        {
        }

        public string RegularExpression
        {
            get => FRegEx;
            set
            {
                if (SetProperty(ref FRegEx, value))
                    SetChanged(TypeChangedFlags.StringRegexp);
            }
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new StringParameter(id, manager, this);

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            if (RegularExpression != "")
                SetChanged(TypeChangedFlags.StringRegexp);
        }

        public override string ReadValue(KaitaiStream input)
        {
            return "";//new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (IsChanged(TypeChangedFlags.StringRegexp))
            {
                writer.Write((byte)RcpTypes.StringOptions.RegularExpression);
                RcpTypes.TinyString.Write(FRegEx, writer);
                writer.Write((byte)0);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            if (base.HandleOption(input, code))
                return true;

            var option = (RcpTypes.StringOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.StringOptions), option))
                throw new RCPDataErrorException("StringDefinition parsing: Unknown option: " + option.ToString());

            //switch (option)
            //{
            //    case RcpTypes.StringOptions.RegularExpression:
            //        RegularExpression = new RcpTypes.TinyString(input).Data;
            //        return true;
            //}

            return false;
        }
    }
}
