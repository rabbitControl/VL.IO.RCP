using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class UriParameter : ValueParameter<string>
    {
        public new UriDefinition TypeDefinition => base.TypeDefinition as UriDefinition;

        public UriParameter(Int16 id, IParameterManager manager, UriDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string Schema
        {
            get => TypeDefinition.Schema;
            set => TypeDefinition.Schema = value;
        }

        public string Filter
        {
            get => TypeDefinition.Filter;
            set => TypeDefinition.Filter = value;
        }
    }
}
