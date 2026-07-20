using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class ArrayParameter<T> : ValueParameter<T[]>
    {
        public new ArrayDefinition<T> TypeDefinition => base.TypeDefinition as ArrayDefinition<T>;

        public ArrayParameter(int id, IParameterManager manager, ArrayDefinition<T> typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }
    }
}
