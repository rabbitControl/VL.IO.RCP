using System;
using System.ComponentModel;
using System.IO;
using Kaitai;
using RCP.Protocol;
using RCP.Types;

namespace RCP.Parameters
{
    public class BangParameter: Parameter, INotifyPropertyChanged
    {
        bool FBangPending;

        public event EventHandler OnBang;

        public BangParameter(Int16 id, IParameterManager manager)
            : base(id, manager, new BangDefinition())
        {
        }

        public void Bang()
        {
            SetChanged(ParameterChangedFlags.Value);
            FBangPending = true;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();
            FBangPending = false;
        }

        public override object ReadValue(KaitaiStream input)
        {
            OnBang?.Invoke(this, null);
            return null;
        }
    }
}
