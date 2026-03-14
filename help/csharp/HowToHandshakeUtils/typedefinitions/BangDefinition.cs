using System;
using System.IO;
using Kaitai;
using RCP.Parameters;
using RCP.Protocol;

namespace RCP.Types
{
    public class BangDefinition : TypeDefinition, IBangDefinition
    {
        public Action OnBang;

        public BangDefinition()
            : base(RcpTypes.Datatype.Bang)
        {
        }

        public override Type ClrType => throw new NotImplementedException();

        public override TypeDefinition CreateArray(int[] structure)
        {
            throw new NotImplementedException();
        }

        public override Parameter CreateParameter(short id, IParameterManager manager) => new BangParameter(id, manager);

        public override TypeDefinition CreateRange()
        {
            throw new NotImplementedException();
        }
    }
 }