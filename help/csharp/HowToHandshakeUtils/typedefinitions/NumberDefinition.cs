using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameters;
using System.Collections.Generic;

namespace RCP.Types
{
    public abstract class NumberDefinition<T> : DefaultDefinition<T>, INumberDefinition<T> /*where T: struct*/
    {
        T FMinimum, FMaximum, FMultipleOf;
        //RcpTypes.NumberScale FScale;
        string FUnit = "";

        public T Minimum
        {
            get => FMinimum;
            set
            {
                if (SetProperty(ref FMinimum, value))
                    SetChanged(TypeChangedFlags.ValueMinimum);
            }
        }

        public T Maximum
        {
            get => FMaximum;
            set
            {
                if (SetProperty(ref FMaximum, value))
                    SetChanged(TypeChangedFlags.ValueMaximum);
            }
        }

        public T MultipleOf
        {
            get => FMultipleOf;
            set
            {
                if (SetProperty(ref FMultipleOf, value))
                    SetChanged(TypeChangedFlags.ValueMultipleOf);
            }
        }

        //public RcpTypes.NumberScale Scale
        //{
        //    get => FScale;
        //    set
        //    {
        //        if (SetProperty(ref FScale, value))
        //            SetChanged(TypeChangedFlags.ValueScale);
        //    }
        //}

        public string Unit
        {
            get => FUnit;
            set
            {
                if (SetProperty(ref FUnit, value))
                    SetChanged(TypeChangedFlags.ValueUnit);
            }
        }

        public NumberDefinition()
            : base(GetDatatype(typeof(T)), default(T))
        {
            FMinimum = DefaultMinimum;
            FMaximum = DefaultMaximum;
            FMultipleOf = DefaultMulitpleOf;
        }

        protected abstract T DefaultMinimum { get; }
        protected abstract T DefaultMaximum { get; }
        protected abstract T DefaultMulitpleOf { get; }

        public override sealed Parameter CreateParameter(short id, IParameterManager manager) => new NumberParameter<T>(id, manager, this);
        public override sealed TypeDefinition CreateRange() => new RangeDefinition<T>(this);

        public override void ResetForInitialize()
        {
            if (!Equals(Minimum, DefaultMinimum))
                SetChanged(TypeChangedFlags.ValueMinimum);
            if (!Equals(Maximum, DefaultMaximum))
                SetChanged(TypeChangedFlags.ValueMaximum);
            if (!Equals(MultipleOf, DefaultMulitpleOf))
                SetChanged(TypeChangedFlags.ValueMultipleOf);
            //if (FScale != RcpTypes.NumberScale.Linear)
            //    SetChanged(TypeChangedFlags.ValueScale);
            if (FUnit != "")
                SetChanged(TypeChangedFlags.ValueUnit);
            base.ResetForInitialize();
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (IsChanged(TypeChangedFlags.ValueMinimum))
            {
                ClearChanged(TypeChangedFlags.ValueMinimum);
                var optionId = (byte)RcpTypes.NumberOptions.Minimum;
                if (!IsDirty)
                    optionId |= 128;
                writer.Write(optionId);

                WriteValue(writer, Minimum);
            }

            if (IsChanged(TypeChangedFlags.ValueMaximum))
            {
                ClearChanged(TypeChangedFlags.ValueMaximum);
                var optionId = (byte)RcpTypes.NumberOptions.Maximum;
                if (!IsDirty)
                    optionId |= 128;
                writer.Write(optionId);

                WriteValue(writer, Maximum);
            }

            //if (IsChanged(TypeChangedFlags.ValueMultipleOf))
            //{
            //    writer.Write((byte)RcpTypes.NumberOptions.Multipleof);
            //    WriteValue(writer, MultipleOf);
            //}

            //if (IsChanged(TypeChangedFlags.ValueScale))
            //{
            //    writer.Write((byte)RcpTypes.NumberOptions.Scale);
            //    writer.Write((byte)Scale);
            //}

            if (IsChanged(TypeChangedFlags.ValueUnit))
            {
                ClearChanged(TypeChangedFlags.ValueUnit);
                var optionId = (byte)RcpTypes.NumberOptions.Unit;
                if (!IsDirty)
                    optionId |= 128;
                writer.Write(optionId);

                writer.Write(Parser.AddStringBytes(Unit));
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.NumberOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
                throw new RCPDataErrorException("NumberDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.NumberOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Minimum:
                    Minimum = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Maximum:
                    Maximum = ReadValue(input);
                    return true;

                //case RcpTypes.NumberOptions.Multipleof:
                //    MultipleOf = ReadValue(input);
                //    return true;

                //case RcpTypes.NumberOptions.Scale:
                //    Scale = (RcpTypes.NumberScale)input.ReadU1();
                //    return true;

                //case RcpTypes.NumberOptions.Unit:
                //    Unit = new RcpTypes.TinyString(input).Data;
                //    return true;
            }

            return false;
        }

        object INumberDefinition.Minimum => Minimum;
        object INumberDefinition.Maximum => Maximum;
        object INumberDefinition.MultipleOf => MultipleOf;
    }
}