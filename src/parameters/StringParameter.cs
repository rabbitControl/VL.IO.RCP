using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class StringParameter : ValueParameter<string>
    {
        public new StringDefinition TypeDefinition => base.TypeDefinition as StringDefinition;

        public StringParameter(int id, IParameterManager manager, StringDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string RegularExpression
        {
            get => TypeDefinition.RegularExpression;
            set => TypeDefinition.RegularExpression = value;
        }
    }
}
