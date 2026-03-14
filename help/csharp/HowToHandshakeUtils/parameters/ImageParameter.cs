using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class ImageParameter : ValueParameter<byte[]>
    {
        public new ImageDefinition TypeDefinition => base.TypeDefinition as ImageDefinition;

        public ImageParameter(Int16 id, IParameterManager manager, ImageDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }
    }
}
