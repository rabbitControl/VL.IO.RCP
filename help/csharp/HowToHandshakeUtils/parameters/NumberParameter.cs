using System;
using RCP.Protocol;
using RCP.Types;

namespace RCP.Parameters
{
    public class NumberParameter<T> : ValueParameter<T>
        //where T : struct
    {
        public new NumberDefinition<T> TypeDefinition => base.TypeDefinition as NumberDefinition<T>;

        public NumberParameter(int id, IParameterManager manager, NumberDefinition<T> typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public T Minimum
        {
            get => TypeDefinition.Minimum;
            set => TypeDefinition.Minimum = value;
        }

        public T Maximum
        {
            get => TypeDefinition.Maximum;
            set => TypeDefinition.Maximum = value;
        }

        public T MultipleOf
        {
            get => TypeDefinition.MultipleOf;
            set => TypeDefinition.MultipleOf = value;
        }

        //public RcpTypes.NumberScale Scale
        //{
        //    get => TypeDefinition.Scale;
        //    set => TypeDefinition.Scale = value;
        //}

        public string Unit
        {
            get => TypeDefinition.Unit;
            set => TypeDefinition.Unit = value;
        }
    }
}
