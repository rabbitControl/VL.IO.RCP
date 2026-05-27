using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using RCP.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Numerics;
using Color = System.Drawing.Color;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace RCP.Types
{
    [Flags]
    public enum TypeChangedFlags : int
    {
        Default = 1 << 0,
        EnumEntires = 1 << 1,
        EnumMultiSelect = 1 << 2,
        ValueMinimum = 1 << 3,
        ValueMaximum = 1 << 4,
        ValueMultipleOf = 1 << 5,
        ValueScale = 1 << 6,
        ValueUnit = 1 << 7,
        StringRegexp = 1 << 8,
        ArrayStructure = 1 << 9,
        UriSchemaChanged = 1 << 10,
        UriFilterChanged = 1 << 11
    }

    public abstract class TypeDefinition : RCPObject, ITypeDefinition
    {
        public static bool HasElementType(RcpTypes.Datatype type)
        {
            switch (type)
            {
                case RcpTypes.Datatype.Array:
                //case RcpTypes.Datatype.List:
                case RcpTypes.Datatype.Range:
                    return true;
            }
            return false;
        }

        public static RcpTypes.Datatype GetDatatype(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return RcpTypes.Datatype.Boolean;
                case TypeCode.Byte:
                    return RcpTypes.Datatype.Uint8;
                case TypeCode.Double:
                    return RcpTypes.Datatype.Float64;
                case TypeCode.Int16:
                    return RcpTypes.Datatype.Int16;
                case TypeCode.Int32:
                    return RcpTypes.Datatype.Int32;
                case TypeCode.Int64:
                    return RcpTypes.Datatype.Int64;
                case TypeCode.SByte:
                    return RcpTypes.Datatype.Int8;
                case TypeCode.Single:
                    return RcpTypes.Datatype.Float32;
                case TypeCode.String:
                    return RcpTypes.Datatype.String;
                case TypeCode.UInt16:
                    return RcpTypes.Datatype.Uint16;
                case TypeCode.UInt32:
                    return RcpTypes.Datatype.Uint32;
                case TypeCode.UInt64:
                    return RcpTypes.Datatype.Uint64;
                default:
                    break;
            }

            if (type == typeof(Color))
                return RcpTypes.Datatype.Rgba;
            if (type == typeof(Vector2))
                return RcpTypes.Datatype.Vector2f32;
            if (type == typeof(Vector3))
                return RcpTypes.Datatype.Vector3f32;
            if (type == typeof(Vector4))
                return RcpTypes.Datatype.Vector4f32;

            throw new NotSupportedException();
        }

        public static TypeDefinition Create(RcpTypes.Datatype type)
        {
            switch (type)
            {
                case RcpTypes.Datatype.Customtype:
                    break;
                case RcpTypes.Datatype.Boolean:
                    return new BooleanDefinition();
                case RcpTypes.Datatype.Int8:
                    return new Integer8Definition();
                case RcpTypes.Datatype.Uint8:
                    return new UInteger8Definition();
                case RcpTypes.Datatype.Int16:
                    return new Integer16Definition();
                case RcpTypes.Datatype.Uint16:
                    return new UInteger16Definition();
                case RcpTypes.Datatype.Int32:
                    return new Integer32Definition();
                case RcpTypes.Datatype.Uint32:
                    return new UInteger32Definition();
                case RcpTypes.Datatype.Int64:
                    return new Integer64Definition();
                case RcpTypes.Datatype.Uint64:
                    return new UInteger64Definition();
                case RcpTypes.Datatype.Float32:
                    return new Float32Definition();
                case RcpTypes.Datatype.Float64:
                    return new Float64Definition();
                case RcpTypes.Datatype.Vector2i32:
                    break;
                case RcpTypes.Datatype.Vector2f32:
                    return new Vector2f32Definition();
                case RcpTypes.Datatype.Vector3i32:
                    break;
                case RcpTypes.Datatype.Vector3f32:
                    return new Vector3f32Definition();
                case RcpTypes.Datatype.Vector4i32:
                    break;
                case RcpTypes.Datatype.Vector4f32:
                    return new Vector4f32Definition();
                case RcpTypes.Datatype.String:
                    return new StringDefinition();
                case RcpTypes.Datatype.Rgb:
                    return new RGBDefinition();
                case RcpTypes.Datatype.Rgba:
                    return new RGBADefinition();
                case RcpTypes.Datatype.Enum:
                    return new EnumDefinition();
                case RcpTypes.Datatype.Array:
                    throw new ArgumentException(nameof(type), "Must not be an array.");
                //case RcpTypes.Datatype.List:
                //    throw new ArgumentException(nameof(type), "Must not be a list.");
                case RcpTypes.Datatype.Bang:
                    return new BangDefinition();
                case RcpTypes.Datatype.Group:
                    return new GroupDefinition();
                case RcpTypes.Datatype.Uri:
                    return new UriDefinition();
                case RcpTypes.Datatype.Image:
                    return new ImageDefinition();
                case RcpTypes.Datatype.Ipv4:
                    break;
                case RcpTypes.Datatype.Ipv6:
                    break;
                case RcpTypes.Datatype.Range:
                    throw new ArgumentException(nameof(type), "Must not be a range.");
                default:
                    throw new NotImplementedException($"Unkown type {type}.");
            }
            throw new NotSupportedException($"{type} is not supported yet.");
        }

        int FChangedFlags;

        public TypeDefinition(RcpTypes.Datatype datatype)
        {
            Datatype = datatype;
        }

        public RcpTypes.Datatype Datatype { get; }
        public abstract Type ClrType { get; }

        public abstract TypeDefinition CreateArray(int[] structure);
        public abstract TypeDefinition CreateRange();
        public abstract Parameter CreateParameter(int id, IParameterManager manager);

        public virtual void ResetForInitialize()
        {
        }

        public virtual void Write(BinaryWriter writer)
        {
            var datatypeId = (byte)Datatype;
            if (!IsDirty)
                datatypeId |= 128;

            writer.Write(datatypeId);

            //write type specific stuff
            WriteOptions(writer);

            //reset changed flags
            ResetChangedFlags();

            //terminate
            //writer.Write((byte)0);
        }

        internal TypeChangedFlags ResetChangedFlags()
        {
            return (TypeChangedFlags)Interlocked.Exchange(ref FChangedFlags, 0);
        }

        protected virtual void WriteOptions(BinaryWriter writer)
        {
        }

        protected virtual bool HandleOption(KaitaiStream input, byte option)
        {
            return false;
        }

        public virtual void ParseOptions(KaitaiStream input)
        {
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                // handle option in specific implementation
                if (!HandleOption(input, code))
                {
                    throw new RCPUnsupportedFeatureException();
                }
            }
        }

        internal bool IsDirty => FChangedFlags != 0;
        protected bool IsChanged(TypeChangedFlags flag) => ((TypeChangedFlags)FChangedFlags).HasFlag(flag);
        protected void SetChanged(TypeChangedFlags flag) => FChangedFlags |= (int)flag;
        protected void ClearChanged(TypeChangedFlags flag) => FChangedFlags &= ~(int)flag;
    }

    public abstract class DefaultDefinition<T>: TypeDefinition, IDefaultDefinition<T>
    {
        T FDefault;

        public DefaultDefinition(RcpTypes.Datatype datatype, T @default) : base(datatype)
        {
            TypeDefault = FDefault = @default;
        }

        public override Type ClrType => typeof(T);
        public T TypeDefault { get; }

        public T Default
        {
            get { return FDefault; }
            set
            {
                if (SetProperty(ref FDefault, value))
                    SetChanged(TypeChangedFlags.Default);
            }
        }

        public override sealed TypeDefinition CreateArray(int[] structure) => new ArrayDefinition<T>(this, structure);
        public override TypeDefinition CreateRange() => throw new NotSupportedException();

        public abstract void WriteValue(BinaryWriter writer, T value);
        public abstract T ReadValue(KaitaiStream input);

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (IsChanged(TypeChangedFlags.Default))
            {
                ClearChanged(TypeChangedFlags.Default);
                var optionId = (byte)RcpTypes.NumberOptions.Default;
                if (!IsDirty)
                    optionId |= 128;                       
                writer.Write(optionId);
                WriteValue(writer, Default);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.NumberOptions)code;

            switch (option)
            {
                case RcpTypes.NumberOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }

        public override void ResetForInitialize()
        {
            if (!Equals(Default, TypeDefault))
                SetChanged(TypeChangedFlags.Default);
            base.ResetForInitialize();
        }
    }
}