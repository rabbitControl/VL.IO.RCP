// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace RCP.Protocol
{
    public partial class RcpTypes : KaitaiStruct
    {
        public static RcpTypes FromFile(string fileName)
        {
            return new RcpTypes(new KaitaiStream(fileName));
        }


        public enum EnumOptions
        {
            Default = 48,
            Entries = 49,
            MinimumSelectionCount = 50,
            MaximumSelectionCount = 51,
        }

        public enum TextboxWidgetOptions
        {
            Multiline = 86,
            Password = 87,
            Placeholder = 88,
        }

        public enum RgbOptions
        {
            Default = 48,
        }

        public enum CustomtypeOptions
        {
            Default = 48,
            Typeid = 49,
            Config = 50,
        }

        public enum WidgetOptions
        {
            LabelVisible = 81,
            ValueVisible = 82,
            NeedsConfirmation = 83,
            Userdata = 84,
        }

        public enum PacketTypes
        {
            Info = 1,
            Initialize = 2,
            Update = 3,
            Updatevalue = 4,
            Remove = 5,
        }

        public enum DialWidgetOptions
        {
            Precision = 86,
            StepsizeMultiplier = 87,
            Cyclic = 88,
            NanMeaning = 89,
        }

        public enum ParameterOptions
        {
            Value = 32,
            Label = 33,
            Description = 34,
            Tags = 35,
            Order = 36,
            Parentid = 37,
            Widget = 38,
            Userdata = 39,
            Userid = 40,
            Readonly = 41,
            Enabled = 42,
        }

        public enum Ipv4Options
        {
            Default = 48,
        }

        public enum VectorOptions
        {
            Default = 48,
            Minimum = 49,
            Maximum = 50,
            Multipleof = 51,
            Scale = 52,
            Unit = 53,
        }

        public enum ImageOptions
        {
            Default = 48,
        }

        public enum RangeWidgetOptions
        {
            Precision = 86,
            StepsizeMultiplier = 87,
            NanMeaning = 88,
        }

        public enum BooleanOptions
        {
            Default = 48,
        }

        public enum Widgettype
        {
            Default = 1,
            Custom = 2,
            Info = 16,
            Textbox = 17,
            Button = 18,
            Switch = 19,
            Checkbox = 20,
            Press = 21,
            Numberbox = 22,
            Dial = 23,
            Slider = 24,
            Slider2d = 25,
            Range = 26,
            Dropdown = 27,
            Radiobutton = 28,
            Colorchooser = 29,
            Table = 30,
            Uri = 31,
            Ip = 32,
            Image = 33,
            List = 16384,
            Tabs = 16385,
        }

        public enum UriWidgetOptions
        {
            Placeholder = 86,
            ButtonLabel = 87,
        }

        public enum TrackfillMode
        {
            None = 0,
            Left = 1,
            Center = 2,
            Right = 3,
        }

        public enum RgbaFloatOptions
        {
            Default = 48,
        }

        public enum CheckboxWidgetOptions
        {
            Indeterminate = 86,
        }

        public enum RgbaOptions
        {
            Default = 48,
        }

        public enum RangeOptions
        {
            Default = 48,
        }

        public enum UriOptions
        {
            Default = 48,
            Filter = 49,
            Schema = 50,
        }

        public enum RgbFloatOptions
        {
            Default = 48,
        }

        public enum ImageWidgetOptions
        {
            OverlayText = 86,
        }

        public enum StringOptions
        {
            Default = 48,
            RegularExpression = 49,
        }

        public enum InfodataOptions
        {
            Applicationid = 26,
            Applicationversion = 27,
        }

        public enum ArrayOptions
        {
            Default = 48,
        }

        public enum Datatype
        {
            Customtype = 1,
            Boolean = 16,
            Int8 = 17,
            Uint8 = 18,
            Int16 = 19,
            Uint16 = 20,
            Int32 = 21,
            Uint32 = 22,
            Int64 = 23,
            Uint64 = 24,
            Float32 = 25,
            Float64 = 26,
            Vector2i32 = 27,
            Vector2f32 = 28,
            Vector3i32 = 29,
            Vector3f32 = 30,
            Vector4i32 = 31,
            Vector4f32 = 32,
            String = 33,
            Rgb = 34,
            Rgba = 35,
            RgbFloat = 36,
            RgbaFloat = 37,
            Enum = 38,
            Array = 39,
            Bang = 40,
            Group = 41,
            Uri = 42,
            Ipv4 = 43,
            Ipv6 = 44,
            Range = 45,
            Image = 46,
        }

        public enum NumberOptions
        {
            Default = 48,
            Minimum = 49,
            Maximum = 50,
            Stepsize = 51,
            Unit = 52,
        }

        public enum Ipv6Options
        {
            Default = 48,
        }

        public enum ButtonWidgetOptions
        {
            ButtonLabel = 86,
            TriggerOnUp = 87,
        }

        public enum SwitchWidgetOptions
        {
            SwitchLabelOn = 86,
            SwitchLabelOff = 87,
        }

        public enum SliderWidgetOptions
        {
            Precision = 86,
            StepsizeMultiplier = 87,
            Horizontal = 88,
            NanMeaning = 89,
            TrackfillMode = 90,
        }

        public enum NumberboxWidgetOptions
        {
            Precision = 86,
            StepsizeMultiplier = 87,
            Cyclic = 88,
            NanMeaning = 89,
        }

        public enum CustomwidgetOptions
        {
            Widgetid = 86,
            Config = 87,
        }

        public enum PressWidgetOptions
        {
            PressLabelOn = 86,
            PressLabelOf = 87,
        }
        public RcpTypes(KaitaiStream p__io, KaitaiStruct p__parent = null, RcpTypes p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
        }
        private RcpTypes m_root;
        private KaitaiStruct m_parent;
        public RcpTypes M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
