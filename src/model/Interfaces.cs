using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using RCP.Protocol;
using Color = System.Drawing.Color;

namespace RCP
{
    public interface IWriteable
    {
        void Write(BinaryWriter writer);
    }

    public interface ITypeDefinition : IWriteable, INotifyPropertyChanged
    {
        RcpTypes.Datatype Datatype { get; }
        Type ClrType { get; }
        void ParseOptions(Kaitai.KaitaiStream input);
        void ResetForInitialize();
    }

    public interface INumberDefinition : ITypeDefinition
    {
        object Minimum { get; }
        object Maximum { get; }
        object MultipleOf { get; }
    }

    public interface IDefaultDefinition<T> : ITypeDefinition
    {
        T Default { get; set; }
        T ReadValue(Kaitai.KaitaiStream input);
        void WriteValue(BinaryWriter writer, T value);
    }

    public interface IBangDefinition : ITypeDefinition
    {
    }

    public interface IBoolDefinition : IDefaultDefinition<bool>
    {
    }

    public interface INumberDefinition<T> : IDefaultDefinition<T>, INumberDefinition /*where T : struct*/
    {
        new T Minimum { get; set; }
        new T Maximum { get; set; }
        new T MultipleOf { get; set; }
        //RcpTypes.NumberScale Scale { get; set; }
        string Unit { get; set; }
    }

    public interface IStringDefinition : IDefaultDefinition<string>
    {
        string RegularExpression { get; set; }
    }

    public interface IRGBADefinition : IDefaultDefinition<Color>
    {
    }

    public interface IUriDefinition : IDefaultDefinition<string>
    {
        string Schema { get; set; }
        string Filter { get; set; }
    }

    public interface IEnumDefinition : IDefaultDefinition<string>
    {
        string[] Entries { get; set; }
        bool MultiSelect { get; set; }
    }

    public interface IImageDefinition : IDefaultDefinition<byte[]>
    {
    }

    public interface IArrayDefinition : ITypeDefinition
    {
        ITypeDefinition ElementDefinition { get; }
        RcpTypes.Datatype ElementType { get; }
        int[] Structure { get; set; }
    }

    public interface IRangeDefinition : ITypeDefinition
    {
        INumberDefinition ElementDefinition { get; }
    }

    public interface IParameter : IWriteable, INotifyPropertyChanged
    {
        int Id { get; }
        ITypeDefinition TypeDefinition { get; }
        string Label { get; set; }
        string Description { get; set; }
        string Tags { get; set; }
        int Order { get; set; }
        int ParentId { get; }
        Widget Widget { get; set; }
        byte[] Userdata { get; set; }
        string UserId { get; set; }
        bool Readonly { get; set; }

        event EventHandler Updated;
    }

    public interface IValueParameter : IParameter
    {
        object Value { get; set; }
        object Default { get; set; }
        event EventHandler ValueUpdated;
    }

    public interface IValueParameter<T>: IValueParameter
    {
        new T Value { get; set; }
        new T Default { get; set; }
    }

    public interface IGroupParameter: IParameter
    {
    }

    public interface INumberParameter : IValueParameter
    {
        new INumberDefinition TypeDefinition { get; }
    }

    public interface IRangeParameter : IValueParameter
    {
        new IRangeDefinition TypeDefinition { get; }
        object Lower { get; set; }
        object Upper { get; set; }
    }
}
