using System;
using System.Collections.Generic;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class RangeParameter<T> : ValueParameter<Range<T>>, IRangeParameter /*where T : struct*/
    {
        public new RangeDefinition<T> TypeDefinition => base.TypeDefinition as RangeDefinition<T>;

        public RangeParameter(int id, IParameterManager manager, RangeDefinition<T> typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }

        public T Minimum
        {
            get => TypeDefinition.ElementType.Minimum;
            set
            {
                if (!Equals(value, Minimum))
                {
                    TypeDefinition.ElementType.Minimum = value;
                    OnPropertyChanged();
                }
            }
        }

        public T Maximum
        {
            get => TypeDefinition.ElementType.Maximum;
            set
            {
                if (!Equals(value, Maximum))
                {
                    TypeDefinition.ElementType.Maximum = value;
                    OnPropertyChanged();
                }
            }
        }

        public T Lower
        {
            get => Value.Lower;
            set
            {
                if (!Equals(value, Lower))
                {
                    Value = new Range<T>(value, Upper);
                    OnPropertyChanged();
                }
            }
        }

        public T Upper
        {
            get => Value.Upper;
            set
            {
                if (!Equals(value, Upper))
                {
                    Value = new Range<T>(Lower, value);
                    OnPropertyChanged();
                }
            }
        }

        IRangeDefinition IRangeParameter.TypeDefinition => TypeDefinition;
        object IRangeParameter.Lower { get => Lower; set => Lower = (T)value; }
        object IRangeParameter.Upper { get => Upper; set => Upper = (T)value; }
    }
}
