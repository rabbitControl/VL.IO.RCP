using System;
using System.IO;
using Kaitai;
using RCP.Parameters;
using RCP.Protocol;

namespace RCP.Types
{                           
    public class GroupDefinition : TypeDefinition
    {
        public GroupDefinition()
        : base(RcpTypes.Datatype.Group)
        {
            
        }

        public override Type ClrType => null;

        public override Parameter CreateParameter(short id, IParameterManager manager) => new GroupParameter(id, manager, this);

        public override TypeDefinition CreateArray(int[] structure)
        {
            throw new NotSupportedException();
        }

        public override TypeDefinition CreateRange()
        {
            throw new NotSupportedException();
        }
    }
}
