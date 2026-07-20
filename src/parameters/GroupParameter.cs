using Kaitai;
using RCP.Protocol;
using RCP.Types;
using System;
using System.Collections.Generic;

namespace RCP.Parameters
{
    public sealed class GroupParameter : Parameter, IGroupParameter
    {
        public new GroupDefinition TypeDefinition => base.TypeDefinition as GroupDefinition;

        private List<IParameter> FParams = new List<IParameter>();
        private List<IParameter> FAddedParams = new List<IParameter>();
        private List<IParameter> FRemovedParams = new List<IParameter>();

        public GroupParameter(int id, IParameterManager manager, GroupDefinition typeDefinition): 
            base (id, manager, typeDefinition)
        {
        }

        public void AddParameter(IParameter param)
        {
            (param as Parameter).SetParent(this);
        }

        //protected override void ParseTypeDefinitionOptions(KaitaiStream input)
        //{
        //    //read the terminator
        //    input.ReadU1();
        //}

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            return false;
        }
    }
}
