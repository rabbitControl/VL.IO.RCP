using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameters;

namespace RCP.Types
{
    public class UriDefinition : DefaultDefinition<string>, IUriDefinition
    {
        string FSchema = "";
        string FFilter = "";

        public UriDefinition()
            : base(RcpTypes.Datatype.Uri, string.Empty)
        {
        }

        public string Schema
        {
            get => FSchema;
            set
            {
                if (SetProperty(ref FSchema, value))
                    SetChanged(TypeChangedFlags.UriSchemaChanged);
            }
        }

        public string Filter
        {
            get => FFilter;
            set
            {
                if (SetProperty(ref FFilter, value))
                    SetChanged(TypeChangedFlags.UriFilterChanged);
            }
        }

        public override Parameter CreateParameter(int id, IParameterManager manager) => new UriParameter(id, manager, this);

        public override void ResetForInitialize()
        {
            if (FSchema != "")
                SetChanged(TypeChangedFlags.UriSchemaChanged);
            if (FFilter != "")
                SetChanged(TypeChangedFlags.UriFilterChanged);
        }

        public override string ReadValue(KaitaiStream input)
        {
            return "";//new RcpTypes.TinyString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.TinyString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (IsChanged(TypeChangedFlags.UriSchemaChanged))
            {
                writer.Write((byte)RcpTypes.UriOptions.Schema);
                WriteValue(writer, FSchema);
            }

            if (IsChanged(TypeChangedFlags.UriFilterChanged))
            {
                writer.Write((byte)RcpTypes.UriOptions.Filter);
                WriteValue(writer, FFilter);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.UriOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.UriOptions), option))
                throw new RCPDataErrorException("UriDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.UriOptions.Default:
                    Default = ReadValue(input);
                    return true;

                //case RcpTypes.UriOptions.Schema:
                //    Schema = new RcpTypes.TinyString(input).Data;
                //    return true;

                //case RcpTypes.UriOptions.Filter:
                //    Filter = new RcpTypes.TinyString(input).Data;
                //    return true;
            }

            return false;
        }
    }
}