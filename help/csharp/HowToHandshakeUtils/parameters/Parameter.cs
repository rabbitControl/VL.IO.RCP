using Kaitai;
using Microsoft.Extensions.Options;
using RCP.Exceptions;
using RCP.IO;
using RCP.Protocol;
using RCP.Types;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace RCP.Parameters
{
    public enum Status { Update, Remove };

    [Flags]
    public enum ParameterChangedFlags : int
    {
        ParentId = 1 << 0,
        Label = 1 << 1,
        Description = 1 << 2,
        Tags = 1 << 3,
        Order = 1 << 4,
        Userdata = 1 << 5,
        UserId = 1 << 6,
        Widget = 1 << 7,
        Value = 1 << 8,
        Type = 1 << 9,
        Readonly = 1 << 10,
        Enabled = 1 << 11
    }

    public abstract class Parameter : RCPObject, IParameter, IWriteable
    {
        public static Parameter Create(IParameterManager manager, int id, RcpTypes.Datatype datatype) => Create(manager, id, datatype, 0);

        public static Parameter Create(IParameterManager manager, int id, RcpTypes.Datatype datatype, RcpTypes.Datatype elementType) => Create(manager, id, datatype, elementType, null);

        public static Parameter Create(IParameterManager manager, int id, RcpTypes.Datatype datatype, RcpTypes.Datatype elementType, int[] structure)
        {
            TypeDefinition typeDefinition, elementTypeDefinition;
            switch (datatype)
            {
                case RcpTypes.Datatype.Array:
                    elementTypeDefinition = TypeDefinition.Create(elementType);
                    typeDefinition = elementTypeDefinition.CreateArray(structure ?? new int[1]);
                    break;
                //case RcpTypes.Datatype.List:
                //    throw new NotImplementedException();
                case RcpTypes.Datatype.Range:
                    elementTypeDefinition = TypeDefinition.Create(elementType);
                    typeDefinition = elementTypeDefinition.CreateRange();
                    break;
                default:
                    typeDefinition = TypeDefinition.Create(datatype);
                    break;
            }
            return typeDefinition.CreateParameter(id, manager);
        }

        IParameterManager FManager;
        int FParentId;
        ImmutableDictionary<string, string> FLabels = ImmutableDictionary<string, string>.Empty;
        ImmutableDictionary<string, string> FDescriptions = ImmutableDictionary<string, string>.Empty;
        string FTags = "";
        int FChangedFlags;
        private int FOrder;
        private byte[] FUserdata = Array.Empty<byte>();
        private string FUserId = "";
        private Widget FWidget = new Widget(RcpTypes.Widgettype.Default);
        private bool FReadonly;
        private bool FEnabled;

        public event EventHandler Updated;
        public event EventHandler ValueUpdated;

        public Parameter(int id, IParameterManager manager, TypeDefinition type)
        {
            Id = id;
            FManager = manager;
            TypeDefinition = type;
            if (TypeDefinition is GroupDefinition)
                FWidget = new ListWidget();
            // Redirect notifications from type
            type.PropertyChanged += (s, p) => OnPropertyChanged(p.PropertyName);
        }

        public int Id { get; }
        public TypeDefinition TypeDefinition { get; }

        public int ParentId
        {
            get { return FParentId; }
            set
            {
                if (SetProperty(ref FParentId, value))
                    SetChanged(ParameterChangedFlags.ParentId);
            }
        }

        public string Label
        {
            get => FLabels.GetValueOrDefault("any", "");
            set
            {
                if (SetProperty(ref FLabels, FLabels.SetItem("any", value)))
                    SetChanged(ParameterChangedFlags.Label);
            }
        }

        public string Description
        {
            get => FDescriptions.GetValueOrDefault("any", "");
            set
            {
                if (SetProperty(ref FDescriptions, FDescriptions.SetItem("any", value)))
                    SetChanged(ParameterChangedFlags.Description);
            }
        }

        public string Tags
        {
            get => FTags;
            set
            {
                if (SetProperty(ref FTags, value))
                    SetChanged(ParameterChangedFlags.Tags);
            }
        }

        public int Order
        {
            get => FOrder;
            set
            {
                if (SetProperty(ref FOrder, value))
                    SetChanged(ParameterChangedFlags.Order);
            }
        }


        public string UserId
        {
            get => FUserId;
            set
            {
                if (SetProperty(ref FUserId, value))
                    SetChanged(ParameterChangedFlags.UserId);
            }
        }

        public Widget Widget
        {
            get => FWidget;
            set
            {
                if (FWidget != null)
                    FWidget.PropertyChanged -= (s, p) => OnPropertyChanged(p.PropertyName);

                if (SetProperty(ref FWidget, value))
                {
                    SetChanged(ParameterChangedFlags.Widget);

                    if (FWidget != null)
                        //Redirect notifications from widget
                        FWidget.PropertyChanged += (s, p) => OnPropertyChanged(p.PropertyName);
                }
            }
        }

        public byte[] Userdata
        {
            get => FUserdata;
            set
            {
                if (SetProperty(ref FUserdata, value))
                    SetChanged(ParameterChangedFlags.Userdata);
            }
        }

        public bool Readonly
        {
            get => FReadonly;
            set
            {
                if (SetProperty(ref FReadonly, value))
                    SetChanged(ParameterChangedFlags.Readonly);
            }
        }

        public bool Enabled
        {
            get => FEnabled;
            set
            {
                if (SetProperty(ref FEnabled, value))
                    SetChanged(ParameterChangedFlags.Enabled);
            }
        }

        public void SetParent(IParameter param)
        {
            ParentId = param.Id;
        }
        
        public void SetLanguageLabel(string iso639_3, string label)
        {
            FLabels = FLabels.SetItem(iso639_3, label);
            SetChanged(ParameterChangedFlags.Label);
        }

        public void RemoveLanguageLabel(string iso639_3)
        {
            if (FLabels.ContainsKey(iso639_3))
            {
                FLabels = FLabels.Remove(iso639_3);
                SetChanged(ParameterChangedFlags.Label);
            }
        }

        public void SetLanguageDescription(string iso639_3, string description)
        {
            FDescriptions = FDescriptions.SetItem(iso639_3, description);
            SetChanged(ParameterChangedFlags.Description);
        }

        public void RemoveLanguageDescription(string iso639_3)
        {
            if (FDescriptions.ContainsKey(iso639_3))
            {
                FDescriptions = FDescriptions.Remove(iso639_3);
                SetChanged(ParameterChangedFlags.Description);
            }
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.WriteValue(Parser.AddIntBytes(Id), ByteOrder.BigEndian);

            TypeDefinition.Write(writer);

            if (IsDirty)
            {
                //optional
                WriteValue(writer);

                if (IsChanged(ParameterChangedFlags.Label))
                {
                    ClearChanged(ParameterChangedFlags.Label);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Label, IsDirty));

                    foreach (var language in FLabels.Keys)
                    {
                        writer.Write(Encoding.ASCII.GetBytes(language));
                        writer.Write(Parser.AddStringBytes(FLabels[language]));
                    }
                    writer.Write((byte)0);
                }

                if (IsChanged(ParameterChangedFlags.Description))
                {
                    ClearChanged(ParameterChangedFlags.Description);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Description, IsDirty));

                    foreach (var language in FDescriptions.Keys)
                    {
                        writer.Write(Encoding.ASCII.GetBytes(language));
                        writer.Write(Parser.AddStringBytes(FDescriptions[language]));
                    }
                    writer.Write((byte)0);
                }

                if (IsChanged(ParameterChangedFlags.Tags))
                {
                    ClearChanged(ParameterChangedFlags.Tags);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Tags, IsDirty));
                    writer.Write(Parser.AddStringBytes(Tags));
                }

                if (IsChanged(ParameterChangedFlags.Order))
                {
                    ClearChanged(ParameterChangedFlags.Order);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Order, IsDirty));
                    writer.Write(Parser.AddIntBytes(Order));
                }

                if (IsChanged(ParameterChangedFlags.ParentId))
                {
                    ClearChanged(ParameterChangedFlags.ParentId);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Parentid, IsDirty));
                    writer.Write(Parser.AddIntBytes(ParentId));
                }

                if (IsChanged(ParameterChangedFlags.Widget) || (Widget?.IsDirty ?? false))
                {
                    ClearChanged(ParameterChangedFlags.Widget);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Widget, IsDirty));
                    Widget.Write(writer);
                }

                if (IsChanged(ParameterChangedFlags.Userdata))
                {
                    ClearChanged(ParameterChangedFlags.Userdata);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Userdata, IsDirty));
                    writer.Write(Parser.AddIntBytes(Userdata.Length));
                    writer.Write(Userdata);
                }

                if (IsChanged(ParameterChangedFlags.UserId))
                {
                    ClearChanged(ParameterChangedFlags.UserId);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Userid, IsDirty));
                    writer.Write(Parser.AddStringBytes(UserId));
                }

                if (IsChanged(ParameterChangedFlags.Readonly))
                {
                    ClearChanged(ParameterChangedFlags.Readonly);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Readonly, IsDirty));
                    writer.Write(Readonly);
                }

                if (IsChanged(ParameterChangedFlags.Enabled))
                {
                    ClearChanged(ParameterChangedFlags.Enabled);
                    writer.Write(Parser.AddOptionId(RcpTypes.ParameterOptions.Enabled, IsDirty));
                    writer.Write(Enabled);
                }
            }
            else //terminate
                writer.Write((byte)0);

            //reset changed flags
            FChangedFlags = 0;
        }

        public virtual void WriteValue(BinaryWriter writer, bool includeOption = true) 
        {
            ClearChanged(ParameterChangedFlags.Value);
        } 

        public virtual object ReadValue(KaitaiStream input) { return null; }

        public static Parameter Parse(KaitaiStream input, IParameterManager manager)
        {
            // get mandatory id
            var id = Parser.ReadInt(input);

            var datatype = Parser.ReadDatatypeId(input, out var hasOptions);

            RcpTypes.Datatype elementType;
            if (TypeDefinition.HasElementType(datatype))
                elementType = Parser.ReadDatatypeId(input, out var elementTypeHasOptions);
            else
                elementType = 0;

            var parameter = manager.GetParameter(id) ?? Create(manager, id, datatype, elementType);
            if (hasOptions)
                parameter.TypeDefinition.ParseOptions(input);
            if (input.PeekChar() != 0x80)
                parameter.ParseOptions(input);
            return parameter;
        }

        

        protected virtual bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions code)
        {
            return false;
        }

        private void ParseOptions(KaitaiStream input)
        {
            // get options from the stream
            var optionsFollow = true;
            while (optionsFollow)
            {
                var option = (RcpTypes.ParameterOptions)Parser.ReadOptionId(input, out optionsFollow);

                if (!Enum.IsDefined(typeof(RcpTypes.ParameterOptions), option))
                    throw new RCPDataErrorException("Parameter parsing: Unknown option: " + option.ToString());

                switch (option)
                {
                    case RcpTypes.ParameterOptions.Label:
                        while (input.PeekChar() > 0)
                        {
                            var language = new string(input.ReadChars(3));
                            var count = Parser.ReadInt(input);
                            FLabels = FLabels.SetItem(language, Encoding.UTF8.GetString(input.ReadBytes(count)));
                            SetChanged(ParameterChangedFlags.Label);
                        }
                        input.ReadByte(); //0 terminator
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        while (input.PeekChar() > 0)
                        {
                            var language = new string(input.ReadChars(3));
                            var count = Parser.ReadInt(input);
                            FDescriptions = FDescriptions.SetItem(language, Encoding.UTF8.GetString(input.ReadBytes(count)));
                            SetChanged(ParameterChangedFlags.Label);
                        }
                        input.ReadByte(); //0 terminator
                        break;

                    case RcpTypes.ParameterOptions.Tags:
                        Tags = Parser.ReadString(input);
                        break;

                    case RcpTypes.ParameterOptions.Order:
                        Order = Parser.ReadInt(input);
                        break;

                    case RcpTypes.ParameterOptions.Parentid:
                        ParentId = Parser.ReadInt(input);
                        break;

                    case RcpTypes.ParameterOptions.Widget:
                        Widget = Widget.Parse(input);
                        break;

                    case RcpTypes.ParameterOptions.Userdata:
                        var byteCount = Parser.ReadInt(input);
                        Userdata = input.ReadBytes(byteCount);
                        break;

                    case RcpTypes.ParameterOptions.Userid:
                        UserId = Parser.ReadString(input);
                        break;

                    case RcpTypes.ParameterOptions.Readonly:
                        Readonly = input.ReadBoolean();
                        break;

                    case RcpTypes.ParameterOptions.Enabled:
                        Enabled = input.ReadBoolean();
                        break;

                    default:
                        if (!HandleOption(input, option))
                        {
                            throw new RCPUnsupportedFeatureException();
                        }
                        break;
                }
            }
        }

        internal bool IsDirty => FChangedFlags != 0 || TypeDefinition.IsDirty || (Widget?.IsDirty ?? false);

        internal bool OnlyValueChanged => (ParameterChangedFlags)FChangedFlags == ParameterChangedFlags.Value;
        protected bool IsChanged(ParameterChangedFlags flags) => ((ParameterChangedFlags)FChangedFlags).HasFlag(flags);
        protected void SetChanged(ParameterChangedFlags flags) => FChangedFlags |= (int)flags;
        protected void ClearChanged(ParameterChangedFlags flags) => FChangedFlags &= ~(int)flags;

        public virtual void ResetForInitialize()
        {
            if (FLabels.Count > 0)
                SetChanged(ParameterChangedFlags.Label);
            if (FDescriptions.Count > 0)
                SetChanged(ParameterChangedFlags.Description);
            if (FTags != "")
                SetChanged(ParameterChangedFlags.Tags);
            if (FOrder != 0)
                SetChanged(ParameterChangedFlags.Order);
            if (FParentId != 0)
                SetChanged(ParameterChangedFlags.ParentId);
            if (FWidget != null)
                SetChanged(ParameterChangedFlags.Widget);
            if (FUserdata.Length != 0)
                SetChanged(ParameterChangedFlags.Userdata);
            if (FUserId != "")
                SetChanged(ParameterChangedFlags.UserId);
            if (FReadonly)
                SetChanged(ParameterChangedFlags.Readonly);

            TypeDefinition.ResetForInitialize();
            Widget?.ResetForInitialize();
        }

        internal void RaiseEvents()
        {
            var flags = (ParameterChangedFlags)Interlocked.Exchange(ref FChangedFlags, 0);
            var typeChangedFlags = TypeDefinition.ResetChangedFlags();
            if (typeChangedFlags != 0)
                flags |= ParameterChangedFlags.Type;

            if (flags != 0)
            {
                if (flags != ParameterChangedFlags.Value)
                    Updated?.Invoke(this, EventArgs.Empty);
                if (flags.HasFlag(ParameterChangedFlags.Value))
                    ValueUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        ITypeDefinition IParameter.TypeDefinition => TypeDefinition;
    }

    public class ValueParameter<T> : Parameter, IValueParameter<T>, INotifyPropertyChanged
    {
        T FValue;

        public ValueParameter(int id, IParameterManager manager, DefaultDefinition<T> type) 
            : base(id, manager, type)
        {
            FValue = type.Default;
        }

        public new DefaultDefinition<T> TypeDefinition => base.TypeDefinition as DefaultDefinition<T>;

        public T Default
        {
            get => TypeDefinition.Default;
            set => TypeDefinition.Default = value;
        }

        public T Value
        {
            get { return FValue; }
            set
            {
                if (SetProperty(ref FValue, value))
                    SetChanged(ParameterChangedFlags.Value);
            }
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();
            if (!Equals(Value, TypeDefinition.Default))
                SetChanged(ParameterChangedFlags.Value);
        }

        public override void WriteValue(BinaryWriter writer, bool includeOption = true)
        {
            if (IsChanged(ParameterChangedFlags.Value))
            {
                if (includeOption)
                    writer.Write((byte)RcpTypes.ParameterOptions.Value);
                TypeDefinition.WriteValue(writer, Value);
            }
            base.WriteValue(writer);
        }

        public override object ReadValue(KaitaiStream input)
        {
            HandleOption(input, RcpTypes.ParameterOptions.Value);
            return Value;
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = TypeDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }

        object IValueParameter.Value { get => Value; set => Value = (T)value; }
        object IValueParameter.Default { get => Default; set => Default = (T)value; }
    }
}
