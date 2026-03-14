using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class EnumParameter : ValueParameter<string>
    {
        public new EnumDefinition TypeDefinition => base.TypeDefinition as EnumDefinition;

        public string[] Entries
        {
            get => TypeDefinition.Entries;
            set => TypeDefinition.Entries = value;
        }

        public bool MultiSelect
        {
            get => TypeDefinition.MultiSelect;
            set => TypeDefinition.MultiSelect = value;
        }

        public EnumParameter(Int16 id, IParameterManager manager, EnumDefinition typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }
    }
}
